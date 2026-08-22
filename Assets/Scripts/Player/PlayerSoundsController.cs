using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    public class PlayerSoundsController : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _audioSource;
        
        [SerializeField]
        private AudioClip[] _audioGenerators;
        
        private PlayerHandGrip _leftPlayerHand;
        private PlayerHandGrip _rightPlayerHand;
        
        public void Setup(PlayerHandGrip left, PlayerHandGrip right)
        {
            _leftPlayerHand = left;
            _rightPlayerHand = right;
            
            _leftPlayerHand.OnGripAudio += PlaySound;
            _rightPlayerHand.OnGripAudio += PlaySound;
        }
        
        private void PlaySound()
        {
            _audioSource.PlayOneShot(_audioGenerators[Random.Range(0, _audioGenerators.Length)]);
        }
        
        private void OnDestroy()
        {
            _leftPlayerHand.OnGripAudio -= PlaySound;
            _rightPlayerHand.OnGripAudio -= PlaySound;
        }
    }
}