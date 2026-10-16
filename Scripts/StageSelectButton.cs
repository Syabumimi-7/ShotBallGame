using UnityEngine;

public class StageSelectButton : MonoBehaviour
{
    public int stageIndex; // インスペクターで 0から19 を入れる

    public void OnClickButton()
    {
        // ヒエラルキーの参照を使わず、Instance（シングルトン）を直接呼ぶ
        if (StageManager.Instance != null)
        {
            StageManager.Instance.SelectStage(stageIndex);
        }
    }
}