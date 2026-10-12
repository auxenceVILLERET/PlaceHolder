using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class RespawnScript : MonoBehaviour
{
    [SerializeField] GameObject _entity;

    [SerializeField] string _respawnSceneName;
    [SerializeField] Vector3 _respawnPosition;
    [SerializeField] Animator _animator;
    [SerializeField] float _respawnTime = 2f;
    [SerializeField] CinemachineCamera _deathCamera;

    private IHealthStats _healthStats;
    private IDamageable _damageable;
    

    private bool _isDying = false; 

    void Awake()
    {
        _healthStats = _entity.GetComponent<IHealthStats>();
        _damageable = _entity.GetComponent<IDamageable>();
    }

    void Update()
    {
        if (IsDead() && !_isDying)
        {
            StartCoroutine(DeathSequence());
        }
    }

    System.Collections.IEnumerator DeathSequence()
    {
        _isDying = true;


        _animator.SetTrigger("IsDead");
        
        if (_deathCamera != null)
        {
            _deathCamera.Priority = 5000000; 

        }


        yield return new WaitForSeconds(_respawnTime);

        if (!string.IsNullOrEmpty(_respawnSceneName))
        {
        
            SceneManager.LoadScene(_respawnSceneName);
        }
        else
        {
        
            Respawn();
        }
    }

    bool IsDead()
    {
        if (_healthStats == null) return false;
        return _healthStats.Hp <= 0;
    }

    public void Respawn()
    {
        if (_damageable == null) return;
        
        _damageable.Heal(_healthStats.MaxHp);
        _entity.transform.position = _respawnPosition;
        
    
        if (_deathCamera != null)
        {
            _deathCamera.Priority = 1000; 
        }

        _isDying = false; 
    }
}