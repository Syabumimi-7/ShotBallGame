using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;    // 追従する対象（ボール）
    public float smoothSpeed = 0.125f; // 追従の滑らかさ（0〜1）
    private Vector3 offset;     // ボールとカメラの距離

    void Start()
    {
        // 最初に設定したカメラとボールの距離を記憶する
        if (target != null)
        {
            offset = new Vector3(0, 3f, -15f);
        }
    }

    // 物理演算（Rigidbody）に合わせて動かすため LateUpdate を使用
    void LateUpdate()
    {
        if (target == null) return;

        // 目標地点を計算（Z軸は固定されたオフセットを維持）
        Vector3 desiredPosition = target.position + offset;

        // カメラの移動を滑らかにする（Lerp）
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Z軸は元の位置をキープ、またはオフセット通りに固定
        transform.position = smoothedPosition;
    }

    public void WarpToTarget()
    {
        if (target == null) return;
        // Lerpを使わずに、直接目標地点を代入する
        transform.position = target.position + offset;
    }
}