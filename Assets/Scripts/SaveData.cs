using UnityEngine;

[System.Serializable]
public class SaveData
{
    // Scene / level info
    public string sceneName;
    public int levelIndex;

    // Player data
    public Vector3 playerPosition;
    public int score;
    public int gearsCollected;
    public int playerHealth;

    // Add more later if needed:
    // public int coins;
    // public string currentWeapon;
}
