using UnityEngine;
using System.Collections;
using System;

public class SpawnData {

    public int min;
    public int max;
    public GameObject prefab;
    public bool isEnemy; // Probably should use an enum instead of 2 bools.
    public bool isChest;

    public SpawnData(int min, int max, GameObject prefab, bool isEnemy = true, bool isChest = false)
    {
        this.min = min;
        this.max = max;
        this.prefab = prefab;
        this.isEnemy = isEnemy;
        this.isChest = isChest;
    }
}
