using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] PlayerStat _playerStat;
    [SerializeField] Slider _healthSlider;

 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
           _playerStat.OnHpChanged += UpdateHealthBar;
    }

    void OnDestroy()
    {
        _playerStat.OnHpChanged -= UpdateHealthBar;
    }


    void UpdateHealthBar(int currentHp)
    {
        float healthPercentage = (float)currentHp / _playerStat.MaxHp;
        _healthSlider.value = healthPercentage;
    }


}
