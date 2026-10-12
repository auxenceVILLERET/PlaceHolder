using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    [Header("Dependence")]
    [SerializeField] DialogueInteractable _dialogue; // Rabbit dialogue that ends the game
    [SerializeField] GameObject _player;
    [SerializeField] GameObject _endScreen;      // End canvas, disabled until the end
    [SerializeField] CanvasGroup _blackScreen;   // Full screen black image
    [SerializeField] Graphic _endText;           // "Merci d'avoir joué !" (TMP text works)

    [Header("Timing")]
    [SerializeField] float _fadeDuration = 1.5f;
    [SerializeField] float _textFadeDuration = 1f;
    [SerializeField] float _inputDelay = 1f;     // Avoids quitting with the press that ended the dialogue

    private bool _canQuit;
    private bool _ending;

    // Subscribed in Awake/OnDestroy so it works even if this sits on the hidden rabbit
    void Awake()
    {
        if(_dialogue != null) _dialogue.OnDialogueFinished += OnDialogueFinished;
    }

    void OnDestroy()
    {
        if(_dialogue != null) _dialogue.OnDialogueFinished -= OnDialogueFinished;
    }

    void OnDialogueFinished()
    {
        if(_ending) return;
        _ending = true;

        // Prevent the dialogue from being restarted during the ending
        _dialogue.OnDialogueFinished -= OnDialogueFinished;
        Destroy(_dialogue);
        _dialogue = null;

        LockPlayer();
        StartCoroutine(EndSequence());
    }

    void LockPlayer()
    {
        if(_player == null) return;

        MoveScript move = _player.GetComponentInChildren<MoveScript>();
        if(move != null) move.SetLocked(true);

        AttackScript attack = _player.GetComponentInChildren<AttackScript>();
        if(attack != null) attack.SetLocked(true);
    }

    IEnumerator EndSequence()
    {
        if(_blackScreen != null) _blackScreen.alpha = 0f;
        SetTextAlpha(0f);
        if(_endScreen != null) _endScreen.SetActive(true);

        yield return Fade(_fadeDuration, a => { if(_blackScreen != null) _blackScreen.alpha = a; });
        yield return Fade(_textFadeDuration, SetTextAlpha);
        yield return new WaitForSecondsRealtime(_inputDelay);
        _canQuit = true;
    }

    void Update()
    {
        if(!_canQuit) return;

        bool keyboardPressed = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
        bool gamepadPressed = Gamepad.current != null
            && (Gamepad.current.buttonSouth.wasPressedThisFrame || Gamepad.current.startButton.wasPressedThisFrame);

        if(keyboardPressed || gamepadPressed)
        {
            Quit();
        }
    }

    void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator Fade(float duration, Action<float> apply)
    {
        float t = 0f;
        while(t < 1f)
        {
            t += Time.unscaledDeltaTime / Mathf.Max(duration, 0.01f);
            apply(Mathf.Clamp01(t));
            yield return null;
        }
    }

    void SetTextAlpha(float alpha)
    {
        if(_endText == null) return;

        Color color = _endText.color;
        color.a = alpha;
        _endText.color = color;
    }
}
