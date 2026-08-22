using Player;
using UnityEngine;

public class PlayerCreator : MonoBehaviour
{
    [SerializeField]
    private PlayerController _playerPrefab;
        
    [SerializeField]
    private Transform _spawnPoint;
        
    [SerializeField]
    private Transform _markTransform;
        
    [SerializeField]
    private LineRenderer _lineRenderer;
        
        
    public PlayerController CreatePlayer()
    {
        var player = Instantiate(_playerPrefab, _spawnPoint.position, Quaternion.identity);
        player.Setup(_markTransform, _lineRenderer);

        return player;
    }
}