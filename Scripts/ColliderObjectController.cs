using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ColliderObjectController : MonoBehaviour
{
    // 動く床の設定
    public float moveHabaX;
    public float moveHabaY;

    // 加速反射床の設定
    public float speedUpBoundBoardForce = 2.0f;

    // 壊れる壁の設定
    public int hp;
    public Material[] materials = new Material[3];
    private Renderer _renderer;
    public ParticleSystem sparkParticle;
    public TextMeshProUGUI indexText;

    // ボタンの設定
    public FadeWall targetWall;
    public GameObject buttonTop;
    public Vector3 pressedOffset = new Vector3(0, -0.2f, 0);
    public float moveSpeed = 5f;
    private bool isPressed = false;
    private Vector3 targetPosition;

    void Start()
    {
        switch (gameObject.tag)
        {
            case "MoveCube":
                MoveCube();
                break;
            default:
                Debug.Log("このTagはスタートTagに対応する処理ではない");
                break;
        }

        // FadeButtonの設定
        if (buttonTop != null)
        {
            targetPosition = buttonTop.transform.localPosition;
        }

        // CanBeBrokenWallの設定
        _renderer = GetComponent<Renderer>();
        if (materials != null || materials.Length == 3)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] != null)
                {
                    _renderer.material = materials[0];
                }
            }
        }
        if (indexText != null)
        {
            indexText.text = hp.ToString();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("AnotherPlayer"))
        {
            CheckMyTagAndExecuteCollider(collision.gameObject);
        }
    }

    void CheckMyTagAndExecuteCollider(GameObject target)
    {
        switch (gameObject.tag)
        {
            case "SpeedUpBoundBoard":
                SpeedUpBoundBoard(target);
                break;
            case "CanBeBrokenWall":
                CanBeBrokenWall();
                break;
            case "FadeButton":
                FadeButton();
                break;
            default:
                Debug.Log("このTagに対応する処理がない");
                break;
        }
    }

    void MoveCube()
    {
        transform.DOMove(new Vector3(transform.position.x - moveHabaX, transform.position.y - moveHabaY, 0), 2.0f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
    }

    void SpeedUpBoundBoard(GameObject target)
    {
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float currentSpeed = rb.velocity.magnitude;
            rb.velocity = rb.velocity * speedUpBoundBoardForce;
        }
    }

    void CanBeBrokenWall()
    {
        hp -= 1;
        if (indexText != null)
        {
            indexText.text = hp.ToString();
        }
        if (hp == 2)
        {
            _renderer.material = materials[1];
        }
        else if(hp == 1)
        {
            _renderer.material = materials[2];
        }
        else if(hp == 0)
        {
            if(StageManager.Instance != null)
{
                int idx = StageManager.Instance.GetCurrentIndex();
                BreakMission mission = StageManager.Instance.stages[idx].GetComponentInChildren<BreakMission>();

                if (mission != null)
                {
                    mission.NotifyBlockBroken(); // これが呼ばれると ScoreManager.Instance.blocksBroken が増える
                }
            }

            if (sparkParticle != null)
            {
                sparkParticle.Play();
            }
            Destroy(this.gameObject);
        }
    }

    void FadeButton()
    {
        if (targetWall != null)
        {
            if (!isPressed)
            {
                isPressed = true;
                // 沈んだ後の目標座標を計算
                targetPosition += pressedOffset;

                if (targetWall != null) targetWall.StartFade();

                // 判定を消す（何度も押せないように）
                GetComponent<Collider>().enabled = false;
            }

            // 壁のフェードアウトを開始
            targetWall.StartFade();
        }
    }

    void Update()
    {
        // FadeButtonの処理
        if (isPressed && buttonTop != null)
        {
            // 目標の高さまでスムーズに移動させる
            buttonTop.transform.localPosition = Vector3.Lerp(
                buttonTop.transform.localPosition,
                targetPosition,
                Time.deltaTime * moveSpeed
            );
        }
    }
}
