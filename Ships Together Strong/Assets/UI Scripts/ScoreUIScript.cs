using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreUIScript : MonoBehaviour
{
    public Scorekeeper scoreKeeper;
    public TMP_Text scoreText;

    // Update is called once per frame
    void Update()
    {
        scoreText.SetText($"HIGH SCORE: {scoreKeeper.highScore} pts\nSCORE: {scoreKeeper.totalScore} pts");
    }
}
