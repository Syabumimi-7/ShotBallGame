using UnityEngine;
using System.Collections;

public class FadeWall : MonoBehaviour
{
    public float fadeSpeed = 1.0f;
    private Renderer rend;
    private Collider col;

    void Start()
    {
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();
    }

    public void StartFade()
    {
        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        // 判定を先に消して、ボールが通れるようにする
        col.enabled = false;

        Color color = rend.material.color;
        while (color.a > 0)
        {
            color.a -= Time.deltaTime * fadeSpeed;
            rend.material.color = color;
            yield return null;
        }

        // 完全に消えたらオブジェクトを破棄（または非アクティブ）
        gameObject.SetActive(false);
    }
}