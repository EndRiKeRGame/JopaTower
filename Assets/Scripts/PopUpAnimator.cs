using Configs;
using PrimeTween;
using TMPro;
using UnityEngine;

public class PopUpAnimator : MonoBehaviour
{
    [SerializeField]
    private TweenConfig _config;
        
    [SerializeField]
    private CanvasGroup _canvasGroupMain;
        
    [SerializeField]
    private RectTransform _rectTransformImage;

    [SerializeField]
    private TMP_Text _text;
        
    [SerializeField]
    private CanvasGroup _canvasGroupText;

    [SerializeField]
    private Vector3 _posDelta = new Vector3(0f, 20f, 0f);

    private Sequence _sequence;
    private float _animationTime = 0.2f;
    private Vector3 _startPos;
    private Vector3 _endPos;

    private void Awake()
    {
        _startPos = _rectTransformImage.transform.position;
        _endPos = _startPos - _posDelta;
    }

    public void StartAnimation(string msg)
    {
        _text.text = msg;
        _canvasGroupMain.alpha = 1f;
        _canvasGroupText.alpha = 0f;
            
        _sequence = Sequence.Create();
        _sequence.Insert(0f, Tween.Position(_rectTransformImage, new (_endPos, _config.DefaultSettings)));
        _sequence.Insert(0.3f, Tween.Alpha(_canvasGroupText, new (1f, _config.DefaultSettings)));
        _sequence.Insert(2.2f, Tween.Alpha(_canvasGroupText, new (0f, _config.DefaultSettings)));
        _sequence.Insert(2.4f, Tween.Position(_rectTransformImage, new (_startPos, _config.DefaultSettings)));
    }
}