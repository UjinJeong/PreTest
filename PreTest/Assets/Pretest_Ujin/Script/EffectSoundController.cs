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

    private void Awake()
    {
        // 싱글톤 처리
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// 효과음 재생 (index 기반)
    /// </summary>
    public static void Play(int index)
    {
        if (Instance == null)
        {
            Debug.Log("EffectSoundController is not initialized.");
            return;
        }

        if (index < 0 || index >= Instance.audioClips.Length)
        {
            Debug.Log($"Invalid sound index : {index}");
            return;
        }

        Instance.audioSource.PlayOneShot(Instance.audioClips[index]);
    }
}

