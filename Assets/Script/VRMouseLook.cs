using UnityEngine;
using UnityEngine.InputSystem;

public class VRMouseLook : MonoBehaviour
{
    [Header("視角靈敏度 (Sensitivity)")]
    [SerializeField] private float mouseSensitivity = 0.15f; // 新版 Input System 的 Delta 數值較大，0.1~0.3 為合適區間

    [Header("上下仰角限制 (Clamp Pitch)")]
    [SerializeField] private float minPitch = -85f;
    [SerializeField] private float maxPitch = 85f;

    private float pitch = 0f;
    private float yaw = 0f;

    void Start()
    {
        // 進入遊戲自動鎖定並隱藏游標
        LockCursor();

        // 讀取當前攝影機初始角度，避免開場視角瞬間歸零
        Vector3 rot = transform.localEulerAngles;
        pitch = rot.x > 180f ? rot.x - 360f : rot.x;
        yaw = rot.y;
    }

    void Update()
    {
        // 若偵測不到滑鼠硬體直接返回
        if (Mouse.current == null) return;

        // 按 Esc 釋放滑鼠游標
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            UnlockCursor();
        }

        // 游標釋放後，點擊畫面左鍵重新鎖定
        if (Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
        {
            LockCursor();
        }

        // 僅在游標鎖定狀態下處理旋轉
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            // 讀取滑鼠每幀位移增量 (Delta)
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            float mouseX = mouseDelta.x * mouseSensitivity;
            float mouseY = mouseDelta.y * mouseSensitivity;

            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            // 直接將歐拉角套用至攝影機
            transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}