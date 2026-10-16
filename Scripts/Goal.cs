using UnityEngine;

public class Goal : MonoBehaviour
{
    private bool isCleared = false;

    private void OnTriggerEnter(Collider other)
    {
        // ボールが触れた、かつ、まだクリア処理前なら
        if (other.CompareTag("Player") && !isCleared)
        {
            isCleared = true;

            // StageManagerにクリアを通知
            StageManager.Instance.StageClear();
        }
    }
}