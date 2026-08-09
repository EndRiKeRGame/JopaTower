using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Шкала усталости одной руки.
/// Вешается на объект руки (Left Hand / Right Hand) вместе с HandGrip.
/// Пока рука НЕ держится за что-то (isResting = true) — стамина восстанавливается.
/// Пока рука держится — HandGrip вызывает Drain() каждый кадр.
/// </summary>
public class StaminaSystem : MonoBehaviour
{
    [Header("Параметры усталости")]
    public float maxStamina = 100f;
    public float regenPerSecond = 20f;

    [Tooltip("Управляется извне: true = рука отдыхает и восстанавливается")]
    public bool isResting = true;

    public float CurrentStamina { get; private set; }

    /// <summary>Событие для UI: передаёт значение 0..1 (для Slider'а на Canvas)</summary>
    public UnityEvent<float> OnStaminaChanged;

    void Awake()
    {
        CurrentStamina = maxStamina;
    }

    void Update()
    {
        if (isResting && CurrentStamina < maxStamina)
        {
            CurrentStamina = Mathf.Min(maxStamina, CurrentStamina + regenPerSecond * Time.deltaTime);
            OnStaminaChanged?.Invoke(CurrentStamina / maxStamina);
        }
    }

    public void Drain(float amount)
    {
        CurrentStamina = Mathf.Max(0f, CurrentStamina - amount);
        OnStaminaChanged?.Invoke(CurrentStamina / maxStamina);
    }

    public bool IsExhausted => CurrentStamina <= 0f;
}
