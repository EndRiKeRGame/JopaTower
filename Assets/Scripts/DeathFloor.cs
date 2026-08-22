using System;
using UnityEngine;
using PrimeTween; // добавлен модуль PrimeTween

public class DeathFloor : MonoBehaviour
{
    [SerializeField]
    private Transform _frontDeathFloor;
    
    [SerializeField]
    private Transform _backDeathFloor;
    
    [SerializeField]
    private Transform _target;
    
    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _speedUpCoef = 2f;

    [SerializeField]
    private bool _isMoving = false;

    [SerializeField]
    private float _distance = 20f;
    
    [SerializeField]
    private bool _speedUp = false;

    // Параметры анимации
    [Header("Анимация обгона (вертикаль)")]
    [SerializeField] private float _amplitudeY = 0.5f;   // амплитуда смещения по вертикали
    [SerializeField] private float _durationY = 2f;      // период колебаний (сек)

    [Header("Анимация ходьбы (горизонталь)")]
    [SerializeField] private float _amplitudeX = 0.2f;   // амплитуда смещения по горизонтали
    [SerializeField] private float _durationX = 1.5f;    // период колебаний (сек)

    // Базовые позиции без анимации (движутся только вверх)
    private Vector3 _frontBasePos;
    private Vector3 _backBasePos;

    // Текущие анимационные смещения (обновляются твинами)
    private float _frontOffsetX, _frontOffsetY;
    private float _backOffsetX, _backOffsetY;

    private Tween _frontTweenX, _frontTweenY;
    private Tween _backTweenX, _backTweenY;
    
    public void SetTarget(Transform target) => _target = target;
    
    public void StartWork()
    {
        if (_isMoving)
            return;
        
        _isMoving = true;

        // Запоминаем текущие позиции как базовые
        _frontBasePos = _frontDeathFloor.position;
        _backBasePos = _backDeathFloor.position;

        // Вертикальные колебания (обгон) – противофаза
        _frontTweenY = Tween.Custom(0, 2 * Mathf.PI, _durationY, val =>
        {
            _frontOffsetY = Mathf.Sin(val) * _amplitudeY;
        }, cycles: -1);

        _backTweenY = Tween.Custom(0, 2 * Mathf.PI, _durationY, val =>
        {
            _backOffsetY = Mathf.Sin(val + Mathf.PI) * _amplitudeY;
        }, cycles: -1);

        // Горизонтальные колебания (ходьба) – со сдвигом фазы
        _frontTweenX = Tween.Custom(0, 2 * Mathf.PI, _durationX, val =>
        {
            _frontOffsetX = Mathf.Sin(val) * _amplitudeX;
        }, cycles: -1);

        _backTweenX = Tween.Custom(0, 2 * Mathf.PI, _durationX, val =>
        {
            _backOffsetX = Mathf.Sin(val + Mathf.PI / 2) * _amplitudeX;
        }, cycles: -1);
    }
    
    public void StopWork()
    {
        _isMoving = false;
        StopAllTweens();
        ResetOffsets();
    }
    
    public void SetDeathFloor(Vector3 pos)
    {
        _frontDeathFloor.position = pos;
        _backDeathFloor.position = pos + Vector3.up * 5f;
        _frontBasePos = _frontDeathFloor.position;
        _backBasePos = _backDeathFloor.position;
    }
    
    public Vector3 GetDeathFloorPos() => _frontDeathFloor.position;

   private void FixedUpdate()
    {
        if (!_isMoving)
            return;

        _speedUp = Math.Abs(_target.position.y - transform.position.y) > _distance;
        float speed = _speedUp ? _speed * Time.fixedDeltaTime * _speedUpCoef : _speed * Time.fixedDeltaTime;

        // Обновляем базовые позиции (движение вверх)
        _frontBasePos += _frontDeathFloor.up * speed;
        _backBasePos += _backDeathFloor.up * speed;

        // Итоговая позиция = базовая + анимационные смещения по правой и верхней осям
        _frontDeathFloor.position = _frontBasePos 
                                    + _frontDeathFloor.right * _frontOffsetX 
                                    + _frontDeathFloor.up * _frontOffsetY;
        _backDeathFloor.position = _backBasePos 
                                   + _backDeathFloor.right * _backOffsetX 
                                   + _backDeathFloor.up * _backOffsetY;
    }

    private void StopAllTweens()
    {
        if (_frontTweenY.isAlive) _frontTweenY.Stop();
        if (_backTweenY.isAlive) _backTweenY.Stop();
        if (_frontTweenX.isAlive) _frontTweenX.Stop();
        if (_backTweenX.isAlive) _backTweenX.Stop();
    }

    private void ResetOffsets()
    {
        _frontOffsetX = _frontOffsetY = 0;
        _backOffsetX = _backOffsetY = 0;
    }
}