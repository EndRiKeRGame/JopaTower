using System;
using Player;
using UnityEngine;

namespace DefaultNamespace
{
    public class BackgroundController : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _fog;

        private float _t3pos;
        private float _t4pos;
        private float _delta;

        private PlayerController _player;

        private bool _isWork = false;

        public void Init(float t3pos, float t4pos)
        {
            _t3pos = t3pos;
            _t4pos = t4pos;
            _delta = _t4pos - _t3pos;
        }

        public void UpdatePlayer(PlayerController player)
        {
            _player = player;
            ResetAlpha();
        }
        
        public void StartFog()
        {
            _isWork = true;
        }

        public void StopFog()
        {
            _isWork = false;
        }

        private void Update()
        {
            if (!_isWork)
                return;
            
            var color = _fog.color;
            color.a = (_t4pos - _player.transform.position.y) / _delta;
            _fog.color = color;
        }

        private void ResetAlpha()
        {
            var color = _fog.color;
            color.a = 1;
            _fog.color = color;
        }
        
    }
}