using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance;

    // Checks if selection is a unit, structure, or other
    [Header("Detection Layers")]
    public LayerMask troopLayer;
    public LayerMask structureLayer;

    // The selection square drawn when you drag the mouse to select units
    [Header("Selection Graphic")]
    public RectTransform selectionSquareTrans;

    [Header("Units")]
    // All currently selected units 
    public List<GameObject> selectedUnits = new List<GameObject>();
    [HideInInspector] public int formationLeader;
    // All of the units in the scene
    [HideInInspector] public List<GameObject> allUnits = new List<GameObject>();

    // This unit has been hovered over, so we can deselect it next update and dont have to loop through all units
    GameObject highlightThisUnit;

    // To determine if we are clicking with left mouse or holding down left mouse
    float delay = 0.15f;
    float clickTime = 0f;

    // The start and end coordinates of the square we are making
    Vector3 squareStartPos;
    Vector3 squareEndPos;

    // If it was possible to create a square
    bool hasCreatedSquare;

    // The selection squares 4 corner positions
    Vector3 topLeft, topRight, bottomLeft, bottomRight;


    void Awake()
    { 
        #region Singleton
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        #endregion
    }

    void Start()
    {
        // Deactivate the square selection image
        ConstructionManager.instance.CloseConstructionPanel();
        selectionSquareTrans.gameObject.SetActive(false);
    }

    void Update()
    {
        // Select one or several units by clicking or draging the mouse
        SelectUnits();

        // Highlight by hovering with mouse above a unit which is not selected
        //HighlightUnit();
    }

    // Select units with click or by draging the mouse
    void SelectUnits()
    {
        // Are we clicking with left mouse or holding down left mouse
        bool isClicking = false;
        bool isHoldingDown = false;

        // Click the mouse button
        if (Input.GetMouseButtonDown(0))
        {
            clickTime = Time.time;

            // We dont yet know if we are drawing a square, but we need the first coordinate in case we do draw a square
            RaycastHit hit;
            // Fire ray from camera
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 2000f))
            {
                // The corner position of the square
                squareStartPos = hit.point;
            }
        }
        // Release the mouse button
        if (Input.GetMouseButtonUp(0))
        {
            if (Time.time - clickTime <= delay)
            {
                isClicking = true;
            }

            // Select all units within the square if we have created a square
            if (hasCreatedSquare)
            {
                hasCreatedSquare = false;

                // Deactivate the square selection image
                selectionSquareTrans.gameObject.SetActive(false);

                // Clear the list with selected unit
                for (int i = 0; i < selectedUnits.Count; i++)
                {
                    Troop troop = selectedUnits[i].GetComponent<Troop>();

                    if (troop != null)
                    {
                        troop.controlState = Troop.ControlState.NONE;
                        troop.HideSelected();
                    }
                }
                selectedUnits.Clear();

                // Select the units
                for (int i = 0; i < allUnits.Count; i++)
                {
                    GameObject currentUnit = allUnits[i];

                    // Is this unit within the square
                    if (IsWithinPolygon(currentUnit.transform.position))
                    {
                        selectedUnits.Add(currentUnit);
                        formationLeader = Random.Range(0, selectedUnits.Count);
                        Troop troop = currentUnit.GetComponent<Troop>();

                        if (troop != null)
                        {
                            troop.controlState = Troop.ControlState.SELECTED;
                            troop.ShowSelected();
                        }
                    }
                    // Otherwise deselect the unit if it's not in the square
                    else
                    {
                        Troop troop = currentUnit.GetComponent<Troop>();
                        if (troop != null)
                        {
                            troop.controlState = Troop.ControlState.NONE;
                            troop.HideSelected();
                        }
                    }
                }
            }
        }
        // Holding down the mouse button
        if (Input.GetMouseButton(0))
        {
            if (Time.time - clickTime > delay)
            {
                isHoldingDown = true;
            }
        }

        // Select one unit with left mouse and deselect all units with left mouse by clicking on what's not a unit
        if (isClicking)
        {
            // Deselect all units
            for (int i = 0; i < selectedUnits.Count; i++)
            {
                Troop troop = selectedUnits[i].GetComponent<Troop>();

                if (troop != null)
                {
                    troop.controlState = Troop.ControlState.NONE;
                    troop.HideSelected();
                }
            }

            // Clear the list with selected units
            selectedUnits.Clear();

            // Try to select a new unit
            RaycastHit hit;
            // Fire ray from camera
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 2000f))
            {
                PlaceBuilding building = hit.transform.gameObject.GetComponent<PlaceBuilding>();
                Troop troop = hit.transform.gameObject.GetComponent<Troop>();

                DeselectBuildings();

                // Did we hit a friendly unit?
                if (troop != null)
                {
                    DeselectBuildings();

                    GameObject activeUnit = hit.transform.gameObject;

                    // Set this unit to selected
                    troop.controlState = Troop.ControlState.SELECTED;
                    troop.ShowSelected();

                    // Add it to the list of selected units, which is now just 1 unit
                    selectedUnits.Add(activeUnit);
                }
                formationLeader = Random.Range(0, selectedUnits.Count);

                // Did we hit a friendly building? 
                if (building != null && building.buildingStatus == PlaceBuilding.BuildingStatus.Finished)
                {
                    DeselectBuildings();

                    GameObject activeBuilding = hit.transform.gameObject;

                    hit.transform.gameObject.GetComponent<PlaceBuilding>().ShowMenu();
                    hit.transform.gameObject.GetComponent<PlaceBuilding>().ShowSelection();
                }
            }
        }

        // Drag the mouse to select all units within the square
        if (isHoldingDown)
        {
            // Activate the square selection image
            if (!selectionSquareTrans.gameObject.activeInHierarchy)
            {
                selectionSquareTrans.gameObject.SetActive(true);
            }

            // Get the latest coordinate of the square
            squareEndPos = Input.mousePosition;

            // Display the selection with a GUI image
            DisplaySquare();

            // Highlight the units within the selection square, but don't select the units
            if (hasCreatedSquare)
            {
                DeselectBuildings();

                for (int i = 0; i < allUnits.Count; i++)
                {
                    GameObject currentUnit = allUnits[i];

                    Troop troop = currentUnit.GetComponent<Troop>();

                    // Is this unit within the square
                    if (IsWithinPolygon(currentUnit.transform.position))
                    {
                        if (troop != null)
                        {
                            troop.ShowSelected();
                        }
                    }
                    // Otherwise deactivate
                    else
                    {
                        if (troop != null)
                        {
                            troop.HideSelected();
                        }
                    }
                }
            }
        }
    }

    // Highlight a unit when mouse is above it
    void HighlightUnit()
    {
        // Change material on the latest unit it's highlighted
        if (highlightThisUnit != null)
        {
            // But make sure the unit we want to change material on is not selected
            bool isSelected = false;
            for (int i = 0; i < selectedUnits.Count; i++)
            {
                if (selectedUnits[i] == highlightThisUnit)
                {
                    isSelected = true;
                    break;
                }
            }

            if (!isSelected)
            {
                for (int i = 0; i < selectedUnits.Count; i++)
                {
                    Troop troop = selectedUnits[i].GetComponent<Troop>();

                    if (troop != null)
                    {
                        troop.HideSelected();
                    }
                }
            }

            highlightThisUnit = null;
        }

        // Fire a ray from the mouse position to get the unit we want to highlight
        RaycastHit hit;
        // Fire ray from camera
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 2000f))
        {
            // Did we hit a friendly unit?
            if (hit.transform.gameObject.layer == troopLayer)
            {
                // Get the object we hit
                GameObject currentObj = hit.transform.gameObject;

                // Highlight this unit if it's not selected
                bool isSelected = false;
                for (int i = 0; i < selectedUnits.Count; i++)
                {
                    if (selectedUnits[i] == currentObj)
                    {
                        isSelected = true;
                        break;
                    }
                }

                if (!isSelected)
                {
                    highlightThisUnit = currentObj;

                    for (int i = 0; i < selectedUnits.Count; i++)
                    {
                        Troop troop = selectedUnits[i].GetComponent<Troop>();

                        if (troop != null)
                        {
                            troop.ShowSelected();
                        }
                    }
                }
            }
        }
    }

    // Is a unit within a polygon determined by 4 corners
    bool IsWithinPolygon(Vector3 unitPos)
    {
        bool isWithinPolygon = false;

        // The polygon forms 2 triangles, so we need to check if a point is within any of the triangles
        // Triangle 1: TL - BL - TR
        if (IsWithinTriangle(unitPos, topLeft, bottomLeft, topRight))
        {
            return true;
        }

        // Triangle 2: TR - BL - BR
        if (IsWithinTriangle(unitPos, topRight, bottomLeft, bottomRight))
        {
            return true;
        }

        return isWithinPolygon;
    }

    // Is a point within a triangle
    bool IsWithinTriangle(Vector3 p, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        bool isWithinTriangle = false;

        // Need to set z -> y because of other coordinate system
        float denominator = ((p2.z - p3.z) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.z - p3.z));

        float a = ((p2.z - p3.z) * (p.x - p3.x) + (p3.x - p2.x) * (p.z - p3.z)) / denominator;
        float b = ((p3.z - p1.z) * (p.x - p3.x) + (p1.x - p3.x) * (p.z - p3.z)) / denominator;
        float c = 1 - a - b;

        // The point is within the triangle if 0 <= a <= 1 and 0 <= b <= 1 and 0 <= c <= 1
        if (a >= 0f && a <= 1f && b >= 0f && b <= 1f && c >= 0f && c <= 1f)
        {
            isWithinTriangle = true;
        }

        return isWithinTriangle;
    }

    // Display the selection with a GUI square
    void DisplaySquare()
    {
        // The start position of the square is in 3D space, or the first coordinate will move
        // as we move the camera which is not what we want
        Vector3 squareStartScreen = Camera.main.WorldToScreenPoint(squareStartPos);

        squareStartScreen.z = 0f;

        // Get the middle position of the square
        Vector3 middle = (squareStartScreen + squareEndPos) / 2f;

        // Set the middle position of the GUI square
        selectionSquareTrans.position = middle;

        // Change the size of the square
        float sizeX = Mathf.Abs(squareStartScreen.x - squareEndPos.x);
        float sizeY = Mathf.Abs(squareStartScreen.y - squareEndPos.y);

        // Set the size of the square
        selectionSquareTrans.sizeDelta = new Vector2(sizeX, sizeY);

        // The problem is that the corners in the 2d square is not the same as in 3d space
        // To get corners, we have to fire a ray from the screen
        // We have 2 of the corner positions, but we don't know which,  
        // so we can figure it out or fire 4 raycasts
        topLeft = new Vector3(middle.x - sizeX / 2f, middle.y + sizeY / 2f, 0f);
        topRight = new Vector3(middle.x + sizeX / 2f, middle.y + sizeY / 2f, 0f);
        bottomLeft = new Vector3(middle.x - sizeX / 2f, middle.y - sizeY / 2f, 0f);
        bottomRight = new Vector3(middle.x + sizeX / 2f, middle.y - sizeY / 2f, 0f);

        // From screen to world
        RaycastHit hit;
        int i = 0;
        // Fire ray from camera
        if (Physics.Raycast(Camera.main.ScreenPointToRay(topLeft), out hit, 2000f))
        {
            topLeft = hit.point;
            i++;
        }
        if (Physics.Raycast(Camera.main.ScreenPointToRay(topRight), out hit, 2000f))
        {
            topRight = hit.point;
            i++;
        }
        if (Physics.Raycast(Camera.main.ScreenPointToRay(bottomLeft), out hit, 2000f))
        {
            bottomLeft = hit.point;
            i++;
        }
        if (Physics.Raycast(Camera.main.ScreenPointToRay(bottomRight), out hit, 2000f))
        {
            bottomRight = hit.point;
            i++;
        }

        // Could we create a square?
        hasCreatedSquare = false;

        // We could find 4 points
        if (i == 4)
        {
            hasCreatedSquare = true;
        }
    }

    public void DeselectBuildings()
    {
        PlaceBuilding[] buildings = FindObjectsOfType<PlaceBuilding>();
        for (int i = 0; i < buildings.Length; i++)
        {
            buildings[i].HideSelection();
            buildings[i].HideMenu();
        }
    }
}
