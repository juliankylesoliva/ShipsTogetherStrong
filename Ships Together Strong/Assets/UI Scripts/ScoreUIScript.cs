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
        if (scoreKeeper.increasedBy > 0)
        {
            scoreText.SetText($"{scoreKeeper.totalScore}\n(+{scoreKeeper.increasedBy})");
        }
        else
        {
            scoreText.SetText($"{scoreKeeper.totalScore}\n({scoreKeeper.increasedBy})");
        }
    }
}
