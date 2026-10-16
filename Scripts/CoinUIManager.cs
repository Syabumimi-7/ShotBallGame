using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic; // Listを使うために必要

public class CoinUIManager : MonoBehaviour
{
    public static CoinUIManager Instance;

    [Header("UI Slots")]
    public Image[] coinIcons; // インスペクターで本物コイン画像をセット

    [Header("Animation Settings")]
    public GameObject movingCoinPrefab;
    public Canvas canvas; // Canvasそのものをセット
    public float moveDuration = 0.8f;

    private int currentDisplayCount = 0;

    // 現在移動中のコインを追跡するためのリスト
    private List<GameObject> activeMovingCoins = new List<GameObject>();

    void Awake()
    {
        // DDOL内のため、インスタンス管理を厳密に
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // ★重要：パネルが非アクティブになった時に呼ばれるUnityのイベント
    private void OnDisable()
    {
        ClearAllMovingCoins();
    }

    // 移動中のコインをすべて削除するメソッド
    public void ClearAllMovingCoins()
    {
        foreach (GameObject coin in activeMovingCoins)
        {
            if (coin != null) Destroy(coin);
        }
        activeMovingCoins.Clear();

        // コルーチンもすべて止める（残骸が残らないように）
        StopAllCoroutines();
    }

    // ステージ開始時（リロード時）にStageManagerから明示的に呼ぶ
    public void ResetCoinUI()
    {
        currentDisplayCount = 0;
        ClearAllMovingCoins(); // リセット時にも掃除する

        // パネルが非アクティブでも、参照さえあればImageのenabledは操作可能です
        if (coinIcons != null)
        {
            foreach (var icon in coinIcons)
            {
                if (icon != null)
                {
                    icon.enabled = false;
                    icon.transform.localScale = Vector3.one;
                }
            }
        }
        Debug.Log("<color=orange>CoinUI: 完全リセット完了</color>");
    }

    public void OnGetCoin(Vector3 worldPosition)
    {
        // パネルが非表示（ゴール後など）なら処理しない
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning("PlayerUIPanelが非表示のため、コインアニメをスキップします");
            return;
        }

        if (currentDisplayCount >= coinIcons.Length) return;

        // アニメーション実行
        StartCoroutine(MoveCoinRoutine(worldPosition, currentDisplayCount));
        currentDisplayCount++;
    }

    IEnumerator MoveCoinRoutine(Vector3 worldPos, int slotIndex)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        GameObject movingCoin = Instantiate(movingCoinPrefab, canvas.transform);
        movingCoin.transform.position = screenPos;

        // リストに追加して追跡開始
        activeMovingCoins.Add(movingCoin);

        float elapsed = 0;
        Vector3 startPos = movingCoin.transform.position;

        while (elapsed < moveDuration)
        {
            // 移動中にリストから消されていたら（ClearAllMovingCoinsが呼ばれたら）終了
            if (movingCoin == null) yield break;

            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;
            t = t * t * (3f - 2f * t);

            if (coinIcons[slotIndex] != null)
            {
                movingCoin.transform.position = Vector3.Lerp(startPos, coinIcons[slotIndex].transform.position, t);
            }
            yield return null;
        }

        // 正常に到着した場合
        if (movingCoin != null)
        {
            activeMovingCoins.Remove(movingCoin); // リストから外す
            Destroy(movingCoin);

            if (coinIcons[slotIndex] != null)
            {
                coinIcons[slotIndex].enabled = true;
                StartCoroutine(PunchScale(coinIcons[slotIndex].transform));
            }
        }
    }

    IEnumerator PunchScale(Transform target)
    {
        target.localScale = Vector3.one * 1.5f;
        yield return new WaitForSeconds(0.2f);
        target.localScale = Vector3.one;
    }
}