using System.Collections.Generic;
using Enums;
using UnityEngine;

/// <summary>
/// Захват точки рукой.
/// Наружу торчат только два метода: StartRaising / StopRaising — их дёргает PlayerMovementSystem.
/// Пока кнопка зажата (StartRaising), рука тянется к точке верхнего предела шарнира.
/// Если в этот момент коллайдер руки касается GrabPoint — персонаж мгновенно
/// подтягивается рукой к точке захвата и фиксируется на ней пружинным суставом.
/// Пока рука держится, отведение мыши от точки захвата задаёт вектор броска;
/// на StopRaising персонаж запускается по этому вектору.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class HandGrip : MonoBehaviour
{
    [Header("Общее")]
    [SerializeField] private HandSide _side;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Rigidbody2D _bodyRigidbody; // главное тело персонажа
    [SerializeField] private StaminaSystem _stamina;

    [Header("Сустав фиксации в точке захвата")]
    [SerializeField] private float _jointFrequency = 4f;
    [SerializeField] private float _jointDamping = 0.6f;

    [Header("Запуск при отпускании")]
    [SerializeField] private float _launchForceMultiplier = 6f;
    [SerializeField] private float _maxLaunchDistance = 4f;

    [Header("Расход стамины")]
    [SerializeField] private float _staminaDrainPerSecond = 12f;
    [SerializeField] private float _staminaDrainUnderLoadMultiplier = 1.8f;
    [SerializeField] private float _loadForceThreshold = 5f;

    [Header("Подъём руки (мотор шарниров)")]
    [SerializeField] private HingeJoint2D _armHinge;
    [SerializeField] private HingeJoint2D _handHinge;
    [SerializeField] private bool _raiseTowardUpperLimit = true;
    [SerializeField] private float _raiseMotorSpeed = 400f;
    [SerializeField] private float _raiseMotorForce = 200f;

    [Header("Визуал")]
    [SerializeField] private SpriteRenderer _handSpriteRenderer;
    [SerializeField] private Sprite _openSprite;
    [SerializeField] private Sprite _grabSprite;
    [SerializeField] private Transform _grabMarker;   // появляется точно в точке захвата
    [SerializeField] private LineRenderer _aimLine;   // необязательно: линия натяжения броска

    private Rigidbody2D _rb;
    private SpringJoint2D _joint;
    private Vector3 _grabWorldPosition;
    private bool _isGripping;
    private bool _buttonHeld;

    private readonly List<Transform> _touchingPoints = new List<Transform>();

    public bool IsGripping => _isGripping;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_mainCamera == null) _mainCamera = Camera.main;
        if (_stamina == null) _stamina = GetComponent<StaminaSystem>();

        SetGripVisual(false);
        SetGrabMarkerVisible(false);
        SetAimLineVisible(false);
    }

    void Update()
    {
        if (_isGripping)
        {
            _stamina.isResting = false;
            DrainStamina();
            UpdateAimLine();
        }
        else
        {
            _stamina.isResting = true;
        }
    }

    // ---------- Публичный API: дёргается извне (PlayerMovementSystem) ----------

    public void StartRaising()
    {
        _buttonHeld = true;

        float speed = _raiseTowardUpperLimit ? Mathf.Abs(_raiseMotorSpeed) : -Mathf.Abs(_raiseMotorSpeed);
        SetMotor(_armHinge, speed, _raiseMotorForce, true);
        // SetMotor(_handHinge, speed, _raiseMotorForce, true);

        TryGrabIfTouching();
    }

    public void StopRaising()
    {
        _buttonHeld = false;
        SetMotor(_armHinge, 0f, 0f, false);
        // SetMotor(_handHinge, 0f, 0f, false);

        if (_isGripping)
        {
            ReleaseAndLaunch();
        }
    }

    // ---------- Захват: мгновенное подтягивание к точке ----------

    void TryGrabIfTouching()
    {
        if (_isGripping) return;
        if (_stamina != null && _stamina.IsExhausted) return;
        
        Transform point = GetNearestTouchingPoint();
        
        Debug.Log($"Point {point}");
        if (point == null) return;

        Grab(point);
    }

    void Grab(Transform point)
    {
        _grabWorldPosition = point.position;

        // мгновенно подтягиваем персонажа: рука встаёт точно в точку захвата,
        // тело сдвигается на ту же дельту вслед за рукой
        Vector2 delta = (Vector2)point.position - _rb.position;
        if (_bodyRigidbody != null) _bodyRigidbody.position += delta;
        _rb.position = point.position;

        CreateJoint();
        _isGripping = true;

        SetGripVisual(true);
        SetGrabMarkerVisible(true);
        SetAimLineVisible(true);
    }

    void CreateJoint()
    {
        _joint = gameObject.AddComponent<SpringJoint2D>();
        _joint.autoConfigureConnectedAnchor = false;
        _joint.connectedAnchor = _grabWorldPosition;
        _joint.frequency = _jointFrequency;
        _joint.dampingRatio = _jointDamping;
        _joint.distance = 0f;
        _joint.enableCollision = false;
    }

    void ReleaseGrip()
    {
        if (_joint != null) Destroy(_joint);
        _isGripping = false;

        SetGripVisual(false);
        SetGrabMarkerVisible(false);
        SetAimLineVisible(false);
    }

    void ReleaseAndLaunch()
    {
        Vector3 mouseWorld = GetMouseWorldPosition();

        Vector2 pullVector = (Vector2)(_grabWorldPosition - mouseWorld);
        float distance = Mathf.Min(pullVector.magnitude, _maxLaunchDistance);

        if (_bodyRigidbody != null && distance > 0.1f)
        {
            Vector2 launchDirection = pullVector.normalized;
            _bodyRigidbody.AddForce(launchDirection * distance * _launchForceMultiplier, ForceMode2D.Impulse);
        }

        ReleaseGrip();
    }

    // ---------- Контакт коллайдеров ----------

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<GrabPoint>() == null) return;
        
        Debug.Log("Enter trigger");

        if (!_touchingPoints.Contains(other.transform))
            _touchingPoints.Add(other.transform);

        if (_buttonHeld) TryGrabIfTouching();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        _touchingPoints.Remove(other.transform);
        
        Debug.Log("Exit trigger");
    }

    Transform GetNearestTouchingPoint()
    {
        Transform best = null;
        float bestDist = float.MaxValue;

        for (int i = _touchingPoints.Count - 1; i >= 0; i--)
        {
            Transform t = _touchingPoints[i];
            if (t == null) { _touchingPoints.RemoveAt(i); continue; }

            float d = Vector2.Distance(transform.position, t.position);
            if (d < bestDist) { bestDist = d; best = t; }
        }

        return best;
    }

    // ---------- Стамина ----------

    void DrainStamina()
    {
        float load = _joint != null ? _joint.reactionForce.magnitude : 0f;
        float drain = _staminaDrainPerSecond * Time.deltaTime;
        if (load > _loadForceThreshold) drain *= _staminaDrainUnderLoadMultiplier;

        _stamina.Drain(drain);

        if (_stamina.IsExhausted) ReleaseGrip();
    }

    // ---------- Мотор подъёма руки ----------

    void SetMotor(HingeJoint2D hinge, float speed, float force, bool enable)
    {
        if (hinge == null) return;

        JointMotor2D motor = hinge.motor;
        motor.motorSpeed = speed;
        motor.maxMotorTorque = force;
        hinge.motor = motor;
        hinge.useMotor = enable;
    }

    // ---------- Визуал ----------

    void SetGripVisual(bool gripping)
    {
        if (_handSpriteRenderer == null) return;

        Sprite target = gripping ? _grabSprite : _openSprite;
        if (target != null) _handSpriteRenderer.sprite = target;
    }

    void SetGrabMarkerVisible(bool visible)
    {
        if (_grabMarker == null) return;

        if (visible) _grabMarker.position = _grabWorldPosition;
        _grabMarker.gameObject.SetActive(visible);
    }

    void SetAimLineVisible(bool visible)
    {
        if (_aimLine == null) return;
        _aimLine.gameObject.SetActive(visible);
    }

    void UpdateAimLine()
    {
        if (_aimLine == null) return;

        _aimLine.SetPosition(0, _grabWorldPosition);
        _aimLine.SetPosition(1, GetMouseWorldPosition());
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = Mathf.Abs(_mainCamera.transform.position.z - transform.position.z);
        return _mainCamera.ScreenToWorldPoint(mouseScreen);
    }
}