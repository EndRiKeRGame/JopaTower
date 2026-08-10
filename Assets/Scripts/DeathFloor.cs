using System;
using UnityEngine;

public class DeathFloor : MonoBehaviour
{
    [SerializeField]
    private Transform _deathFloor;
    
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

    private void FixedUpdate()
    {
        if (!_isMoving)
            return;

        _speedUp = Math.Abs(_target.position.y - transform.position.y) > _distance;
        var speed = _speedUp ? _speed * Time.fixedDeltaTime * _speedUpCoef : _speed * Time.fixedDeltaTime;

        _deathFloor.transform.position += _deathFloor.transform.up * speed;
    }

    public void Setup(Transform target)
    {
        _target = target;
    }
    
    public void StartDeathFloor()
    {
        _isMoving = true;   
    }
    
    public void StopDeathFloor()
    {
        _isMoving = false; 
    }
    
    public void SetDeathFloor(Vector3 pos)
    {
        _isMoving = false;
        _deathFloor.transform.position = pos;
    }
}
