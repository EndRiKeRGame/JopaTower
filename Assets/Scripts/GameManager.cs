using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Следит за максимальной достигнутой высотой (счёт) и перезапускает уровень,
/// если игрок упал ниже видимой области камеры.
/// </summary>
public class GameManager : MonoBehaviour
{
    public Transform player;
    public Camera mainCamera;
    public float fallMarginBelowCamera = 8f;

    public static float MaxHeightReached { get; private set; }

    void Update()
    {
        if (player.position.y > MaxHeightReached)
        {
            MaxHeightReached = player.position.y;
        }

        float cameraBottom = mainCamera.transform.position.y - mainCamera.orthographicSize;
        if (player.position.y < cameraBottom - fallMarginBelowCamera)
        {
            RestartGame();
        }
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
