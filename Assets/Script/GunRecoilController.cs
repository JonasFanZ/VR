using UnityEngine;
using UnityEngine.InputSystem;

public class GunRecoilController : MonoBehaviour
{
    [Header("後座力位移 (Position Kick)")]
    [Tooltip("開火瞬間槍身位移向量 (X: 左右晃動, Y: 槍身微抬, Z: 向後衝)")]
    [SerializeField] private Vector3 kickBackAmount = new Vector3(0f, 0.02f, -0.09f);

    [Header("槍口上揚旋轉 (Rotation Kick)")]
    [Tooltip("開火瞬間旋轉角度 (X: 槍口上跳, Y: 左右隨機微扭, Z: 槍身旋轉)")]
    [SerializeField] private Vector3 kickRotationAmount = new Vector3(-6f, 1f, 0f);

    [Header("平滑動力學參數 (Dynamics)")]
    [Tooltip("後座力爆發反應速度 (數值越大越乾脆)")]
    [SerializeField] private float snappiness = 25f;

    [Tooltip("槍枝回彈復位速度 (數值越小回彈越沉穩)")]
    [SerializeField] private float returnSpeed = 12f;

    // 初始位置與旋轉基準
    private Vector3 defaultLocalPos;
    private Quaternion defaultLocalRot;

    // 當前後座力累積量
    private Vector3 targetRecoilPos;
    private Vector3 targetRecoilRot;
    private Vector3 currentRecoilPos;
    private Vector3 currentRecoilRot;

    void Start()
    {
        // 記錄初始右下角位置與角度
        defaultLocalPos = transform.localPosition;
        defaultLocalRot = transform.localRotation;
    }

    void Update()
    {
        // 監聽滑鼠左鍵點擊 (New Input System)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // 只有游標鎖定時才觸發射擊，避免點擊編輯器視窗誤射
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                TriggerRecoil();
            }
        }

        // 1. 後座力累積值向 0 快速平滑衰減 (復位目標)
        targetRecoilPos = Vector3.Lerp(targetRecoilPos, Vector3.zero, returnSpeed * Time.deltaTime);
        targetRecoilRot = Vector3.Lerp(targetRecoilRot, Vector3.zero, returnSpeed * Time.deltaTime);

        // 2. 當前槍枝動態向目標狀態插值 (產生爆發衝擊感)
        currentRecoilPos = Vector3.Lerp(currentRecoilPos, targetRecoilPos, snappiness * Time.deltaTime);
        currentRecoilRot = Vector3.Lerp(currentRecoilRot, targetRecoilRot, snappiness * Time.deltaTime);

        // 3. 套用至 Transform
        transform.localPosition = defaultLocalPos + currentRecoilPos;
        transform.localRotation = defaultLocalRot * Quaternion.Euler(currentRecoilRot);
    }

    /// <summary>
    /// 觸發一次後座力衝擊
    /// </summary>
    public void TriggerRecoil()
    {
        // 槍身向後衝擊
        targetRecoilPos += kickBackAmount;

        // 加入一點點隨機左右偏移，模擬不完全對稱的後座力晃動
        float randomHorizontalKick = Random.Range(-0.5f, 0.5f);
        Vector3 randomizedRot = new Vector3(kickRotationAmount.x, kickRotationAmount.y * randomHorizontalKick, kickRotationAmount.z);
        
        targetRecoilRot += randomizedRot;
    }
}