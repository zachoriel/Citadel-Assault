using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCameraController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] float keyPannSpeed = 100f;
    [SerializeField] float mousePannSpeed = 200f;
    [SerializeField] float borderPannSpeed = 50f;
    [Space]
    [SerializeField] float keyRotSpeed = 100f;
    [SerializeField] float mouseRotSpeed = 200f;
    [Space]
    [SerializeField] float targetFollowSpeed = 5f;

    [Header("Height Settings")]
    public LayerMask groundMask;
    [SerializeField] float heightDampening = 5f;
    [SerializeField] float keyboardZoomSensitivity = 25f;
    [SerializeField] float scrollZoomSensitivity = 25f;
    float zoomPos = 0.1f;

    [Header("Map Limits")]
    [SerializeField] Vector2 panLimit = new Vector2(450f, 1500f);
    [SerializeField] Vector2 zoomLimit = new Vector2(60f, 380f);

    [Header("Screen Border Movement Zone")]
    [SerializeField] float borderThickness = 20f;
    bool usingScreenBorder;

    [Header("Targeting")]
    public Transform targetToFollow;
    public Vector3 targetOffset;
    public bool FollowingTarget
    {
        get
        {
            return targetToFollow != null;
        }
    }

    [Header("Panning Input - Keyboard")]
    [SerializeField] string vertical = "Vertical"; // w, s, up, down
    [SerializeField] string horizontal = "Horizontal"; // a, d, left, right

    [Header("Panning Input - Mouse")]
    [SerializeField] KeyCode mousePannKey = KeyCode.Mouse2; // middle mouse button

    [Header("Zooming Input - Keyboard")]
    [SerializeField] KeyCode zoomInKey = KeyCode.LeftControl;
    [SerializeField] KeyCode zoomOutKey = KeyCode.Space;

    [Header("Zooming Input - Mouse")]
    [SerializeField] string mouse = "Mouse ScrollWheel";
    bool usingScrollWheel;

    [Header("Rotation Input - Keyboard")]
    [SerializeField] KeyCode rotRightKey = KeyCode.E;
    [SerializeField] KeyCode rotLeftKey = KeyCode.Q;
    [HideInInspector] public bool usingKeyRotation = true;

    [Header("Rotation Input - Mouse")]
    [SerializeField] KeyCode mouseRotKey = KeyCode.Mouse1; // right mouse button

    #region Input Getters
    Vector2 KeyboardInput
    {
        get
        {
            return new Vector2(Input.GetAxisRaw(horizontal), Input.GetAxisRaw(vertical));
        }
    }

    Vector2 MouseInput
    {
        get
        {
            return new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }

    float ScrollWheel
    {
        get
        {
            return Input.GetAxis(mouse);
        }
    }

    Vector2 MouseAxis
    {
        get
        {
            return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }
    }

    int ZoomDirection
    {
        get
        {
            bool zoomIn = Input.GetKey(zoomInKey);
            bool zoomOut = Input.GetKey(zoomOutKey);

            if (zoomIn && zoomOut)
                return 0;
            else if (!zoomIn && zoomOut)
                return 1;
            else if (zoomIn && !zoomOut)
                return -1;
            else
                return 0;
        }
    }

    int RotationDirection
    {
        get
        {
            bool rotateRight = Input.GetKey(rotRightKey);
            bool rotateLeft = Input.GetKey(rotLeftKey);

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
    #endregion


    void Update()
    {
        UpdateCamera();
    }

    void UpdateCamera()
    {
        if (FollowingTarget)
        {
            FollowTarget();
        }
        else
        {
            PannCamera();
        }

        HeightCalculation();
        RotateCamera();
        LimitPosition();
    }

    void PannCamera()
    {
        #region Keyboard Input
        Vector3 keyMove = new Vector3(KeyboardInput.x, 0, KeyboardInput.y);

        keyMove *= keyPannSpeed * Time.deltaTime;
        keyMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * keyMove;
        keyMove = transform.InverseTransformDirection(keyMove);

        transform.Translate(keyMove, Space.Self);
        #endregion

        #region Mouse Button Input
        if (Input.GetKey(mousePannKey))
        {
            usingScrollWheel = false;
            usingScreenBorder = false;
            Cursor.visible = false;

            if (MouseAxis != Vector2.zero)
            {
                Vector3 mouseMove = new Vector3(-MouseAxis.x, 0, -MouseAxis.y);

                mouseMove *= mousePannSpeed * Time.deltaTime;
                mouseMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * mouseMove;
                mouseMove = transform.InverseTransformDirection(mouseMove);

                transform.Translate(mouseMove, Space.Self);
            }
        }
        else
        {
            usingScrollWheel = true;
            usingScreenBorder = true;
            Cursor.visible = true;
        }
        #endregion

        #region Screen Edge Input
        if (usingScreenBorder)
        {
            Vector3 edgeMove = new Vector3();

            Rect leftRect = new Rect(0, 0, borderThickness, Screen.height);
            Rect rightRect = new Rect(Screen.width - borderThickness, 0, borderThickness, Screen.height);
            Rect upRect = new Rect(0, Screen.height - borderThickness, Screen.width, borderThickness);
            Rect downRect = new Rect(0, 0, Screen.width, borderThickness);

            edgeMove.x = leftRect.Contains(MouseInput) ? -1 : rightRect.Contains(MouseInput) ? 1 : 0;
            edgeMove.z = upRect.Contains(MouseInput) ? 1 : downRect.Contains(MouseInput) ? -1 : 0;

            edgeMove *= borderPannSpeed * Time.deltaTime;
            edgeMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * edgeMove;
            edgeMove = transform.InverseTransformDirection(edgeMove);

            transform.Translate(edgeMove, Space.Self);
        }
        #endregion
    }

    void HeightCalculation()
    {
        float distanceToGround = DistanceToGround();

        if (usingScrollWheel)
        {
            zoomPos -= ScrollWheel * Time.deltaTime * scrollZoomSensitivity;
        }
        zoomPos += ZoomDirection * Time.deltaTime * keyboardZoomSensitivity;

        zoomPos = Mathf.Clamp01(zoomPos);

        float targetHeight = Mathf.Lerp(zoomLimit.x / 2, zoomLimit.y / 2, zoomPos);
        float difference = 0;

        if (distanceToGround != targetHeight)
        {
            difference = targetHeight - distanceToGround;
        }

        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, targetHeight + difference, transform.position.z), Time.deltaTime * heightDampening);
    }

    void RotateCamera()
    {
        if (Input.GetKey(mouseRotKey))
        {
            usingScreenBorder = false;
            Cursor.visible = false;
            transform.Rotate(Vector3.up, -MouseAxis.x * Time.deltaTime * mouseRotSpeed, Space.World);
        }
        else
        {
            if (usingKeyRotation)
            {
                usingScreenBorder = true;
                Cursor.visible = true;
                transform.Rotate(Vector3.up, RotationDirection * Time.deltaTime * keyRotSpeed, Space.World);
            }
        }
    }

    void FollowTarget()
    {
        Vector3 targetPos = new Vector3(targetToFollow.position.x, transform.position.y, targetToFollow.position.z) + targetOffset;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * targetFollowSpeed);
    }

    void LimitPosition()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, panLimit.x, panLimit.y), transform.position.y, Mathf.Clamp(transform.position.z, panLimit.x, panLimit.y));
    }

    public void SetTarget(Transform target)
    {
        targetToFollow = target;
    }

    public void ResetTarget()
    {
        targetToFollow = null;
    }

    float DistanceToGround()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, groundMask.value))
        {
            return (hit.point - transform.position).magnitude;
        }

        return 0f;
    }
}
