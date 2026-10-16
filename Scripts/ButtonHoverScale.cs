using UnityEngine;
using UnityEngine.EventSystems; // イベントシステムを使用するために必要

public class ButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("設定")]
    [SerializeField] private float hoverScale = 1.1f;    // ホバー時の倍率
    [SerializeField] private float animationSpeed = 0.1f; // アニメーションの速さ

    private Vector3 initialScale; // 元のサイズ
    private Vector3 targetScale;  // 目標のサイズ

    void Start()
    {
        // 最初のサイズを記録
        initialScale = transform.localScale;
        targetScale = initialScale;
    }

    void Update()
    {
        // 毎フレーム、目標のサイズへ滑らかに変化させる
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, animationSpeed);
    }

    // マウスがボタンに入った時
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = initialScale * hoverScale;
    }

    // マウスがボタンから離れた時
    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = initialScale;
    }

    // スクリプトが無効になった時にサイズをリセット（念のため）
    void OnDisable()
    {
        transform.localScale = initialScale;
    }
}