using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)] 
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public QuestData CurrentQuest { get; private set; }
    public float CurrentAmount { get; private set; }

    public event Action<QuestData> OnQuestStarted;
    public event Action<QuestData, float> OnProgressChanged;
    public event Action<QuestData> OnQuestCompleted;

    private readonly HashSet<QuestData> _completed = new HashSet<QuestData>();

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); 
    }

    void OnDestroy()
    {
        if(Instance == this) Instance = null;
    }

    public static void Report(QuestAction action, float amount = 1f)
    {
        if(Instance == null) return;
        Instance.AddProgress(action, amount);
    }

    public bool IsCompleted(QuestData quest)
    {
        return quest != null && _completed.Contains(quest);
    }

    public bool StartQuest(QuestData quest)
    {
        if(quest == null || CurrentQuest != null || _completed.Contains(quest)) return false;

        CurrentQuest = quest;
        CurrentAmount = 0f;
        OnQuestStarted?.Invoke(quest);
        return true;
    }

    void AddProgress(QuestAction action, float amount)
    {
        if(CurrentQuest == null || CurrentQuest.Action != action) return;

        CurrentAmount = Mathf.Min(CurrentAmount + amount, CurrentQuest.RequiredAmount);
        OnProgressChanged?.Invoke(CurrentQuest, CurrentAmount);

        if(CurrentAmount >= CurrentQuest.RequiredAmount)
        {
            Complete();
        }
    }

    void Complete()
    {
        QuestData finished = CurrentQuest;
        CurrentQuest = null;
        CurrentAmount = 0f;
        _completed.Add(finished);
        OnQuestCompleted?.Invoke(finished);

        if(finished.NextQuest != null)
        {
            StartCoroutine(StartNextAfterDelay(finished.NextQuest, finished.NextQuestDelay));
        }
    }

    IEnumerator StartNextAfterDelay(QuestData next, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartQuest(next);
    }
}