using DefaultNamespace;
using Enums;
using UnityEngine;

namespace Player
{
    public class ProgressionSystem : MonoBehaviour
    {
        [SerializeField]
        private float _deathFloorStep = 20f;
        
        private PopUpAnimator _popUpAnimator;
        private DeathFloor _deathFloor;

        private bool _prologue = false;
        private bool _act1 = false;
        private bool _act2 = false;
        private bool _act3 = false;
        private bool _finale = false;

        public void Init(DeathFloor deathFloor, PopUpAnimator popUpAnimator)
        {
            _deathFloor = deathFloor;
            _popUpAnimator = popUpAnimator;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            switch (other.gameObject.tag)
            {
                case nameof(Triggers.Prologue):
                    OnPrologue();
                    break;
                
                case nameof(Triggers.Act1):
                    OnAct1();
                    break;
                
                case nameof(Triggers.Act2):
                    OnAct2();
                    break;
                
                case nameof(Triggers.Act3):
                    OnAct3();
                    break;
                
                case nameof(Triggers.Finale):
                    OnFinale();
                    break;
                
                default:
                    break;
            }
        }

        private void OnPrologue()
        {
            if (_prologue)
                return;
            
            _prologue = true;
            
            Debug.Log($"Player has prologue: {nameof(Triggers.Prologue)}");
            StopDeathFloorAndSetNewPos();
            _popUpAnimator.StartAnimation(nameof(Triggers.Prologue));
        }
        
        private void OnAct1()
        {
            if (_act1)
                return;
            
            _act1 = true;
            
            Debug.Log($"Player has prologue: {nameof(Triggers.Act1)}");
            StopDeathFloorAndSetNewPos();
            StartDeathFloor();
            _popUpAnimator.StartAnimation(nameof(Triggers.Act1));
        }
        
        private void OnAct2()
        {
            if (_act2)
                return;
            
            _act2 = true;
            
            Debug.Log($"Player has prologue: {nameof(Triggers.Act2)}");
            StopDeathFloorAndSetNewPos();
            StartDeathFloor();
            _popUpAnimator.StartAnimation(nameof(Triggers.Act2));
        }
        
        private void OnAct3()
        {
            if (_act3)
                return;
            
            _act3 = true;
            
            Debug.Log($"Player has prologue: {nameof(Triggers.Act3)}");
            StopDeathFloorAndSetNewPos();
            StartDeathFloor();
            _popUpAnimator.StartAnimation(nameof(Triggers.Act3));
        }
        
        private void OnFinale()
        {
            if (_finale)
                return;
            
            _finale = true;
            
            Debug.Log($"Player has prologue: {nameof(Triggers.Finale)}");
            StopDeathFloorAndSetNewPos();
            _popUpAnimator.StartAnimation(nameof(Triggers.Finale));
        }

        private void StopDeathFloorAndSetNewPos()
        {
            var curPos = _deathFloor.GetDeathFloorPos();
            curPos.y = transform.position.y - _deathFloorStep;
            _deathFloor.SetDeathFloor(curPos);
        }
        
        private void StartDeathFloor()
        {
            _deathFloor.StartDeathFloor();
        }
    }
}