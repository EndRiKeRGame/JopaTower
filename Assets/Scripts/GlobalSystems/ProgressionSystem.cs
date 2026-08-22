using System;
using System.Collections.Generic;
using Configs;
using DefaultNamespace;
using Enums;
using Player;
using PrimeTween;
using Ui;
using UnityEngine;

namespace GlobalSystems
{
    public class ProgressionSystem : MonoBehaviour
    {
        public event Action OnComicsComplete;
        
        [SerializeField] private DeathFloor _deathFloor;
        [SerializeField] private ObstacleSpawner _obstacleSpawner;
        
        [SerializeField] private PopUpAnimator _popUpAnimator;
        [SerializeField] private ComicsView _comicsView;
        [SerializeField] private DialogueView _dialogueView;
        
        [SerializeField] private DialogueTextConfig _dialogueText;
        [SerializeField] private float _deathFloorStep = 20f;
        [SerializeField] private BackgroundController _backgroundController;
        
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _audioClip;

        private PlayerController _playerController;

        private bool _prologue = false;
        private bool _act1 = false;
        private bool _act2 = false;
        private bool _act3 = false;
        private bool _finale = false;
        
        private Action PauseGame;
        private Action StartGame;

        public void Init(Action pauseGame, Action startGame)
        {
            PauseGame = pauseGame;
            StartGame = startGame;
        }

        public void UpdatePlayerController(PlayerController playerController)
        {
            if (_playerController != null)
                _playerController.Progression.OnTriggerEnter -= CheckTrigger;
            
            _playerController = playerController;
            _playerController.Progression.OnTriggerEnter += CheckTrigger;
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

        public void Reset()
        {
            _prologue = false;
            _act1 = false;
            _act2 = false;
            _act3 = false;
            _finale = false;
        }

        private void OnPrologue()
        {
            if (_prologue)
                return;
            
            _prologue = true;
            
            PauseGame();
            SetNewPosForDeathFloor();
            
            _dialogueView.ShowDialogue(_dialogueText.Prologue, () =>
            {
                _popUpAnimator.StartAnimation("ПРОЛОГ");
                
                StartGame();
                _deathFloor.StopWork();
            },
            new Dictionary<int, Action>
            {
                // звук поломки толчка 3
                {3, () =>
                {
                    _audioSource.PlayOneShot(_audioClip);
                }}
            }
            );
        }
        
        private void OnAct1()
        {
            if (_act1)
                return;
            
            _act1 = true;
            
            PauseGame();
            SetNewPosForDeathFloor();
            
            _dialogueView.ShowDialogue(_dialogueText.Act1, () =>
            {
                _popUpAnimator.StartAnimation("Тюрьма");
                
                StartGame();
            },
            new Dictionary<int, Action>
            {
                // звук поломки толчка 1
                {1, () =>
                {
                    _audioSource.PlayOneShot(_audioClip);
                }}
            }
            );
        }
        
        private void OnAct2()
        {
            if (_act2)
                return;
            
            _act2 = true;
            
            PauseGame();
            SetNewPosForDeathFloor();
            
            _dialogueView.ShowDialogue(_dialogueText.Act2, () =>
            {
                _popUpAnimator.StartAnimation("Витражная башня");
                _obstacleSpawner.StartSpawning();
                
                StartGame();
            },
            new Dictionary<int, Action> { {2, () =>
            {
                var mainCamera = Camera.main;
                Tween.ShakeLocalPosition(mainCamera.transform, 
                    new Vector3(0.33f, 0.33f, 0f), 
                    0.4f);
            }}});
        }
        
        private void OnAct3()
        {
            if (_act3)
                return;
            
            _act3 = true;
            
            PauseGame();
            SetNewPosForDeathFloor();
            _obstacleSpawner.StopSpawning();
            
            _dialogueView.ShowDialogue(_dialogueText.Act3, () =>
            {
                _popUpAnimator.StartAnimation("Как в тумане");
                _backgroundController.StartFog();
                StartGame();
            });
        }
        
        private void OnFinale()
        {
            if (_finale)
                return;
            
            _finale = true;

            var seq = Sequence.Create();
            seq.ChainCallback(() => _popUpAnimator.StartAnimation("????????????"));
            seq.ChainDelay(3f);
            seq.ChainCallback(PauseGame);
            seq.ChainCallback(() =>
            {
                _dialogueView.ShowDialogue(_dialogueText.Finale, 
                    () =>
                    {
                        _comicsView.ShowComics(
                            () =>
                            {
                                OnComicsComplete?.Invoke();
                            });
                    });
            });
        }

        private void SetNewPosForDeathFloor()
        {
            var curPos = _deathFloor.GetDeathFloorPos();
            curPos.y = _playerController.transform.position.y - _deathFloorStep;
            _deathFloor.SetDeathFloor(curPos);
        }
    }
}