using UnityEngine;

/// <summary>
/// 전역 효과음 재생용 컨트롤러
/// 어디서든 static 메서드로 효과음 재생 가능
/// </summary>
public class EffectSoundController : MonoBehaviour
{
    public static EffectSoundController Instance;

    [Header("Effect Sounds")]
    public AudioClip[] audioClips;

    private AudioSource audioSource;

    private void Start()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// 효과음 재생 (index 기반)
    /// </summary>
    public static void Play(int index)
    {
        Instance?.audioSource.PlayOneShot(Instance.audioClips[index]);
    }
}

