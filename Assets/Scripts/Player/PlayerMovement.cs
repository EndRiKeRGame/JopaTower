using UnityEngine;

/// <summary>
/// Movement Left and Right
/// Arms Movement
/// </summary>

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 5f;

    // Разгон/торможение на земле
    [SerializeField]
    private float _groundAcceleration = 40f;
    
    [SerializeField]
    private float _groundDeceleration = 50f;

    // Коэффициент разгона/торможения в воздухе
    [SerializeField]
    private float _airAccelerationCoef = 0.25f;
    
    // TODO: добавить проверку на землю
    private Rigidbody2D _rb;
    private float _launchLockTimer;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Вызывается извне (HandGrip) в момент броска — на время lockDuration
    /// AddForce не гасит горизонтальную скорость при отсутствии ввода,
    /// иначе боковой импульс броска гасится этим же кадром автоторможения.
    /// </summary>
    public void NotifyLaunched(float lockDuration)
    {
        _launchLockTimer = Mathf.Max(_launchLockTimer, lockDuration);
    }

    public void AddForce(float input)
    {
        bool isPressingDirection = Mathf.Abs(input) > 0.01f;

        if (_launchLockTimer > 0f)
        {
            _launchLockTimer -= Time.fixedDeltaTime;

            // во время окна броска не гасим импульс, если игрок не управляет активно
            if (!isPressingDirection) return;
        }

        float targetSpeed = input * _moveSpeed;
        float speedDiff = targetSpeed - _rb.linearVelocity.x;

        float accel;
        if (isPressingDirection)
            accel = _groundAcceleration * _airAccelerationCoef;
        else
            accel = _groundDeceleration * _airAccelerationCoef;

        float force = speedDiff * accel;
        _rb.AddForce(Vector2.right * force);
    }
}