﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyPopulationCalculator : MonoBehaviour {
    public GameObject basicEnemyPrefab;

    public List<EnemySpawnData> getEnemyData(int level)
    {
        List<EnemySpawnData> result = new List<EnemySpawnData>();
        EnemySpawnData basicEnemySpawn = new EnemySpawnData(15, 17, basicEnemyPrefab);
        result.Add(basicEnemySpawn);

        return result;
    }
}
