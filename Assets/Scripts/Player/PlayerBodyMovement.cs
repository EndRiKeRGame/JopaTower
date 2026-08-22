using UnityEngine;

namespace Player
{
    /// <summary>
    /// Movement Left and Right
    /// Arms Movement
    /// </summary>

    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerBodyMovement : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;

        // Разгон/торможение на земле
        [SerializeField] private float _groundAcceleration = 40f;
        [SerializeField] private float _groundDeceleration = 50f;

        // Коэффициент разгона/торможения в воздухе
        [SerializeField] private float _airAccelerationCoef = 0.25f;

        private Rigidbody2D _rb;
        private float _launchLockTimer;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }
    
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
            float coef = CheckForCollideWithPlatform() ? 1f : _airAccelerationCoef;

            if (isPressingDirection)
                accel = _groundAcceleration * coef;
            else
                accel = _groundDeceleration * coef;

            float force = speedDiff * accel;
            _rb.AddForce(Vector2.right * force);
        }

        private bool CheckForCollideWithPlatform()
        {
            Collider2D collider = GetComponent<Collider2D>();
            if (collider == null) return false;

            // Получаем все контакты коллайдера
            ContactPoint2D[] contacts = new ContactPoint2D[10];
            int contactCount = collider.GetContacts(contacts);

            for (int i = 0; i < contactCount; i++)
            {
                // Проверяем тег объекта, с которым произошёл контакт
                if (contacts[i].collider.CompareTag("Platform"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}