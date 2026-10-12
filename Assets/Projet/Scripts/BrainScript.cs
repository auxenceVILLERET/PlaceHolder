using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Runtime.Serialization;

public class BrainScript : MonoBehaviour
{
    [Header("Dependence")]
    [SerializeField] GameObject _moveEntity;
    [SerializeField] GameObject _attackEntity;
    [SerializeField] GameObject _interactEntity;

    [Header("Input")]
    [SerializeField] InputActionReference _moveInput;
    [SerializeField] InputActionReference _runInput;
    [SerializeField] InputActionReference _attackInput;
    [SerializeField] InputActionReference _dodgeInput;
    [SerializeField] InputActionReference _interactInput;
    
    public InputAction RunInput => _runInput.action;
    public InputAction AttackInput => _attackInput.action;
    public InputAction DodgeInput => _dodgeInput.action;
    public InputAction InteractInput => _interactInput.action;

    void OnEnable()
    {
        if(_moveInput != null)
        {
            _moveInput.action.performed += UpdateMove;
            _moveInput.action.canceled += StopMove;
        }
        if(_runInput != null)
        {
            _runInput.action.performed += UpdateMove;
            _runInput.action.canceled += UpdateMove;
        }
        if(_attackInput != null)
        {
            _attackInput.action.started += Attack;
        }
        if(_dodgeInput != null)
        {
            _dodgeInput.action.started += Dodge;
        }
        if(_interactInput != null)
        {
            _interactInput.action.started += Interact;
        }
    }

    void OnDisable()
    {
        if(_moveInput != null)
        {
            _moveInput.action.performed -= UpdateMove;
            _moveInput.action.canceled -= StopMove;
        }
        if(_runInput != null)
        {
            _runInput.action.performed -= UpdateMove;
            _runInput.action.canceled -= UpdateMove;
        }
        if(_attackInput != null)
        {
            _attackInput.action.started -= Attack;
        }
        if(_dodgeInput != null)
        {
            _dodgeInput.action.started -= Dodge;
        }
        if(_interactInput != null)
        {
            _interactInput.action.started -= Interact;
        }
    }

    void UpdateMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = _moveInput.action.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        bool isRunning = _runInput.action.IsPressed();
        if(isRunning)
        {
            _moveEntity.GetComponent<MoveScript>().SetRunDirection(moveDirection);
        }
        else
        {
            _moveEntity.GetComponent<MoveScript>().SetDirection(moveDirection);
        }

    }

    void StopMove(InputAction.CallbackContext context)
    {
        _moveEntity.GetComponent<MoveScript>().SetDirection(Vector3.zero);
    }

    void Attack(InputAction.CallbackContext context)
    {
        if(_attackEntity.GetComponent<AttackScript>().Attack())
        {
            QuestManager.Report(QuestAction.Attack);
        }
    }

    void Dodge(InputAction.CallbackContext context)
    {
        Vector2 input = _moveInput.action.ReadValue<Vector2>();
        Vector3 dodgeDirection = new Vector3(input.x, 0f, input.y);
        if(_moveEntity.GetComponent<MoveScript>().Dodge(dodgeDirection))
        {
            QuestManager.Report(QuestAction.Dodge);
        }
    }

    void Interact(InputAction.CallbackContext context)
    {
        _interactEntity.GetComponent<InteractScript>().Interact();
    }

    void Update()
    {
        if(_runInput == null || _moveInput == null) return;
        bool isRunning = _runInput.action.IsPressed() && _moveInput.action.ReadValue<Vector2>().magnitude > 0.01f;
        if(isRunning)
        {
            QuestManager.Report(QuestAction.Run, Time.deltaTime);
        }
    }
}
