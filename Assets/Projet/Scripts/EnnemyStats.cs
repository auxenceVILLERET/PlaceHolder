using UnityEngine;
using System;
public class EnnemyStats : MonoBehaviour, IMoveStats, IAttackStats, IDamageable
{
    [Header("Health")]
    [SerializeField] int _hp;
    [SerializeField] int _maxHp;
    [Header("Attack")]
    [SerializeField] int _damage;
    [SerializeField] float _attackCooldown;
    [SerializeField] float _attackHitDelay;
    [SerializeField] float _attackLockDuration;
    [Header("Speed")]
    [SerializeField] float _walkSpeed;
    [SerializeField] float _runSpeed;
    [Header("Detection")]
    [SerializeField] Vector3 _detectionSize;
    [Header("Chase")]
    [SerializeField] bool _runWhenChasing;
    [SerializeField] float _returnStopDistance;
    [Header("Death")]
    [SerializeField] float _destroyDelay;

    public int Hp { get => _hp;}
    public int MaxHp { get => _maxHp;}
    public int Damage { get => _damage;}
    public float AttackCooldown { get => _attackCooldown;}
    public float AttackHitDelay { get => _attackHitDelay;}
    public float AttackLockDuration { get => _attackLockDuration;}
    public float WalkSpeed { get => _walkSpeed;}
    public float RunSpeed { get => _runSpeed;}
    public Vector3 DetectionSize { get => _detectionSize;}
    public bool RunWhenChasing { get => _runWhenChasing;}
    public float ReturnStopDistance { get => _returnStopDistance;}
    public float DestroyDelay { get => _destroyDelay;}



    public event Action<int> OnEnnemyHpChanged;
    void Reset()
    {
        _hp = 100;
        _maxHp = 100;
        _damage = 10;
        _attackCooldown = 1f;
        _attackHitDelay = 0.4f;
        _attackLockDuration = 1f;
        _walkSpeed = 5f;
        _runSpeed = 10f;
        _detectionSize = new Vector3(6f, 2f, 6f);
        _runWhenChasing = true;
        _returnStopDistance = 0.5f;
        _destroyDelay = 5f;
    }

    public void TakeDamage(int damage)
    {
        _hp -= damage;
        OnEnnemyHpChanged?.Invoke(_hp);
        if(_hp <= 0)
        {
            _hp = 0;
            OnEnnemyHpChanged?.Invoke(_hp);
            Debug.Log("Ennemy is dead");
        }
    }

    public void Heal(int heal)
    {
        _hp += heal;
        OnEnnemyHpChanged?.Invoke(_hp);
        if(_hp > _maxHp)
        {
            _hp = _maxHp;
        }
    }
}