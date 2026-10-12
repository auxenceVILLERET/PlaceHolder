using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CanvasGroup _panel;
    [SerializeField] TMP_Text _titleText;
    [SerializeField] TMP_Text _descriptionText;
    [SerializeField] TMP_Text _progressText;
    [SerializeField] Slider _progressBar;

    [Header("End Quest")]
    [SerializeField] string _completedLabel = "Quest Completed !";
    [SerializeField] float _hideDelay = 2f;
    [SerializeField] float _fadeDuration = 0.4f;

    private Coroutine _hideRoutine;

    void Awake()
    {
        _panel.alpha = 0f;
    }

    void OnEnable()
    {
        QuestManager manager = QuestManager.Instance;
        if(manager == null) return;

        manager.OnQuestStarted += OnQuestStarted;
        manager.OnProgressChanged += OnProgressChanged;
        manager.OnQuestCompleted += OnQuestCompleted;

        if(manager.CurrentQuest != null)
        {
            Show(manager.CurrentQuest, manager.CurrentAmount);
        }
    }

    void OnDisable()
    {
        QuestManager manager = QuestManager.Instance;
        if(manager == null) return;

        manager.OnQuestStarted -= OnQuestStarted;
        manager.OnProgressChanged -= OnProgressChanged;
        manager.OnQuestCompleted -= OnQuestCompleted;
    }

    void OnQuestStarted(QuestData quest)
    {
        Show(quest, 0f);
    }

    void OnProgressChanged(QuestData quest, float amount)
    {
        SetProgress(quest, amount);
    }

    void OnQuestCompleted(QuestData quest)
    {
        _progressBar.value = 1f;
        _progressText.text = _completedLabel;

        CancelHide();
        _hideRoutine = StartCoroutine(HideAfterDelay());
    }

    void Show(QuestData quest, float amount)
    {
        CancelHide();
        _panel.alpha = 1f;
        if(_titleText != null) _titleText.text = quest.Title;
        if(_descriptionText != null) _descriptionText.text = quest.Description;
        SetProgress(quest, amount);
    }

    void SetProgress(QuestData quest, float amount)
    {
        _progressBar.value = Mathf.Clamp01(amount / quest.RequiredAmount);

        string current = Mathf.FloorToInt(amount).ToString();
        string required = Mathf.CeilToInt(quest.RequiredAmount).ToString();
        _progressText.text = current + "/" + required + quest.ProgressSuffix;
    }

    void CancelHide()
    {
        if(_hideRoutine != null)
        {
            StopCoroutine(_hideRoutine);
            _hideRoutine = null;
        }
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSecondsRealtime(_hideDelay);

        float t = 0f;
        while(t < 1f)
        {
            t += Time.unscaledDeltaTime / Mathf.Max(_fadeDuration, 0.01f);
            _panel.alpha = 1f - Mathf.Clamp01(t);
            yield return null;
        }

        _hideRoutine = null;
    }
}