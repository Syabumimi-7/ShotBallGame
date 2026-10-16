using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    // シングルトン（どこからでもアクセスできるようにする）
    public static StageManager Instance;

    // ステージごとの開始座標リスト（インスペクターで20個設定）
    public Vector3[] stagePositions;

    // 現在選択されているステージ番号
    private static int currentStageIndex = 0;

    private TextMeshProUGUI stageText;

    [Header("Clear UI")]
    public GameObject clearPanel;
    public TextMeshProUGUI clearText;
    public GameObject stageSelectPanel;
    public GameObject HUDPanel;

    [Header("Clear Panel Buttons")]
    public GameObject nextStageButton; // 「次のステージへ」ボタンをアタッチ
    public Button retryButton;         // 「もう一度遊ぶ」ボタンをアタッチ

    [Header("Auto Transition Settings")]
    public float autoTransitionTime = 10f; // 待ち時間
    private float elapsedClearTime = 0f;    // 経過時間
    private bool isCountingDown = false;    // カウント中か
    public TextMeshProUGUI autoTransitionText;

    // コルーチンを管理するための変数
    private Coroutine autoTransitionCoroutine;

    // ステージ
    public GameObject[] stages = new GameObject[10];

    void Start()
    {
        // リトライボタンにクリックイベントを登録
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(RetryCurrentStage);
        }
    }

    void Awake()
    {
        // シーンを跨いでもこのオブジェクトを消さない設定
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        // シーンが読み込まれたら実行されるイベントを登録
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ステージを選択してリロードする関数
    public void SelectStage(int index)
    {
        StopTransitionCountdown();
        StartCoroutine(StageChangeRoutine(index));
    }

    IEnumerator StageChangeRoutine(int index)
    {
        // 1. 画面を暗くする (0.2秒)
        yield return StartCoroutine(FadeManager.Instance.FadeOut(0.2f));

        // 2. シーンを読み直す
        currentStageIndex = index;
        // ロードが終わるまで待つための設定
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // --- 【追加】UIを初期状態にリセット ---
        if (clearPanel != null)
        {
            clearPanel.SetActive(false); // クリアパネルを隠す
        }
        if (stageSelectPanel != null)
        {
            stageSelectPanel.SetActive(false);
        }
        if (HUDPanel != null)
        {
            HUDPanel.SetActive(true); // プレイ中のUIを表示する
        }
        if (CoinUIManager.Instance != null)
        {
            CoinUIManager.Instance.ResetCoinUI();
        }

        GameObject textObj = GameObject.Find("StageDisplayText");
        if (textObj != null)
        {
            stageText = textObj.GetComponent<TextMeshProUGUI>();
            UpdateUI();
        }

        // 3. ボールとカメラを瞬間移動させる（ロード直後に実行）
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.GetComponent<Rigidbody>().position = stagePositions[currentStageIndex];
            player.transform.position = stagePositions[currentStageIndex];
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;

            // カメラも瞬間移動（CameraControllerにWarpToTarget関数を作っておく）
            Camera.main.GetComponent<CameraController>().WarpToTarget();
        }

        // 4. 少しだけ待機（ロード直後のカクつきを見せないため）
        yield return new WaitForSeconds(0.1f);

        // 5. 画面を明るくする (0.3秒)
        yield return StartCoroutine(FadeManager.Instance.FadeIn(0.3f));
    }

    // シーン読み込み完了時に呼ばれる
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // スコアのリセット
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
            Debug.Log("ResetScoreを呼びました");
        }

        if (currentStageIndex >= 0 && currentStageIndex < stagePositions.Length)
        {
            // ボール（Player）を探して、座標を書き換える
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerController pc = player.GetComponent<PlayerController>();
                if (pc != null) pc.ResetShotCount();

                Rigidbody rb = player.GetComponent<Rigidbody>();
                rb.isKinematic = true; // 物理演算を一時停止
                rb.position = stagePositions[currentStageIndex]; // transformではなくrbの座標を書き換える
                rb.transform.position = stagePositions[currentStageIndex];
                rb.isKinematic = false; // 再開
                rb.velocity = Vector3.zero;
            }
        }

        // 現在のステージから全ミッションを探してリセットする
        GameObject currentStageObj = stages[currentStageIndex];
        if (currentStageObj != null)
        {
            // NoTouchMission だけでなく、今後「状態」を持つミッションが増えてもいいように
            // 全てのミッションをリセットできるようにしておくと楽です
            NoTouchMission ntm = currentStageObj.GetComponentInChildren<NoTouchMission>();
            if (ntm != null) ntm.ResetMission();
        }
    }

    void UpdateUI()
    {
        if (stageText != null)
        {
            // インデックスは0から始まるので、表示は +1 する
            stageText.text = "STAGE: " + (currentStageIndex + 1);
        }
    }

    public void StageClear()
    {
        // 元のUIを隠す
        if (HUDPanel != null) HUDPanel.SetActive(false);
        if (stageSelectPanel != null) stageSelectPanel.SetActive(false);

        /*        // お題の判定例
                // お題1: 60秒以内にクリア
                if (ScoreManager.Instance.GetCurrentTime() > 240)
                    ScoreManager.Instance.mission1Cleared = true;

                // お題2: ショット数
                // プレイヤーを探してショット数を取得
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                PlayerController pc = player.GetComponent<PlayerController>();
                int currentShots = pc.GetShotCount();
                // お題判定：目標ショット数以内か？
                if (currentShots <= targetShots[currentStageIndex])
                {
                    ScoreManager.Instance.mission2Cleared = true;
                }*/

        // 1. 今のステージの親オブジェクトを特定する
        // ステージをリストや配列で管理している場合（例：stages[currentStageIndex]）
        GameObject currentStageObj = stages[currentStageIndex];
        if (currentStageObj == null)
        {
            Debug.LogError("現在のステージオブジェクトが設定されていません。");
            return;
        }

        // 2. その親オブジェクトの子要素からだけ MissionBase を探す
        // GetComponentsInChildren を使うのがポイントです
        MissionBase[] missions = currentStageObj.GetComponentsInChildren<MissionBase>();
        if (missions == null || missions.Length == 0)
        {
            // ゲームを止めず、ログだけ出して処理を続行する
            Debug.LogWarning($"ステージ {currentStageIndex + 1} にミッションが設定されていません。判定をスキップします。");
            ScoreManager.Instance.mission1Cleared = false;
            ScoreManager.Instance.mission2Cleared = false;
        }
        else
        {
            // 1つ目のミッション判定
            if (missions.Length >= 1 && missions[0] != null)
            {
                ScoreManager.Instance.mission1Cleared = missions[0].IsCleared();
            }

            // 2つ目のミッション判定
            if (missions.Length >= 2 && missions[1] != null)
            {
                ScoreManager.Instance.mission2Cleared = missions[1].IsCleared();
            }
        }

        ScoreManager sm = ScoreManager.Instance;
        int finalScore = sm.CalculateFinalScore();

        // 1. スコアとテキストの表示
        clearText.text = "STAGE " + (currentStageIndex + 1) + " CLEAR!";

        if (sm.resultTimeText != null)
            sm.resultTimeText.text = "TIME LEFT: " + Mathf.FloorToInt(sm.GetCurrentTime()) + "s";

        if (sm.resultCoinText != null)
            sm.resultCoinText.text = "COINS: " + sm.coinsCollected + " / 3";

        // 2. お題のチェックマーク表示
        if (sm.mission1Check != null)
            sm.mission1Check.SetActive(sm.mission1Cleared);

        if (sm.mission2Check != null)
            sm.mission2Check.SetActive(sm.mission2Cleared);

        SaveManager.SaveBestScore(currentStageIndex, finalScore);

        if (currentStageIndex >= 9)
        {
            if (nextStageButton != null) nextStageButton.SetActive(false);
            Debug.Log("最終ステージクリア：次へボタンを非表示にします");
        }
        else
        {
            if (nextStageButton != null) nextStageButton.SetActive(true);
        }

        // クリア画面を表示
        clearPanel.SetActive(true);
        clearText.text = "ステージ " + (currentStageIndex + 1) + " クリア！" + "\n" + "SCORE: " + finalScore;

        // 前のコルーチンがもし動いていたら止めてから、新しく開始する
        if (autoTransitionCoroutine != null)
        {
            StopCoroutine(autoTransitionCoroutine);
        }
        autoTransitionCoroutine = StartCoroutine(AutoTransitionRoutine());
    }

    public void OnNextStageButton()
    {
        // ボタンが押されたらカウントダウンを即座に停止
        //StopTransitionCountdown();

        clearPanel.SetActive(false);
        int nextIndex = currentStageIndex + 1;

        // 20ステージを超えないようにチェック
        if (nextIndex < stagePositions.Length)
        {
            SelectStage(nextIndex);
        }
        else
        {
            // 全ステージクリア時の処理（タイトルに戻るなど）
            OnHomeButton();
        }
    }

    public void OnHomeButton()
    {
        stageSelectPanel.SetActive(true);
    }

    public int GetCurrentIndex()
    {
        return currentStageIndex;
    }

    void Update()
    {
        // 現在のステージにあるミッションを取得
        GameObject currentStageObj = stages[currentStageIndex];
        MissionBase[] missions = currentStageObj.GetComponentsInChildren<MissionBase>(true);

        foreach (var m in missions)
        {
            m.UpdateUI();
        }
    }

    public void RetryCurrentStage()
    {
        // 1. スコアやミッションの状態をリセット
        ScoreManager.Instance.ResetScore();

        // ボタンが押されたらカウントダウンを即座に停止
        //StopTransitionCountdown();

        // 2. クリアパネルを閉じる
        clearPanel.SetActive(true); // 一旦閉じる処理
        clearPanel.SetActive(false);

        // 3. プレイヤーを初期位置に戻すなどの開始処理を呼ぶ
        // すでに実装済みの「ステージ開始処理」を呼び出してください
        SelectStage(currentStageIndex);

        Debug.Log("ステージ " + (currentStageIndex + 1) + " をリトライします");
    }

    // カウントダウンを完全に止めるための共通メソッド
    public void StopTransitionCountdown()
    {
        if (autoTransitionCoroutine != null)
        {
            StopCoroutine(autoTransitionCoroutine);
            autoTransitionCoroutine = null;
        }
        Debug.Log("自動遷移カウントダウンを停止しました。");
    }

    IEnumerator AutoTransitionRoutine()
    {
        elapsedClearTime = 0f;
        isCountingDown = true;

        while (elapsedClearTime < autoTransitionTime)
        {
            // ステージ選択パネルが表示されていない時だけ時間を進める
            if (stageSelectPanel != null && !stageSelectPanel.activeSelf)
            {
                elapsedClearTime += Time.deltaTime;

                if(currentStageIndex < 9)
                {
                    if (autoTransitionText != null)
                    {
                        autoTransitionText.text = (Mathf.Ceil(autoTransitionTime - elapsedClearTime)) + "秒後";
                    }
                }
                else
                {
                    if (autoTransitionText != null)
                    {
                        autoTransitionText.text = "";
                    }
                }
            }

            yield return null;
        }

        isCountingDown = false;
        ExecuteAutoTransition();
    }

    void ExecuteAutoTransition()
    {
        // 10秒経過後の処理
        if (currentStageIndex < 9)
        {
            SelectStage(currentStageIndex + 1);
        }
    }
}