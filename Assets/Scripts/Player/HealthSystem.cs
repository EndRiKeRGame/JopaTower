using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField]
        private int _health = 3;
        
        public bool IsAlive => _health > 0;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_health > 0 && !other.gameObject.CompareTag("DeadZone"))
                return;
            
            _health--;

            if (_health == 0)
            {
                var list = GetComponentsInChildren<HingeJoint2D>();
                foreach (var item in list)
                {
                    item.enabled = false;
                }
            }
        }
    }
}