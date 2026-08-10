using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField]
        private int _health = 3;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_health > 0 && !other.gameObject.CompareTag("DeadZone"))
                return;
            
            _health--;

            if (_health == 0)
                Debug.Log("Death");
        }
    }
}