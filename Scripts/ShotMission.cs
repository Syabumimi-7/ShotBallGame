using UnityEngine;

public class ShotMission : MissionBase
{
    public int targetShotCount = 3; // 目標ショット数（インスペクターで設定）

    public override bool IsCleared()
    {
        // 1. シーン内からプレイヤーを探す
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        // 2. PlayerControllerを取得
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc == null) return false;

        // 3. 現在のショット数が目標以下ならクリア！
        int currentShots = pc.GetShotCount();
        return currentShots <= targetShotCount;
    }

    public override void UpdateUI()
    {
        int current = GameObject.FindWithTag("Player").GetComponent<PlayerController>().GetShotCount();
        description.text = (targetShotCount) + "ショット以内でゴール";
        counterText.text = $"{current} / {targetShotCount}";
        SetStatusIcon(current > targetShotCount ? false : (bool?)null);
    }
}