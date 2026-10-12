using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class DoorScript : Interactable
{
    [SerializeField] private string _sceneName;
    [SerializeField] PlayableDirector _timeline;

    void Awake()
    {
        if(_timeline != null)
        {
            _timeline.stopped += OnTimelineStopped;
        }
    }

    void OnDestroy()
    {
        if(_timeline != null)
        {
            _timeline.stopped -= OnTimelineStopped;
        }
    }

    void OnTimelineStopped(PlayableDirector director)
    {
        LoadScene();
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(_sceneName);
    }

    public override void Interact(GameObject interactor)
    {
        StartTimeline();
    }

    void StartTimeline()
    {
        if(_timeline != null)
        {
            _timeline.Play();
        }
    }

}
