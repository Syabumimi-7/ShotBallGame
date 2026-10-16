public class BreakMission : MissionBase
{
    public int targetBreakCount = 5;

    // ブロックからの通知。ScoreManagerの値を増やす
    public void NotifyBlockBroken()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.blocksBroken++;
        }
    }

    public override bool IsCleared()
    {
        return ScoreManager.Instance.blocksBroken >= targetBreakCount;
    }

    public override void UpdateUI()
    {
        int current = ScoreManager.Instance.blocksBroken;
        description.text = "ブロックを" + (targetBreakCount) + "個壊す";
        counterText.text = $"{current} / {targetBreakCount}";

        if (current >= targetBreakCount)
        {
            SetStatusIcon(true);
        }
        else
        {
            SetStatusIcon(null);
        }
    }
}