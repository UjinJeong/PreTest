using UnityEngine;

public class ReceiverStateEffect : MonoBehaviour
{
    /* ===============================
     * State
     * =============================== */
    private bool isHit;   // 이번 프레임에 레이저가 맞았는지 여부

    /* ===============================
     * Effect Object
     * =============================== */
    [Header("Effect")]
    public GameObject effectObject;   // 레이저 히트 시 켜질 이펙트 오브젝트

    /* ===============================
     * Scale Effect (풍선처럼 커졌다 작아짐)
     * =============================== */
    [Header("Scale (Balloon)")]
    public float hitScaleMultiplier = 1.25f; // 맞았을 때 커지는 비율
    public float scaleSmoothTime = 0.12f;    // 스케일 변화 속도 (작을수록 빠름)

    /* ===============================
     * Color Effect
     * =============================== */
    [Header("Color Effect")]
    public Color hitColor = Color.red;        // 맞았을 때 색상

    /* ===============================
     * Cached Values
     * =============================== */
    private Vector3 originalScale;             // 원래 스케일
    private Vector3 targetScale;               // 목표 스케일
    private Vector3 scaleVelocity;             // SmoothDamp 내부 계산용

    private Renderer cachedRenderer;            // 색상 변경용 Renderer
    private Color originalColor;                // 원래 색상

    private void Start()
    {
        // 초기 스케일 저장
        originalScale = transform.localScale;
        targetScale = originalScale;

        // Renderer 캐싱 (자식 포함)
        cachedRenderer = GetComponentInChildren<Renderer>();
        if (cachedRenderer != null && cachedRenderer.material.HasProperty("_Color"))
        {
            originalColor = cachedRenderer.material.color;
        }
    }

    private void Update()
    {
        // 현재 스케일 -> 목표 스케일로 부드럽게 변화 (풍선 느낌)
        transform.localScale = Vector3.SmoothDamp(
            transform.localScale,
            targetScale,
            ref scaleVelocity,
            scaleSmoothTime
        );
    }

    /// <summary>
    /// 레이저가 Receiver에 맞았을 때 호출
    /// </summary>
    public void OnLaserHit()
    {
        // 이번 프레임 첫 히트일 때만 이펙트/색상 변경
        if (!isHit)
        {
            if (effectObject != null)
                effectObject.SetActive(true);

            // 색상 변경
            if (cachedRenderer != null && cachedRenderer.material.HasProperty("_Color"))
                cachedRenderer.material.color = hitColor;
        }

        // 맞았을 때 목표 스케일을 크게 설정
        targetScale = originalScale * hitScaleMultiplier;

        isHit = true;
    }

    private void LateUpdate()
    {
        // 레이저가 이번 프레임에 맞지 않았다면 상태 해제
        if (!isHit)
            OnLaserNotHit();

        // 다음 프레임을 위해 초기화
        isHit = false;
    }

    /// <summary>
    /// 레이저가 맞지 않았을 때 처리
    /// </summary>
    public void OnLaserNotHit()
    {
        if (effectObject != null)
            effectObject.SetActive(false);

        // 스케일 원래대로
        targetScale = originalScale;

        // 색상 원복
        if (cachedRenderer != null && cachedRenderer.material.HasProperty("_Color"))
            cachedRenderer.material.color = originalColor;
    }
}
