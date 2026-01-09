using UnityEngine;

/// <summary>
/// 게임 내 효과음을 재생하기 위한 컨트롤러
/// 
/// 씬에 하나만 존재하는 싱글톤 구조로 사용되며,
/// AudioClip 배열에 등록된 효과음을 index로 재생한다.
/// </summary>
public class EffectSoundController : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static EffectSoundController Instance;

    [Header("Effect Sounds")]
    [Tooltip("재생할 효과음 클립 배열 (index 기준)")]
    public AudioClip[] audioClips;
    private AudioSource audioSource;

    private void Start()
    {
        // 이미 인스턴스가 존재하면 중복 생성 방지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // AudioSource가 없으면 자동으로 추가
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 효과음 재생
    /// audioClips 배열의 index 사용
    /// </summary>
    public void Play(int index)
    {
        // AudioSource가 존재할 경우에만 안전하게 재생
        audioSource?.PlayOneShot(audioClips[index]);
    }
}
