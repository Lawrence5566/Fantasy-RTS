using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CommanderInput : MonoBehaviour {

	public Camera playerCamera;
	public Commander thisPlayer;

    public List<WorldObject> selectedObjects;

    void Start()
    {
        thisPlayer = GetComponent<Commander>();
		playerCamera = GetComponentInChildren<Camera> ();
    }

    public void updateSelectedObjects(List<SelectableObject> selectedObjs){
        selectedObjects.Clear();
        if (selectedObjs != null) {
            foreach (SelectableObject i in selectedObjs) {
                Battalion battalionToAdd = i.GetComponentInParent<Battalion>();
                WorldObject objToAdd = i.GetComponent<WorldObject>();
				Buildings buildingToAdd = i.GetComponent<Buildings> ();

				if (battalionToAdd) { //add commander, not unit
					objToAdd = battalionToAdd.GetComponent<WorldObject> ();
				} else if (buildingToAdd) { //open building menu if it has one
					buildingToAdd.openMenu();
				}

                if (!selectedObjects.Contains(objToAdd)){
                    selectedObjects.Add(objToAdd);
                }
            }
        }
        //update HUD here with the selected objects
        HUD.updateSelection(selectedObjects);
    }

    public void updateSelectedObjects(WorldObject selectedObj)
    { //overload for single worldobject
        selectedObjects.Clear();

		if (selectedObj != null) {
			selectedObjects.Add (selectedObj);
			Buildings buildingToAdd = selectedObj.GetComponent<Buildings> ();
			if (buildingToAdd) {
				buildingToAdd.openMenu ();
			}
		}

        //update HUD here with the selected objects
        HUD.updateSelection(selectedObjects);
    }

    void Update () {
		
		/// Left clicks ///

		if (Input.GetMouseButtonDown (0) && !EventSystem.current.IsPointerOverGameObject()) {
			Ray toMouse = playerCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit rayInfo;
			bool didHit = Physics.Raycast (toMouse, out rayInfo, 500.0f); //ray info going into rayinfo

			if (didHit) { //if hit something
				if (rayInfo.transform.gameObject.layer == LayerMask.NameToLayer("Terrain")) { //its terrain (uses terrain layer name)

				}

			}

		}

		/// right clicks ///

		if (Input.GetMouseButtonDown (1) && !EventSystem.current.IsPointerOverGameObject()) {
			Ray toMouse =  playerCamera.ScreenPointToRay(Input.mousePosition); 
			RaycastHit rayInfo;
			bool didHit = Physics.Raycast (toMouse, out rayInfo, 500.0f); //ray info going into rayinfo
			//could use movement mask here to allow multiple places to be clicked to move on, not just terrain?

			WorldObject worldObjectHit = rayInfo.transform.GetComponent<WorldObject> ();

			if (didHit) { //if hit something

                List<Battalion> Battalions = new List<Battalion>();

                foreach (WorldObject obj in selectedObjects) { //get list of all battalions selected
                    Battalions.Add(obj.GetComponent<Battalion>());
                    //you could also add any wizards, or any other single units here
                }

                if (rayInfo.transform.gameObject.layer == LayerMask.NameToLayer ("Terrain")) { //its hit terrain (uses terrain layer name)
					foreach (Battalion B in Battalions) {		//foreach selected battalion
						B.moveBattalion(rayInfo.point);//move to that location
					}
				
				}else if (worldObjectHit && rayInfo.transform.GetComponentInParent<Commander>().team != thisPlayer.team){ //rightclicked something thats a worldObject and on a different team
					//foreach unit
					//attack rayinfo.transform.gameobject
						
				}

			}

		}



	}

}
