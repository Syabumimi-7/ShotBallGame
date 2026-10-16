using UnityEngine;

public class TextureScroll : MonoBehaviour
{
    public float scrollSpeed = 0.5f; // 動くスピード
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        // 時間経過に合わせてオフセット（位置）を計算
        float offset = Time.time * scrollSpeed;

        // メインテクスチャのオフセットを更新（X軸方向に動かす場合）
        rend.material.SetTextureOffset("_MainTex", new Vector2(offset, 0));

        // ※URP（ユニバーサルレンダーパイプライン）を使用している場合はこちら
        // rend.material.SetTextureOffset("_BaseMap", new Vector2(offset, 0));
    }
}