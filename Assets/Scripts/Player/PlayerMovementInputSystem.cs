using System;
using Enums;
using UnityEngine;

namespace Player
{
    public class PlayerMovementInputSystem : MonoBehaviour
    {
        public bool IsMovable { get; set; } = true;

        public PlayerHandGrip LeftHandGrip => _leftPlayerHand;
        public PlayerHandGrip RightHandGrip => _rightPlayerHand;
    
        [SerializeField]
        private PlayerBodyMovement _playerBody;
    
        [SerializeField]
        private PlayerHandGrip _leftPlayerHand;

        [SerializeField]
        private PlayerHandGrip _rightPlayerHand;
        
        [SerializeField]
        private Rigidbody2D[] _playerBodyRigidBodies;

        private void Update()
        {
            if (!IsMovable)
                return;
        
            float input = 0f;
        
            if (Input.GetKey(KeyCode.A))
                input -= 1f;
        
            if (Input.GetKey(KeyCode.D))
                input += 1f;

            _playerBody.AddForce(input);
        
            if (Input.GetMouseButtonDown((int)HandSide.Left))
            {
                _leftPlayerHand.StartRaising();
            }
        
            if (Input.GetMouseButtonUp((int)HandSide.Left))
            {
                _leftPlayerHand.StopRaising();
            }
        
            if (Input.GetMouseButtonDown((int)HandSide.Right))
            {
                _rightPlayerHand.StartRaising();
            }
        
            if (Input.GetMouseButtonUp((int)HandSide.Right))
            {
                _rightPlayerHand.StopRaising();
            }
        }

        public void StartPlayer()
        {
            IsMovable = true;
            foreach (Rigidbody2D bodyRigidBod in _playerBodyRigidBodies)
            {
                bodyRigidBod.simulated = true;
            }
        }
        
        public void StopPlayer()
        {
            IsMovable = false;
            foreach (Rigidbody2D bodyRigidBod in _playerBodyRigidBodies)
            {
                bodyRigidBod.simulated = false;
            }
        }
    }
}
