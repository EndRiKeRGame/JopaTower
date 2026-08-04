using UnityEngine;

public enum HandSide { Left, Right }

/// <summary>
/// Вешается на объект руки/ладони (Rigidbody2D + Collider2D).
/// ЛКМ управляет Left, ПКМ управляет Right (или наоборот — настраивается полем side).
///
/// Логика одной руки:
/// 1. MouseButtonDown -> рука СРАЗУ начинает "тянуться" мотором HingeJoint2D к верхнему
///    пределу (визуальный фидбек "кнопка нажата"), и параллельно ищем GrabPoint рядом —
///    если есть силы и точка в радиусе, хватаемся (создаём SpringJoint2D к точке в мире).
/// 2. Пока держим кнопку — рука закреплена (если ухватилась), стамина тратится,
///    игрок двигает мышь, "отводя" тело — прицеливание.
/// 3. MouseButtonUp -> мотор подъёма выключается; если рука держалась — считаем вектор
///    "оттягивания" (от точки захвата до текущей позиции мыши в мире) и толкаем
///    ГЛАВНОЕ ТЕЛО импульсом в противоположную сторону (рывок вверх), убираем сустав захвата.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class HandGrip : MonoBehaviour
{
    [Header("Общее")]
    public HandSide side;
    public Camera mainCamera;
    public Rigidbody2D bodyRigidbody;      // главное тело персонажа, которое будем запускать
    public StaminaSystem stamina;          // компонент на этом же объекте руки

    [Header("Поиск точки захвата")]
    public float grabRadius = 0.7f;
    public LayerMask grabbableLayer;

    [Header("Сустав захвата (пока держимся)")]
    public float jointFrequency = 4f;
    public float jointDamping = 0.6f;

    [Header("Запуск при отпускании")]
    public float launchForceMultiplier = 6f;
    public float maxLaunchDistance = 4f;   // ограничение силы рывка, чтобы не улетать в космос

    [Header("Расход стамины")]
    public float staminaDrainPerSecond = 12f;
    public float staminaDrainUnderLoadMultiplier = 1.8f;
    public float loadForceThreshold = 5f;

    [Header("Фидбек 'рука тянется' при нажатии кнопки")]
    [Tooltip("HingeJoint2D плеча (Left Arm / Right Arm), которым рука физически поднимается")]
    public HingeJoint2D armHinge;
    [Tooltip("Необязательно: HingeJoint2D кисти, если хотите поднимать и её тоже")]
    public HingeJoint2D handHinge;
    [Tooltip("Если рука поднимается не в ту сторону — переключите этот флажок")]
    public bool raiseTowardUpperLimit = true;
    public float raiseMotorSpeed = 400f;
    public float raiseMotorForce = 200f;

    private SpringJoint2D _joint;
    private Rigidbody2D _rb;
    private Vector3 _grabWorldPosition;
    private bool _isGripping;

    public bool IsGripping => _isGripping;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (mainCamera == null) mainCamera = Camera.main;
        if (stamina == null) stamina = GetComponent<StaminaSystem>();
    }

    void Update()
    {
        int mouseButton = side == HandSide.Left ? 0 : 1;

        if (Input.GetMouseButtonDown(mouseButton))
        {
            StartRaising();
            if (!_isGripping) TryGrab();
        }

        if (_isGripping)
        {
            stamina.isResting = false;
            DrainStamina();
        }
        else
        {
            stamina.isResting = true;
        }

        if (Input.GetMouseButtonUp(mouseButton))
        {
            StopRaising();
            if (_isGripping) ReleaseAndLaunch();
        }
    }

    // ---------- Фидбек нажатия: мотор тянет руку к верхнему пределу шарнира ----------

    void StartRaising()
    {
        float speed = raiseTowardUpperLimit ? Mathf.Abs(raiseMotorSpeed) : -Mathf.Abs(raiseMotorSpeed);
        SetMotor(armHinge, speed, raiseMotorForce, true);
        SetMotor(handHinge, speed, raiseMotorForce, true);
    }

    void StopRaising()
    {
        SetMotor(armHinge, 0f, 0f, false);
        SetMotor(handHinge, 0f, 0f, false);
    }

    void SetMotor(HingeJoint2D hinge, float speed, float force, bool enable)
    {
        if (hinge == null) return;
        JointMotor2D motor = hinge.motor;
        motor.motorSpeed = speed;
        motor.maxMotorTorque = force;
        hinge.motor = motor;
        hinge.useMotor = enable;
    }

    // ---------- Захват точки ----------

    void TryGrab()
    {
        if (stamina.IsExhausted) return; // рука слишком устала, чтобы ухватиться

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, grabRadius, grabbableLayer);
        if (hits.Length == 0) return;

        Transform best = null;
        float bestDist = float.MaxValue;
        foreach (var hit in hits)
        {
            float d = Vector2.Distance(transform.position, hit.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = hit.transform;
            }
        }
        if (best == null) return;

        _grabWorldPosition = best.position;
        CreateJoint();
        _isGripping = true;
    }

    void CreateJoint()
    {
        _joint = gameObject.AddComponent<SpringJoint2D>();
        _joint.autoConfigureConnectedAnchor = false;
        _joint.connectedAnchor = _grabWorldPosition; // связь с точкой в мировых координатах
        _joint.frequency = jointFrequency;
        _joint.dampingRatio = jointDamping;
        _joint.distance = 0f;
        _joint.enableCollision = false;
    }

    void DrainStamina()
    {
        float load = _joint != null ? _joint.reactionForce.magnitude : 0f;
        float drain = staminaDrainPerSecond * Time.deltaTime;
        if (load > loadForceThreshold) drain *= staminaDrainUnderLoadMultiplier;

        stamina.Drain(drain);

        if (stamina.IsExhausted)
        {
            // рука срывается сама, без импульса рывка — просто отпускает
            if (_joint != null) Destroy(_joint);
            _isGripping = false;
        }
    }

    void ReleaseAndLaunch()
    {
        Vector3 mouseWorld = GetMouseWorldPosition();

        // вектор "оттягивания": от текущей позиции мыши к точке захвата
        Vector2 pullVector = (Vector2)(_grabWorldPosition - mouseWorld);
        float distance = Mathf.Min(pullVector.magnitude, maxLaunchDistance);
        Vector2 launchDirection = pullVector.normalized;

        if (bodyRigidbody != null && distance > 0.1f)
        {
            bodyRigidbody.AddForce(launchDirection * distance * launchForceMultiplier, ForceMode2D.Impulse);
        }

        if (_joint != null) Destroy(_joint);
        _isGripping = false;
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        return mainCamera.ScreenToWorldPoint(mouseScreen);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, grabRadius);
    }
}