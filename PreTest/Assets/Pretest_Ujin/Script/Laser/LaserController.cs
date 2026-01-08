using UnityEngine;

/// <summary>
/// 레이저를 발사하고,
/// 거울에 반사되며,
/// Receiver와의 충돌을 판정하는 레이저 컨트롤러
/// </summary>
public class LaserController : MonoBehaviour
{
    [Header("레이저 설정")]
    [Header("레이저 최대 사거리")]
    public float maxDistance = 500f;        // 레이저 최대 사거리

    [Header("최대 반사 횟수")]
    public int maxReflectionCount = 10;     // 허용되는 최대 반사 횟수

    [Header("재충돌 방지용 오프셋")]
    public float startOffset = 0.02f;       // 시작 지점을 살짝 띄워 자기 콜라이더 재히트 방지

    [Header("참조 오브젝트")]
    public Transform muzzle;                // 레이저 발사 기준 위치
    public LineRenderer lineRenderer;       // 레이저 시각화를 담당하는 LineRenderer

    [Header("UI")]
    public LaserReflectionCounterUI reflectionCounterUI; // 반사 횟수 표시 UI

    private void Awake()
    {
        // muzzle이 지정되지 않았으면 자식 BoxCollider를 기준 위치로 사용
        if (muzzle == null)
            muzzle = GetComponentInChildren<BoxCollider>().transform;

        // LineRenderer가 지정되지 않았으면 자식에서 자동 검색
        if (lineRenderer == null)
            lineRenderer = GetComponentInChildren<LineRenderer>();

        // LineRenderer 기본 설정
        lineRenderer.useWorldSpace = true;  // 월드 좌표 기준으로 선을 그림
        lineRenderer.positionCount = 2;    // 기본은 시작점 + 끝점 (직선)

        // UI가 연결되지 않았으면 씬에서 자동 검색
        if (reflectionCounterUI == null)
            reflectionCounterUI = FindAnyObjectByType<LaserReflectionCounterUI>();

        // UI 초기화 (최대 반사 횟수 표시)
        if (reflectionCounterUI != null)
            reflectionCounterUI.Initialize(maxReflectionCount);
    }

    private void Update()
    {
        // 매 프레임 레이저 경로를 다시 계산 (항상 활성화된 레이저)
        FireLaser();
    }

    /// <summary>
    /// 레이저 발사 및 반사 처리
    /// - Raycast로 충돌 지점 계산
    /// - MirrorSurface에 맞으면 반사
    /// - ReceiverStateEffect에 맞으면 히트 처리
    /// </summary>
    private void FireLaser()
    {
        // muzzle이 있으면 muzzle 기준, 없으면 자기 자신 기준
        Transform t = (muzzle != null) ? muzzle : transform;

        // 레이저 시작 위치 (자기 콜라이더 재히트 방지용 오프셋 포함)
        Vector3 origin = t.position + t.forward * startOffset;

        // 레이저 진행 방향
        Vector3 direction = t.forward;

        // LineRenderer에 전달할 포인트 배열
        // (시작점 + 최대 반사 횟수 + 마지막 끝점)
        Vector3[] points = new Vector3[maxReflectionCount + 2];
        int pointCount = 0;

        // 시작점 등록
        points[pointCount++] = origin;

        // 남은 레이저 거리
        float remainDistance = maxDistance;

        // 이번 프레임에서 사용할 수 있는 반사 횟수
        int remainReflection = maxReflectionCount;

        // 실제 사용한 반사 횟수
        int usedReflectionThisFrame = 0;

        // 최대 반사 횟수만큼 반복
        for (int bounce = 0; bounce <= maxReflectionCount; bounce++)
        {
            // 남은 거리가 거의 없으면 종료
            if (remainDistance <= 0.0001f)
                break;

            // 레이캐스트 발사
            if (Physics.Raycast(origin, direction, out RaycastHit hit, remainDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                // 충돌 지점 저장
                points[pointCount++] = hit.point;

                // Receiver에 맞았는지 검사
                var receiver = hit.collider.GetComponentInParent<ReceiverStateEffect>();
                if (receiver != null)
                    receiver.OnLaserHit();

                // Mirror에 맞았는지 검사
                var mirror = hit.collider.GetComponentInParent<MirrorSurface>();
                if (mirror != null && bounce < maxReflectionCount)
                {
                    // 반사 횟수 소모
                    usedReflectionThisFrame++;
                    remainReflection = maxReflectionCount - usedReflectionThisFrame;

                    // 반사 방향 계산
                    direction = Vector3.Reflect(direction, hit.normal).normalized;

                    // 다음 레이 시작 위치 (자기 재충돌 방지)
                    origin = hit.point + direction * startOffset;

                    // 남은 거리 차감
                    remainDistance -= hit.distance;

                    continue;
                }

                // 거울이 아니면 더 이상 진행하지 않음
                break;
            }
            else
            {
                // 아무 것도 맞지 않았을 경우
                // 남은 거리만큼 직진한 끝점을 추가
                points[pointCount++] = origin + direction * remainDistance;
                break;
            }
        }

        // UI는 프레임당 한 번만 갱신
        if (reflectionCounterUI != null)
            reflectionCounterUI.UpdateCounter(remainReflection);

        // 계산된 모든 포인트를 LineRenderer에 적용
        lineRenderer.positionCount = pointCount;
        for (int i = 0; i < pointCount; i++)
            lineRenderer.SetPosition(i, points[i]);
    }
}
