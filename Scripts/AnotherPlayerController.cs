using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnotherPlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float powerMultiplier = 10f;
    public float maxPower = 30f;
    public float minBounceVelocity = 0.5f;    // これ以下の速度なら停止判定
    public float bounceMaintainForce = 1.5f; // 跳ね返り時に少し加速させる（減衰防止）
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Z軸固定
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
    }

    // 壁に当たった時の挙動
    [Header("Effects")]
    public GameObject hitEffectPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        float currentSpeed = rb.velocity.magnitude;

        if (currentSpeed < minBounceVelocity)
        {
            // 速度が低い場合は完全に止める
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            // 速度がある程度ある場合は、勢いを維持（または少し増幅）させる
            rb.velocity = rb.velocity * bounceMaintainForce;
        }

        // 衝突時の速度をチェック
        float speed = rb.velocity.magnitude;

        // 一定以上の速度で当たった時だけエフェクトを出す
        if (speed > minBounceVelocity)
        {
            // 衝突した地点（接点）を取得
            ContactPoint contact = collision.contacts[0];
            Quaternion rotation = Quaternion.LookRotation(contact.normal);

            // エフェクトを生成
            GameObject effect = Instantiate(hitEffectPrefab, contact.point, rotation);
        }

        // --- 以前の停止処理 ---
        if (speed < minBounceVelocity)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
