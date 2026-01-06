using UnityEngine;

public class LaserSimple : MonoBehaviour
{
    [Header("Laser Settings")]
    public float maxDistance = 100f;

    [Header("References")]
    public Transform muzzle;                 // ✅ 레이저 시작점(총구)
    public LineRenderer lineRenderer;

    [Header("Raycast")]
    public float startOffset = 0.02f;        // ✅ 자기 콜라이더에 겹쳐서 이상해지는 것 방지

    private void Awake()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponentInChildren<LineRenderer>();

        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        FireLaser();
    }

    private void FireLaser()
    {
        // muzzle이 없으면 현재 오브젝트 기준으로 fallback
        Transform t = (muzzle != null) ? muzzle : transform;

        Vector3 origin = t.position + t.forward * startOffset;
        Vector3 direction = t.forward;

        Vector3 endPoint = origin + direction * maxDistance;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            //Debug.Log($"HIT: {hit.collider.name}  layer:{LayerMask.LayerToName(hit.collider.gameObject.layer)}");
            endPoint = hit.point;
        }

        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, endPoint);
    }
}
