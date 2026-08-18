using DefaultNamespace.Ui;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    public class Boot : MonoBehaviour
    {
        [SerializeField]
        private ActTowerGenerator _towerGenerator;
        
        [SerializeField]
        private TowerConfig _towerConfig;
        
        [SerializeField]
        private TowerConfig _backgroundTowerConfig;
        
        [SerializeField]
        private DeathFloor _deathFloor;
        
        [SerializeField]
        private Transform _spawnPoint;
        
        [SerializeField]
        private Player.Player _playerPrefab;
        
        [SerializeField]
        private CameraFollowUp _cameraFollowUp;
        
        [SerializeField]
        private Transform _markTransform;
        
        [SerializeField]
        private LineRenderer _lineRenderer;

        [SerializeField]
        private PopUpAnimator _popUpAnimator;
        
        [SerializeField]
        private MainMenuView _mainMenuView;
        
        [SerializeField]
        private HealthSystem _healthSystem;
        
        [SerializeField]
        private ProgressionSystem _progressionSystem;

        [SerializeField]
        private int _sectionsPerAct;
        
        private Player.Player _curPlayer;

        private void Awake()
        {
            _cameraFollowUp.enabled = false;
            _deathFloor.enabled = false;
            
            _mainMenuView.Init(_healthSystem);
            
            _mainMenuView.OnStartButtonPressed += StartGame;
            _mainMenuView.OnRestartButtonPressed += RestartGame;
            _healthSystem.OnDeath += _progressionSystem.Restart;
            _healthSystem.OnDeath += StopGame;
            _progressionSystem.Init(_deathFloor, _popUpAnimator);
        }

        public void StartGame()
        {
            _cameraFollowUp.enabled = true;
            _deathFloor.enabled = true;

            GenerateTower();
            _curPlayer = Instantiate(_playerPrefab, _spawnPoint.position, Quaternion.identity);
            _curPlayer.Setup(_markTransform, _lineRenderer);
            
            _cameraFollowUp.SetTarget(_curPlayer.transform);
            _cameraFollowUp.ChangeCameraZoomTo(7, 600f);
            
            _deathFloor.Setup(_curPlayer.transform);
            _deathFloor.StopDeathFloor();
            
            _progressionSystem.UpdateBodyProgression(_curPlayer.Progression);
            _progressionSystem.Restart();
            _healthSystem.UpdateBodyHealth(_curPlayer.Health);
            _healthSystem.Restart();
        }

        public void RestartGame()
        {
            _towerGenerator.DestroyTower();
            Destroy(_curPlayer.gameObject);
            
            _deathFloor.SetDeathFloor(new Vector3(0, -100f, 0));
            
            StartGame();
        }

        public void StopGame()
        {
            _cameraFollowUp.enabled = false;
            _deathFloor.enabled = false;
            _curPlayer.GetComponent<PlayerMovementSystem>().IsMoveable = false;
        }
        
        public void GenerateTower()
        {
            _towerGenerator.GenerateFullTower(_towerConfig, 5f, _sectionsPerAct);
        }
        
        // dialogue here
    }
}