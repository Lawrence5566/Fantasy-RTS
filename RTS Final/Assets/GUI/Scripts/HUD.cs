using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
	public static List<GameObject> currentlySelected; 	//the user's currently selected units(only owning units are selected here)
	private static GameObject HUDPortraitName; 			//text on portrait for currently selected object
	private static Image HUDPortraitImage; 				//image on portrait for currently selected object
	public static Text HUDResourcesAmount;
	public static Text HUDCommandAmount;

    public static UnitTrayController unitTray;

	// Use this for initialization
	void Awake () { //awake happens before start
		currentlySelected = new List<GameObject> ();

		HUDResourcesAmount = GameObject.Find("ResAmount").GetComponent<Text>();
		HUDCommandAmount = GameObject.Find("ComAmount").GetComponent<Text>();
		HUDPortraitName = GameObject.Find("PortUnitName"); 
		HUDPortraitImage = GameObject.Find("Portrait").GetComponent<Image>();

	}

    void Start(){
        unitTray = GetComponentInChildren<UnitTrayController>();
    }

    public static void updateSelection(List<WorldObject> currSelect){
		currentlySelected.Clear (); //clear current selection
        unitTray.clearSelectedObjectTray();

		if (currSelect == null || currSelect.Count == 0) { //deselect
			//nothing is selected
			//display nothing selected stuff
			HUDPortraitName.GetComponent<UnityEngine.UI.Text>().text = null;
			HUDPortraitImage.sprite = null;

		} else {
            currentlySelected.Add(currSelect[0].gameObject);
			WorldObject currSelectWorldObject = currentlySelected[0].GetComponent<WorldObject>();

			HUDPortraitName.GetComponent<UnityEngine.UI.Text>().text = currSelectWorldObject.objectName;
			HUDPortraitImage.sprite = currSelectWorldObject.buildImage;
		}

        foreach (WorldObject obj in currSelect) {
            unitTray.addToSelectionTray(obj.gameObject);
        }
        
        unitTray.createTray();

    }

}