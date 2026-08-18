using UnityEngine;

/// <summary>
/// Тряска камеры. Висит на том же объекте, где Camera и CameraFollowUp.
/// Приём "снял старый оффсет → добавил новый" не конфликтует
/// со следованием камеры в LateUpdate.
/// </summary>
public class CameraShake : MonoBehaviour
{
    private float _timer;
    private float _duration;
    private float _magnitude;
    private Vector3 _offset;

    /// <summary>Запустить тряску. Вызывается режиссёром осколков.</summary>
    public void Shake(float duration, float magnitude)
    {
        _duration = duration;
        _timer = duration;
        _magnitude = magnitude;
    }

    void LateUpdate()
    {
        // 1. Всегда снимаем оффсет прошлого кадра, чтобы камера не уползала
        transform.position -= _offset;

        // 2. Если тряска активна — добавляем новый случайный оффсет
        if (_timer > 0f)
        {
            _timer -= Time.deltaTime;
            float fade = Mathf.Clamp01(_timer / Mathf.Max(0.0001f, _duration));
            _offset = Random.insideUnitSphere * _magnitude * fade;
            _offset.z = 0f; // трясём только по X/Y
            transform.position += _offset;
        }
        else
        {
            _offset = Vector3.zero;
        }
    }
}