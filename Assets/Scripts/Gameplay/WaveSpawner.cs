using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WaveSpawner : MonoBehaviour
{

    [Header("Fog Walls")]
    public FogWall[] fogWalls; // assign the 4 walls on the road piece in the inspector

    [Header("Enemy Prefabs (3 Types)")]
    public GameObject[] enemyPrefabs; // Assign your 3 enemy prefabs here

    [Header("Spawn Settings")]
    public int minEnemies = 3;
    public int maxEnemies = 8;
    

    private List<GameObject> currentEnemies = new List<GameObject>();
    private bool waveSpawned = false;
    private bool waveComplete = false;
    private BoxCollider spawnArea;

    void Awake()
    {
        spawnArea = GetComponent<BoxCollider>();
        if (!spawnArea.isTrigger)
            spawnArea.isTrigger = true; // Make sure collider is a trigger
    }

    void OnTriggerEnter(Collider other)
    {
        if (!waveSpawned && other.CompareTag("Player"))
        {
            SpawnWave();
        }
    }

    void SpawnWave()
    {
        int enemyCount = Random.Range(minEnemies, maxEnemies + 1);

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Vector3 spawnPos = GetRandomPointInBox(spawnArea);
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            currentEnemies.Add(newEnemy);
        }

        waveSpawned = true;
        Debug.Log("Wave spawned!");
    }

    Vector3 GetRandomPointInBox(BoxCollider box)
    {
        Vector3 center = box.center + box.transform.position;
        Vector3 size = box.size;

        float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);
        float y = center.y; // Keep Y fixed or adjust for ground height

        return new Vector3(x, y, z);
    }

    void Update()
    {
        if (waveSpawned && !waveComplete)
        {
            currentEnemies.RemoveAll(enemy => enemy == null);

            if (currentEnemies.Count == 0)
            {
                waveComplete = true;
                Debug.Log("Wave Complete!");

                // Trigger fog walls to fall
                foreach (FogWall wall in fogWalls)
                {
                    wall.TryFall();
                }
            }
        }
    }
}
