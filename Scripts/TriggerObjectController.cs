using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEditorInternal;
using UnityEngine;

public class TriggerObjectController : MonoBehaviour
{
    // 加速板の設定
    public float speedUpBoardForceYoko = 50.0f;
    public float speedUpBoardForceTate = 0f;

    // ワープ装置の設定
    public GameObject warpSaki;
    private static float lastWarpTime; 
    private float cooldown = 0.5f;
    [Header("Warp Settings")]
    public float shrinkDuration = 0.3f; // 縮小にかかる時間
    public float expandDuration = 0.3f; // 拡大にかかる時間
    private bool isWarping = false;     // 二重ワープ防止

    // 角度変更板の設定
    public float angleChangeForce = 50.0f;
    private float currentAngle;
    public GameObject arrowIndicator;
    public float cycleTime = 4.0f;

    // 反重力空間の設定
    public float antiGravityForce = 2.0f;

    private void Update()
    {
        //90度 から -90度 まで変化させる計算
        float sinValue = Mathf.Sin(Time.time * Mathf.PI * 2 / cycleTime);
        currentAngle = sinValue * 90f;

        // 矢印インジケーターの回転
        if (arrowIndicator != null)
        {
            arrowIndicator.transform.localRotation = Quaternion.Euler(0, 0, currentAngle);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("AnotherPlayer"))
        {
            CheckMyTagAndExecute(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("AnotherPlayer"))
        {
            CheckMyTagAndExecuteStay(other.gameObject);
        }
    }

    void CheckMyTagAndExecute(GameObject target)
    {
        switch (gameObject.tag)
        {
            case "SpeedUpBoard":
                SpeedUpBoard(target);
                break;
            case "Warp":
                if (!isWarping && Time.time >= lastWarpTime + cooldown)
                {
                    StartCoroutine(WarpSequence(target));
                }
                break;
            case "AngleChangeBoard":
                AngleChangeBoard(target);
                break;
            default:
                Debug.Log("このTagに対応する処理がない");
                break;
        }
    }

    void CheckMyTagAndExecuteStay(GameObject target)
    {
        switch (gameObject.tag)
        {
            case "AntiGravity":
                AntiGravity(target);
                break;
            default:
                Debug.Log("このTagに対応する処理がない");
                break;
        }
    }

    void SpeedUpBoard(GameObject target)
    {
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.velocity = Vector3.zero;
            Vector3 force = new Vector3(speedUpBoardForceYoko, speedUpBoardForceTate, 0);
            rb.AddForce(force, ForceMode.Impulse);
        }
    }

/*    void Warp(GameObject target)
    {
        if (Time.time < lastWarpTime + cooldown) return;
        lastWarpTime = Time.time;
        if (warpSaki != null)
        {
            target.transform.position = warpSaki.transform.position;
        }
    }*/

    IEnumerator WarpSequence(GameObject target)
    {
        isWarping = true;
        lastWarpTime = Time.time;

        Vector3 originalScale = target.transform.localScale;
        Rigidbody rb = target.GetComponent<Rigidbody>();

        Vector3 savedVelocity = Vector3.zero;
        Vector3 savedAngularVelocity = Vector3.zero;
        if (rb != null)
        {
            savedVelocity = rb.velocity;
            savedAngularVelocity = rb.angularVelocity;
        }

        if (warpSaki != null){
            // 1. 縮小フェーズ
            float elapsed = 0;
            while (elapsed < shrinkDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / shrinkDuration;
                target.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
                yield return null;
            }
            target.transform.localScale = Vector3.zero;

            // --- 【重要】物理演算の干渉を防ぐ処理 ---
            if (rb != null)
            {
                rb.isKinematic = true;            // 物理演算を一時停止（引き戻し防止）
            }

            // 2. 位置の移動
                target.transform.position = warpSaki.transform.position;
                // Unityに座標変更を強制同期させる
                Physics.SyncTransforms();

            // 3. 拡大フェーズ
            elapsed = 0;
            while (elapsed < expandDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / expandDuration;
                target.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
                yield return null;
            }
            target.transform.localScale = originalScale;

            // --- 【重要】物理演算を再開 ---
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.velocity = savedVelocity;           // 速度を復元
                rb.angularVelocity = savedAngularVelocity; // 回転を復元
            }

            isWarping = false;
        }
    }

void AngleChangeBoard(GameObject target)
    {
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            // 計算した角度から方向ベクトルを作成し、力を加える
            Vector3 forceDir = GetDirection();
            rb.AddForce(forceDir * angleChangeForce, ForceMode.Impulse);
        }
    }

    // 現在の角度から「飛ばす方向」のベクトルを計算するメソッド
    Vector3 GetDirection()
    {
        float rad = currentAngle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
    }

    void AntiGravity(GameObject target)
    {
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 normalGravity = Physics.gravity;
            rb.AddForce(-normalGravity * antiGravityForce, ForceMode.Acceleration);
        }
    }
}
