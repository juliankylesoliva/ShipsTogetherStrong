using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FormationName { Wing, Avian, Snake, Defense }

public class FormationSlotScript : MonoBehaviour
{
    [HideInInspector] public FormationName currentFormation = FormationName.Wing;

    // Fill out the position and rotation values in the editor
    public float[] xPositions;
    public float[] yPositions;
    public float[] zRotations;

    // Switching cooldown
    public float delayTime = 1.0f;
    private bool isDelayed = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        changeCurrentFormation();
        goToFormationPosition();
    }

    // Sets formation mode based on what key was pressed
    void changeCurrentFormation()
    {
        if (isDelayed) { return; }

        if (Input.GetKey(KeyCode.W) && currentFormation != FormationName.Wing)
        {
            currentFormation = FormationName.Wing;
            StartCoroutine(DoDelay());
        }
        else if (Input.GetKey(KeyCode.A) && currentFormation != FormationName.Avian)
        {
            currentFormation = FormationName.Avian;
            StartCoroutine(DoDelay());
        }
        else if (Input.GetKey(KeyCode.S) && currentFormation != FormationName.Snake)
        {
            currentFormation = FormationName.Snake;
            StartCoroutine(DoDelay());
        }
        else if (Input.GetKey(KeyCode.D) && currentFormation != FormationName.Defense)
        {
            currentFormation = FormationName.Defense;
            StartCoroutine(DoDelay());
        }
        else { }
    }

    // Helper function for setting the delay time
    IEnumerator DoDelay()
    {
        isDelayed = true;
        yield return new WaitForSeconds(delayTime);
        isDelayed = false;
    }

    // Sets the position relative to the main ship depending on formation mode
    void goToFormationPosition()
    {

        switch (currentFormation)
        {
            case FormationName.Wing:
                this.transform.localPosition = new Vector3(xPositions[0], yPositions[0], 0.0f);
                this.transform.localRotation = Quaternion.AngleAxis(zRotations[0], Vector3.forward);
                break;
            case FormationName.Avian:
                this.transform.localPosition = new Vector3(xPositions[1], yPositions[1], 0.0f);
                this.transform.localRotation = Quaternion.AngleAxis(zRotations[1], Vector3.forward);
                break;
            case FormationName.Snake:
                this.transform.localPosition = new Vector3(xPositions[2], yPositions[2], 0.0f);
                this.transform.localRotation = Quaternion.AngleAxis(zRotations[2], Vector3.forward);
                break;
            case FormationName.Defense:
                this.transform.localPosition = new Vector3(xPositions[3], yPositions[3], 0.0f);
                this.transform.localRotation = Quaternion.AngleAxis(zRotations[3], Vector3.forward);
                break;
            default:
                break;
        }
    }
}
