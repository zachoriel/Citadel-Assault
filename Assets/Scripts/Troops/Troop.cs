using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Troop : MonoBehaviour
{
    public enum ControlState
    {
        NONE,
        SELECTED
    };
    public ControlState controlState;

    public Image selectionMarker;

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

    void Start()
    {
        controlState = ControlState.NONE;
        ToggleSelected(false);
    }

    public void ToggleSelected(bool isEnabled)
    {
        selectionMarker.enabled = isEnabled;
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
