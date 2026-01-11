using UnityEngine;

/// <summary>
/// 레이저가 Receiver에 맞았을 때
/// 시각적 피드백(이펙트, 스케일, 색상)을 처리하는 컴포넌트
/// 
/// - 레이저 히트 시: 이펙트 활성화 + 커짐 + 색상 변경
/// - 히트가 끊기면: 원래 상태로 복귀
/// </summary>
public class ReceiverStateEffect : MonoBehaviour
{
    // 프레임에 레이저가 맞았는지 여부
    private bool isHit;

    [Header("히트 이펙트 오브젝트")]
    public GameObject effectObject;              // 레이저 히트 시 활성화될 이펙트 오브젝트

    [Header("스케일 이펙트 설정")]
    public float hitScaleMultiplier = 1.25f;    // 레이저에 맞았을 때 커지는 비율
    public float scaleSmoothTime = 0.12f;       // 스케일 변화의 부드러움 정도 (값이 작을수록 빠름)

    [Header("색상 이펙트 설정")]
    public Color hitColor = Color.red;          // 레이저에 맞았을 때 변경될 색상


    #region 내부 상태 값
    // 원래 스케일 값
    private Vector3 originalScale;

    // 목표 스케일 값
    private Vector3 targetScale;

    // SmoothDamp 계산용 속도 값
    private Vector3 scaleVelocity;

    // 색상 변경을 위한 Renderer 캐시
    private Renderer cachedRenderer;

    // 원래 색상 값
    private Color originalColor;
    #endregion

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
        UpdateScaleEffect();
    }
    private void LateUpdate()
    {
        // 이번 프레임에 한 번도 맞지 않았다면 상태 해제
        if (!isHit)
            OnLaserNotHit();

        // 다음 프레임을 위해 히트 상태 초기화
        isHit = false;
    }

    #region 스케일 이펙트
    // 현재 스케일을 목표 스케일로 부드럽게 보간
    // 풍선처럼 커졌다 작아지는 느낌 연출
    private void UpdateScaleEffect()
    {
        transform.localScale = Vector3.SmoothDamp(
            transform.localScale,
            targetScale,
            ref scaleVelocity,
            scaleSmoothTime
        );
    }
    #endregion

    #region 레이저 히트 처리
    /// <summary>
    /// 레이저가 Receiver에 맞았을 때 호출
    /// LaserController.cs 에서 매 프레임 호출
    /// </summary>
    public void OnLaserHit()
    {
        // 이번 프레임의 첫 히트일 때만 이펙트/색상 변경
        if (!isHit)
        {
            if (effectObject != null)
                effectObject.SetActive(true);

            if (cachedRenderer != null && cachedRenderer.material.HasProperty("_Color"))
                cachedRenderer.material.color = hitColor;
        }

        // 맞았을 때 목표 스케일을 크게 설정
        targetScale = originalScale * hitScaleMultiplier;

        // 히트 상태 기록
        isHit = true;
    }
    /// <summary>
    /// 레이저가 맞지 않았을 때 처리
    /// - 이펙트 비활성화
    /// - 스케일 및 색상 원래 상태로 복구
    /// </summary>
    public void OnLaserNotHit()
    {
        if (effectObject != null)
            effectObject.SetActive(false);

        // 스케일 원복
        targetScale = originalScale;

        // 색상 원복
        if (cachedRenderer != null && cachedRenderer.material.HasProperty("_Color"))
            cachedRenderer.material.color = originalColor;
    }
    #endregion
}
