using UnityEngine;

public class AttackScript : MonoBehaviour
{

    [Header("Dependence")]
    [SerializeField] GameObject _entity;
    [SerializeField] string _targetTag; 
    [SerializeField] Animator _animator;


    private bool _isAttacking;
    private bool _hitPending;
    private float _hitDelayTimer;
    private IAttackStats _attackStats;
    private float _attackCooldownTimer;
    private int _collidersInside;
    private Collider _targetCollider;
    private IDamageable _targetDamageable;
    private bool _isLocked;

    public bool HasTarget => _collidersInside > 0;

    void Awake()
    {
        _attackStats = _entity.GetComponent<IAttackStats>();
    }

    public void SetLocked(bool locked)
    {
        _isLocked = locked;
    }

    public bool Attack(float hitDelay = 0f)
    {
        if(_isLocked) return false;
        
        if(_attackCooldownTimer > 0f) return false;
    
        _attackCooldownTimer = _attackStats.AttackCooldown;

        if(hitDelay > 0f)
        {
            _hitPending = true;
            _hitDelayTimer = hitDelay;
        }
        else
        {
            _isAttacking = true;
        }
        if(_animator != null) _animator.SetTrigger("IsAttacking");
        return true;
    }

    bool TryGetTarget(Collider other, out IDamageable damageable)
    {
        damageable = null;

        if(other.isTrigger) return false;

        bool tagMatches = other.CompareTag(_targetTag) || (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag(_targetTag));
        if(!tagMatches) return false;

        damageable = other.GetComponentInParent<IDamageable>();
        return damageable != null;
    }

    void ClearTarget()
    {
        _collidersInside = 0;
        _targetCollider = null;
        _targetDamageable = null;
    }

    void DealDamage()
    {
        if(_targetDamageable != null)
        {
            _targetDamageable.TakeDamage(_attackStats.Damage);
        }
    }

    void Update()
    {
        if(_attackCooldownTimer > 0f)
        {
            _attackCooldownTimer -= Time.deltaTime;
            if(_attackCooldownTimer <= 0f)
            {
                _isAttacking = false;
            }
        }

        if(_collidersInside > 0 && _targetCollider == null)
        {
            ClearTarget();
        }

        if(_hitPending)
        {
            _hitDelayTimer -= Time.deltaTime;
            if(_hitDelayTimer <= 0f)
            {
                _hitPending = false;
                if(_collidersInside > 0)
                {
                    DealDamage();
                }
            }
        }
        if(_isAttacking && _collidersInside > 0)
        {
            DealDamage();
            _isAttacking = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(!TryGetTarget(other, out IDamageable damageable)) return;

        _collidersInside++;
        _targetCollider = other;
        _targetDamageable = damageable;
    }

    void OnTriggerExit(Collider other)
    {
        if(!TryGetTarget(other, out _)) return;
        _attackCooldownTimer = 0f;
        _collidersInside = Mathf.Max(0, _collidersInside - 1);
        if(_collidersInside == 0)
        {
            ClearTarget();
        }
    }
}