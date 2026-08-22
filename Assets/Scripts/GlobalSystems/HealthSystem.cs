using System;
using Player;
using UnityEngine;

namespace GlobalSystems
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField]
        private int _maxHp = 3;
        
        [SerializeField]
        private int _currentHp = 3;
        
        public bool IsAlive => _currentHp > 0;
        
        public event Action OnDeath;
        
        private PlayerHealth _playerHealth;

        public void UpdateBodyHealth(PlayerHealth playerHealth)
        {
            if (_playerHealth != null)
                _playerHealth.OnHit -= TakeDamage;
            
            _playerHealth = playerHealth;
            _playerHealth.OnHit += TakeDamage;
        }
        
        public void TakeDamage()
        {
            if (!IsAlive)
                return;
            
            _currentHp--;

            if (_currentHp != 0)
                return;
            
            var list = GetComponentsInChildren<HingeJoint2D>();
            foreach (var item in list)
                item.enabled = false;
                
            Debug.Log("Dead");
            OnDeath?.Invoke();
        }

        public void Restart()
        {
            _currentHp = _maxHp;
        }
        
    }
}