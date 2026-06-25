using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseSaveLoadTestUI : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] private Button _saveButton;
    [SerializeField] private Button _loadButton;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _statusText;

    private void Start()
    {
        _saveButton.onClick.AddListener(() => OnSaveClickedAsync().Forget());
        _loadButton.onClick.AddListener(() => OnLoadClickedAsync().Forget());
    }

    private async UniTaskVoid OnSaveClickedAsync()
    {
        if (SaveManager.Instance == null)
        {
            SetStatus("SaveManager가 없습니다.");
            return;
        }

        SetButtonsInteractable(false);
        SetStatus("Firebase 저장 중...");

        bool success = await SaveManager.Instance.SaveToFirebaseAsync();

        SetStatus(success ? "Firebase 저장 완료!" : "Firebase 저장 실패");
        SetButtonsInteractable(true);
    }

    private async UniTaskVoid OnLoadClickedAsync()
    {
        if (SaveManager.Instance == null)
        {
            SetStatus("SaveManager가 없습니다.");
            return;
        }

        SetButtonsInteractable(false);
        SetStatus("Firebase 불러오기 중...");

        bool success = await SaveManager.Instance.LoadFromFirebaseAsync();

        SetStatus(success ? "Firebase 불러오기 완료!" : "Firebase 저장 데이터 없음 또는 불러오기 실패");
        SetButtonsInteractable(true);
    }

    private void SetButtonsInteractable(bool interactable)
    {
        if (_saveButton != null)
            _saveButton.interactable = interactable;

        if (_loadButton != null)
            _loadButton.interactable = interactable;
    }

    private void SetStatus(string message)
    {
        if (_statusText != null)
            _statusText.text = message;

        Debug.Log($"[FirebaseSaveLoadTestUI] {message}");
    }
}