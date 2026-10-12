using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(1000)]
public class InteractScript : MonoBehaviour
{
    [Header("Dependence")]
    [SerializeField] GameObject _entity;
    [SerializeField] GameObject _interactIconKeyboard;
    [SerializeField] GameObject _interactIconController;

    [Header("Hint")]
    [SerializeField] Transform _hintRoot;
    [SerializeField] bool _constantScreenSize = true;
    [SerializeField] float _sizeFactor = 0.0015f;

    private bool _usingController;
    private Camera _cam;
    private Interactable _hintTarget;

    private readonly List<Interactable> _inRange = new List<Interactable>();

    void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;
    }

    void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;
    }

    void OnActionChange(object obj, InputActionChange change)
    {
        if(change != InputActionChange.ActionPerformed) return;

        InputAction action = obj as InputAction;
        if(action == null || action.activeControl == null) return;

        InputDevice device = action.activeControl.device;
        if(device is Gamepad)
        {
            _usingController = true;
        }
        else if(device is Keyboard || device is Mouse)
        {
            _usingController = false;
        }
    }

    public void Interact()
    {
        Interactable target = GetClosest();
        if(target == null) return;
        target.Interact(_entity);
    }

    Interactable GetClosest()
    {
        _inRange.RemoveAll(i => i == null);

        Interactable closest = null;
        float closestDistance = float.MaxValue;
        foreach(Interactable interactable in _inRange)
        {
            float distance = Vector3.Distance(_entity.transform.position, interactable.transform.position);
            if(distance < closestDistance)
            {
                closestDistance = distance;
                closest = interactable;
            }
        }
        return closest;
    }

    void Update()
    {
        Interactable target = GetClosest();
        bool show = target != null && !target.IsInteracting;
        _hintTarget = show ? target : null;

        if(_interactIconKeyboard != null) _interactIconKeyboard.SetActive(show && !_usingController);
        if(_interactIconController != null) _interactIconController.SetActive(show && _usingController);
    }

    void LateUpdate()
    {
        if(_hintTarget == null || _hintRoot == null) return;

        if(_cam == null) _cam = Camera.main;
        if(_cam == null) return;

        _hintRoot.position = _hintTarget.HintPosition;
        _hintRoot.rotation = _cam.transform.rotation;

        if(_constantScreenSize && !_cam.orthographic)
        {
            float distance = Vector3.Distance(_cam.transform.position, _hintRoot.position);
            _hintRoot.localScale = Vector3.one * (distance * _sizeFactor);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Interactable interactable = other.GetComponentInParent<Interactable>();
        if(interactable != null && !_inRange.Contains(interactable))
        {
            _inRange.Add(interactable);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Interactable interactable = other.GetComponentInParent<Interactable>();
        if(interactable != null && _inRange.Remove(interactable))
        {
            interactable.EndInteraction();
        }
    }
}