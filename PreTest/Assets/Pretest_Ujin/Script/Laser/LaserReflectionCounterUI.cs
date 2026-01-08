using UnityEngine;
using TMPro;

public class LaserReflectionCounterUI : MonoBehaviour
{
    public TextMeshProUGUI counterText;

    public Color normalColor = Color.white;
    public Color exhaustedColor = Color.red;

    private int maxCount;

    /// <summary>
    /// 초기화
    /// </summary>
    public void Initialize(int maxReflectionCount)
    {
        maxCount = maxReflectionCount;
        UpdateCounter(maxCount);
    }

    /// <summary>
    /// 남은 횟수 갱신
    /// </summary>
    public void UpdateCounter(int remainCount)
    {
        if (remainCount > 0)
        {
            counterText.text = remainCount.ToString();
            counterText.color = normalColor;
        }
        else
        {
            counterText.text = "No Reflections Left";
            counterText.color = exhaustedColor;
        }

    }
}
