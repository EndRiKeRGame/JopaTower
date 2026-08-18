using System;
using UnityEngine;

namespace Player
{
    public class BodyHealth : MonoBehaviour
    {
        public event Action OnHit;

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("DeadZone"))
                return;
            
            OnHit?.Invoke();
            Debug.Log("Hit");
        }
    }
}