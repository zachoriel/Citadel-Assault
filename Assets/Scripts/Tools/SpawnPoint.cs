using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public bool canSpawnHere;
    public LayerMask troopLayer;
    Collider[] troops;

    void Update()
    {
        canSpawnHere = CheckForTroop();
    }

    bool CheckForTroop()
    {
        troops = Physics.OverlapSphere(transform.position, 0.5f, troopLayer);

        if (troops.Length != 0)
        {
            return false;
        }

        return true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
