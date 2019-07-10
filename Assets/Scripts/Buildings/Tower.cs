using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    [Header("Building UI")]
    public GameObject progressBarBG;
    public Image progressBar;


    void Start()
    {
        progressBarBG.SetActive(false);
    }
}
