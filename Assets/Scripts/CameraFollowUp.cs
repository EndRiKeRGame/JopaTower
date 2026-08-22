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
    
    [SerializeField]
    public float smoothTime = 0.25f;
    
    private Transform _target;
    private Vector3 _velocity;
    private bool _isFollowing = false;

    public void StartWork()
    {
        _isFollowing = true;
    }
    
    public void StopWork()
    {
        _isFollowing = false;
    }
    
    public void SetTarget(Transform newTarget)
    {
        _target = newTarget;
    }

    public void ChangeCameraZoomTo(float val, float animDur)
    {
        Tween.CameraOrthographicSize(_camera, val, animDur);
    }
    
    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }
    
    public void ChangeCameraZoomToInstantly(float val)
    {
        _camera.orthographicSize = val;
    }
    
    private void LateUpdate()
    {
        if (!_isFollowing)
            return;
        
        Vector3 targetPos = new Vector3(transform.position.x, _target.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _velocity, smoothTime);
    }
}
