using UnityEngine;
using System.Collections;

public class EnemySpawner0 : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public float[] spawnWeights = { 6f, 3f, 1f }; // A:60% B:30% C:10%
    public Transform[] spawnPoints;  // 인스펙터에서 spawnPoint_0~4 연결
    public float spawnInterval = 1f;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    GameObject GetWeightedRandom()
    {
        float total = 0f;
        for (int i = 0; i < enemyPrefabs.Length; i++)
            total += (i < spawnWeights.Length ? spawnWeights[i] : 1f);

        float rand = Random.Range(0f, total);
        float cumulative = 0f;
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            cumulative += (i < spawnWeights.Length ? spawnWeights[i] : 1f);
            if (rand < cumulative) return enemyPrefabs[i];
        }
        return enemyPrefabs[enemyPrefabs.Length - 1];
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(GetWeightedRandom(), point.position, Quaternion.identity);
        }
    }

    void OnDrawGizmos()
    {
        if (spawnPoints == null) return;
        foreach (Transform point in spawnPoints)
        {
            if (point == null) continue;
            DrawArrow.ForGizmo(point.position, Vector3.down * 10f, Color.red);
        }
    }
}
