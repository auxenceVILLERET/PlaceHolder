using UnityEngine;
using UnityEngine.Playables;


public class EndCinema : MonoBehaviour
{
    [SerializeField] PlayableDirector _timeline;
    [SerializeField] GameObject _cinemachineCamera;

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
        _cinemachineCamera.SetActive(false);
    }
}
