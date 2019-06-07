using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class BuildingMenuButton : MonoBehaviour, IPointerClickHandler {
	Commander owningPlayer;
	private WorldObject representingWorldObject;
	private GameObject descPopup;
	private Image timerImage;
	private float cooldown;
	private float time;
	private bool onCooldown;

	private int spawnQueue;
	private Text spawnQueueText;

	public void setParameters(WorldObject RepresentingObject){		//when this object is created this is called from other script
		representingWorldObject = RepresentingObject;
		descPopup = gameObject.transform.Find ("Desc").gameObject;
		descPopup.SetActive (false);

		if (representingWorldObject && descPopup) { //if we actually got an object and have a descPopup
			cooldown = representingWorldObject.spawnTime;

			gameObject.GetComponent<Image>().sprite = representingWorldObject.buildImage;
			timerImage = gameObject.transform.Find ("TimerCountdownImage").GetComponent<Image> ();
			timerImage.sprite = representingWorldObject.buildImage; //set timer image

			Text descName = descPopup.transform.Find ("name").GetComponent<Text>();
			Text descShortcut = descPopup.transform.Find ("shortcut").GetComponent<Text>();
			Text descResourcesCost = descPopup.transform.Find ("resourcesCost").GetComponent<Text>();
			Text descCommandCost = descPopup.transform.Find ("commandCost").GetComponent<Text>();
			Text descText = descPopup.transform.Find ("desc").GetComponent<Text>();

			descName.text = representingWorldObject.objectName;
			descShortcut.text = "Shortcut: " + representingWorldObject.shortcut;
			descResourcesCost.text = representingWorldObject.cost.ToString();
			descCommandCost.text = representingWorldObject.commandCost.ToString();
			descText.text = representingWorldObject.desc;
		}
	
	}

	void startCooldownTimer(float t){
		if (!onCooldown) { //only start if somethings not already being spawned
			time = t;
			timerImage.fillAmount = 0;
			onCooldown = true;
		}
	}

	void Update(){
		if (spawnQueue <= 1) { //don't show queue if one or less is being produced
			spawnQueueText.gameObject.SetActive (false);
		}

		if (onCooldown) {
			timerImage.fillAmount += 1 / time * Time.deltaTime;
			if (timerImage.fillAmount == 1) {
				onCooldown = false;
				timerImage.fillAmount = 0;
				transform.GetComponentInParent<Buildings> ().Spawn (representingWorldObject);
				spawnQueue -= 1;
				spawnQueueText.text = spawnQueue.ToString();
				if (spawnQueue > 0) {
					startCooldownTimer (cooldown);
				} 
			}
		}
	}

	public void OnPointerClick(PointerEventData eventData){
		if (eventData.button == PointerEventData.InputButton.Left) {
			TaskOnLeftClick();
		}
		if (eventData.button == PointerEventData.InputButton.Right) {
			TaskOnRightClick();
		}
	}

	void TaskOnLeftClick(){ //when this button is left clicked
		// building.spawn()//call spawn function of building, passing in the unit/upgrade script (upgrades can be world objects)

		if (owningPlayer.canAfford(representingWorldObject) && spawnQueue < 20){ //if they can afford and spawn Queue isn't at max
			//subtracts cost first, refunding if they cancel
			owningPlayer.updateResourcesAmount(-representingWorldObject.cost);
			owningPlayer.updateCommandAmount(representingWorldObject.commandCost, 0);
			spawnQueue += 1;
			spawnQueueText.gameObject.SetActive (true);
			spawnQueueText.text = spawnQueue.ToString();
			startCooldownTimer (representingWorldObject.spawnTime); 	//start timer first, then spawn unit in update
		}
	}
	void TaskOnRightClick(){ //when this button is right clicked, cancel cooldown if there is one and refund
		if(onCooldown){
			//refund
			owningPlayer.updateResourcesAmount(representingWorldObject.cost);
			owningPlayer.updateCommandAmount(-representingWorldObject.commandCost, 0);

			spawnQueue -= 1;
			spawnQueueText.text = spawnQueue.ToString();

			if (spawnQueue <= 0) {
				onCooldown = false;  			//only stop producing if queue is empty
				timerImage.fillAmount = 0;	//only reset timer if queue is empty
			}
		}
	
	}

	public void OnMouseEnter(){
		//show build/upgrade info, like desc and price
		descPopup.SetActive (true);

	}
	public void OnMouseExit(){
		descPopup.SetActive (false);
	}

	void Start(){
		owningPlayer = GetComponentInParent<Commander>();
		time = 0f;
		onCooldown = false;

		spawnQueueText = transform.Find ("BuildQueueText").GetComponent<Text>();
		spawnQueueText.gameObject.SetActive (false);
		spawnQueue = 0;

		timerImage = gameObject.transform.Find ("TimerCountdownImage").GetComponent<Image> ();
	}
		
}