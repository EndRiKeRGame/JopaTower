using System;
using DefaultNamespace;
using DefaultNamespace.Ui;
using Enums;
using PrimeTween;
using UnityEngine;

namespace Player
{
    public class ProgressionSystem : MonoBehaviour
    {
        [SerializeField]
        private float _deathFloorStep = 20f;
        
        private PopUpAnimator _popUpAnimator;
        private DeathFloor _deathFloor;
        private BodyProgression _bodyProgression;
        private DialogueTextConfig _dialogueText;
        private DialogueView _dialogueView;

        private bool _prologue = false;
        private bool _act1 = false;
        private bool _act2 = false;
        private bool _act3 = false;
        private bool _finale = false;

        public event Action OnFinaleDo;

        public void Init(DeathFloor deathFloor, PopUpAnimator popUpAnimator, DialogueTextConfig dialogueText, DialogueView dialogueView)
        {
            _deathFloor = deathFloor;
            _popUpAnimator = popUpAnimator;
            _dialogueText = dialogueText;
            _dialogueView = dialogueView;
        }

        public void UpdateBodyProgression(BodyProgression bodyProgression)
        {
            if (_bodyProgression != null)
                _bodyProgression.OnTriggerEnter -= CheckTrigger;
            
            _bodyProgression = bodyProgression;
            _bodyProgression.OnTriggerEnter += CheckTrigger;
        }
        
        
        private void CheckTrigger(string triggerName)
        {
            switch (triggerName)
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

        public void Restart()
        {
            bool _prologue = false;
            bool _act1 = false;
            bool _act2 = false;
            bool _act3 = false;
            bool _finale = false;
        }

        private void OnPrologue()
        {
            if (_prologue)
                return;
            
            _prologue = true;
            
            Debug.Log($"Player has prologue: {nameof(Triggers.Prologue)}");
            StopDeathFloorAndSetNewPos();
            StopPlayer();
            
            _dialogueView.ShowDialogue(_dialogueText.Prologue, () =>
            {
                StartPlayer();
                _popUpAnimator.StartAnimation("ПРОЛОГ");
            });
        }
        
        private void OnAct1()
        {
            if (_act1)
                return;
            
            _act1 = true;
            
            Debug.Log($"Player has АКТ 1: {nameof(Triggers.Act1)}");
            StopDeathFloorAndSetNewPos();
            StopPlayer();
            _dialogueView.ShowDialogue(_dialogueText.Act1, () =>
            {
                StartPlayer();
                _popUpAnimator.StartAnimation("АКТ 1");
            });
        }
        
        private void OnAct2()
        {
            if (_act2)
                return;
            
            _act2 = true;
            
            Debug.Log($"Player has АКТ 2: {nameof(Triggers.Act2)}");
            StopDeathFloorAndSetNewPos();
            StopPlayer();
            _dialogueView.ShowDialogue(_dialogueText.Act2, () =>
            {
                StartPlayer();
                _popUpAnimator.StartAnimation("АКТ 2");
            });
        }
        
        private void OnAct3()
        {
            if (_act3)
                return;
            
            _act3 = true;
            
            Debug.Log($"Player has АКТ 3: {nameof(Triggers.Act3)}");
            StopDeathFloorAndSetNewPos();
            StopPlayer();
            _dialogueView.ShowDialogue(_dialogueText.Act3, () =>
            {
                StartPlayer();
                _popUpAnimator.StartAnimation("АКТ 3");
            });
        }
        
        private void OnFinale()
        {
            if (_finale)
                return;
            
            _finale = true;
            
            Debug.Log($"Player has ФИНАЛ: {nameof(Triggers.Finale)}");
            StopDeathFloorAndSetNewPos();
            _popUpAnimator.StartAnimation("????????????");
            StopPlayer();
            _dialogueView.ShowDialogue(_dialogueText.Finale, () =>
            {
                var seq = Sequence.Create();
                seq.ChainDelay(5f);
                seq.ChainCallback(() => OnFinaleDo?.Invoke());
            });
            
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

        private void StopPlayer()
        {
            _bodyProgression.GetComponent<PlayerMovementSystem>().IsMoveable = false;
        }
        
        private void StartPlayer()
        {
            _bodyProgression.GetComponent<PlayerMovementSystem>().IsMoveable = true;
        }
    }
}