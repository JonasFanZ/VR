using UnityEngine;
using TMPro; // 使用 TextMeshPro (推薦)
using UnityEngine.UI; // 若使用傳統 Text 也相容

public class TargetScoreManager : MonoBehaviour
{
    // 單例模式 (Singleton)，讓標靶隨時都能直接呼叫加分
    public static TargetScoreManager Instance;

    [Header("UI 文字綁定 (二選一指定即可)")]
    [SerializeField] private TextMeshProUGUI tmpDisplay; // 若使用 TextMeshPro
    [SerializeField] private Text legacyTextDisplay;     // 若使用一般 Legacy Text

    private int score = 0;
    private float elapsedTime = 0f;
    private bool isRunning = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // 1. 計算時間累加
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateDisplayUI();
        }

        // 2. 監聽 G 鍵重置
        if (Input.GetKeyDown(KeyCode.G))
        {
            ResetGame();
        }
    }

    // 擊中標靶時呼叫 (+1 分)
    public void AddScore(int points = 1)
    {
        score += points;
        UpdateDisplayUI();
    }

    // 重設時間與分數
    public void ResetGame()
    {
        score = 0;
        elapsedTime = 0f;
        UpdateDisplayUI();
        Debug.Log("Reset！");
    }

    // 更新畫面文字顯示
    private void UpdateDisplayUI()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);

        // 格式化為 00:00 樣式
        string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);
        string content = $"Time: {timeString}\nScore: {score}";

        if (tmpDisplay != null)
        {
            tmpDisplay.text = content;
        }
        else if (legacyTextDisplay != null)
        {
            legacyTextDisplay.text = content;
        }
    }
}