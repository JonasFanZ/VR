using UnityEngine;

public class ShootingTarget : MonoBehaviour
{
    [Header("標靶設定")]
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private float lifeTime = 5f; // 若太久沒打到，幾秒後自動消失
    [SerializeField] private GameObject hitEffectPrefab; // 可選：擊中特效

    private Transform playerTransform;

    void Start()
    {
        // 抓取場景中的攝影機或玩家位置，讓標靶正面面向玩家
        if (Camera.main != null)
        {
            playerTransform = Camera.main.transform;
            transform.LookAt(playerTransform);
        }

        // 超時自動銷毀
        Destroy(gameObject, lifeTime);
    }

    // 支援通用受傷介面呼叫
    public void TakeDamage(float damage)
    {
        OnHit();
    }

    // 被子彈射中時觸發
    public void OnHit()
    {
        // 同一標靶若同時收到多個命中回呼，只計分及銷毀一次。
        if (hasBeenHit) return;
        hasBeenHit = true;

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
        TargetScoreManager.Instance?.AddScore(scoreValue);
        Debug.Log($"擊中標靶！得分: +{scoreValue}");

        // 通知生成器目前場上靶子數量減少
        //TargetSpawner.OnTargetDestroyed?.Invoke();

        Destroy(gameObject);
    }

    private bool hasBeenHit;
}
