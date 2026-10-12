using UnityEngine;

public interface IMoveStats
{
    float WalkSpeed { get; }
    float RunSpeed { get; }
}

public interface IAttackStats
{
    int Damage { get; }
    float AttackCooldown { get; }
}

public interface IDodgeStats
{
    int Stamina { get; }
    float DodgeDistance { get; }
    float DodgeDuration { get; }
    float DodgeCooldown { get; }
    int DodgeStaminaCost { get; }
    void UseStamina(int stamina);
}

public interface IDamageable
{
    void TakeDamage(int damage);
    void Heal(int heal);
}

public interface IHealthStats
{
    int Hp { get; }
    int MaxHp { get; }
}


