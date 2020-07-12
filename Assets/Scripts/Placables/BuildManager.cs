﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuildManager : MonoBehaviour
{

    [SerializeField]
    private Buildable buildPreset;

    [SerializeField]
    private BuildOrder buildOrder;

    [SerializeField]
    private int maxAvailableWorkers = 15;

    [SerializeField]
    private int availableWorkers = 15;

    private List<BuildOrder> buildQueue = new List<BuildOrder>();

    public void RequestBuilding(Tile tile)
    {
        BuildOrder order = Instantiate(buildOrder);
        order.transform.SetParent(tile.transform);
        order.transform.localPosition = new Vector3(0,0,0);

        order.Init(buildPreset.WorkersNeeded, buildPreset.BuildTime);
        order.OnBuildingFinished += () => 
        {
            tile.PlaceObject(buildPreset.BuildingPreset, tile.transform.position);
            availableWorkers += buildPreset.WorkersNeeded;
            buildQueue.Remove(order);
            Destroy(order);
        };

        tile.IsOcupied = true;
        buildQueue.Add(order);
    }

    private void Start()
    {
        StartCoroutine(BuildQueueCoroutine());
    }

    private IEnumerator BuildQueueCoroutine()
    {
        while (true)
        {
            for (int i = 0; i < buildQueue.Count; i++)
            {
                if (buildQueue[i].WorkersNeeded <= availableWorkers)
                {
                    availableWorkers -= buildQueue[i].WorkersNeeded;
                    buildQueue[i].AddWorkers(buildQueue[i].WorkersNeeded);
                    yield return new WaitForSeconds(0.3f);
                }
            }
            yield return new WaitForSeconds(1);
        }
    }

    public void ChangeBuildPreset(Buildable preset)
    {
        buildPreset = preset;
    }
}
