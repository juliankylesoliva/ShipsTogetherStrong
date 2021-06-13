using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FormationUIScript : MonoBehaviour
{
    public FormationSlotScript formSlot;
    public TMP_Text formText;

    // Update is called once per frame
    void Update()
    {
        formText.SetText($"Form (WASD): {formSlot.currentFormation.ToString()}");
    }
}
