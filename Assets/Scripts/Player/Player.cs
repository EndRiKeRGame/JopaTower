using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private HandGrip _left;
        
        [SerializeField]
        private HandGrip _right;
        
        public void Setup(Transform grabMarker, LineRenderer aimLine)
        {
            _left.Setup(grabMarker, aimLine);
            _right.Setup(grabMarker, aimLine);
        }
    }
}