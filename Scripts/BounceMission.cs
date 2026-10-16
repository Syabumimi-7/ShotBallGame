using Unity.Burst.CompilerServices;
using UnityEngine;

public class BounceMission : MissionBase
{
    public int maxBounceCount = 20; // 最大跳ね返り回数

    // 衝突時にPlayerから呼ばれる関数
    public void AddBounce()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.currentBounceCount++;
        }
    }

    // クリア判定：カウントが最大値以内ならクリア
    public override bool IsCleared()
    {
        return ScoreManager.Instance.currentBounceCount -1 <= maxBounceCount;
    }


    public override void UpdateUI()
    {
        description.text = "壁との跳ね返り" + (maxBounceCount) + "以内でゴール";
        counterText.text = $"{ScoreManager.Instance.currentBounceCount -1} / {maxBounceCount}";
        SetStatusIcon(ScoreManager.Instance.currentBounceCount -1 > maxBounceCount ? false : (bool?)null);
    }
}