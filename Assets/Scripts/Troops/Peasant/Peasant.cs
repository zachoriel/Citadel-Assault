using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Peasant : MonoBehaviour
{
    public enum ControlState
    {
        NONE,
        SELECTED
    };
    public ControlState controlState;

    public Image selectionMarker;

    // Use this for initialization
    void Start ()
    {
        controlState = ControlState.NONE;
        HideSelected();
    }

    public void ShowSelected()
    {
        selectionMarker.enabled = true;
    }

    public void HideSelected()
    {
        selectionMarker.enabled = false;
    }
}
