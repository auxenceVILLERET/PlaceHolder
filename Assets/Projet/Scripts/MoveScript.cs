using UnityEngine;

public class MoveScript : MonoBehaviour
{
    [SerializeField] GameObject _entity;
    [SerializeField] Animator _animator;

    private Vector3 _currentDirection;
    private bool _isRunning;

    //Dodge
    private bool _isDodging;
    private Vector3 _dodgeDirection;
    private float _dodgeTime;
    private float _dodgeCooldownTime;

    private IMoveStats _moveStats;
    private IDodgeStats _dodgeStats;
    private Rigidbody _rb;

    private bool _isLocked;

    void Awake()
    {
        _moveStats = _entity.GetComponent<IMoveStats>();
        _dodgeStats = _entity.GetComponent<IDodgeStats>();
        _rb = _entity.GetComponent<Rigidbody>();
    }

    public void SetLocked(bool locked)
    {
        _isLocked = locked;
        if(locked)
        {
            _isDodging = false;
            SetHorizontalVelocity(Vector3.zero);
            _animator.SetFloat("Speed", 0f);
        }
    }

    public void SetDirection(Vector3 direction)
    {
        _currentDirection = direction;
        _isRunning = false;
    }

    public void SetRunDirection(Vector3 direction)
    {
        _currentDirection = direction;
        _isRunning = true;
    }

    public bool Dodge(Vector3 direction)
    {
        if(_isLocked) return false;
        if(_dodgeStats == null) return false;

        if(_isDodging || _dodgeCooldownTime > 0f || _dodgeStats.Stamina < _dodgeStats.DodgeStaminaCost)
        {
            return false;
        }

        if(direction == Vector3.zero)
        {
            direction = _entity.transform.forward;
        }

        if(_animator != null) _animator.SetTrigger("IsDodging");
        _dodgeStats.UseStamina(_dodgeStats.DodgeStaminaCost);
        _dodgeDirection = direction.normalized;
        _dodgeTime = 0f;
        _isDodging = true;
        return true;
    }

    static float Ease(float x)
    {
        float inv = 1f - x;
        return 1f - inv * inv;
    }

    void SetHorizontalVelocity(Vector3 horizontal)
    {
        Vector3 velocity = _rb.linearVelocity;
        _rb.linearVelocity = new Vector3(horizontal.x, velocity.y, horizontal.z);
    }

    void UpdateDodge(float deltaTime)
    {
        float previous = _dodgeTime / _dodgeStats.DodgeDuration;
        _dodgeTime += deltaTime;
        float current = Mathf.Clamp01(_dodgeTime / _dodgeStats.DodgeDuration);

        float step = Ease(current) - Ease(previous);
        SetHorizontalVelocity(_dodgeDirection * (step * _dodgeStats.DodgeDistance / deltaTime));

        if(current >= 1f)
        {
            _isDodging = false;
            _dodgeCooldownTime = _dodgeStats.DodgeCooldown;
        }
    }

    void FixedUpdate()
    {
        if(_isLocked) return;
        
        float dt = Time.fixedDeltaTime;

        if(_currentDirection.sqrMagnitude > 0.0001f)
        {
            _entity.transform.forward = Vector3.Lerp(_entity.transform.forward, _currentDirection.normalized, dt * 10f);
        }

        if(_dodgeCooldownTime > 0f)
        {
            _dodgeCooldownTime -= dt;
        }

        if(_isDodging)
        {
            UpdateDodge(dt);
            return;
        }

        float speed = _isRunning ? _moveStats.RunSpeed : _moveStats.WalkSpeed;
        SetHorizontalVelocity(_currentDirection.normalized * speed);

        if(_animator != null)
        {
            _animator.SetFloat("Speed", _currentDirection.magnitude * speed / Mathf.Max(_moveStats.RunSpeed, 0.01f));
        }
    }
}