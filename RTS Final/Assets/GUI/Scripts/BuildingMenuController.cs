using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMenuController : MonoBehaviour {

	[SerializeField]
	private GameObject buildingButtonPrefab;

	public void createMenu(List<WorldObject> objectsInMenu){
		foreach (WorldObject objectInMenu in objectsInMenu ){ 
			GameObject menuItem = Instantiate (buildingButtonPrefab) as GameObject;
			menuItem.SetActive (true);
			menuItem.transform.SetParent (this.gameObject.transform, false); //set parent to be parent its spawning from, and false so button doesnt position in world space

			menuItem.GetComponentInChildren<BuildingMenuButton>().setParameters(objectInMenu);
		}
	}
		
}
