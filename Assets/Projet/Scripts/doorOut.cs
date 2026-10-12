using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class DoorOut : Interactable
{
    [SerializeField] private string _sceneName;


    public void LoadScene()
    {
        SceneManager.LoadScene(_sceneName);
    }

    public override void Interact(GameObject interactor)
    {
        LoadScene();
    }


}
