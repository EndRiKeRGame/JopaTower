using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    public class Player : MonoBehaviour
    {
        public BodyHealth Health => _bodyHealth;
        public BodyProgression Progression => _bodyProgression;
        
        [SerializeField]
        private BodyHealth _bodyHealth;
        
        [SerializeField]
        private BodyProgression _bodyProgression;
        
        [SerializeField]
        private HandGrip _left;
        
        [SerializeField]
        private HandGrip _right;
        
        [SerializeField]
        private AudioSource _audioSource;
        
        [SerializeField]
        private AudioClip[] _audioGenerators;
        
        [SerializeField]
        private float _forceDown = 10f;

        private void OnValidate()
        {
            _bodyHealth = GetComponent<BodyHealth>();
            _bodyProgression = GetComponent<BodyProgression>();
        }

        public void Setup(Transform grabMarker, LineRenderer aimLine)
        {
            _left.Setup(grabMarker, aimLine);
            _right.Setup(grabMarker, aimLine);

            _left.OnGripAudio += PlaySound;
            _right.OnGripAudio += PlaySound;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Crystal"))
            {
                _left.ForceReleaseGrip();
                _right.ForceReleaseGrip(); 
                var body = GetComponent<Rigidbody2D>();
                body.AddForce(Vector2.down * _forceDown);
            }
        }

        private void PlaySound()
        {
            _audioSource.PlayOneShot(_audioGenerators[Random.Range(0, _audioGenerators.Length)]);
        }

        private void OnDestroy()
        {
            _left.OnGripAudio -= PlaySound;
            _right.OnGripAudio -= PlaySound;
        }
    }
}