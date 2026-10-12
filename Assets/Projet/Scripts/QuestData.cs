using UnityEngine;

public enum QuestAction
{
    Run,
    Dodge,
    Attack,
    KillEnemy,
    ReturnToNPC,
    FindRabbit
}

[CreateAssetMenu(fileName = "QuestData", menuName = "Scriptable Objects/QuestData")]
public class QuestData : ScriptableObject
{
    [Header("Display")]
    [SerializeField] string _title;
    [SerializeField, TextArea] string _description;
    [SerializeField] string _progressSuffix;

    [Header("Quest")]
    [SerializeField] QuestAction _action;
    [SerializeField] float _requiredAmount = 1f;

    [Header("Sequence")]
    [SerializeField] QuestData _nextQuest;
    [SerializeField] float _nextQuestDelay = 0f;

    public string Title => _title;
    public string Description => _description;
    public string ProgressSuffix => _progressSuffix;
    public QuestAction Action => _action;
    public float RequiredAmount => _requiredAmount;
    public QuestData NextQuest => _nextQuest;
    public float NextQuestDelay => _nextQuestDelay;

}
