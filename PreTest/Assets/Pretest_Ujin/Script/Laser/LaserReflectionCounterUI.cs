using UnityEngine;
using TMPro;

/// <summary>
/// 레이저의 남은 반사 횟수를
/// 화면에 표시하기 위한 UI 컨트롤러
/// 
/// UI 오브젝트에 추가
/// 
/// 반사 횟수가 남아 있을 경우 숫자로 표시하고,
/// 모두 소진되면 경고 메시지와 색상을 변경한다.
/// </summary>
public class LaserReflectionCounterUI : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("반사 횟수를 표시할 TextMeshPro 텍스트")]
    public TextMeshProUGUI counterText;

    [Header("Color Settings")]
    [Tooltip("반사 횟수가 남아 있을 때의 색상")]
    public Color normalColor = Color.white;

    [Tooltip("반사 횟수가 모두 소진되었을 때의 색상")]
    public Color exhaustedColor = Color.red;

    /// <summary>
    /// UI를 초기화
    /// 최대 반사 횟수 설정
    /// 초기 상태 화면에 반영
    /// </summary>
    public void Initialize(int maxReflectionCount)
    {
        UpdateCounter(maxReflectionCount);
    }

    /// <summary>
    /// 남은 반사 횟수를 UI에 갱신
    /// </summary>
    /// <param name="remainCount">현재 남은 반사 횟수</param>
    public void UpdateCounter(int remainCount)
    {
        if (remainCount > 0)
        {
            // 반사 가능 횟수가 남아 있을 경우
            counterText.text = remainCount.ToString();
            counterText.color = normalColor;
        }
        else
        {
            // 반사 횟수를 모두 소진했을 경우
            counterText.text = "No Reflections Left";
            counterText.color = exhaustedColor;
        }
    }
}
