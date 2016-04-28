using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BalancedIndicator : MonoBehaviour {

	public Image UIImage;
	Sprite balance;
	Sprite noBalance;
	public static bool balanced; 

	public void Start(){
		balance=Resources.Load<Sprite> ("Sprites/static");
		noBalance=Resources.Load<Sprite> ("Sprites/noStatic");
		UIImage = GameObject.Find ("BalancedIndicator").GetComponent<Image> ();
		balanced = false;
	}
	public void Update(){
	
		if (balanced) {
			UIImage.sprite = balance;
			UIImage.color = new Color (10, 50, 20, 1);

		}
		else{
			UIImage.sprite = noBalance;
			UIImage.color = new Color (10, 50, 20, 1);

		}
	}
}