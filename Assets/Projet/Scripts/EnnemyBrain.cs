using UnityEngine;

public class EnnemyBrain : MonoBehaviour
{
    enum State { Idle, Chase, Attack, Return, Dead }

    [Header("Dependence")]
    [SerializeField] GameObject _moveEntity;
    [SerializeField] GameObject _attackEntity;
    [SerializeField] DetectionZone _detectionZone;
    [SerializeField] EnnemyStats _stats;
    [SerializeField] Transform _body;
    [SerializeField] Animator _animator;

    private MoveScript _move;
    private AttackScript _attack;
    private State _state = State.Idle;
    private Vector3 _spawnPosition;
    private float _lockeTimer;

    void Awake()
    {
        _move = _moveEntity.GetComponent<MoveScript>();
        _attack = _attackEntity.GetComponent<AttackScript>();

        if(_body == null)
        {
            Rigidbody rb = GetComponentInParent<Rigidbody>();
            _body = rb != null ? rb.transform : transform;
        }

        _spawnPosition = _body.position;
    }

    void OnEnable()
    {
        if(_stats != null) _stats.OnEnnemyHpChanged += OnHpChanged;
    }

    void OnDisable()
    {
        if(_stats != null) _stats.OnEnnemyHpChanged -= OnHpChanged;
    }

    void Update()
    {
        if(_lockeTimer > 0f)
        {
            _lockeTimer -= Time.deltaTime;
            return;
        }

        switch(_state)
        {
            case State.Idle:   UpdateIdle();   break;
            case State.Chase:  UpdateChase();  break;
            case State.Attack: UpdateAttack(); break;
            case State.Return: UpdateReturn(); break;
        }
    }

    void UpdateIdle()
    {
        if(_detectionZone.HasTarget)
        {
            ChangeState(State.Chase);
        }
    }


    void UpdateChase()
    {
        if(!_detectionZone.HasTarget)
        {
            ChangeState(State.Return);
            return;
        }

        if(_attack.HasTarget)
        {
            ChangeState(State.Attack);
            return;
        }

        MoveToward(_detectionZone.Target.position, _stats.RunWhenChasing);
    }

    void UpdateAttack()
    {
        if(!_detectionZone.HasTarget)
        {
            ChangeState(State.Return);
            return;
        }

        if(!_attack.HasTarget)
        {
            ChangeState(State.Chase);
            return;
        }
        if(_attack.Attack(_stats.AttackHitDelay))
        {
            _lockeTimer = _stats.AttackLockDuration;
        }
    }

    void UpdateReturn()
    {
        if(_detectionZone.HasTarget)
        {
            ChangeState(State.Chase);
            return;
        }

        Vector3 toSpawn = _spawnPosition - _body.position;
        toSpawn.y = 0f;

        if(toSpawn.magnitude <= _stats.ReturnStopDistance)
        {
            ChangeState(State.Idle);
            return;
        }

        MoveToward(_spawnPosition, false);
    }

    void ChangeState(State newState)
    {
        if(_state == newState) return;
        _state = newState;

        if(newState == State.Idle || newState == State.Attack)
        {
            _move.SetDirection(Vector3.zero);
        }
    }

    void MoveToward(Vector3 point, bool run)
    {
        Vector3 toPoint = point - _body.position;
        toPoint.y = 0f;

        if(toPoint.sqrMagnitude < 0.0001f)
        {
            _move.SetDirection(Vector3.zero);
            return;
        }

        Vector3 direction = toPoint.normalized;
        if(run) _move.SetRunDirection(direction);
        else _move.SetDirection(direction);
    }


    void OnHpChanged(int hp)
    {
        if(hp > 0 || _state == State.Dead) return;
        
        _animator.SetTrigger("IsDead");
        
        Die();
    }

    void Die()
    {
        _state = State.Dead;
        _move.SetDirection(Vector3.zero);
        _attack.enabled = false;
        QuestManager.Report(QuestAction.KillEnemy);
        if(_stats.DestroyDelay >= 0f)
        {
            Destroy(_body.gameObject, _stats.DestroyDelay);
        }
    }
}