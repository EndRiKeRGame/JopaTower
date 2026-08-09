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

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void AddForce(float input)
    {
        float targetSpeed = input * _moveSpeed;
        float speedDiff = targetSpeed - _rb.linearVelocity.x;

        bool isPressingDirection = Mathf.Abs(input) > 0.01f;
        float accel;
        if (isPressingDirection)
            accel = _groundAcceleration * _airAccelerationCoef;
        else
            accel = _groundDeceleration * _airAccelerationCoef;

        float force = speedDiff * accel;
        _rb.AddForce(Vector2.right * force);
    }
}
