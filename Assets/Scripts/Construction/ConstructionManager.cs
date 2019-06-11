using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionManager : MonoBehaviour
{
    public static ConstructionManager instance;

    [Header("Construction UI")]
    public GameObject constructionPanel;

    [Header("Buildings")]
    public GameObject barracks;
    public GameObject armory;
    public GameObject forge;
    public GameObject arcanaeum;
    public GameObject throne;
    public GameObject tower;


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
            switch (constructionPanel.activeInHierarchy)
            {
                case false:
                    constructionPanel.SetActive(true);
                    break;
                case true:
                    constructionPanel.SetActive(false);
                    break;
            }
            SelectionManager.instance.DeselectBuildings();
        }
    }

    public void ConstructBarracks()
    {
        constructionPanel.SetActive(false);

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

        GameObject newBarracks = Instantiate(barracks, worldPos, Quaternion.identity);
        newBarracks.gameObject.name = "Barracks";
        newBarracks.transform.position = new Vector3(newBarracks.transform.position.x, 2.6f, newBarracks.transform.position.z);
    }

    public void ConstructArmory()
    {
        constructionPanel.SetActive(false);

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

        GameObject newArmory = Instantiate(armory, worldPos, Quaternion.identity);
        newArmory.gameObject.name = "Armory";
        newArmory.transform.position = new Vector3(newArmory.transform.position.x, 2.6f, newArmory.transform.position.z);
    }

    public void ConstructForge()
    {
        constructionPanel.SetActive(false);

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

        GameObject newForge = Instantiate(forge, worldPos, Quaternion.identity);
        newForge.gameObject.name = "Forge";
        newForge.transform.position = new Vector3(newForge.transform.position.x, 2.6f, newForge.transform.position.z);
    }

    public void ConstructArcanaeum()
    {
        constructionPanel.SetActive(false);

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

        GameObject newArcanaeum = Instantiate(arcanaeum, worldPos, Quaternion.identity);
        newArcanaeum.gameObject.name = "Arcanaeum";
        newArcanaeum.transform.position = new Vector3(newArcanaeum.transform.position.x, 2.6f, newArcanaeum.transform.position.z);
    }

    public void ConstructThrone()
    {
        constructionPanel.SetActive(false);

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

        GameObject newThrone = Instantiate(throne, worldPos, Quaternion.identity);
        newThrone.gameObject.name = "Throne";
        newThrone.transform.position = new Vector3(newThrone.transform.position.x, 2.6f, newThrone.transform.position.z);
    }

    public void ConstructTower()
    {
        constructionPanel.SetActive(false);

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

        GameObject newTower = Instantiate(tower, worldPos, Quaternion.identity);
        newTower.gameObject.name = "Tower";
        newTower.transform.position = new Vector3(newTower.transform.position.x, 2.6f, newTower.transform.position.z);
    }

    public void CloseConstructionPanel()
    {
        constructionPanel.SetActive(false);
    }
}
