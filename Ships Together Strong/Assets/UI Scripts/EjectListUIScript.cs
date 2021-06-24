using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EjectListUIScript : MonoBehaviour
{
    /* DRAG AND DROP */
    public MainShipMovement playerShip;
    public Animator ejectAnim;
    public RawImage ejectTray;
    public Texture[] trayStates;
    public RawImage[] ejectSlots;
    public Texture[] allyIcons;

    /* PRIVATE VARIABLES */
    private bool lastEjectState = false;

    void Update()
    {
        // Sprite change
        if (playerShip.getIsEjectModeOn())
        {
            ejectTray.texture = trayStates[1];
        }
        else
        {
            ejectTray.texture = trayStates[0];
        }

        // Animation
        if (playerShip.getIsEjectModeOn() && !lastEjectState) // Open
        {
            ejectAnim.Play("EjectTrayOpen");
        }
        else if (!playerShip.getIsEjectModeOn() && lastEjectState) // Close
        {
            ejectAnim.Play("EjectTrayClose");
        }
        else { }

        // Icon change
        SetSlotIcon(ejectSlots[0], playerShip.getEjectionList(0));
        SetSlotIcon(ejectSlots[1], playerShip.getEjectionList(1));
        SetSlotIcon(ejectSlots[2], playerShip.getEjectionList(2));
        SetSlotIcon(ejectSlots[3], playerShip.getEjectionList(3));

        // Set previous eject mode state
        lastEjectState = playerShip.getIsEjectModeOn();
    }

    void SetSlotIcon(RawImage slot, AllyType type)
    {
        slot.color = Color.white;
        switch (type)
        {
            case AllyType.Speed:
                slot.texture = allyIcons[0];
                break;
            case AllyType.Rapid:
                slot.texture = allyIcons[1];
                break;
            case AllyType.Magnify:
                slot.texture = allyIcons[2];
                break;
            case AllyType.Score:
                slot.texture = allyIcons[3];
                break;
            case AllyType.Shield:
                slot.texture = allyIcons[4];
                break;
            case AllyType.Reflector:
                slot.texture = allyIcons[5];
                break;
            case AllyType.Copycat:
                slot.texture = allyIcons[6];
                break;
            case AllyType.Bomb:
                slot.texture = allyIcons[7];
                break;
            case AllyType.Parasite:
                slot.texture = allyIcons[8];
                break;
            case AllyType.Life:
                slot.texture = allyIcons[9];
                break;
            default:
                slot.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                break;
        }
    }
}
