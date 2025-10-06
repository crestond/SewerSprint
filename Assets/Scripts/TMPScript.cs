using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMPScript : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    void Update()
    {
        scoreText.text = "Score: " + ScoreCard.Instance.getScore();
    }
}
