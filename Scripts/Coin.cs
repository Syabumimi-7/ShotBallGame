using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // シーンが切り替わり中、あるいは自身がアクティブでないなら無視
        if (!gameObject.activeInHierarchy) return;

        if (other.CompareTag("Player"))
        {
            // インスタンスを直接参照せず、常に最新をチェック
            if (CoinUIManager.Instance != null)
            {
                CoinUIManager.Instance.OnGetCoin(transform.position);
            }

            ScoreManager.Instance.coinsCollected++;
            // 判定が残らないように、即座に無効化
            gameObject.SetActive(false);
        }
    }
}