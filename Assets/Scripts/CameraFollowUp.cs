using PrimeTween;
using UnityEngine;

/// <summary>
/// Камера как в Doodle Jump: плавно следует за игроком вверх,
/// но никогда не опускается ниже максимальной достигнутой высоты.
/// </summary>
public class CameraFollowUp : MonoBehaviour
{
    [SerializeField]
    public Camera _camera;
    
    public Transform target;
    public float smoothTime = 0.25f;

    private Vector3 _velocity;
    
    void LateUpdate()
    {
        Vector3 targetPos = new Vector3(transform.position.x, target.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _velocity, smoothTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ChangeCameraZoomTo(float val, float animDur = 0.3f)
    {
        Tween.CameraOrthographicSize(_camera, val, animDur);
    }
}
