using UnityEngine;

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

        private void OnValidate()
        {
            _bodyHealth = GetComponent<BodyHealth>();
            _bodyProgression = GetComponent<BodyProgression>();
        }

        public void Setup(Transform grabMarker, LineRenderer aimLine)
        {
            _left.Setup(grabMarker, aimLine);
            _right.Setup(grabMarker, aimLine);
        }
    }
}