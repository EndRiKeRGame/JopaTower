using Configs;
using DefaultNamespace;
using GlobalSystems;
using Player;
using PrimeTween;
using Ui;
using UnityEngine;

public class Boot : MonoBehaviour
{
    [SerializeField] private UiController _uiController;
    [SerializeField] private PlayerCreator _playerCreator;
        
    [SerializeField]
    private ActTowerGenerator _towerGenerator;
        
    [SerializeField]
    private TowerConfig _towerConfig;
        
    [SerializeField]
    private TowerConfig _backgroundTowerConfig;
        
    [SerializeField]
    private DeathFloor _deathFloor;
        
    [SerializeField]
    private CameraFollowUp _cameraFollowUp;
        
    [SerializeField]
    private HealthSystem _healthSystem;
        
    [SerializeField]
    private ProgressionSystem _progressionSystem;
        
    [SerializeField]
    private ObstacleSpawner _obstacleSpawner;
    
    [SerializeField]
    private BackgroundController _backgroundController;

    [SerializeField]
    private int _sectionsPerAct;
        
    private PlayerController _curPlayer;
    private float[] _triggersPositions = new float[5];

    private void Awake()
    {
        _uiController.OnStartButtonPressed += StartGame;
        _uiController.OnPauseButtonPressed += PauseGame;
        _uiController.OnContinueButtonPressed += ResumeGame;
        _uiController.OnMainMenuButtonPressed += ResetGame;
        _uiController.OnRestartButtonPressed += ResetGame;
        _uiController.OnRestartButtonPressed += StartGame;
        
        _healthSystem.OnDeath += () =>
        {
            var seq = Sequence.Create();
            seq.ChainCallback(_curPlayer.StopPlayer);
            seq.ChainDelay(3f);
            seq.ChainCallback(PauseGame);
            seq.ChainCallback(_uiController.ShowDeathScreen);
            // TODO: звук смерти
            seq.ChainCallback(_progressionSystem.Reset);
        };
            
        _progressionSystem.OnComicsComplete += ResetGame;
        _progressionSystem.OnComicsComplete += _uiController.ShowFinaleScreen;
        
        _progressionSystem.Init(PauseGame, ResumeGame);
        
        // высота 1 этажа = 5
        var sectionHeight = 5f;
        // позиция триггера = 1.9
        var triggerOffset = 1.9f;
        // количество уровней пролога (3)
        var prologueLevels = _towerConfig.Prologue.poolA.Length;
        // высота башни для акта
        var actTowerHeight = sectionHeight * _sectionsPerAct;
        
        _triggersPositions[0] = triggerOffset;
        _triggersPositions[1] = triggerOffset + prologueLevels * sectionHeight;
        _triggersPositions[2] = _triggersPositions[1] + actTowerHeight;
        _triggersPositions[3] = _triggersPositions[2] + actTowerHeight;
        _triggersPositions[4] = _triggersPositions[3] + actTowerHeight;
        
        _backgroundController.Init(_triggersPositions[3], _triggersPositions[4]);
        _obstacleSpawner.Init(_triggersPositions[3] - triggerOffset, _triggersPositions[2] - triggerOffset);
    }

    public void StartGame()
    {
        GenerateTower();
        _curPlayer = _playerCreator.CreatePlayer();
            
        _cameraFollowUp.SetTarget(_curPlayer.transform);
        _cameraFollowUp.ChangeCameraZoomTo(7f, 600f);
        _cameraFollowUp.SetPosition(Vector3.up);
        _cameraFollowUp.StartWork();
            
        _deathFloor.SetTarget(_curPlayer.transform);
        _deathFloor.StopWork();
            
        _progressionSystem.UpdatePlayerController(_curPlayer);
        _progressionSystem.Reset();
            
        _healthSystem.UpdateBodyHealth(_curPlayer.Health);
        _healthSystem.Restart();
        
        _backgroundController.UpdatePlayer(_curPlayer);
    }

    public void ResetGame()
    {
        DestroyTower();
        Destroy(_curPlayer.gameObject);
            
        _cameraFollowUp.StopWork();
        _cameraFollowUp.ChangeCameraZoomToInstantly(4f);
            
        _deathFloor.StopWork();
        _deathFloor.SetDeathFloor(new Vector3(0, -100f, 0));
            
        _obstacleSpawner.StopSpawning();
            
        _progressionSystem.Reset();
        _healthSystem.Restart();
        _backgroundController.StopFog();
    }
        
    public void ResumeGame()
    {
        _cameraFollowUp.StartWork();
        _deathFloor.StartWork();
        _curPlayer.StartPlayer();
    }

    public void PauseGame()
    {
        _cameraFollowUp.StopWork();
        _deathFloor.StopWork();
        _curPlayer.StopPlayer();
    }
        
    public void GenerateTower()
    {
        _towerGenerator.GenerateFullTower(_towerConfig, 5f, _sectionsPerAct);
    }
        
    public void DestroyTower()
    {
        _towerGenerator.DestroyTower();
    }
}