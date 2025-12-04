using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    public Button continueButton;          // assign in Inspector
    public Button deleteSaveButton;        // optional, assign if you make one

    [Header("Scenes")]
    public string firstLevelSceneName = ""; 

    private void Start()
    {
        RefreshContinueButton();
    }

    private void RefreshContinueButton()
    {
        if (continueButton != null)
        {
            bool hasSave = SaveSystem.SaveFileExists();
            continueButton.interactable = hasSave;

            // If you want to hide it completely instead:
            // continueButton.gameObject.SetActive(hasSave);
        }

        if (deleteSaveButton != null)
        {
            bool hasSave = SaveSystem.SaveFileExists();
            deleteSaveButton.interactable = hasSave;
            // or hide it:
            // deleteSaveButton.gameObject.SetActive(hasSave);
        }
    }

    // New Game
    public void PlayGame()
    {
        // Optional: wipe old save when starting totally fresh
        SaveSystem.DeleteSave();
        GameData.score = 0;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.gearsCollected = 0;
        }

        if (!string.IsNullOrEmpty(firstLevelSceneName))
        {
            SceneManager.LoadScene(firstLevelSceneName);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    // Continue Game
    public void ContinueGame()
    {
        if (!SaveSystem.SaveFileExists())
        {
            Debug.LogWarning("MainMenu: No save file found when trying to continue.");
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ContinueGame();
        }
        else
        {
            Debug.LogWarning("MainMenu: No GameManager found. Make sure GameManager is in the Main Menu scene.");
        }
    }

    // 🔹 Delete Save button
    public void DeleteSave()
    {
        if (!SaveSystem.SaveFileExists())
        {
            Debug.Log("MainMenu: No save file to delete.");
            return;
        }

        SaveSystem.DeleteSave();
        GameData.score = 0;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.gearsCollected = 0;
        }

        Debug.Log("MainMenu: Save file deleted.");
        RefreshContinueButton();
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
