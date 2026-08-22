using UnityEngine;

namespace Player
{
    public class PlayerHitController : MonoBehaviour
    {
        [SerializeField]
        private float _knockbackDownForce = 3f;

        [SerializeField]
        private float _stunDuration = 0.3f;
        
        private PlayerHandGrip _leftPlayerHand;
        private PlayerHandGrip _rightPlayerHand;

        public void Setup(PlayerHandGrip leftHand, PlayerHandGrip rightHand)
        {
            _leftPlayerHand = leftHand;
            _rightPlayerHand = rightHand;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Crystal"))
            {
                _leftPlayerHand.Stun(_stunDuration);
                _rightPlayerHand.Stun(_stunDuration);

                var body = GetComponent<Rigidbody2D>();
                body.linearVelocity = new Vector2(body.linearVelocity.x, -_knockbackDownForce);
            }
        }
    }
}