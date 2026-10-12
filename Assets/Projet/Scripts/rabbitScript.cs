using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class rabbitScript : MonoBehaviour
{
    [Header("Dependence")]
    [SerializeField] GameObject _visual; // Rabbit object to show (can be this object)
    [SerializeField] CinemachineCamera _rabbitCamera;

    [Header("Quest")]
    [SerializeField] QuestData _triggerQuest; // Quest that makes the rabbit appear

    [Header("Camera")]
    [SerializeField] float _cameraDuration = 2.5f;

    private bool _revealed;

    // Subscribed in Awake/OnDestroy (not OnEnable/OnDisable) so it still listens while hidden
    void Awake()
    {
        QuestManager manager = QuestManager.Instance;
        if(manager != null)
        {
            manager.OnQuestStarted += OnQuestStarted;
        }

        if(_visual != null) _visual.SetActive(false);

        // Quest already running (e.g. scene reloaded): show the rabbit without the camera shot
        if(manager != null && manager.CurrentQuest == _triggerQuest)
        {
            Reveal(false);
        }
    }

    void OnDestroy()
    {
        QuestManager manager = QuestManager.Instance;
        if(manager == null) return;

        manager.OnQuestStarted -= OnQuestStarted;
    }

    void OnQuestStarted(QuestData quest)
    {
        if(quest == _triggerQuest)
        {
            Reveal(true);
        }
    }

    void Reveal(bool withCamera)
    {
        if(_revealed) return;
        _revealed = true;

        if(_visual != null) _visual.SetActive(true);

        if(withCamera && _rabbitCamera != null && isActiveAndEnabled)
        {
            StartCoroutine(ShowCamera());
        }
    }

    IEnumerator ShowCamera()
    {
        // Wait for the rabbit's DialogueInteractable.Start, which resets its camera priority
        yield return null;

        int previousPriority = _rabbitCamera.Priority.Value;
        _rabbitCamera.Priority = 100000000;
        yield return new WaitForSeconds(_cameraDuration);
        _rabbitCamera.Priority = previousPriority;
    }
}
