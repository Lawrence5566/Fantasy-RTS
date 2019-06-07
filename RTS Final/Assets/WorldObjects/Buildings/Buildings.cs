using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

[RequireComponent(typeof(WorldObject))]
public class Buildings : MonoBehaviour {
	private Commander owningPlayer;

	public GameObject unitFlagSpawn; //spawn location
	public GameObject unitFlagStart; //the exit to the building
	public GameObject unitFlag;		 //user flag location
	public List<WorldObject> objectsCanSpawn; //link stuff in unity that the building can spawn
	public Animator animator;
	public GameObject ConstructEffect;
	private BoxCollider col;

	[SerializeField]
	private GameObject BuildingMenu;
	public float buildTime;
	public bool isBuilding;
	public GameObject BuildTimer;
	public Image TimerCountdownImage; 

	private CharacterController unitToMove;
	private bool movingUnit;

	public void openMenu (){
		//instead of SetActive, we turn off renderers so that cooldowns can carry on when menu is not visible
		if (!isBuilding && BuildingMenu) { //you can't open the build menu if the building is still being built!
			BuildingMenu.GetComponent<Canvas> ().enabled = true;
		}
	}
	public void closeMenu (){
		//instead of SetActive, we turn off renderers so that cooldowns can carry on when menu is not visible
		if(BuildingMenu){
			BuildingMenu.GetComponent<Canvas>().enabled = false;
		}
	}

	public void Spawn(WorldObject objToSpawn){ //spawns unit or starts upgrade
		//pass in name of world object so commander knows what to build
		owningPlayer.SpawnUnit(objToSpawn.objectName, unitFlagSpawn.transform.position, unitFlagStart.transform.position , unitFlag.transform.position, objToSpawn.cost);
	}

	public void startBuilding(){
		//instantiate timer, and start it
		isBuilding = true;
		animator.SetBool ("IsBeingBuilt", true);
		animator.SetFloat ("BuildSpeed", 1/buildTime); //all animations happen in 1 second, so make them happen in the build time 
		ConstructEffect.SetActive (true);
	}

	void Awake(){
		col = GetComponent<BoxCollider> ();
		Bounds bounds = col.bounds;
		AstarPath.active.UpdateGraphs(bounds); //update A* graph
	}

	void Start () {
		owningPlayer = GetComponentInParent<Commander> ();

		ConstructEffect = Instantiate (ConstructEffect, this.transform);
		startBuilding (); //when created, building starts constructing first (so that it sinks to the ground first)

		if (BuildingMenu) { //if building has a menu
			BuildingMenu = Instantiate (BuildingMenu) as GameObject;

			BuildingMenu.GetComponent<Canvas>().enabled = false; //is hidden initialy
			BuildingMenu.transform.SetParent (this.gameObject.transform, false);
			Vector3 menuPos = new Vector3 (0f, 11f, 0f);
			BuildingMenu.transform.SetPositionAndRotation (gameObject.transform.position + menuPos, gameObject.transform.rotation);
			BuildingMenuController thisBuildingMenuController = BuildingMenu.GetComponentInChildren<BuildingMenuController> ();

			thisBuildingMenuController.createMenu(objectsCanSpawn);
		}
			

	}

	// Update is called once per frame
	void Update () {
		if (isBuilding) {
			TimerCountdownImage.fillAmount += 1 / buildTime * Time.deltaTime; //fill timer
			if (TimerCountdownImage.fillAmount >= 0.920 && TimerCountdownImage.fillAmount <= 0.925 ){ //building almost done, so start stopping construction effect
				foreach (ParticleSystem sys in GetComponentsInChildren<ParticleSystem>()){
					var main = sys.main;
					main.loop = false;
				}
			}

			if (TimerCountdownImage.fillAmount == 1) { //building finished
				isBuilding = false;
				BuildTimer.SetActive (false);
				animator.SetBool ("IsBeingBuilt", false);
				TimerCountdownImage.fillAmount = 0;
				ConstructEffect.SetActive (false);

				col.size = col.size - new Vector3 (1f, 0f, 1f); //shrink the collider so units don't get stuck on it, with radius pathing this could be obsolte

				MeshCombiner meshComb = GetComponentInChildren<MeshCombiner> ();
				//meshComb.changeColour ();										//use mesh combiner to change colour (its on parts wrapper)
				meshComb.CombineMeshes ();
			}
		}
		
	}


}
