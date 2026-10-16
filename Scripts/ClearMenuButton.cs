using UnityEngine;

public class ClearMenuButtons : MonoBehaviour
{
    // 「次のステージへ」ボタン用
    public void OnClickNext()
    {
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnNextStageButton();
        }
    }

    // 「ステージ選択へ戻る」ボタン用
    public void OnClickHome()
    {
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnHomeButton();
        }
    }
}