using UnityEngine;

/// <summary>
/// 레이저를 발사하고
/// Raycast를 이용해 충돌 지점까지 LineRenderer로 시각화하며
/// MirrorSurface에 닿으면 정반사를 수행하는 컨트롤러
/// 
/// Laser 오브젝트에 추가
/// 
/// - 최대 반사 횟수 제한
/// - Receiver 도달 시 상태 변화 트리거
/// - Mirror / Receiver Enter 순간에만 효과음 재생
/// </summary>
public class LaserController : MonoBehaviour
{
    [Header("레이저 설정")]
    [Tooltip("레이저 최대 사거리")]
    public float maxDistance = 500f;

    [Tooltip("허용되는 최대 반사 횟수")]
    public int maxReflectionCount = 10;

    [Tooltip("자기 자신 재충돌 방지를 위한 시작 오프셋")]
    public float startOffset = 0.02f;

    [Header("참조 오브젝트")]
    [Tooltip("레이저 발사 기준 위치")]
    public Transform muzzle;

    [Tooltip("레이저 시각화를 위한 LineRenderer")]
    public LineRenderer lineRenderer;

    [Header("UI")]
    [Tooltip("반사 횟수 UI 표시")]
    public LaserReflectionCounterUI reflectionCounterUI;

    // 이전 프레임에 레이저가 닿았던 Mirror / Receiver
    // → Enter 순간만 효과음을 재생하기 위한 기록용
    private MirrorSurface prevMirror;
    private ReceiverStateEffect prevReceiver;

    private void Awake()
    {
        Initialize();
    }

    #region 레이저 컨트롤러의 필수 참조 초기화
    /// <summary>
    /// - 발사 위치(muzzle) 자동 설정
    /// - LineRenderer 설정
    /// - 반사 횟수 UI 탐색 및 초기화
    /// </summary>
    private void Initialize()
    {
        // muzzle이 지정되지 않았을 경우, 자식 BoxCollider 기준으로 자동 할당
        if (muzzle == null)
            muzzle = GetComponentInChildren<BoxCollider>().transform;

        // LineRenderer 자동 참조
        if (lineRenderer == null)
            lineRenderer = GetComponentInChildren<LineRenderer>();

        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;

        // 반사 횟수 UI 자동 탐색
        if (reflectionCounterUI == null)
            reflectionCounterUI = FindAnyObjectByType<LaserReflectionCounterUI>();

        // UI 초기화
        if (reflectionCounterUI != null)
            reflectionCounterUI.Initialize(maxReflectionCount);
    }
    #endregion

    private void Update()
    {
        // 매 프레임 레이저 발사 및 갱신
        FireLaser();
    }

    #region 레이저 발사 기능
    /// <summary>
    /// 레이저를 발사하고
    /// Raycast 반사 로직을 통해 경로를 계산한다.
    /// </summary>
    private void FireLaser()
    {
        // muzzle이 없으면 자신의 Transform 기준
        Transform t = (muzzle != null) ? muzzle : transform;

        // 시작 위치 및 방향
        Vector3 origin = t.position + t.forward * startOffset;
        Vector3 direction = t.forward;

        // LineRenderer에 전달할 포인트 배열
        Vector3[] points = new Vector3[maxReflectionCount + 2];
        int pointCount = 0;
        points[pointCount++] = origin;

        float remainDistance = maxDistance;

        int remainReflection = maxReflectionCount;
        int usedReflectionThisFrame = 0;

        // 이번 프레임에 실제로 닿은 마지막 Mirror / Receiver
        MirrorSurface hitMirrorThisFrame = null;
        ReceiverStateEffect hitReceiverThisFrame = null;

        // 반사 처리 루프
        for (int bounce = 0; bounce <= maxReflectionCount; bounce++)
        {
            if (remainDistance <= 0.0001f) break;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, remainDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                // 충돌 지점 추가
                points[pointCount++] = hit.point;

                // Receiver 처리
                var receiver = hit.collider.GetComponentInParent<ReceiverStateEffect>();
                if (receiver != null)
                {
                    receiver.OnLaserHit();
                    hitReceiverThisFrame = receiver;
                }

                // Mirror 처리
                var mirror = hit.collider.GetComponentInParent<MirrorSurface>();
                if (mirror != null && bounce < maxReflectionCount)
                {
                    usedReflectionThisFrame++;
                    remainReflection = maxReflectionCount - usedReflectionThisFrame;

                    hitMirrorThisFrame = mirror;

                    // 반사 방향 계산
                    direction = Vector3.Reflect(direction, hit.normal).normalized;
                    origin = hit.point + direction * startOffset;

                    // 남은 거리 갱신
                    remainDistance -= hit.distance;
                    continue;
                }

                // Mirror가 아니거나 반사 한계 도달 시 종료
                break;
            }
            else
            {
                // 아무것도 맞지 않았을 경우 최대 거리까지 직진
                points[pointCount++] = origin + direction * remainDistance;
                break;
            }
        }

        // Mirror Enter 순간에만 효과음 재생
        if (hitMirrorThisFrame != null && hitMirrorThisFrame != prevMirror && EffectSoundController.Instance != null)
            EffectSoundController.Instance.Play(0);

        // Receiver Enter 순간에만 효과음 재생
        if (hitReceiverThisFrame != null && hitReceiverThisFrame != prevReceiver && EffectSoundController.Instance != null)
            EffectSoundController.Instance.Play(1);

        // 이전 프레임 기록 갱신
        prevMirror = hitMirrorThisFrame;
        prevReceiver = hitReceiverThisFrame;

        // UI 반사 횟수 갱신
        if (reflectionCounterUI != null)
            reflectionCounterUI.UpdateCounter(remainReflection);

        // LineRenderer 갱신
        lineRenderer.positionCount = pointCount;
        for (int i = 0; i < pointCount; i++)
            lineRenderer.SetPosition(i, points[i]);
    }
    #endregion
}
