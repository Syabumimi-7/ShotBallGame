using UnityEngine;

public class CoinMission : MissionBase
{
    // 目標枚数（今回は3枚）
    public int targetCoinCount = 3;

    public override bool IsCleared()
    {
        // ScoreManagerに記録されている「現在のステージで取ったコイン数」を確認
        if (ScoreManager.Instance == null) return false;

        int currentCoins = ScoreManager.Instance.coinsCollected;

        // 集めた枚数が目標以上ならクリア！
        return currentCoins >= targetCoinCount;
    }

    public override void UpdateUI()
    {
        int current = ScoreManager.Instance.coinsCollected;
        description.text = "コインを" + (targetCoinCount) + "枚取ってゴール";
        counterText.text = $"{current} / {targetCoinCount}";
        // 達成したらチェック、それ以外は空
        SetStatusIcon(current >= targetCoinCount ? true : (bool?)null);
    }
}