using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField] DialogueInteractable _dialogue;
    [SerializeField] QuestData _quest;

    [Header("Return")]
    [SerializeField] QuestData _returnQuest; // ReturnToNPC quest completed by talking to this NPC
    [SerializeField] Vector2Int _firstVisitLines = new Vector2Int(0, 4); // x = first line, y = last line
    [SerializeField] Vector2Int _returnLines = new Vector2Int(5, 7);
    [SerializeField] Vector2Int _afterReturnLines = new Vector2Int(7, 7); // Said once the return quest is done

    void OnEnable()
    {
        if(_dialogue != null) _dialogue.OnDialogueFinished += OnDialogueFinished;

        QuestManager manager = QuestManager.Instance;
        if(manager != null)
        {
            manager.OnQuestStarted += OnQuestChanged;
            manager.OnQuestCompleted += OnQuestChanged;
        }

        UpdateDialogueLines();
    }

    void OnDisable()
    {
        if(_dialogue != null) _dialogue.OnDialogueFinished -= OnDialogueFinished;

        QuestManager manager = QuestManager.Instance;
        if(manager != null)
        {
            manager.OnQuestStarted -= OnQuestChanged;
            manager.OnQuestCompleted -= OnQuestChanged;
        }
    }

    void OnQuestChanged(QuestData quest)
    {
        UpdateDialogueLines();
    }

    bool IsWaitingForReturn()
    {
        QuestManager manager = QuestManager.Instance;
        return manager != null && _returnQuest != null && manager.CurrentQuest == _returnQuest;
    }

    void UpdateDialogueLines()
    {
        // Without a return quest the NPC keeps its full dialogue
        if(_dialogue == null || _returnQuest == null) return;

        Vector2Int range = _firstVisitLines;
        if(IsWaitingForReturn())
        {
            range = _returnLines;
        }
        else if(QuestManager.Instance != null && QuestManager.Instance.IsCompleted(_returnQuest))
        {
            range = _afterReturnLines;
        }
        _dialogue.SetLineRange(range.x, range.y);
    }

    void OnDialogueFinished()
    {
        QuestManager manager = QuestManager.Instance;
        if(manager == null) return;

        // Player came back to this NPC: complete the return quest instead of giving a new one
        if(IsWaitingForReturn())
        {
            QuestManager.Report(QuestAction.ReturnToNPC);
            return;
        }

        manager.StartQuest(_quest);
    }
}
