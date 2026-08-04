using UnityEngine;

/// <summary>
/// Вешается на пустой дочерний GameObject внутри каждого префаба платформы —
/// в том месте, за которое персонаж физически может ухватиться (край, труба, уступ).
/// Объект должен иметь Collider2D (isTrigger = true) и слой "Grabbable".
/// </summary>
public class GrabPoint : MonoBehaviour
{
    [Tooltip("Некоторые точки можно пометить как более скользкие/сложные — используйте по желанию")]
    public bool isSlippery = false;

    [Range(0.5f, 2f)]
    public float grabDifficultyMultiplier = 1f; // можно домножать на расход стамины в HandGrip

    void OnDrawGizmos()
    {
        Gizmos.color = isSlippery ? Color.red : Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.12f);
    }
}
