using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Boot : MonoBehaviour
    {
        [SerializeField]
        private ActTowerGenerator _towerGenerator;
        
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
            _cameraFollowUp.ChangeCameraZoomTo(7, 10f);
            
            _deathFloor.Setup(go.transform);
            _deathFloor.StartDeathFloor();
        }
        
        public void GenerateTower()
        {
            _towerGenerator.GenerateFullTower();
        }

        // wait for player adapt
        // dialogue here
        // start death floor
        // end game when player on last platform
    }
}