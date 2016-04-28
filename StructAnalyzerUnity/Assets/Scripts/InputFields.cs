using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputFields : MonoBehaviour {

    public InputField magnitudeInput, angleInput;
    public static float angle, magnitude;

	// Use this for initialization
	void Start () {
        magnitudeInput = transform.GetChild(0).GetComponent<InputField>();
        angleInput = transform.GetChild(1).GetComponent<InputField>();
        angle = 0;
        magnitude = 0;
    }
	
	// Update is called once per frame
	void Update () {

        magnitudeInput.enabled = Main.selectedForce != null;

        
    }

    public void changeMagnitude()
    {
        if(Main.selectedForce != null)
        {
            magnitude = float.Parse(magnitudeInput.text);
            Main.selectedForce.magnitude = magnitude;
        }
    }

    public void changeAngle()
    {
        if (Main.selectedForce != null)
        {
            angle = float.Parse(angleInput.text);
            Main.selectedForce.angle = angle;
        }
    }

}
