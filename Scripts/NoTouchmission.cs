using UnityEngine;

public class NoTouchMission : MissionBase
{
    [Header("Settings")]
    public string obstacleTag = "MoveCube"; // 触れてはいけないオブジェクトのタグ

    private bool hasTouched = false; // 一度でも触れたら true になる

    // クリア判定：一度も触れていなければクリア
    public override bool IsCleared()
    {
        return !hasTouched;
    }

    // プレイヤーが何かに触れた時に呼ばれる
    public void NotifyTouch()
    {
        hasTouched = true;
        Debug.Log("禁止オブジェクトに接触！ミッション失敗状態です。");
    }

    public void ResetMission()
    {
        hasTouched = false;
    }

    public override void UpdateUI()
    {
        description.text = "動く箱に一度も当たらずにゴール";
        counterText.text = "";
        SetStatusIcon(hasTouched ? false : (bool?)null); // 失敗時のみバツ
    }
}