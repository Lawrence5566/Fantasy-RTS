using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//maybe move this into buildings master?
[RequireComponent(typeof(Buildings))]
public class Farm : MonoBehaviour {
	public GameObject floatingText;
	public float resourceCooldown;
	public int resourcesPerTick;

	Commander owningPlayer; //take this player reference from somewhere else if you need to?
	Buildings thisBuilding;

	private float resourceCooldownTick;

	void Start(){
		thisBuilding = GetComponent <Buildings> ();
		owningPlayer = GetComponentInParent<Commander> ();
	}

	void Update(){
		if (thisBuilding.isBuilding ){ 		//still building so make sure they can never reach cooldown (cus you cant generate resources yet)
			resourceCooldownTick = Time.time + resourceCooldown;
		}
			
		if (Time.time >= resourceCooldownTick) {
			resourceCooldownTick = Time.time + resourceCooldown; //refresh cooldown
			GameObject floatingTextObj = Instantiate (floatingText, transform) as GameObject; //create resource popup, parent to this object
			floatingTextObj.GetComponentInChildren<FloatingText>().SetText(resourcesPerTick.ToString());
			owningPlayer.updateResourcesAmount(resourcesPerTick);
		}
	}
}
