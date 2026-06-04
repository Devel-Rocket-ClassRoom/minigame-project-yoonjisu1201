using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;

    private const string BGM_KEY = "BGMVolume";
    private const string SFX_KEY = "SFXVolume";

    public float BGMVolume { get; private set; }
    public float SFXVolume { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        BGMVolume = PlayerPrefs.GetFloat(BGM_KEY, 1f);
        SFXVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);
        ApplyVolumes();
    }

    public void PlayBGM(AudioClip clip)
    {
        if (_bgmSource == null) return;
        _bgmSource.clip = clip;
        _bgmSource.loop = true;
        _bgmSource.Play();
    }

    public void StopBGM()
    {
        if (_bgmSource != null) _bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (_sfxSource != null && clip != null)
            _sfxSource.PlayOneShot(clip, SFXVolume);
    }

    public void SetBGMVolume(float volume)
    {
        BGMVolume = volume;
        PlayerPrefs.SetFloat(BGM_KEY, volume);
        if (_bgmSource != null) _bgmSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        SFXVolume = volume;
        PlayerPrefs.SetFloat(SFX_KEY, volume);
        if (_sfxSource != null) _sfxSource.volume = volume;
    }

    private void ApplyVolumes()
    {
        if (_bgmSource != null) _bgmSource.volume = BGMVolume;
        if (_sfxSource != null) _sfxSource.volume = SFXVolume;
    }
}
