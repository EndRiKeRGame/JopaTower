using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public PlayerHealth Health => _playerHealth;
        public PlayerProgression Progression => _playerProgression;
        public PlayerMovementInputSystem MovementInput => _playerInput;

        [SerializeField] private PlayerHealth _playerHealth;

        [SerializeField] private PlayerProgression _playerProgression;

        [SerializeField] private PlayerMovementInputSystem _playerInput;

        [SerializeField] private PlayerSoundsController _playerSoundsController;

        [SerializeField] private PlayerHitController _playerHitController;

        private void OnValidate()
        {
            _playerHealth ??= GetComponent<PlayerHealth>();
            _playerProgression ??= GetComponent<PlayerProgression>();
            _playerInput ??= GetComponent<PlayerMovementInputSystem>();
            _playerSoundsController ??= GetComponent<PlayerSoundsController>();
            _playerHitController ??= GetComponent<PlayerHitController>();
        }

        public void Setup(Transform grabMarker, LineRenderer aimLine)
        {
            _playerInput.LeftHandGrip.Setup(grabMarker, aimLine);
            _playerInput.RightHandGrip.Setup(grabMarker, aimLine);

            _playerSoundsController.Setup(_playerInput.LeftHandGrip, _playerInput.RightHandGrip);
            _playerHitController.Setup(_playerInput.LeftHandGrip, _playerInput.RightHandGrip);
        }

        public void StartPlayer()
        {
            MovementInput.StartPlayer();
        }
        
        public void StopPlayer()
        {
            MovementInput.StopPlayer();
        }
        
        




}
}