﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpriteTile;
using System.Linq;

public class NearbyEnemyPositionAssigner : MonoBehaviour {

    public float timeBetweenReassignments = .5f;
    // TODO: Tweak.
    public float meleeEnemyAssignmentRadius = 5f;
    public float attackOffsetHorizontalVertical = .5f;
    public float attackOffsetDiagonal = .35f;

    private List<MeleeEnemyAI> nearbyMeleeEnemies = new List<MeleeEnemyAI>();
    private List<Vector2> claimableTokens;
    private List<Vector2> unclaimableTokens = new List<Vector2>();
    private NearbyMeleeEnemyComparer comparer;
    private Vector3 lastPosition;
    private HashSet<MeleeEnemyAI> lastEnemies = new HashSet<MeleeEnemyAI>();

    // Temporary hack.
    private Vector2 gordoSize = new Vector2(.6f, .6f);

    private float lastAssignmentTime;

    void Awake()
    {
        lastPosition = transform.position;
        comparer = new NearbyMeleeEnemyComparer();
        claimableTokens = new List<Vector2> { 
            new Vector2(0, attackOffsetHorizontalVertical), // Top
            new Vector2(0, -attackOffsetHorizontalVertical), // Bottom
            new Vector2(-attackOffsetHorizontalVertical, 0), // Left
            new Vector2(attackOffsetHorizontalVertical, 0), // Right
            new Vector2(-attackOffsetDiagonal, attackOffsetDiagonal), // Top Left
            new Vector2(attackOffsetDiagonal, attackOffsetDiagonal), // Top Right
            new Vector2(-attackOffsetDiagonal, -attackOffsetDiagonal), // Bottom Left
            new Vector2(attackOffsetDiagonal, -attackOffsetDiagonal) // Bottom Right
        };
    }

    void Update()
    {
        if (Time.time > lastAssignmentTime + timeBetweenReassignments)
        {
            nearbyMeleeEnemies.Clear();
            lastAssignmentTime = Time.time;

            nearbyMeleeEnemies = MeleeEnemyAI.meleeEnemies.Where(
                enemy => Vector3.SqrMagnitude(transform.position - enemy.transform.position) < enemy.squaredAttackDistance).ToList();

            if (nearbyMeleeEnemies.Count == 0 || (lastPosition.Equals(transform.position) && lastEnemies.SetEquals(nearbyMeleeEnemies)))
            {
                return;
            }

            comparer.playerPosition = transform.position;
            nearbyMeleeEnemies.Sort(comparer);

            InvalidateTokens();
            AssignTokens();
            ResetTokens();

            lastPosition = transform.position;
            lastEnemies = new HashSet<MeleeEnemyAI>(nearbyMeleeEnemies);
        }
    }

    void ResetTokens()
    {
        claimableTokens.AddRange(unclaimableTokens);
        unclaimableTokens.Clear();
    }
    
    void InvalidateTokens()
    {
        for (int x = 0; x < claimableTokens.Count;)
        {
            if (Physics2D.BoxCast(tokenToWorldSpace(claimableTokens[x]), gordoSize, 0f, Vector2.zero, 0f, LayerMasks.WallLayerMask).transform != null)
            {
                unclaimableTokens.Add(claimableTokens[x]);
                claimableTokens.RemoveAt(x);
            }
            else
            {
                x++;
            }
        }
    }

    void AssignTokens()
    {
        for (int x = 0; x < nearbyMeleeEnemies.Count; x++)
        {
            MeleeEnemyAI enemy = nearbyMeleeEnemies[x];

            if (claimableTokens.Count == 0)
            {
                enemy.targetIsAssigned = false;
                enemy.shouldWait = true;
                continue;
            }

            Vector2 minToken = claimableTokens[0];
            Vector2 minTarget = tokenToWorldSpace(minToken);
            int indexOfMin = 0;
            float minSqrDistance = getSqrDistanceFromPotentialPositionToEnemy(minToken, enemy.transform.position);
            
            // Nested loop shouldn't hurt performance since claimableTokens has a fixed max size of 8.
            for (int n = 1; n < claimableTokens.Count; n++)
            {
                float currentSqrDistance = getSqrDistanceFromPotentialPositionToEnemy(claimableTokens[n], enemy.transform.position);
                if (currentSqrDistance < minSqrDistance)
                {
                    minSqrDistance = currentSqrDistance;
                    minToken = claimableTokens[n];
                    minTarget = tokenToWorldSpace(minToken);
                    indexOfMin = n;
                }
            }

            enemy.targetIsAssigned = true;
            enemy.target = minTarget;
            enemy.token = minToken;
            unclaimableTokens.Add(claimableTokens[indexOfMin]);
            claimableTokens.RemoveAt(indexOfMin); // might be able to just use Remove instead of RemoveAt.
        }
    }

    Vector2 tokenToWorldSpace(Vector2 token)
    {
        return new Vector2(transform.position.x + token.x, transform.position.y + token.y);
    }

    float getSqrDistanceFromPotentialPositionToEnemy(Vector2 potentialPosition, Vector3 enemyPosition)
    {
        float a = transform.position.x + potentialPosition.x - enemyPosition.x;
        float b = transform.position.y + potentialPosition.y - enemyPosition.y;
        return a * a + b * b;
    }

    private class NearbyMeleeEnemyComparer : IComparer<MeleeEnemyAI>
    {
        public Vector3 playerPosition;

        public int Compare(MeleeEnemyAI enemy1, MeleeEnemyAI enemy2)
        {
            float sqrDistance1 = (playerPosition - enemy1.transform.position).sqrMagnitude;
            float sqrDistance2 = (playerPosition - enemy2.transform.position).sqrMagnitude;
            return sqrDistance1.CompareTo(sqrDistance2);
        }
    }
}
