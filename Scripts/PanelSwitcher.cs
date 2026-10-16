using UnityEngine;

public class PanelSwitcher : MonoBehaviour
{
    // 非アクティブにしたいパネル
    public GameObject panelToDeactivate;

    // アクティブにしたいパネル
    public GameObject panelToActivate;

    // ボタンがクリックされたときに呼び出されるメソッド
    public void TogglePanels()
    {
        if (panelToDeactivate != null)
        {
            panelToDeactivate.SetActive(false);
        }

        if (panelToActivate != null)
        {
            panelToActivate.SetActive(true);
        }
    }
}