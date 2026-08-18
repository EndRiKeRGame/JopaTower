using UnityEngine;

/// <summary>
/// Падающий осколок Акта 2.
/// Летит вниз, крутится, при контакте с игроком сбивает его через HazardHitReceiver.
/// Не выходит за пределы акта: умирает на нижней границе, которую передаёт режиссёр.
/// </summary>
public class FallingDebris : MonoBehaviour
{
    [Header("Падение — можно различать по типам осколков")]
    [SerializeField] private float _fallSpeed = 9f;   // скорость падения
    [SerializeField] private float _spinSpeed = 120f; // градусов в секунду

    private Transform _player;
    private float _killBelowY = -9999f; // ниже этой высоты осколок умирает

    /// <summary>Режиссёр вызывает сразу после спавна.</summary>
    public void Init(Transform player, float killBelowY)
    {
        _player = player;
        _killBelowY = killBelowY;
    }

    void Update()
    {
        // Падение и вращение без физики — предсказуемо и дёшево
        transform.position += Vector3.down * (_fallSpeed * Time.deltaTime);
        transform.Rotate(0f, 0f, _spinSpeed * Time.deltaTime);

        // Улетел за нижнюю границу акта — убираем
        if (transform.position.y < _killBelowY)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Поймать нас мог коллайдер тела, руки или кисти —
        // приёмник висит на корне игрока, поэтому лезем вверх по иерархии.
        HazardHitReceiver receiver = other.GetComponentInParent<HazardHitReceiver>();
        if (receiver == null) return;

        // Сбиваем игрока: рвём хват и отбрасываем вниз
        receiver.OnHazardHit(Vector2.down);

        // Осколок свою работу сделал — разбивается
        Destroy(gameObject);
    }
}