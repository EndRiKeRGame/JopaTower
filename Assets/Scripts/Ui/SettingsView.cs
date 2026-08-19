using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class SettingsView : MonoBehaviour
{
    [SerializeField] private Image _soundImg;
    [SerializeField] private Sprite _soundOn;
    [SerializeField] private Sprite _soundOff;
    [SerializeField] private Button _switch;
    [SerializeField] private AudioSource _audioSource;
    
    private bool _isSoundOn = true;

    private void Awake()
    {
        _switch.onClick.AddListener(ChangeSoundSettings);
    }

    private void ChangeSoundSettings()
    {
        _isSoundOn = !_isSoundOn;
        _soundImg.sprite = _isSoundOn ? _soundOn : _soundOff;
        _audioSource.mute = _isSoundOn;
    }
}