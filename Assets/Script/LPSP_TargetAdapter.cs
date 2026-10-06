using UnityEngine;

public class LPSP_TargetAdapter : MonoBehaviour
{
    [SerializeField] private float autoDestroyTime = 12f;

    private Transform playerCamera;
    private bool isScored = false;
    private TargetScript targetScript;

    void Awake()
    {
        // LPSP 的彈丸直接切換子物件 TargetScript.isHit；在父物件偵測
        // 這個狀態，便能和套件原本的倒下動畫同步計分。
        targetScript = GetComponentInChildren<TargetScript>();
    }

    void Start()
    {
        if (Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }

        // 超時自我銷毀
        Destroy(gameObject, autoDestroyTime);
    }

    void Update()
    {
        if (targetScript != null && targetScript.isHit)
        {
            ScoreAndDestroy();
        }

        // 保持面向玩家
        if (playerCamera != null)
        {
            Vector3 dir = playerCamera.position - transform.position;
            dir.y = 0;
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(-dir);
            }
        }
    }

    // 1. 支援套件子彈常用的 SendMessage 方式
    public void TakeDamage(float damage)
    {
        ScoreAndDestroy();
    }

    public void Damage(float damage)
    {
        ScoreAndDestroy();
    }

    // 2. 支援實體子彈（帶 Rigidbody/Collider）的碰撞檢測。
    // 套件子彈透過 TargetScript.isHit 命中，已在 Update 統一處理，
    // 因此這裡不以物件名稱猜測，避免把一般碰撞誤判成命中。
    private void OnCollisionEnter(Collision collision)
    {
        // 命中回呼由套件 TargetScript 狀態或 TakeDamage/Damage 處理。
    }

    // 3. 支援 Trigger 穿透式子彈
    private void OnTriggerEnter(Collider other)
    {
        // 命中回呼由套件 TargetScript 狀態或 TakeDamage/Damage 處理。
    }

    // 核心加分與銷毀
    public void ScoreAndDestroy()
    {
        if (isScored) return;
        isScored = true;

        // 加分
        if (TargetScoreManager.Instance != null)
        {
            TargetScoreManager.Instance.AddScore(1);
        }

        // 倒下後銷毀
        Destroy(gameObject, 1.2f);
    }
}
