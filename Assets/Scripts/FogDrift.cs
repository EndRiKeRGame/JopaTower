using UnityEngine;

/// <summary>
/// Облачко тумана Акта 3.
/// Летает влево-вправо на СВОЕЙ фиксированной высоте (пинг-понг между границами).
/// По вертикали за камерой НЕ следует: игрок сам поднимается в зону тумана.
/// </summary>
public class FogDrift : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float _speedX = 1.5f;   // скорость полёта по X
    [SerializeField] private float _speedY = 1.5f;   // скорость полёта по Y
    
    [SerializeField] private float _minX = -9f;     // левая граница гуляния
    [SerializeField] private float _maxX = 9f;      // правая граница гуляния
    
    [SerializeField] private float _minY = -1f;     // отступ от уровня вниз
    [SerializeField] private float _maxY = 1f;      // отступ от уровня вверх
    
    [SerializeField] private bool _startMovingLeft; // старт влево, чтобы облака не летели синхронно
    [SerializeField] private bool _startMovingUp; // старт влево, чтобы облака не летели синхронно

    private int _directionX = 1;
    private int _directionY = 1;
    
    [SerializeField]
    private float _baseY;

    void Awake()
    {
        _directionX = _startMovingLeft ? -1 : 1;
        _directionY = _startMovingUp ? 1 : -1;
        _baseY = transform.position.y;
    }

    void Update()
    {
        Vector3 p = transform.position;
        p.x += _speedX * _directionX * Time.deltaTime;
        p.y += _speedY * _directionY * Time.deltaTime;

        // Дошли до границы — разворачиваемся
        if (p.x >= _maxX) { p.x = _maxX; _directionX = -1; }
        if (p.x <= _minX) { p.x = _minX; _directionX = 1; }
        
        if (p.y >= _baseY + _maxY) { p.y = _baseY + _maxY; _directionY = -1; }
        if (p.y <= _baseY + _minY) { p.y = _baseY + _minY; _directionY = 1; }

        transform.position = p;
    }
}