using UnityEngine;

/// <summary>
/// Облачко тумана Акта 3.
/// Летает влево-вправо на СВОЕЙ фиксированной высоте (пинг-понг между границами).
/// По вертикали за камерой НЕ следует: игрок сам поднимается в зону тумана.
/// </summary>
public class FogDrift : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float _speed = 1.5f;   // скорость полёта по X
    [SerializeField] private float _minX = -9f;     // левая граница гуляния
    [SerializeField] private float _maxX = 9f;      // правая граница гуляния
    [SerializeField] private bool _startMovingLeft; // старт влево, чтобы облака не летели синхронно

    private int _direction = 1;

    void Awake()
    {
        _direction = _startMovingLeft ? -1 : 1;
    }

    void Update()
    {
        Vector3 p = transform.position;
        p.x += _speed * _direction * Time.deltaTime;

        // Дошли до границы — разворачиваемся
        if (p.x >= _maxX) { p.x = _maxX; _direction = -1; }
        if (p.x <= _minX) { p.x = _minX; _direction = 1; }

        transform.position = p;
    }
}