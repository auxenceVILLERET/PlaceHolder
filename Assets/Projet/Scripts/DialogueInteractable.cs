using UnityEngine;
using Unity.Cinemachine;
using System;

public class DialogueInteractable : Interactable
{
    [Header("Dependence")]
    [SerializeField] DialogueBubble _bubble;
    [SerializeField] CinemachineCamera _dialogueCamera;

    [Header("Dialogue")]
    [SerializeField, TextArea] string[] _lines;
    [SerializeField] bool _followThis = true;

    private int _currentLine = -1;
    private int _firstLine = 0;
    private int _lastLine = -1; // -1 = until the last line
    private MoveScript _interactorMove;
    private AttackScript _interactorAttack;

    public bool IsTalking => _currentLine >= 0;
    public override bool IsInteracting => IsTalking;
    public event Action OnDialogueFinished;

    // Restricts the next dialogue to lines [first, last] (inclusive)
    public void SetLineRange(int first, int last)
    {
        if(IsTalking) return;
        _firstLine = Mathf.Max(first, 0);
        _lastLine = last;
    }

    void Start()
    {
        if(_bubble != null && _followThis)
        {
            _bubble.SetTarget(transform);
        }
        if(_dialogueCamera != null)
        {
            _dialogueCamera.Priority = 10000;
        }
    }

    public override void Interact(GameObject interactor)
    {
        if(_bubble == null || _lines == null || _lines.Length == 0) return;

        // First press finishes the current line instead of skipping it
        if(_bubble.isTyping)
        {
            _bubble.Skip();
            return;
        }

        int lastLine = _lastLine < 0 ? _lines.Length - 1 : Mathf.Min(_lastLine, _lines.Length - 1);

        if(!IsTalking)
        {
            _currentLine = _firstLine;
            StartDialogue(interactor);
        }
        else
        {
            _currentLine++;
        }

        if(_currentLine > lastLine)
        {
            EndInteraction();
            OnDialogueFinished?.Invoke();
            return;
        }

        _bubble.Show(_lines[_currentLine]);
    }

    void StartDialogue(GameObject interactor)
    {
        if(_dialogueCamera != null)
        {
            _dialogueCamera.Priority = 10000000;
        }

        _interactorMove = interactor.GetComponentInChildren<MoveScript>();
        if(_interactorMove == null)
        {
            _interactorMove = interactor.GetComponentInParent<MoveScript>();
        }
        if(_interactorMove != null)
        {
            _interactorMove.SetLocked(true);
        }

        _interactorAttack = interactor.GetComponentInChildren<AttackScript>();
        if(_interactorAttack == null)
        {
            _interactorAttack = interactor.GetComponentInParent<AttackScript>();
        }
        if(_interactorAttack != null)
        {
            _interactorAttack.SetLocked(true);
        }
    }

    public override void EndInteraction()
    {
        _currentLine = -1;
        if(_bubble != null)
        {
            _bubble.Hide();
        }
        if(_dialogueCamera != null)
        {
            _dialogueCamera.Priority = 10000;
        }
        if(_interactorMove != null)
        {
            _interactorMove.SetLocked(false);
            _interactorMove = null;
        }
        if(_interactorAttack != null)
        {
            _interactorAttack.SetLocked(false);
            _interactorAttack = null;
        }
    }
}
