using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Time Settings")]
    public float timeLimit = 300f; // 5分
    private float currentTime;
    private bool isTimerRunning = false;
    private bool hasStarted = false; // 最初の操作待ち

    [Header("Score Data")]
    public int coinsCollected = 0;
    public bool mission1Cleared = false;
    public bool mission2Cleared = false;
    private int totalScore = 0;

    [Header("UI Reference")]
    public TextMeshProUGUI timeText;
    //    public TextMeshProUGUI scoreText;

    [Header("Result UI References")]
    public TextMeshProUGUI resultTimeText;
    public TextMeshProUGUI resultCoinText;
    public GameObject mission1Check; // チェックマークのImageオブジェクト
    public GameObject mission2Check;

    [Header("Mission Stats")]
    public int blocksBroken = 0; // 追加：壊したブロック数

    [Header("Bounce Mission")]
    public int currentBounceCount = 0;

    void Awake() {
        if (Instance == null)
        {
            Instance = this;
            // もしStageManagerの子要素でないなら、これもDontDestroyにする必要があります
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); // 重複した方を消す
            return;
        }
    }

    void Start()
    {
        currentTime = timeLimit;
        UpdateUI();
    }

    // プレイヤーがドラッグを開始した時に呼ぶ
    public void StartGame()
    {
        if (!hasStarted)
        {
            isTimerRunning = true;
            hasStarted = true;
        }
    }

    void Update()
    {
        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                TimeUp();
            }
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }

    void TimeUp()
    {
        isTimerRunning = false;
        // StageManagerにリロードを頼む（現在のステージをやり直し）
        StageManager.Instance.SelectStage(StageManager.Instance.GetCurrentIndex());
    }

    public int CalculateFinalScore()
    {
        isTimerRunning = false;
        // 1. 残り時間
        int timeScore = Mathf.FloorToInt(currentTime);
        // 2. コイン (1枚50点)
        int coinScore = coinsCollected * 50;
        // 3. お題 (1つ50点)
        int missionScore = 0;
        if (mission1Cleared) missionScore += 50;
        if (mission2Cleared) missionScore += 50;

        totalScore = timeScore + coinScore + missionScore;
        return totalScore;
    }

    // データを初期化する関数
    public void ResetScore()
    {
        currentTime = timeLimit;
        coinsCollected = 0;
        blocksBroken = 0;
        currentBounceCount = 0;
        mission1Cleared = false;
        mission2Cleared = false;
        isTimerRunning = false;
        hasStarted = false;
        UpdateUI();
        Debug.Log("ScoreManagerをリセットしました");
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 参照が切れていたら探し直す
        if (timeText == null)
        {
            GameObject tObj = GameObject.Find("TimeText");
            if (tObj != null) timeText = tObj.GetComponent<TextMeshProUGUI>();
        }
        ResetScore();
    }
}