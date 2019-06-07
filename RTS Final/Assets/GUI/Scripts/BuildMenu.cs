using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class BuildMenu : MonoBehaviour {
	Commander owningPlayer;
	public GameObject buildMenu; //need to link buildMenu Object
	private Camera playerCam;

	private GameObject placematInst;
	private string ObjToBuildName;
	private WorldObject ObjToBuildWorldScript;

	private bool building;

	void Start(){
		owningPlayer = GetComponentInParent<Commander>();
		buildMenu.SetActive (false);
		playerCam = owningPlayer.GetComponentInChildren<Camera> ();
	}

	public void closeMenu(){
		buildMenu.SetActive (false);
	}

	public void onMouseEnter(){
		buildMenu.SetActive (true);
	}

	public void onMouseExit(){
		closeMenu ();
	}

	//building a building stuff
	public void build(WorldObject objToBuild, GameObject ObjToBuildPlacemat){
        if (ObjToBuildPlacemat && objToBuild) {
            ObjToBuildName = objToBuild.objectName;
            ObjToBuildWorldScript = objToBuild;
            placematInst = Instantiate(ObjToBuildPlacemat as GameObject); //create placemat
            building = true;
        }
	}
	public void stopBuild(){ //when this button is right clicked, cancel cooldown if there is one and refund
		building = false;
		Destroy (placematInst);  //destroy placemat
	}

	void Update(){
		if (building) {
			//using rays might Lag the game? try to reduce them?
			Ray toMouse = playerCam.ScreenPointToRay(Input.mousePosition); //uses main camera
			RaycastHit rayInfo;
			bool didHit = Physics.Raycast (toMouse, out rayInfo, 500.0f); //ray info going into rayinfo

			if (didHit) { //if hit something
				placematInst.transform.position = rayInfo.point;	//lock placematInst to where cursor is hitting the ground
				if (Input.GetMouseButtonDown (0) && !EventSystem.current.IsPointerOverGameObject()){ //if left click and not over HUD
					if (owningPlayer.canAfford (ObjToBuildWorldScript)) {
						//spawn building
						owningPlayer.Build(ObjToBuildName,  rayInfo.point, placematInst.transform.rotation, ObjToBuildWorldScript.cost);
						stopBuild (); //stop building and remove placemat
					} else {
						Debug.Log ("can't afford!!");
						//display "can't afford" error sound and message
					}


				}
			}

		}
	}
		
}
