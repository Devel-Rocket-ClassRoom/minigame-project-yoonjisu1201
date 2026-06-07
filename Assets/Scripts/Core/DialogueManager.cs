using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private DialogueUI _dialogueUI;
    [SerializeField] private RankUpPopupUI _rankUpPopup;
    [SerializeField] private List<DialogueSO> _dialogues;

    private Dictionary<string, DialogueSO> _dialogueById;
    private string _pendingId;

    public bool IsDialoguePlaying => _dialogueUI != null && _dialogueUI.IsPlaying;

    private const string SEEN_KEY = "seen_dialogues";

    private void Awake()
    {
        Instance = this;
        BuildDictionary();
    }

    private void Start()
    {
        UnlockManager.instance.OnGhostUnlocked += OnGhostUnlocked;
        if (_rankUpPopup != null)
            _rankUpPopup.OnClosed += OnPopupClosed;
    }

    private void OnDestroy()
    {
        Instance = null;
        if (UnlockManager.instance != null)
            UnlockManager.instance.OnGhostUnlocked -= OnGhostUnlocked;
        if (_rankUpPopup != null)
            _rankUpPopup.OnClosed -= OnPopupClosed;
    }

    private void BuildDictionary()
    {
        _dialogueById = new Dictionary<string, DialogueSO>();

        foreach (var dialogue in _dialogues)
        {
            if (dialogue == null) continue;
            if (string.IsNullOrEmpty(dialogue.id)) continue;
            if (_dialogueById.ContainsKey(dialogue.id)) continue;
            _dialogueById.Add(dialogue.id, dialogue);
        }
    }

    public void Play(string dialogueId)
    {
        if (string.IsNullOrEmpty(dialogueId)) return;
        if (WasSeen(dialogueId)) return;

        if (_rankUpPopup != null && _rankUpPopup.IsOpen)
        {
            _pendingId = dialogueId;
            return;
        }

        if (_dialogueById.TryGetValue(dialogueId, out DialogueSO dialogue))
        {
            MarkSeen(dialogueId);
            _dialogueUI.Play(dialogue);
        }
        else
            Debug.LogWarning($"해당 id의 대화를 찾을 수 없습니다: {dialogueId}");
    }

    public static void ResetSeenDialogues()
    {
        PlayerPrefs.DeleteKey(SEEN_KEY);
    }

    private void OnPopupClosed()
    {
        if (string.IsNullOrEmpty(_pendingId)) return;
        string id = _pendingId;
        _pendingId = null;
        Play(id);
    }

    private void OnGhostUnlocked(GhostSO ghost)
    {
        Play(ghost.firstMeetDialogueID);
    }

    private bool WasSeen(string id)
    {
        string seen = PlayerPrefs.GetString(SEEN_KEY, "|");
        return seen.Contains("|" + id + "|");
    }

    private void MarkSeen(string id)
    {
        string seen = PlayerPrefs.GetString(SEEN_KEY, "|");
        PlayerPrefs.SetString(SEEN_KEY, seen + id + "|");
    }
}
