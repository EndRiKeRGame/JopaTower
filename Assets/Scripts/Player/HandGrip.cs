using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;

/// <summary>
/// Захват точки рукой.
/// Наружу торчат только два метода: StartRaising / StopRaising — их дёргает PlayerMovementSystem.
/// Пока кнопка зажата (StartRaising), рука тянется к точке верхнего предела шарнира
/// (плечо и кисть двигаются каждый со своей скоростью — так движение выглядит естественнее).
/// Если в этот момент коллайдер руки касается GrabPoint — персонаж подтягивается
/// рукой к точке захвата и фиксируется на ней пружинным суставом. Если персонаж уже
/// держится другой рукой, тело не дёргаем (иначе рвётся первый хват) — рука просто
/// довисает пружиной.
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
    [SerializeField] private BodyMovement bodyMovement;
    [SerializeField] private StaminaSystem _stamina;
    [SerializeField] private HandGrip _otherHand; // вторая рука — нужна, чтобы не рвать её хват своим захватом

    [Header("Сустав фиксации в точке захвата")]
    [SerializeField] private float _jointFrequency = 4f;
    [SerializeField] private float _jointDamping = 0.6f;
    [SerializeField] private float _stepBetweenHands = 0.2f;

    [Header("Запуск при отпускании")]
    [SerializeField] private float _launchForceMultiplier = 6f;
    [SerializeField] private float _maxLaunchDistance = 4f;
    [SerializeField] private float _twoHandLaunchForceMultiplier = 4f; // отдельная сила для броска двумя руками
    [SerializeField] private float _twoHandMaxLaunchDistance = 4f;
    [SerializeField] private float _launchLockDuration = 0.4f; // окно, пока PlayerMovement не гасит боковой импульс броска

    [Header("Расход стамины")]
    [SerializeField] private float _staminaDrainPerSecond = 12f;
    [SerializeField] private float _staminaDrainUnderLoadMultiplier = 1.8f;
    [SerializeField] private float _loadForceThreshold = 5f;

    [Header("Подъём руки (мотор шарниров, у плеча и кисти — своя скорость)")]
    [SerializeField] private HingeJoint2D _armHinge;
    [SerializeField] private HingeJoint2D _handHinge;
    [SerializeField] private bool _raiseTowardUpperLimit = true;
    [SerializeField] private float _armMotorSpeed = 400f;
    [SerializeField] private float _armMotorForce = 200f;
    [SerializeField] private float _handMotorSpeed = 250f;
    [SerializeField] private float _handMotorForce = 150f;

    [Header("Визуал")]
    [SerializeField] private SpriteRenderer _handSpriteRenderer;
    [SerializeField] private Sprite _openSprite;
    [SerializeField] private Sprite _grabSprite;
    
    [SerializeField] private Transform _grabMarker;   // появляется точно в точке захвата
    [SerializeField] private LineRenderer _aimLine;   // линия натяжения броска
    [SerializeField] private Transform _arrowHead;    // стрелка на конце линии (опционально)
    
    [Header("Покраснение руки от усталости")]
    [SerializeField] private Color _restedHandColor = Color.white;
    [SerializeField] private Color _exhaustedHandColor = new Color(1f, 0.25f, 0.25f);
    [SerializeField] private float _colorLerpSpeed = 6f; // сглаживание, чтобы цвет не дёргался покадрово

    public event Action OnGripAudio;

    private Rigidbody2D _rb;
    private SpringJoint2D _joint;
    private Vector3 _grabWorldPosition;
    private Vector3 _grabWorldPositionForHand;
    private bool _isGripping;
    private bool _buttonHeld;
    private bool _gripInvolvedBothHands;
    
    private bool _wantsToRelease = false;

    private readonly List<Transform> _touchingPoints = new List<Transform>();

    public bool IsGripping => _isGripping;
    public Vector3 LastGrabPosition => _grabWorldPositionForHand;

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
        bool anyHandGripping = _isGripping || (_otherHand != null && _otherHand.IsGripping);
        _stamina.isResting = !anyHandGripping;
        
        UpdateHandColor();

        if (_isGripping)
        {
            DrainStamina();
            UpdateAimLine();
            
            if (_wantsToRelease)
            {
                bool otherReady = _otherHand == null || !_otherHand.IsGripping || _otherHand._wantsToRelease;
                if (otherReady)
                {
                    // Принудительно отцепляем другую руку (если ещё висит)
                    if (_otherHand != null && _otherHand.IsGripping)
                        _otherHand.ForceReleaseGrip();

                    // Выполняем бросок с той руки, которая сейчас обрабатывается
                    ReleaseAndLaunch();
                    // Выключаем моторы, так как рука больше не в захвате
                    SetMotor(_armHinge, 0f, 0f, false);
                }
            }
        }
        else if (_buttonHeld)
        {
            TryGrabIfTouching();
        }
    }

    public void Setup(Transform grabMarker, LineRenderer aimLine, Transform arrowHead = null)
    {
        _grabMarker = grabMarker;
        _aimLine = aimLine;
        _arrowHead = arrowHead;
    }

    // ---------- Публичный API: дёргается извне (PlayerMovementSystem) ----------

    public void StartRaising()
    {
        _buttonHeld = true;
        _wantsToRelease = false;

        float armSpeed = _raiseTowardUpperLimit ? Mathf.Abs(_armMotorSpeed) : -Mathf.Abs(_armMotorSpeed);
        float handSpeed = _raiseTowardUpperLimit ? Mathf.Abs(_handMotorSpeed) : -Mathf.Abs(_handMotorSpeed);

        SetMotor(_armHinge, armSpeed, _armMotorForce, true);
        SetMotor(_handHinge, handSpeed, _handMotorForce, true);

        TryGrabIfTouching();
    }

    public void StopRaising()
    {
        _buttonHeld = false;

        if (_isGripping)
        {
            // Если захват двуручный и вторая рука всё ещё держится — не отцепляемся, а запоминаем намерение
            if (_gripInvolvedBothHands && _otherHand != null && _otherHand.IsGripping)
            {
                _wantsToRelease = true;
                // моторы оставляем включёнными, чтобы рука не болталась
                return;
            }
            else
            {
                ReleaseAndLaunch();
            }
        }

        // Если не в захвате (или после одиночного броска) — выключаем моторы
        SetMotor(_armHinge, 0f, 0f, false);
        SetMotor(_handHinge, 0f, 0f, false);
    }
    
    public void ForceReleaseGrip()
    {
        _wantsToRelease = false;
        ReleaseGrip();
        SetMotor(_armHinge, 0f, 0f, false);
        SetMotor(_handHinge, 0f, 0f, false);
    }

    // ---------- Захват: подтягивание к точке ----------

    void TryGrabIfTouching()
    {
        if (_isGripping) return;
        if (_stamina != null && _stamina.IsExhausted) return;

        Transform point = GetNearestTouchingPoint();
        if (point == null) return;
        
        OnGripAudio?.Invoke();

        Grab(point);
    }

    void Grab(Transform point)
    {
        _grabWorldPosition = point.position;
        _grabWorldPositionForHand = point.position;
        if (_side == HandSide.Left)
            _grabWorldPositionForHand += Vector3.left * _stepBetweenHands;
        else
            _grabWorldPositionForHand -= Vector3.left * _stepBetweenHands;

        _gripInvolvedBothHands = false;
        _wantsToRelease = false;

        bool otherHandGripping = _otherHand != null && _otherHand.IsGripping;

        if (otherHandGripping)
        {
            _gripInvolvedBothHands = true;
            _otherHand._gripInvolvedBothHands = true;
        }
        else
        {
            Vector2 delta = (Vector2)point.position - _rb.position;
            if (_bodyRigidbody != null) _bodyRigidbody.position += delta;
            _rb.position = _grabWorldPositionForHand;
        }

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
        _joint.connectedAnchor = _grabWorldPositionForHand;
        _joint.frequency = _jointFrequency;
        _joint.dampingRatio = _jointDamping;
        _joint.distance = 0f;
        _joint.enableCollision = false;
    }

    void ReleaseGrip()
    {
        if (_joint != null) Destroy(_joint);
        _isGripping = false;
        _gripInvolvedBothHands = false;
        _wantsToRelease = false;
        _buttonHeld = false;

        SetGripVisual(false);
        SetGrabMarkerVisible(false);
        SetAimLineVisible(false);
    }

    void ReleaseAndLaunch()
    {
        Vector3 mouseWorld = GetMouseWorldPosition();

        Vector3 referencePoint = _grabWorldPosition;
        float forceMultiplier = _launchForceMultiplier;
        float maxDistance = _maxLaunchDistance;

        if (_gripInvolvedBothHands && _otherHand != null)
        {
            referencePoint = (_grabWorldPosition + _otherHand.LastGrabPosition) * 0.5f;
            forceMultiplier = _twoHandLaunchForceMultiplier;
            maxDistance = _twoHandMaxLaunchDistance;
        }

        Vector2 pullVector = (Vector2)(referencePoint - mouseWorld);
        float distance = Mathf.Min(pullVector.magnitude, maxDistance);

        if (_bodyRigidbody != null && distance > 0.1f)
        {
            Vector2 launchDirection = pullVector.normalized;
            _bodyRigidbody.linearVelocity = launchDirection * (distance * forceMultiplier);
            if (bodyMovement != null) bodyMovement.NotifyLaunched(_launchLockDuration);
        }

        ReleaseGrip();
    }

    // ---------- Контакт коллайдеров ----------

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<GrabPoint>() == null) return;

        if (!_touchingPoints.Contains(other.transform))
            _touchingPoints.Add(other.transform);

        if (_buttonHeld) TryGrabIfTouching();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        _touchingPoints.Remove(other.transform);
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
        
        if (_arrowHead != null)
            _arrowHead.gameObject.SetActive(visible);
    }

    void UpdateAimLine()
    {
        if (_aimLine == null) return;

        Vector3 mouseWorld = GetMouseWorldPosition();
        Vector3 pullVector = mouseWorld - _grabWorldPosition;
        
        // Ограничиваем длину линии максимальной дистанцией запуска
        float maxDistance = _gripInvolvedBothHands ? _twoHandMaxLaunchDistance : _maxLaunchDistance;
        Vector3 clampedVector = Vector3.ClampMagnitude(pullVector, maxDistance);
        Vector3 lineEnd = _grabWorldPosition + clampedVector;

        // Рисуем линию фиксированной максимальной длины
        _aimLine.positionCount = 2;
        _aimLine.SetPosition(0, _grabWorldPosition);
        _aimLine.SetPosition(1, lineEnd);

        // Позиционируем стрелку на конце линии
        if (_arrowHead != null)
        {
            _arrowHead.position = lineEnd;
            
            // Поворачиваем стрелку в направлении от точки захвата
            if (pullVector.magnitude > 0.01f)
            {
                float angle = Mathf.Atan2(pullVector.y, pullVector.x) * Mathf.Rad2Deg;
                _arrowHead.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = Mathf.Abs(_mainCamera.transform.position.z - transform.position.z);
        return _mainCamera.ScreenToWorldPoint(mouseScreen);
    }
    
    void UpdateHandColor()
    {
        if (_handSpriteRenderer == null || _stamina == null) return;

        Color target = Color.Lerp(_exhaustedHandColor, _restedHandColor, _stamina.StaminaFraction01);
        _handSpriteRenderer.color = Color.Lerp(_handSpriteRenderer.color, target, _colorLerpSpeed * Time.deltaTime);
    }
}