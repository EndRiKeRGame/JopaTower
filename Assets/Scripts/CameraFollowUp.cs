using UnityEngine;

/// <summary>
/// Камера как в Doodle Jump: плавно следует за игроком вверх,
/// но никогда не опускается ниже максимальной достигнутой высоты.
/// </summary>
public class CameraFollowUp : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.25f;

    private float _highestY;
    private Vector3 _velocity;

    void Start()
    {
        _highestY = transform.position.y;
    }

    void LateUpdate()
    {
        if (target.position.y > _highestY)
        {
            _highestY = target.position.y;
        }

        // Vector3 targetPos = new Vector3(transform.position.x, _highestY, transform.position.z);
        Vector3 targetPos = new Vector3(transform.position.x, target.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _velocity, smoothTime);
    }
}
