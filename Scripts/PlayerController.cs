using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float powerMultiplier = 10f;
    public float maxPower = 30f;
    public float minBounceVelocity = 0.5f;    // これ以下の速度なら停止判定
    public float bounceMaintainForce = 1.5f; // 跳ね返り時に少し加速させる（減衰防止）

    [Header("Visuals (3D Arrow)")]
    public GameObject arrowModel;
    public float arrowScaleMultiplier = 0.1f;

    private Rigidbody rb;
    private Vector3 dragStartPos;
    private bool isDragging = false;
    public GameObject stageCamera;
    private Camera cam;

    private int shotCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = stageCamera.GetComponent<Camera>();
        arrowModel.SetActive(false);

        // Z軸固定
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
    }

    void Update()
    {
        // もし stageCamera が Inspector で入れ直す必要があるなら、ここで null チェック
        if (stageCamera != null && stageCamera.activeSelf)
        {
            // cam が Missing になっていたら再取得する（シーン跨ぎ対策）
            if (cam == null) cam = stageCamera.GetComponent<Camera>();

            if (Input.GetMouseButtonDown(0)) OnDragStart();
            if (isDragging && Input.GetMouseButton(0)) OnDragUpdate();
            if (isDragging && Input.GetMouseButtonUp(0)) OnDragEnd();
        }
    }

    void OnDragStart()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.StartGame();
        }
        // マウス位置取得に失敗しないよう cam をチェック
        if (cam == null) cam = stageCamera.GetComponent<Camera>();
        isDragging = true;
        dragStartPos = GetMouseWorldPosition();
    }

    void OnDragUpdate()
    {
        Vector3 currentPos = GetMouseWorldPosition();
        Vector3 dragVector = dragStartPos - currentPos;
        UpdateArrow(dragVector);
    }

    void OnDragEnd()
    {
        shotCount++;

        isDragging = false;
        arrowModel.SetActive(false);

        Vector3 dragVector = dragStartPos - GetMouseWorldPosition();
        // 瞬間的に大きな力を加える
        Vector3 force = Vector3.ClampMagnitude(dragVector * powerMultiplier, maxPower);
        rb.AddForce(force, ForceMode.Impulse);
    }

    void UpdateArrow(Vector3 dragVector)
    {
        if (dragVector.magnitude < 0.1f)
        {
            arrowModel.SetActive(false);
            return;
        }

        arrowModel.SetActive(true);

        // 1. 位置をボールに合わせる
        arrowModel.transform.position = transform.position;

        // 2. 向きを計算（ドラッグした方向の逆、つまり飛ばす方向）
        // XY平面での角度を算出
        float angle = Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;
        arrowModel.transform.rotation = Quaternion.Euler(0, 0, angle);

        // 3. 長さを変える（X軸方向に伸びるモデルの場合）
        float length = dragVector.magnitude * arrowScaleMultiplier;
        // 長くなりすぎないよう制限（maxPowerに合わせる）
        length = Mathf.Min(length, maxPower * arrowScaleMultiplier);

        // YとZは元の太さを維持し、X（長さ）だけ変える
        arrowModel.transform.localScale = new Vector3(length, 1f, 1f);
    }

    Vector3 GetMouseWorldPosition()
    {
        if (cam == null) return transform.position; // エラー防止用

        Vector3 mousePoint = Input.mousePosition;
        // z軸の計算を絶対値にするか、カメラとの距離を動的に取る
        mousePoint.z = Mathf.Abs(cam.transform.position.z - transform.position.z);
        return cam.ScreenToWorldPoint(mousePoint);
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

        if (!collision.gameObject.CompareTag("LightBox"))
        {
            NotifyBounce();
        }

        // 【追加】触れたオブジェクトに特定のタグがついていた場合
        if (collision.gameObject.CompareTag("MoveCube"))
        {
            Debug.Log("MoveCubeに当たった");
            // 現在のステージにある NoTouchMission を探して通知する
            NoTouchMission mission = FindObjectOfType<NoTouchMission>();
            if (mission != null)
            {
                mission.NotifyTouch();
            }
        }
    }

    // クリア判定で使うために、現在のショット数を返す関数
    public int GetShotCount()
    {
        return shotCount;
    }

    // シーンリロード時に0にリセットする関数
    public void ResetShotCount()
    {
        shotCount = 0;
        UpdateShotUI();
    }

    void UpdateShotUI()
    {
        // 画面の隅に「SHOT: 1」のように表示する場合
        // GameObject.Find("ShotText").GetComponent<TextMeshProUGUI>().text = "SHOT: " + shotCount;
    }

    void NotifyBounce()
    {
        if (StageManager.Instance != null)
        {
            int idx = StageManager.Instance.GetCurrentIndex();
            GameObject currentStage = StageManager.Instance.stages[idx];

            // BounceMissionを探してカウントを増やす
            BounceMission mission = currentStage.GetComponentInChildren<BounceMission>();
            if (mission != null)
            {
                mission.AddBounce();
            }
        }
    }
}