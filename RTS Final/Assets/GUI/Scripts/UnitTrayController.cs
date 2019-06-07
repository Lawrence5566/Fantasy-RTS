using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTrayController : MonoBehaviour {
	[SerializeField]
	private GameObject trayIconPrefab;
	public static List<GameObject> SelectionTray;

	void Awake(){
		SelectionTray = new List<GameObject> ();
	}

	public void addToSelectionTray(GameObject unitToAdd){
		if (!SelectionTray.Contains (unitToAdd)) { 		//if selection tray doesnt already contain unit
			SelectionTray.Add (unitToAdd);
		}
	}

	public void createTray(){							//creates tray from SelectionTray
		foreach (var trayIcon in GameObject.FindGameObjectsWithTag("trayIconTag") ){ //reset tray
			Destroy (trayIcon);
		}

		if (SelectionTray.Count > 1) {					//selection tray appears only if more than 1 thing is in it
			foreach (var unit in SelectionTray) {
				GameObject trayIcon = Instantiate (trayIconPrefab) as GameObject;
				trayIcon.SetActive (true);

				trayIcon.transform.SetParent (this.gameObject.transform, false); //set parent to be parent its spawning from, and false so button doesnt position in world space

				UnitTrayButton thisUnitTrayButton = trayIcon.GetComponent<UnitTrayButton> ();
				thisUnitTrayButton.setTrayParent(this); //pass in this script to the button
				thisUnitTrayButton.setParameters(unit.GetComponent<WorldObject>());
			}
		}
	}

	public void clearSelectedObjectTray(){
		SelectionTray.Clear ();
		foreach (var trayIcon in GameObject.FindGameObjectsWithTag("trayIconTag") ){
			Destroy (trayIcon);
		}

	}
}
