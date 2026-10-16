using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class MissionBase : MonoBehaviour
{
    [Header("Base UI Settings")]
    public TextMeshProUGUI description;
    public TextMeshProUGUI counterText; // UIの数字部分
    public Image statusImage;           // UIのアイコン部分

    [Header("Icons")]
    public Sprite defaultIcon; // 空枠
    public Sprite successIcon; // チェック
    public Sprite failIcon;    // バツ

    public abstract bool IsCleared();

    // UIを更新するための関数（各ミッションで中身を書く）
    public virtual void UpdateUI() { }

    // アイコンの状態を更新する共通処理
    protected void SetStatusIcon(bool? isSuccess)
    {
        if (statusImage == null) return;
        if (isSuccess == true) statusImage.sprite = successIcon;
        else if (isSuccess == false) statusImage.sprite = failIcon;
        else statusImage.sprite = defaultIcon;
    }
}