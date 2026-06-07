using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _speakerNameText;
    [SerializeField] private TextMeshProUGUI _lineText;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Image _speakerImage;

    private DialogueSO _currentDialogue;
    private int _lineIndex;

    public bool IsPlaying => _panel.activeSelf;

    private void Awake()
    {
        _panel.SetActive(false);
        _nextButton.onClick.AddListener(ShowNextLine);
    }

    public void Play(DialogueSO dialogue)
    {
        if (dialogue == null) return;

        _currentDialogue = dialogue;
        _lineIndex = 0;

        _panel.SetActive(true);
        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        string lineKey = $"{_currentDialogue.id}_line_{_lineIndex}";
        if (!LocalizationManager.HasKey(lineKey))
        {
            Close();
            return;
        }

        int speakerIndex = _currentDialogue.lineSpeakerIndices[_lineIndex];
        _speakerNameText.text = LocalizationManager.Get(_currentDialogue.speakerNames[speakerIndex]);
        _speakerImage.sprite = _currentDialogue.speakerSprites[speakerIndex];
        _lineText.text = LocalizationManager.GetDialogueLine(_currentDialogue.id, _lineIndex);
    }

    private void ShowNextLine()
    {
        if (_currentDialogue == null) return;
        _lineIndex++;
        ShowCurrentLine();
    }
    private void Close()
    {
        _panel.SetActive(false);
        _currentDialogue = null;
        _lineIndex = 0;
    }
}
