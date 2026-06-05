using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingsPanelUI : MonoBehaviour
{
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private TextMeshProUGUI _bgmPercentText;
    [SerializeField] private TextMeshProUGUI _sfxPercentText;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _closeButton;

    private void Awake()
    {
        _bgmSlider.onValueChanged.AddListener(OnBGMChanged);
        _sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        _quitButton.onClick.AddListener(GoToTitle);
        _closeButton.onClick.AddListener(Close);
    }

    private void OnEnable()
    {
        if (AudioManager.instance == null) return;
        _bgmSlider.SetValueWithoutNotify(AudioManager.instance.BGMVolume);
        _sfxSlider.SetValueWithoutNotify(AudioManager.instance.SFXVolume);
        _bgmPercentText.text = $"{Mathf.RoundToInt(AudioManager.instance.BGMVolume * 100)}%";
        _sfxPercentText.text = $"{Mathf.RoundToInt(AudioManager.instance.SFXVolume * 100)}%";
    }

    private void OnBGMChanged(float value)
    {
        AudioManager.instance?.SetBGMVolume(value);
        if (_bgmPercentText != null) _bgmPercentText.text = $"{Mathf.RoundToInt(value * 100)}%";
    }

    private void OnSFXChanged(float value)
    {
        AudioManager.instance?.SetSFXVolume(value);
        if (_sfxPercentText != null) _sfxPercentText.text = $"{Mathf.RoundToInt(value * 100)}%";
    }

    private void GoToTitle()
    {
        SaveManager.Instance.Save();
        SceneManager.LoadScene("Title");
    }

    public void Open() => gameObject.SetActive(true);
    public void Close() => gameObject.SetActive(false);
}
