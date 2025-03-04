using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMove : MonoBehaviour
{
    public int sceneBuildIndex;
    public Vector2 playerPositionInNewScene;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Get or create the GameManager
            if (GameManager.instance == null)
            {
                GameObject gameManagerObj = new GameObject("GameManager");
                gameManagerObj.AddComponent<GameManager>();
            }

            // Set player spawn position in the new scene
            GameManager.instance.SetPlayerSpawnPoint(playerPositionInNewScene);

            // Load the new scene
            SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Single);
        }
    }
}