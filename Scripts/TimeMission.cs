using UnityEngine;

public class TimeMission : MissionBase
{
    public float targetTime = 30f; // 目標タイム（インスペクターで設定）

    // 親の命令通り「クリア判定」を実装
    public override bool IsCleared()
    {
        if (ScoreManager.Instance == null) return false;

        // ScoreManagerの残り時間（残り300秒から減っていく値）から
        // 経過時間を計算：制限時間(300) - 残り時間
        float elapsedTime = ScoreManager.Instance.timeLimit - ScoreManager.Instance.GetCurrentTime();

        // 経過時間が目標タイム以内ならクリア！
        return elapsedTime <= targetTime;
    }

    public override void UpdateUI()
    {
        float elapsedTime = ScoreManager.Instance.timeLimit - ScoreManager.Instance.GetCurrentTime();
        bool failed = elapsedTime > targetTime;

        description.text = "04:" + (60-targetTime) + "秒以上残してゴール";
        counterText.text = "";
        SetStatusIcon(failed ? false : (bool?)null); // 失敗時のみバツ
    }
}