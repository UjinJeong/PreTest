using UnityEngine;

/// <summary>
/// 플레이 타임 중 거울 오브젝트의 이동 및 회전을 제어하는 컨트롤러
/// Mirror_ujin.prefab에 추가
/// </summary>
public class MirrorTransformController : MonoBehaviour
{
     // Drag Move (마우스 드래그 이동)
    private float dragPlaneY;                 // 드래그 시 기준이 되는 현재 거울 높이(Y)
    [Tooltip("바닥에 파묻히는 현상 방지용 오프셋")]
    public float surfaceOffset = 0.01f;       // 바닥에 파묻히는 현상 방지용 오프셋

    // Vertical Move(위/아래 이동)
    [Header("오브젝트 위/아래 이동")]
    public float verticalMoveSpeed = 1.5f;    // W/S 키로 위/아래 이동 속도
    public KeyCode moveUpKey = KeyCode.W;      // 위로 이동
    public KeyCode moveDownKey = KeyCode.S;    // 아래로 이동

    // Rotate (회전)
    [Header("오브젝트 회전")]
    public float rotateSpeed = 10;           // 회전 속도 (deg/sec)

    public KeyCode rotateYawLeftKey = KeyCode.Q;    // 좌/우 회전 (Y-)
    public KeyCode rotateYawRightKey = KeyCode.E;   // 좌/우 회전 (Y+)

    public KeyCode rotatePitchUpKey = KeyCode.R;    // 위/아래 기울기 (X+)
    public KeyCode rotatePitchDownKey = KeyCode.F;  // 위/아래 기울기 (X-)

    // State(상태)
    private bool isSelected;                   // 현재 선택된 거울인지 여부
    private bool isDragging;                   // 드래그 중인지 여부
    private Vector3 dragOffset;                // 클릭 지점과 오브젝트 중심 간 보정값

    /// <summary>
    /// 거울 선택/해제 처리
    /// </summary>
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        isDragging = false;
    }

    private void Update()
    {
        if (!isSelected) return;

        HandleDragMove();      // 마우스 드래그 이동 (X/Z)
        HandleVerticalMove();  // W/S 키 위/아래 이동 (Y)
        HandleRotation();      // 회전 처리
    }

    #region Rotation
    /// <summary>
    /// 거울 회전 처리
    /// - Q / E : 좌우 회전 (Yaw, Y축)
    /// - R / F : 위/아래 기울기 (Pitch, X축)
    /// </summary>
    private void HandleRotation()
    {
        float yaw = 0f;
        if (Input.GetKey(rotateYawLeftKey)) yaw -= 1f;
        if (Input.GetKey(rotateYawRightKey)) yaw += 1f;

        float pitch = 0f;
        if (Input.GetKey(rotatePitchUpKey)) pitch += 1f;
        if (Input.GetKey(rotatePitchDownKey)) pitch -= 1f;

        // 좌우 회전(Yaw) : 월드 기준으로 항상 지면 기준 회전
        if (Mathf.Abs(yaw) > 0.01f)
        {
            transform.Rotate(Vector3.up, yaw * rotateSpeed * Time.deltaTime, Space.World);
        }

        // 위/아래 기울기(Pitch) : 로컬 기준으로 거울 면 기준 회전
        if (Mathf.Abs(pitch) > 0.01f)
        {
            transform.Rotate(Vector3.right, pitch * rotateSpeed * Time.deltaTime, Space.Self);
        }
    }
    #endregion

    #region Move
    /// <summary>
    /// 마우스 드래그로 거울을 수평(X/Z) 이동
    /// </summary>
    private void HandleDragMove()
    {
        // 드래그 시작 시 현재 높이를 기준 평면으로 설정
        if (Input.GetMouseButtonDown(0))
        {
            dragPlaneY = transform.position.y;

            if (TryRaycastToPlane(out Vector3 hitPoint))
            {
                isDragging = true;
                dragOffset = transform.position - hitPoint;
            }
        }

        // 드래그 중 위치 갱신
        if (isDragging && Input.GetMouseButton(0))
        {
            if (TryRaycastToPlane(out Vector3 hitPoint))
            {
                Vector3 target = hitPoint + dragOffset;
                target.y = dragPlaneY + surfaceOffset;
                transform.position = target;
            }
        }

        // 드래그 종료
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    /// <summary>
    /// W / S 키를 이용한 위/아래(Y) 이동
    /// </summary>
    private void HandleVerticalMove()
    {
        float dir = 0f;

        if (Input.GetKey(moveUpKey)) dir += 1f;
        if (Input.GetKey(moveDownKey)) dir -= 1f;

        if (Mathf.Abs(dir) < 0.01f) return;

        Vector3 pos = transform.position;
        pos.y += dir * verticalMoveSpeed * Time.deltaTime;
        transform.position = pos;
    }
    #endregion

    #region Raycast
    /// <summary>
    /// 마우스의 2D 화면 좌표를
    /// 거울이 위치한 3D 공간 좌표로 변환하기 위한 계산
    /// 
    /// 마우스 드래그 시
    /// 거울이 위아래로 튀지 않고 X/Z 방향으로만 자연스럽게 이동
    /// </summary>
    private bool TryRaycastToPlane(out Vector3 hitPoint)
    {
        hitPoint = default;

        // 메인 카메라 가져오기
        Camera cam = Camera.main;
        if (cam == null) return false;

        // 마우스 위치에서 화면 → 월드로 레이 생성
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // 현재 거울 높이(dragPlaneY)에 있는 수평 평면 생성
        Plane plane = new Plane(Vector3.up, new Vector3(0f, dragPlaneY, 0f));

        // 레이가 평면과 교차하는지 검사
        if (plane.Raycast(ray, out float enter))
        {
            // 교차 지점의 월드 좌표 계산
            hitPoint = ray.GetPoint(enter);
            return true;
        }

        return false;
    }
    #endregion
}
