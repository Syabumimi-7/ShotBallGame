using UnityEngine;
using UnityEngine.EventSystems; // これが必要
using TMPro;

public class StageButtonInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int stageIndex; // 0から9 をインスペクターで設定
    public GameObject infoPanel; // さっき作った情報パネル
    public TextMeshProUGUI infoText; // パネル内のテキスト

    // ステージごとの「お題内容」をここにメモ（インスペクターから入力）
    [TextArea]
    public string missionDescription;

    // マウスが入った時
    public void OnPointerEnter(PointerEventData eventData)
    {
        int bestScore = SaveManager.GetBestScore(stageIndex);

        infoText.text = $"BEST SCORE: {bestScore}pt\n\n【MISSIONS】\n{missionDescription}";

        infoPanel.SetActive(true);
    }

    // マウスが出た時
    public void OnPointerExit(PointerEventData eventData)
    {
        infoPanel.SetActive(false);
    }
}