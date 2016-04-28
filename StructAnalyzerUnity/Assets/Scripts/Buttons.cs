using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Buttons : MonoBehaviour {
	//Reference to button to access its components
	private Button theButton;
	private ColorBlock theColor;

	// Use this for initialization
	void Awake () {
		theButton = GetComponent<Button>();
		theColor = GetComponent<Button>().colors;

	}
	public void ButtonTransitionColors()
	{
		theColor.highlightedColor = Color.red;
		theColor.normalColor = Color.white;
		theColor.pressedColor = Color.red;
		theButton.colors = theColor;
	}

}