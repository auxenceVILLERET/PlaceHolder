using UnityEngine;
using System;


public class PlayerStat : MonoBehaviour, IMoveStats, IAttackStats, IDodgeStats, IDamageable, IHealthStats
{
    [Header("Health")]
    [SerializeField] int _hp;
    [SerializeField] int _maxHp;
    [Header("Attack")]
    [SerializeField] int _damage;
    [SerializeField] float _attackCooldown;
    [Header("Speed")]
    [SerializeField] float _walkSpeed;
    [SerializeField] float _runSpeed;
    [Header("Stamina")]
    [SerializeField] int _stamina;
    [SerializeField] int _maxStamina;
    [SerializeField] float _staminaRegenInterval;
    [SerializeField] int _staminaRegenAmount;
    [Header("Dodge")]
    [SerializeField] float _dodgeDistance;
    [SerializeField] float _dodgeDuration;
    [SerializeField] float _dodgeCooldown;
    [SerializeField] int _dodgeStaminaCost;


    public int Hp { get => _hp;}
    public int MaxHp { get => _maxHp;}
    public int Damage { get => _damage;}
    public float AttackCooldown { get => _attackCooldown;}
    public float WalkSpeed { get => _walkSpeed;}
    public float RunSpeed { get => _runSpeed;}
    public int Stamina { get => _stamina;}
    public int MaxStamina { get => _maxStamina;}
    public float DodgeDistance { get => _dodgeDistance;}
    public float DodgeDuration { get => _dodgeDuration;}
    public float DodgeCooldown { get => _dodgeCooldown;}
    public int DodgeStaminaCost { get => _dodgeStaminaCost;}

    public event Action<int> OnHpChanged;
    public event Action<int> OnStaminaChanged;


    private float _regenDelayTimer;
    private float _regenTickTimer;

    void Reset()
    {
        _hp = 100;
        _maxHp = 100;
        _damage = 10;
        _attackCooldown = 1f;
        _walkSpeed = 5f;
        _runSpeed = 10f;
        _stamina = 100;
        _maxStamina = 100;
        _staminaRegenInterval = 1f;
        _staminaRegenAmount = 1;
        _dodgeDistance = 5f;
        _dodgeDuration = 0.5f;
        _dodgeCooldown = 1f;
        _dodgeStaminaCost = 20;
    }

    void Update()
    {
        UpdateStamina();
    }

    void UpdateStamina()
    {
        if(_stamina >=_maxStamina)
        {
            _regenTickTimer = 0f;
            return;
        }

        if(_regenDelayTimer > 0f)
        {
            _regenDelayTimer -= Time.deltaTime;
            return;
        }

        _regenTickTimer += Time.deltaTime;
        if(_regenTickTimer >= _staminaRegenInterval)
        {
            _regenTickTimer -= _staminaRegenInterval;
            RecoverStamina(_staminaRegenAmount);
        }
       
    }
    public void TakeDamage(int damage)
    {
        _hp -= damage;
        OnHpChanged?.Invoke(_hp);
        if(_hp <= 0)
        {
            _hp = 0;
        }
    }

    public void Heal(int heal)
    {
        _hp += heal;
        OnHpChanged?.Invoke(_hp);
        if(_hp > _maxHp)
        {
            _hp = _maxHp;
        }
    }

    public void UseStamina(int stamina)
    {
        _stamina -= stamina;
        OnStaminaChanged?.Invoke(_stamina);
        if(_stamina < 0)
        {
            _stamina = 0;
        }
    }

    public void RecoverStamina(int stamina)
    {
        _stamina += stamina;
        OnStaminaChanged?.Invoke(_stamina);
        if(_stamina > _maxStamina)
        {
            _stamina = _maxStamina;
        }
    }

}
