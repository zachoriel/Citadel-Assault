using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TroopMovement : MonoBehaviour
{
    public static TroopMovement instance;
    SelectionManager selectionInstance;

    Vector3 worldPos;
    Vector3 posOffset;
    

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
        selectionInstance = SelectionManager.instance;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            GetClickWorldPosition();
            MoveToDestination(worldPos);
        }
    }

    void GetClickWorldPosition()
    {
        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            worldPos = hit.point;
        }
        else
        {
            worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        }
    }

    void MoveToDestination(Vector3 leadPosition)
    {
        posOffset = new Vector3(2f, 0f, 0f);
        
        for (int i = 0; i < selectionInstance.selectedUnits.Count; i++)
        {
            Troop troop = selectionInstance.selectedUnits[i].GetComponent<Troop>();

            troop.animator.SetBool("moving", true);

            if (i == selectionInstance.formationLeader)
            {
                troop.navAgent.SetDestination(leadPosition);
            }
            else
            {
                troop.navAgent.SetDestination(leadPosition + posOffset);
            }

            posOffset += new Vector3(2f, 0f, 0f);

            StartCoroutine(troop.CheckIfRouteCompleted());
        }
    }
}
