using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionManager : MonoBehaviour
{
    public static ConstructionManager instance;

    public enum CurrentMode
    {
        SelectionMode,
        BuildMode
    };
    public CurrentMode currentMode;

    [Header("Construction UI")]
    public GameObject constructionPanel;

    [Header("Buildings")]
    public GameObject[] buildings;


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

    void Update()
    {
        BuildMenu();
    }

    void BuildMenu()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            switch (currentMode)
            {
                case CurrentMode.SelectionMode:
                    OpenConstructionPanel();
                    currentMode = CurrentMode.BuildMode;
                    break;
                case CurrentMode.BuildMode:
                    CloseConstructionPanel();
                    currentMode = CurrentMode.SelectionMode;
                    break;
            }
            SelectionManager.instance.DeselectBuildings();
        }
    }

    public void ConstructBuilding(int prefabIndex)
    {
        CloseConstructionPanel();
        currentMode = CurrentMode.SelectionMode;

        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector3 worldPos;

        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f))
        {
            worldPos = hit.point;
        }
        else
        {
            worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        }

        GameObject newBuilding = Instantiate(buildings[prefabIndex], worldPos, Quaternion.identity);
        newBuilding.transform.position = new Vector3(newBuilding.transform.position.x, 2.6f, newBuilding.transform.position.z);
    }

    void OpenConstructionPanel()
    {
        constructionPanel.SetActive(true);
    }

    public void CloseConstructionPanel()
    {
        constructionPanel.SetActive(false);
    }
}
