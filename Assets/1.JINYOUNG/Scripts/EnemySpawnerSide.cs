using UnityEngine;
using System.Collections;

public class EnemySpawnerSide : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public float[] spawnWeights = { 6f, 3f, 1f }; // A:60% B:30% C:10%
    public Transform startPoint;
    public Transform endPoint;
    public float spawnInterval = 40f;

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

            GameObject enemy = Instantiate(GetWeightedRandom(), startPoint.position, Quaternion.identity);

            Vector3 dir = (endPoint.position - startPoint.position).normalized;
            enemy.GetComponent<Enemy>().moveDirection = dir;
        }
    }

    void OnDrawGizmos()
    {
        if (startPoint == null || endPoint == null) return;
        DrawArrow.ForGizmo(startPoint.position, (endPoint.position - startPoint.position).normalized * 8f, Color.cyan, false, 1f);
    }
}
