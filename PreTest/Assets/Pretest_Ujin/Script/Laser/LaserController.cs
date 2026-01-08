using UnityEngine;

public class LaserController : MonoBehaviour
{
    [Header("레이저 설정")]
    public float maxDistance = 500f;
    public int maxReflectionCount = 10;
    public float startOffset = 0.02f;

    [Header("참조 오브젝트")]
    public Transform muzzle;
    public LineRenderer lineRenderer;

    [Header("UI")]
    public LaserReflectionCounterUI reflectionCounterUI;

    // LaserController 클래스 멤버에 추가
    private MirrorSurface prevMirror;
    private ReceiverStateEffect prevReceiver;

    private void Awake()
    {
        if (muzzle == null)
            muzzle = GetComponentInChildren<BoxCollider>().transform;

        if (lineRenderer == null)
            lineRenderer = GetComponentInChildren<LineRenderer>();

        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;

        if (reflectionCounterUI == null)
            reflectionCounterUI = FindAnyObjectByType<LaserReflectionCounterUI>();

        if (reflectionCounterUI != null)
            reflectionCounterUI.Initialize(maxReflectionCount);
    }

    private void Update()
    {
        FireLaser();
    }


    private void FireLaser()
    {
        Transform t = (muzzle != null) ? muzzle : transform;

        Vector3 origin = t.position + t.forward * startOffset;
        Vector3 direction = t.forward;

        Vector3[] points = new Vector3[maxReflectionCount + 2];
        int pointCount = 0;
        points[pointCount++] = origin;

        float remainDistance = maxDistance;

        int remainReflection = maxReflectionCount;
        int usedReflectionThisFrame = 0;

        // 이번 프레임에 '레이저가 닿은' 마지막 mirror/receiver 기록
        MirrorSurface hitMirrorThisFrame = null;
        ReceiverStateEffect hitReceiverThisFrame = null;

        for (int bounce = 0; bounce <= maxReflectionCount; bounce++)
        {
            if (remainDistance <= 0.0001f) break;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, remainDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                points[pointCount++] = hit.point;

                // Receiver
                var receiver = hit.collider.GetComponentInParent<ReceiverStateEffect>();
                if (receiver != null)
                {
                    receiver.OnLaserHit();
                    hitReceiverThisFrame = receiver;
                }

                // Mirror
                var mirror = hit.collider.GetComponentInParent<MirrorSurface>();
                if (mirror != null && bounce < maxReflectionCount)
                {
                    usedReflectionThisFrame++;
                    remainReflection = maxReflectionCount - usedReflectionThisFrame;

                    hitMirrorThisFrame = mirror;

                    direction = Vector3.Reflect(direction, hit.normal).normalized;
                    origin = hit.point + direction * startOffset;
                    remainDistance -= hit.distance;
                    continue;
                }

                break;
            }
            else
            {
                points[pointCount++] = origin + direction * remainDistance;
                break;
            }
        }

        // Enter 순간에만 효과음 1회
        if (hitMirrorThisFrame != null && hitMirrorThisFrame != prevMirror)
            EffectSoundController.Play(0);

        if (hitReceiverThisFrame != null && hitReceiverThisFrame != prevReceiver)
            EffectSoundController.Play(1);

        // prev 갱신 (이번 프레임에 안 맞았으면 null로 떨어짐 → 다음에 다시 Enter 가능)
        prevMirror = hitMirrorThisFrame;
        prevReceiver = hitReceiverThisFrame;

        if (reflectionCounterUI != null)
            reflectionCounterUI.UpdateCounter(remainReflection);

        lineRenderer.positionCount = pointCount;
        for (int i = 0; i < pointCount; i++)
            lineRenderer.SetPosition(i, points[i]);
    }
}
