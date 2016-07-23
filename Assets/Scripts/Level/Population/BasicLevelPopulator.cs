using UnityEngine;
using System.Collections.Generic;

public class BasicLevelPopulator
{
    private static int MinimumGridDistanceFromPlayer = 5;
    private static int MinimumChestDistance = 10;

    private List<Vector2> chestPositions = new List<Vector2>();

    public void spawnEntities(List<SpawnData> spawnData, List<Vector2> potentialSpawnPositions,
        Vector2 playerSpawn)
    {
        GameState.LockOnTargets.Clear();
        chestPositions.Clear();

        // This is no longer semantically correct, since the populator can now lay traps in addition
        // to enemies. Consider renaming, or creating a separate container for traps.
        Transform enemyContainer = GameObject.Find("Enemies").transform;
        Vector2 playerPosition = AStar.positionToArrayIndicesVector(playerSpawn);

        int totalNumEnemiesPlaced = 0;
        foreach (SpawnData spawnDatum in spawnData)
        {
            int numEntitiesOfTypePlaced = 0;
            int count = Random.Range(spawnDatum.min, spawnDatum.max);
            GameObject enemyPrefab = spawnDatum.prefab;

            while (numEntitiesOfTypePlaced < count && potentialSpawnPositions.Count > 0)
            {
                int index = Random.Range(0, potentialSpawnPositions.Count);
                Vector2 spawnPosition = potentialSpawnPositions[index];
                potentialSpawnPositions.RemoveAt(index);

                if (farEnoughFromPlayer(spawnPosition, playerPosition))
                {
                    GameObject obj = MonoBehaviour.Instantiate(enemyPrefab, new Vector3(
                        spawnPosition.x * GameSettings.TileSize, spawnPosition.y * GameSettings.TileSize, 0),
                        Quaternion.identity) as GameObject;
                    obj.transform.SetParent(enemyContainer);

                    if (spawnDatum.isEnemy)
                    {
                        totalNumEnemiesPlaced++;
                        GameState.LockOnTargets.Add(obj.GetComponent<LockOnIndicator>());
                        GameState.Enemies.Add(obj.GetComponent<EnemyAI>());
                    }
                    else if (spawnDatum.isChest)
                    {
                        chestPositions.Add(spawnPosition);
                    }

                    numEntitiesOfTypePlaced++;
                    continue;
                }
            }

            if (numEntitiesOfTypePlaced < count)
            {
                Debug.Log("Could not place all " + count + " entities - ran out of valid positions. Placed "
                    + numEntitiesOfTypePlaced + " entities");
            }
        }

        GameState.NumEnemiesRemaining = totalNumEnemiesPlaced;
    }

    bool canPlace(Vector2 spawnPosition, Vector2 playerPosition, SpawnData spawnDatum)
    {
        if (spawnDatum.isChest)
        {
            return farEnoughFromPlayer(spawnPosition, playerPosition) &&
                farEnoughFromOtherChests(spawnPosition, chestPositions);
        }

        return farEnoughFromPlayer(spawnPosition, playerPosition);
    }

    bool farEnoughFromPlayer(Vector2 enemyPosition, Vector2 playerPosition)
    {
        return (Mathf.Abs(playerPosition.x - enemyPosition.x)
            + Mathf.Abs(playerPosition.y - enemyPosition.y)) > MinimumGridDistanceFromPlayer;
    }

    bool farEnoughFromOtherChests(Vector2 chestPosition, List<Vector2> otherChestPositions)
    {
        for (int i = 0; i < otherChestPositions.Count; i++)
        {
            if (manhattanDistance(chestPosition, otherChestPositions[i]) < MinimumChestDistance)
            {
                return false;
            }
        }

        return true;
    }

    int manhattanDistance(Vector2 obj1, Vector2 obj2)
    {
        return (int)(Mathf.Abs(obj1.x - obj2.x) + Mathf.Abs(obj1.y - obj2.y));
    }
}
