using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    public class BodyProgression : MonoBehaviour
    {
        public event Action<string> OnTriggerEnter;
        
        private List<string> _triggersTags = new()
        {
            "Prologue",
            "Act1",
            "Act2",
            "Act3",
            "Finale",
        };

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_triggersTags.Contains(other.tag))
            {
                OnTriggerEnter?.Invoke(other.tag);
            }
        }
    }
}