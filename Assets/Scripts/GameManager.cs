using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public int currentLevelIndex = 1;
    public int gearsCollected;

    [Header("Player Reference")]
    public PlayerControl player;   // uses your PlayerControl script

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentLevelIndex = scene.buildIndex;

        if (player == null)
        {
            player = FindObjectOfType<PlayerControl>();
        }
    }

    // Called by the NPC when you choose "Rest / Save"
    public void SaveGame()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerControl>();
        }

        if (player == null)
        {
            Debug.LogWarning("GameManager: No PlayerControl found – cannot save.");
            return;
        }

        int heartsRemaining = 0;
        if (player.hearts != null)
        {
            heartsRemaining = player.hearts.Count;
        }

        SaveData data = new SaveData
        {
            sceneName      = SceneManager.GetActiveScene().name,
            levelIndex     = currentLevelIndex,
            playerPosition = player.transform.position,

            score          = GameData.score,
            gearsCollected = gearsCollected,
            playerHealth   = heartsRemaining
        };

        SaveSystem.SaveGame(data);
        Debug.Log("GameManager: SaveGame called by NPC.");
    }

    // Called from the Main Menu "Continue" button
    public void ContinueGame()
    {
        SaveData data = SaveSystem.LoadGame();
        if (data == null)
        {
            Debug.LogWarning("GameManager: No save file to continue from.");
            return;
        }

        StartCoroutine(LoadFromSave(data));
    }

    private IEnumerator LoadFromSave(SaveData data)
    {
        // 1. Load the saved scene
        AsyncOperation op = SceneManager.LoadSceneAsync(data.sceneName);
        while (!op.isDone)
            yield return null;

        // 2. Re-hook the player in the loaded scene
        player = FindObjectOfType<PlayerControl>();
        if (player == null)
        {
            Debug.LogWarning("GameManager: No PlayerControl found after loading scene.");
            yield break;
        }

        // 3. Restore position
        player.transform.position = data.playerPosition;

        // 4. Restore score
        GameData.score = data.score;
        player.UpdateScoreUI();

        // 5. Restore gears
        gearsCollected = data.gearsCollected;

        // 6. Restore hearts based on saved playerHealth
        ApplyHeartsFromSave(player, data.playerHealth);

        Debug.Log("GameManager: Loaded game from save.");
    }

    private void ApplyHeartsFromSave(PlayerControl player, int heartsToHave)
    {
        if (player.hearts == null) return;

        // Clamp to the current max hearts in that scene
        heartsToHave = Mathf.Clamp(heartsToHave, 0, player.hearts.Count);

        // Remove extra hearts from the end, so Count matches lives
        for (int i = player.hearts.Count - 1; i >= heartsToHave; i--)
        {
            if (player.hearts[i] != null)
                player.hearts[i].SetActive(false);

            player.hearts.RemoveAt(i);
        }

        // Ensure remaining hearts are enabled
        for (int i = 0; i < player.hearts.Count; i++)
        {
            if (player.hearts[i] != null)
                player.hearts[i].SetActive(true);
        }
    }
}
