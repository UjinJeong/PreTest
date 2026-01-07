using UnityEngine;

public class LaserController : MonoBehaviour
{
    /* ===============================
     * Laser 설정
     * =============================== */
    [Header("Laser Settings")]
    public float maxDistance = 500f;        // 레이저 최대 사거리
    public int maxReflectionCount = 10;     // 최대 반사 횟수
    public float startOffset = 0.02f;       // 자기 콜라이더 재충돌 방지 오프셋

    /* ===============================
     * 참조 오브젝트
     * =============================== */
    [Header("References")]
    public Transform muzzle;                // 레이저 발사 위치
    public LineRenderer lineRenderer;       // 레이저 시각화용 LineRenderer

    private void Awake()
    {
        // muzzle이 지정되지 않았으면 자식 콜라이더 기준으로 설정
        if (muzzle == null)
            muzzle = GetComponentInChildren<BoxCollider>().transform;

        // LineRenderer 자동 할당
        if (lineRenderer == null)
            lineRenderer = GetComponentInChildren<LineRenderer>();

        lineRenderer.useWorldSpace = true;   // 월드 좌표 기준 사용
        lineRenderer.positionCount = 2;     // 기본은 직선 2포인트
    }

    private void Update()
    {
        FireLaser(); // 매 프레임 레이저 발사
    }

    /// <summary>
    /// 레이저 발사 + 반사 처리
    /// </summary>
    private void FireLaser()
    {
        // muzzle이 있으면 muzzle 기준, 없으면 자기 자신 기준
        Transform t = (muzzle != null) ? muzzle : transform;

        Vector3 origin = t.position + t.forward * startOffset; // 시작 위치
        Vector3 direction = t.forward;                          // 발사 방향

        // LineRenderer에 넣을 포인트 배열
        Vector3[] points = new Vector3[maxReflectionCount + 2];
        int pointCount = 0;

        // 시작점 등록
        points[pointCount++] = origin;

        float remainDistance = maxDistance;

        // 반사 최대 횟수만큼 반복
        for (int bounce = 0; bounce <= maxReflectionCount; bounce++)
        {
            // 남은 거리가 없으면 종료
            if (remainDistance <= 0.0001f)
                break;

            // 레이캐스트 발사
            if (Physics.Raycast(origin, direction, out RaycastHit hit, remainDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                // 충돌 지점 저장
                points[pointCount++] = hit.point;

                // Receiver에 맞았는지 판정
                ReceiverStateEffect receiver = hit.collider.GetComponentInParent<ReceiverStateEffect>();
                if (receiver != null)
                {
                    receiver.OnLaserHit();
                }

                // Mirror에 맞았는지 판정
                MirrorSurface mirror = hit.collider.GetComponentInParent<MirrorSurface>();

                // 거울이면 반사 처리
                if (mirror != null && bounce < maxReflectionCount)
                {
                    // 반사 방향 계산
                    direction = Vector3.Reflect(direction, hit.normal).normalized;

                    // 다음 시작점 (자기 재충돌 방지)
                    origin = hit.point + direction * startOffset;

                    // 남은 거리 갱신
                    remainDistance -= hit.distance;

                    continue;
                }

                // 거울이 아니면 종료
                break;
            }
            else
            {
                // 아무것도 맞지 않았을 경우 끝점 추가 후 종료
                points[pointCount++] = origin + direction * remainDistance;
                break;
            }
        }

        // LineRenderer에 최종 포인트 적용
        lineRenderer.positionCount = pointCount;
        for (int i = 0; i < pointCount; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }
    }
}
