using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitTrayButton : MonoBehaviour {
	UnitTrayController parentTray;
    CommanderInput commanderInput;

	private Slider healthSlider;
	private WorldObject representingObject;

	void Start(){
		gameObject.GetComponent<Button>().onClick.AddListener(TaskOnClick);
        commanderInput = GetComponentInParent<CommanderInput>();
    }

	void Update(){
		healthSlider.value = representingObject.hitPoints;
	}

	public void setTrayParent(UnitTrayController thisParent){
		parentTray = thisParent;
	}

	public void setParameters(WorldObject RepresentingObject){				//when this object is created this is called from other script
		representingObject = RepresentingObject;
		gameObject.GetComponent<Image>().sprite = representingObject.buildImage;
		healthSlider = this.gameObject.GetComponentInChildren <Slider>(); 	//get slider from children
		healthSlider.maxValue = representingObject.maxHitPoints;
    }

	public void TaskOnClick(){
		parentTray.clearSelectedObjectTray();   //clear selection tray
        BattalionSelectionComponent unitSelectObj = FindObjectOfType<BattalionSelectionComponent>();
		unitSelectObj.deselectAll (); //deselect all units
		unitSelectObj.createCircle(representingObject.GetComponent<SelectableObject>()); //add circle to unit
        commanderInput.updateSelectedObjects(representingObject);

	}

}
