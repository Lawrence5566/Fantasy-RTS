using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	public MultiPlayer multiplayer;
	public int team;
	public string playerColor;
	public string username;
	public int Resources;
	public int CommandAmount;
	public int CommandMax;


	// Use this for initialization
	void Start () {
		//ownedHUD = FindObjectOfType<HUD>;

		updateResourcesAmount (0); //sets as current values from player ^^
		updateCommandAmount(0, 0);

		//AstarPath.active.Scan (); //scan map on startup (we will change this to loading depending on level later)
	}

	public GameObject DebugTextPrefab;
	public GameObject DebugConsole;

	public void debugWrite(string t){
		GameObject newtext = Instantiate (DebugTextPrefab, DebugConsole.transform);
		newtext.GetComponent<Text> ().text = t;
	}

	public void Build(string name, Vector3 location, Quaternion rotation){
		//if multiplayer:
		multiplayer.Build(name, location, rotation);

		//if singleplayer (for testing):
		/*if (name == "Barracks") {
			GameObject obj = Instantiate (multiplayer.BarracksPrefab, location, rotation, transform);

		} else if (name == "Farm") {
			GameObject obj = Instantiate (multiplayer.FarmPrefab, location, rotation, transform);
		} else if (name == "Knight") {
			GameObject obj = Instantiate (multiplayer.KnightPrefab, location, rotation, transform);
		}*/
	}

	public bool canAfford(WorldObject objToBuy){ //need a multiplayer version
		if (objToBuy.cost <= Resources) {
			//can buy, try command points
			if (objToBuy.commandCost + CommandAmount <= CommandMax){
				return true;	//can buy!
			}
			else {//can't buy, give not ennough command points message
				return false;
			}
		} else {
			return false; //can't buy, send can't afford message
		}
	}

	public void updateResourcesAmount(int amount){
		Resources += amount;
		HUD.HUDResourcesAmount.text = Resources.ToString();
	}
	public void updateCommandAmount(int amount, int max){ //pass in 0 to either if you don't want to update it
		CommandAmount += amount;
		CommandMax += max;

		HUD.HUDCommandAmount.text = CommandAmount.ToString () + "/" + CommandMax.ToString ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
