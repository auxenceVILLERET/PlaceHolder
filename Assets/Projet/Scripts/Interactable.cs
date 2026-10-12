using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("Hint")]
    [SerializeField] Transform _hintAnchor;
    [SerializeField] float _hintHeight = 2f;
 
    public Vector3 HintPosition => _hintAnchor != null ? _hintAnchor.position : transform.position + Vector3.up * _hintHeight;
 
    public abstract void Interact(GameObject interactor);
 
    // Called when the interactor walks out of range
    public virtual void EndInteraction() { }
 
    public virtual bool IsInteracting => false;
}
