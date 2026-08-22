using System;
using Player;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DeathZoneHeartbeat : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _heartbeatClip;
    [SerializeField] private VolumeProfile _volumeProfile;
    private Transform _player;
    private Transform _deathZone;

    [Header("Дистанции реакции")]
    [SerializeField] private float _maxDistance = 15f; // дальше этой дистанции — тишина и обычная виньетка
    [SerializeField] private float _minDistance = 2f;  // ближе этой дистанции — максимальная интенсивность

    [Header("Ритм сердцебиения")]
    [SerializeField] private float _beatIntervalFar = 1.2f;  // пауза между ударами у границы _maxDistance
    [SerializeField] private float _beatIntervalNear = 0.25f; // пауза между ударами у _minDistance
    [SerializeField] private float _volumeFar = 0.1f;
    [SerializeField] private float _volumeNear = 1f;

    [Header("Виньетка")]
    [SerializeField] private float _vignetteIntensityFar = 0f;
    [SerializeField] private float _vignetteIntensityNear = 0.5f;
    [SerializeField] private float _vignetteSmooth = 0.7f;

    private Vignette _vignette;
    private float _beatTimer;

    void Awake()
    {
        if (_volumeProfile != null)
            _volumeProfile.TryGet(out _vignette);
    }

    public void Init(PlayerController player, DeathFloor deathZone)
    {
        _player = player.transform;
        _deathZone = deathZone.Counter.transform;
    }

    public void StopHeartbeat()
    {
        _player = null;
    }

    void Update()
    {
        if (_player == null || _deathZone == null) return;

        float distance = Math.Abs(_player.position.y - _deathZone.transform.position.y);

        // 0 — на границе _maxDistance (эффекта нет), 1 — на _minDistance или ближе (максимум)
        float t = Mathf.InverseLerp(_maxDistance, _minDistance, distance);
        t = Mathf.Clamp01(t);

        UpdateHeartbeat(t, distance);
        UpdateVignette(t);
    }

    // ---------- Сердцебиение ----------

    void UpdateHeartbeat(float t, float distance)
    {
        if (distance >= _maxDistance)
        {
            _beatTimer = 0f;
            return;
        }

        _beatTimer -= Time.deltaTime;
        if (_beatTimer <= 0f)
        {
            float interval = Mathf.Lerp(_beatIntervalFar, _beatIntervalNear, t);
            float volume = Mathf.Lerp(_volumeFar, _volumeNear, t);

            if (_audioSource != null && _heartbeatClip != null)
                _audioSource.PlayOneShot(_heartbeatClip, volume);

            _beatTimer = interval;
        }
    }

    // ---------- Виньетка ----------

    void UpdateVignette(float t)
    {
        if (_vignette == null) return;
        
        _vignette.intensity.value = Mathf.Lerp(_vignetteIntensityFar, _vignetteIntensityNear, t);
    }
}