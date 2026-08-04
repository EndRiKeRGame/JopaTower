using UnityEngine;

/// <summary>
/// Даёт части тела (плечо, ладонь) эффект "дрыгания" — она физически связана
/// с родительской частью через HingeJoint2D (ограничивает разброс), а этот скрипт
/// добавляет пружинный крутящий момент, тянущий часть тела к "нейтральному" углу
/// относительно родителя.
///
/// v2: сила нормируется под момент инерции тела (Rigidbody2D.inertia), поэтому
/// поведение не "взрывается" при маленьком/неравномерном коллайдере — раньше
/// то же значение springStrength могло давать разное угловое ускорение в
/// зависимости от массы/формы коллайдера. Плюс есть жёсткие ограничители.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class JointJiggle : MonoBehaviour
{
    [Header("Родительская часть тела")]
    public Rigidbody2D parentBody;

    [Header("Пружина возврата к нейтральному углу")]
    [Tooltip("Угол в состоянии покоя относительно поворота родителя, градусы")]
    public float restAngleOffset = 0f;
    public float springStrength = 15f;
    public float springDamping = 1.5f;

    [Header("Защита от нестабильности")]
    [Tooltip("Жёсткий предел крутящего момента за шаг физики")]
    public float maxTorque = 5f;
    [Tooltip("Жёсткий предел угловой скорости, град/сек")]
    public float maxAngularVelocity = 720f;

    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (parentBody == null) return;

        float targetAngle = parentBody.rotation + restAngleOffset;
        float angleDiff = Mathf.DeltaAngle(_rb.rotation, targetAngle);

        // Нормируем под момент инерции: AddTorque даёт угловое ускорение = torque / inertia.
        // Умножая на inertia заранее, получаем предсказуемое угловое ускорение
        // ~ (angleDiff*springStrength - angularVelocity*springDamping) независимо
        // от того, насколько маленький/кривой коллайдер получился из-за масштаба.
        float inertia = Mathf.Max(_rb.inertia, 0.001f);
        float torque = (angleDiff * springStrength - _rb.angularVelocity * springDamping) * inertia;
        torque = Mathf.Clamp(torque, -maxTorque, maxTorque);

        _rb.AddTorque(torque);

        if (Mathf.Abs(_rb.angularVelocity) > maxAngularVelocity)
        {
            _rb.angularVelocity = Mathf.Sign(_rb.angularVelocity) * maxAngularVelocity;
        }
    }
}