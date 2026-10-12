using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DetectionZone : MonoBehaviour
{
    [SerializeField] EnnemyStats _stats;
    [SerializeField] string _targetTag = "Player";

    public event Action<Transform> OnTargetEnter;
    public event Action<Transform> OnTargetExit;

    public Transform Target => _target;
    public bool HasTarget => _colliderInside > 0 && _target != null;
    private BoxCollider _box;
    private int _colliderInside;
    private Transform _target;

    void Awake()
    {
        _box = GetComponent<BoxCollider>();
        _box.isTrigger = true;
        _box.size = _stats.DetectionSize;
    }

    bool IsTarget(Collider other)
    {
        if(other.isTrigger) return false;
        if(other.CompareTag(_targetTag)) return true;
        return other.attachedRigidbody != null && other.attachedRigidbody.CompareTag(_targetTag);
    }

    Transform GetRoot(Collider other)
    {
        return other.attachedRigidbody != null ? other.attachedRigidbody.transform : other.transform;
    }

    void OnTriggerEnter(Collider other)
    {
        if(!IsTarget(other)) return;
        _colliderInside++;
        if(_colliderInside == 1)
        {
            _target = GetRoot(other);
            OnTargetEnter?.Invoke(_target);
        }  
    }

    void OnTriggerExit(Collider other)
    {
        if(!IsTarget(other)) return;
        
        _colliderInside--;
        
        if(_colliderInside == 0)
        {
            Transform lostTarget = _target;
            _target = null;
            OnTargetExit?.Invoke(lostTarget);
        }
    }

    void OnDrawGizmosSelected()
    {
        if(_stats == null) return;
        BoxCollider box = GetComponent<BoxCollider>();
        Vector3 center = box != null ? box.center : Vector3.zero;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, _stats.DetectionSize);
    }
}