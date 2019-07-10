using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barracks : MonoBehaviour
{
    [Header("Troop Types")]
    public GameObject[] troops;

    [Header("Troop Train Times")]
    [SerializeField] float[] trainingTimes;

    [Header("Troop Upgrade Times")]
    [SerializeField] float[] upgradeTimes;

    [Header("Building UI")]
    public GameObject progressBarBG;
    public Image progressBar;

    [Header("Spawn Points")]
    public SpawnPoint[] spawnPoints;

    bool trainingTroop, upgradingTroop;


    void Start()
    {
        progressBarBG.SetActive(false);
    }

    public void TrainTroop(int troopNum)
    {
        if (!trainingTroop && !upgradingTroop)
        {
            StartCoroutine(TrainTroop(troops[troopNum], troopNum));
        }
    }

    public void UpgradeTroop(int troopNum)
    {
        if (!upgradingTroop && !trainingTroop)
        {
            StartCoroutine(UpgradeTroop(troops[troopNum], troopNum));
        }
    }

    IEnumerator TrainTroop(GameObject troopPrefab, int troopNum)
    {
        trainingTroop = true;
        progressBarBG.SetActive(true);

        for (float time = 0.00f; time < trainingTimes[troopNum]; time += 0.01f)
        {
            progressBar.fillAmount = time / trainingTimes[troopNum];
            yield return null;
        }

        progressBarBG.SetActive(false);

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (!spawnPoints[i].canSpawnHere)
            {
                continue;
            }
            else if (spawnPoints[i].canSpawnHere)
            {
                GameObject Troop = Instantiate(troopPrefab, spawnPoints[i].transform.position, Quaternion.identity);
                SelectionManager.instance.allUnits.Add(Troop);
                break;
            }
        }

        trainingTroop = false;
        StopAllCoroutines();
    }

    IEnumerator UpgradeTroop(GameObject troopPrefab, int troopNum)
    {
        upgradingTroop = true;
        progressBarBG.SetActive(true);

        for (float time = 0.00f; time < upgradeTimes[troopNum]; time += 0.01f)
        {
            progressBar.fillAmount = time / upgradeTimes[troopNum];
            yield return null;
        }

        progressBarBG.SetActive(false);

        // TODO: Upgrade troop

        upgradingTroop = false;
        StopAllCoroutines();
    }
}
