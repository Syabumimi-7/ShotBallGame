using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;
    public Image fadeImage;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンを跨いでも消さない
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 暗くする（フェードアウト）
    public IEnumerator FadeOut(float duration)
    {
        float timer = 0f;
        Color c = fadeImage.color;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, timer / duration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = 1;
        fadeImage.color = c;
    }

    // 明るくする（フェードイン）
    public IEnumerator FadeIn(float duration)
    {
        float timer = 0f;
        Color c = fadeImage.color;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(1, 0, timer / duration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = 0;
        fadeImage.color = c;
    }
}