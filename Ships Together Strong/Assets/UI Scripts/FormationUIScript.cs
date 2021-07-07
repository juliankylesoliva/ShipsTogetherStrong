using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FormationUIScript : MonoBehaviour
{
    public FormationSlotScript formSlot;
    public Texture[] formSprites;
    public RawImage formImage;
    public Animator formAnim;
    public AudioSource soundPlayer;

    private FormationName prevForm = FormationName.Wing;

    // Update is called once per frame
    void Update()
    {
        //formText.SetText($"Form (WASD): {formSlot.currentFormation.ToString()}");
        switch (formSlot.currentFormation)
        {
            case FormationName.Wing:
                formImage.texture = formSprites[0];
                break;
            case FormationName.Avian:
                formImage.texture = formSprites[1];
                break;
            case FormationName.Snake:
                formImage.texture = formSprites[2];
                break;
            case FormationName.Defense:
                formImage.texture = formSprites[3];
                break;
            default:
                break;
        }

        AnimateOnChange();

        prevForm = formSlot.currentFormation;
    }

    void AnimateOnChange()
    {
        if (prevForm != formSlot.currentFormation)
        {
            formAnim.Play("UI_Form_Anim");
            soundPlayer.Play();
        }
    }
}
