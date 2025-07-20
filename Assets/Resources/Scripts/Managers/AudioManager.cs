using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("-------Audio Source-------")]
    [SerializeField] AudioSource bgm;
    [SerializeField] AudioSource sfx;

    [Header("-------Audio Clip---------")]
    public AudioClip Ambient_Music;
    public AudioClip Snowball_Hit_SFX;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayBGM(Ambient_Music);
    }
    public void PlaySFX(AudioClip clip)
    {
        sfx.PlayOneShot(clip);
    }

    public void PlayBGM(AudioClip clip)
    {
        bgm.PlayOneShot(clip);
    }
}
