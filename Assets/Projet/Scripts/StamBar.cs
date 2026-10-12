using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] PlayerStat _playerStat;
    [SerializeField] Slider _staminaSlider;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerStat.OnStaminaChanged += UpdateStaminaBar;
    }

    void OnDestroy()
    {
        _playerStat.OnStaminaChanged -= UpdateStaminaBar;
    }


    void UpdateStaminaBar(int currentStamina)
    {
        float staminaPercentage = (float)currentStamina / _playerStat.MaxStamina;
        _staminaSlider.value = staminaPercentage;
    }
}
