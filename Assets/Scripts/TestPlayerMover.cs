using UnityEngine;

/// <summary>
/// Временный скрипт для тестирования генератора башни.
/// Просто двигает объект вверх каждый кадр.
/// Не используй его как финального игрока.
/// </summary>
public class TestPlayerMover : MonoBehaviour
{
    [Header("Настройки тестового движения")]
    public float climbSpeed = 10f;
    public bool move = true;

    void Update()
    {
        if (!move)
        {
            return;
        }

        // Двигаем объект вверх со временем
        transform.position += Vector3.up * climbSpeed * Time.deltaTime;
    }
}