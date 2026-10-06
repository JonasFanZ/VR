using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [Header("生成設定")]
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private float spawnInterval = 1.0f;   // 檢查生成間隔
    [SerializeField] private int maxConcurrentTargets = 4; // 同時存在的活靶數量

    [Header("隨機距離設定 (Random Radius)")]
    [Tooltip("標靶與玩家的最短距離")]
    [SerializeField] private float minSpawnRadius = 6f;

    [Tooltip("標靶與玩家的最遠距離")]
    [SerializeField] private float maxSpawnRadius = 14f;

    [Header("高度設定 (貼地)")]
    [Tooltip("標靶生成的垂直高度 (依全景照片路面高度微調)")]
    [SerializeField] private float groundHeight = -1.5f;

    private List<GameObject> activeTargets = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // 清理已銷毀的標靶參考
            activeTargets.RemoveAll(t => t == null);

            // 場上數量未達上限就補生
            if (activeTargets.Count < maxConcurrentTargets && targetPrefab != null)
            {
                SpawnGroundTarget();
            }
        }
    }

    private void SpawnGroundTarget()
    {
        // 1. 水平 360 度隨機方位角
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        // 2. 隨機距離 (在最小與最大半徑之間隨機)
        float currentRadius = Random.Range(minSpawnRadius, maxSpawnRadius);

        // 3. 換算水平位置
        float x = currentRadius * Mathf.Sin(angle);
        float z = currentRadius * Mathf.Cos(angle);

        // 4. 固定 Y 軸高度保持貼地
        Vector3 spawnPosition = new Vector3(x, groundHeight, z);

        // 5. 生成並加入追蹤清單
        GameObject newTarget = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
        activeTargets.Add(newTarget);
    }
}