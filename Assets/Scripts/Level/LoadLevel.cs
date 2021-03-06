﻿using UnityEngine;
using System.Collections.Generic;
using SpriteTile;

public class LoadLevel : MonoBehaviour {
    // This is a list (and not a set) because it is called a bunch of times in the pathfinding code, and HashSet.Contains
    // is allocating memory somewhere and creating garbage.
    public static List<int> FloorIndices;
    public static bool IsFirstLoad = true;

    public static bool TestingCityLevel = false;

    public static LoadLevel instance = null;

    void Awake ()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        if (IsFirstLoad)
        {
            InitLevel();
        }
    }

    void Start()
    {
        IsFirstLoad = false;
    }

    void OnLevelWasLoaded(int index)
    {
        if (!IsFirstLoad)
        {
            InitLevel();
        }
    }

    void InitLevel()
    {
        GameState.Enemies.Clear();
        Vector3 playerSpawn;
        GameObject player = GameObject.Find("Soldier");
        GameObject tileCam = GameObject.Find("TileCamera");

        Tile.SetCamera(tileCam.GetComponent<Camera>());
        Player.PlayerMovement.SetFollowCamera(tileCam);

        if (TestingCityLevel)
        {
            FloorIndices = new List<int>() { CityGridCreator.DefaultWalkableIndex, CityGridCreator.GridArrayRoadIndex };
            AStar.Init(new CityGenerator().GenerateLevel(GameState.LevelIndex, out playerSpawn));
        } else
        {
            FloorIndices = new List<int>() { BasicLevelDecorator.BaseDark, BasicLevelDecorator.BaseLight };
            AStar.Init(new BasicLevelGenerator().GenerateLevel(GameState.LevelIndex, out playerSpawn));
            Tile.SetColliderLayer(GameSettings.WallLayerNumber, BasicLevelDecorator.CliffTileLayer);
            Tile.SetColliderLayer(GameSettings.WaterLayer, BasicLevelDecorator.WaterTileLayer);
        }

        player.transform.position = playerSpawn;
    }

    private bool hasAdjacentFloor(int[,] level, int x, int y)
    {
        return (x < level.GetLength(0) - 1 && FloorIndices.Contains(level[x + 1, y]))
            || (x > 0 && FloorIndices.Contains(level[x - 1, y]))
            || (y < level.GetLength(1) - 1 && FloorIndices.Contains(level[x, y + 1]))
            || (y > 0 && FloorIndices.Contains(level[x, y - 1]));
    }

    // Debug code
    void OnDrawGizmos()
    {
        return;
        //foreach (PerimeterRect r in PerimeterRects)
        //{
        //    foreach (CityGenerator.PerimeterPoint p in r.points)
        //    {
        //        Gizmos.color = Color.blue;
        //        Gizmos.DrawCube(new Vector2(p.x, p.y), new Vector3(1, 1, 1));
        //    }
        //}
    }
}
