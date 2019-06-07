using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

//link representing object manually for each button
public class BuildMenuButton : MonoBehaviour, IPointerClickHandler {
	//link in editor
	public WorldObject repObjWorldScript;			
	public GameObject ObjToBuildPlacemat;
	//

	private GameObject descPopup;
	private bool building; 

	public void OnPointerClick(PointerEventData eventData){
		if (eventData.button == PointerEventData.InputButton.Left) {
			TaskOnLeftClick();
		}
		if (eventData.button == PointerEventData.InputButton.Right) {
			TaskOnRightClick();
		}
	}

	void TaskOnLeftClick(){ //when this button is left clicked, call build menu 'build' method, passing worldobject script and placemat
		descPopup.SetActive(false);
		GetComponentInParent<BuildMenu>().build(repObjWorldScript, ObjToBuildPlacemat);
	}

	void TaskOnRightClick(){ //when this button is right clicked, cancel cooldown if there is one and refund
		GetComponentInParent<BuildMenu>().stopBuild();
	}

	public void OnMouseEnter(){
		//show build/upgrade info, like desc and price
		descPopup.SetActive (true);

	}
	public void OnMouseExit(){
		descPopup.SetActive (false);
	}

	void Start(){
        descPopup = transform.Find("Desc").gameObject;

        if (repObjWorldScript)
        {
            gameObject.GetComponent<Image>().sprite = repObjWorldScript.buildImage;

            Text descName = descPopup.transform.Find("name").GetComponent<Text>();
            Text descShortcut = descPopup.transform.Find("shortcut").GetComponent<Text>();
            Text descResourcesCost = descPopup.transform.Find("resourcesCost").GetComponent<Text>();
            Text descCommandCost = descPopup.transform.Find("commandCost").GetComponent<Text>();
            Text descText = descPopup.transform.Find("desc").GetComponent<Text>();

            descName.text = repObjWorldScript.objectName;
            descShortcut.text = "Shortcut: " + repObjWorldScript.shortcut;
            descResourcesCost.text = repObjWorldScript.cost.ToString();
            descCommandCost.text = repObjWorldScript.commandCost.ToString();
            descText.text = repObjWorldScript.desc;
        }

        descPopup.SetActive(false);
    }
		

}