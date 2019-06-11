using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceBuilding : MonoBehaviour
{
    public enum BuildingStatus
    {
        None,
        Planning,
        Constructing,
        Finished
    };

    [Header("Building Status")]
    public BuildingStatus buildingStatus;
    public float buildTime = 5f;

    [Header("Placement Configurations")]
    public LayerMask collisionMask;
    public RectTransform placementTransform;
    [SerializeField] float keyRotSpeed = 100f;
    Collider[] collisionColliders;
    float detectionRadius;
    float slope;
    float yPos = 0f;

    [Header("Placement Visuals")]
    public Image placementMarker;
    public Color canPlaceColor;
    public Color cannotPlaceColor;

    [Header("Selection UI")]
    public Image selectionMarker;

    // Components
    Camera mainCam;
    BaseCameraController camController;

    int RotationDirection
    {
        get
        {
            bool rotateRight = Input.GetKey(KeyCode.E);
            bool rotateLeft = Input.GetKey(KeyCode.Q);

            if (rotateLeft && rotateRight)
                return 0;
            else if (rotateLeft && !rotateRight)
                return -1;
            else if (!rotateLeft && rotateRight)
                return 1;
            else
                return 0;
        }
    }

    bool canPlaceBuilding;

    void Awake ()
    {
        buildingStatus = BuildingStatus.Planning;
        mainCam = Camera.main;
        camController = mainCam.GetComponent<BaseCameraController>();
        gameObject.layer = 10; // Sets layer to planned building
        detectionRadius = new Vector2(placementTransform.rect.width, placementTransform.rect.height).magnitude / 3.5f;
    }

    void Update ()
    {
		if (buildingStatus == BuildingStatus.Planning)
        {
            camController.usingKeyRotation = false;
            MoveObject();
            canPlaceBuilding = CanPlaceHere();

            if (canPlaceBuilding && Input.GetKeyDown(KeyCode.Mouse0))
            {               
                buildingStatus = BuildingStatus.Constructing;
                StartCoroutine(ConstructBuilding());
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && buildingStatus != BuildingStatus.Finished)
        {
            camController.usingKeyRotation = true;
            Destroy(this.gameObject);
        }
	}

    void MoveObject()
    {
        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector3 worldPos;

        Ray ray = mainCam.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            worldPos = hit.point;
        }
        else
        {
            worldPos = mainCam.ScreenToWorldPoint(mousePos);
        }
        slope = -hit.normal.x / hit.normal.y;

        transform.position = new Vector3(worldPos.x, yPos, worldPos.z);
        if (RotationDirection != 0)
            transform.Rotate(Vector3.up, RotationDirection * Time.deltaTime * keyRotSpeed, Space.World);
    }

    bool CanPlaceHere()
    {
        collisionColliders = Physics.OverlapSphere(transform.position, detectionRadius, collisionMask);

        if (collisionColliders.Length != 0 || slope != 0f)
        {
            placementMarker.color = cannotPlaceColor;
            return false;
        }

        placementMarker.color = canPlaceColor;
        return true;
    }

    IEnumerator ConstructBuilding()
    {
        StartCoroutine(LerpBuilding());

        camController.usingKeyRotation = true;
        gameObject.layer = 9;

        Image[] images = GetComponentsInChildren<Image>();
        foreach (Image image in images)
        {
            if (image.gameObject.name == "PlacementMarker")
            {
                Destroy(image.gameObject);
            }
        }

        yield return new WaitForSeconds(buildTime);

        buildingStatus = BuildingStatus.Finished;
        yield return null;
    }

    IEnumerator LerpBuilding()
    {
        for (float time = 0.00f; time < buildTime + 0.01f; time += 0.01f)
        {
            transform.position = Vector3.Lerp(new Vector3(transform.position.x, -5f, transform.position.z), new Vector3(transform.position.x, 0f, transform.position.z), time / buildTime);
            yield return null;
        }
        StopCoroutine(LerpBuilding());
    }

    public void ShowSelection()
    {
        selectionMarker.enabled = true;
    }

    public void HideSelection()
    {
        selectionMarker.enabled = false;
    }
}
