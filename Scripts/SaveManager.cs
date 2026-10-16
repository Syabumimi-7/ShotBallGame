using UnityEngine;

public class SaveManager : MonoBehaviour
{
    // スコアを保存する関数（ステージクリア時に呼ぶ）
    public static void SaveBestScore(int stageIndex, int score)
    {
        string key = "StageBestScore_" + stageIndex;
        int currentBest = PlayerPrefs.GetInt(key, 0);

        if (score > currentBest)
        {
            PlayerPrefs.SetInt(key, score);
            PlayerPrefs.Save(); // 確実に保存
        }
    }

    // スコアを取得する関数（UI表示時に呼ぶ）
    public static int GetBestScore(int stageIndex)
    {
        return PlayerPrefs.GetInt("StageBestScore_" + stageIndex, 0);
    }
}