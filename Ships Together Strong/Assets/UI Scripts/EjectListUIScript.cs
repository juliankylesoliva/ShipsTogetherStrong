using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EjectListUIScript : MonoBehaviour
{
    public MainShipMovement playerShip;
    public TMP_Text ejectText;

    // Update is called once per frame
    void Update()
    {
        if (playerShip.getIsEjectModeOn())
        {
            ejectText.SetText($"Press a number to eject (Q - Quit)\n1. {playerShip.ejectionList[0].ToString()}\n2. {playerShip.ejectionList[1].ToString()}\n3. {playerShip.ejectionList[2].ToString()}\n4. {playerShip.ejectionList[3].ToString()}");
        }
        else
        {
            ejectText.SetText($"Current Allies (E - Eject Mode)\n1. {playerShip.ejectionList[0].ToString()}\n2. {playerShip.ejectionList[1].ToString()}\n3. {playerShip.ejectionList[2].ToString()}\n4. {playerShip.ejectionList[3].ToString()}");
        }
    }
}
