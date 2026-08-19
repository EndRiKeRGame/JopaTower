using UnityEngine;

/// <summary>
/// Принимает попадания опасностей (осколки и т.п.) и сбивает игрока.
/// НЕ модифицирует HandGrip — только вызывает его существующий публичный метод.
/// Висит на корне игрока, чтобы его можно было найти из любого детского коллайдера.
/// </summary>
public class HazardHitReceiver : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private HandGrip[] _hands; // обе руки: левая и правая
    [SerializeField] private float _knockbackForce = 6f; // сила сбивания

    private Rigidbody2D _body;

    void Awake()
    {
        // Тело персонажа висит на корне игрока (там же, где BodyMovement)
        _body = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Вызывается извне (осколком) в момент попадания.
    /// hitDirection — куда отбрасывает игрока (например, вниз от падающего осколка).
    /// </summary>
    public void OnHazardHit(Vector2 hitDirection)
    {
        // Отпускаем ОБЕ руки: если игрок висел на двух хватах, рвём оба
        foreach (HandGrip hand in _hands)
        {
            if (hand != null) hand.ForceReleaseGrip();
        }

        // Отбрасываем тело, чтобы попадание чувствовалось
        if (_body != null && hitDirection.sqrMagnitude > 0.01f)
        {
            _body.linearVelocity = hitDirection.normalized * _knockbackForce;
        }
    }
}