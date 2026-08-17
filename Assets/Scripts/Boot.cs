using Player;
using UnityEngine;

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
        private Player.Player _player;
        
        [SerializeField]
        private CameraFollowUp _cameraFollowUp;
        
        [SerializeField]
        private Transform _markTransform;
        
        [SerializeField]
        private LineRenderer _lineRenderer;

        [SerializeField]
        private PopUpAnimator _popUpAnimator;

        [SerializeField]
        private int _sectionsPerAct;

        private void Awake()
        {
            _cameraFollowUp.enabled = false;
            _deathFloor.enabled = false;
        }

        public void StartGame()
        {
            _cameraFollowUp.enabled = true;
            _deathFloor.enabled = true;

            GenerateTower();
            
            var go = Instantiate(_player, _spawnPoint.position, Quaternion.identity);
            go.Setup(_markTransform, _lineRenderer);
            
            _cameraFollowUp.SetTarget(go.transform);
            _cameraFollowUp.ChangeCameraZoomTo(7, 600f);
            
            _deathFloor.Setup(go.transform);
            _deathFloor.StartDeathFloor();

            go.GetComponent<ProgressionSystem>().Init(_deathFloor, _popUpAnimator);
        }
        
        public void GenerateTower()
        {
            //_towerGenerator.GenerateFullTower(_backgroundTowerConfig, 28.8f, 2);
            _towerGenerator.GenerateFullTower(_towerConfig, 5f, _sectionsPerAct);
        }

        // wait for player adapt
        // dialogue here
        // end game when player on last platform
    }
}