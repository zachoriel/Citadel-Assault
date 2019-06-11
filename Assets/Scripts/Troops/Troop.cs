using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Troop : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public Animator animator;


    void Awake()
    {
        if (navAgent == null)
        {
            navAgent = GetComponent<NavMeshAgent>();
        }
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public IEnumerator CheckIfRouteCompleted()
    {
        while (true)
        {
            if (!navAgent.pathPending)
            {
                if (navAgent.remainingDistance <= navAgent.stoppingDistance)
                {
                    if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f)
                    {
                        animator.SetBool("moving", false);
                        yield break;
                    }
                }
            }
            yield return null;
        }
    }
}
