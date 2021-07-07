using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LivesUIScript : MonoBehaviour
{
    public MainShipMovement playerShip;
    public TMP_Text livesText;

    // Update is called once per frame
    void Update()
    {
        livesText.SetText($"{playerShip.getLivesLeft()}");
    }
}
