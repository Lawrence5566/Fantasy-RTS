using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

//WorldObjects require health bars attached to them

//if this script is attached to an object, the game knows its a world object
public class WorldObject : MonoBehaviour {
	public string objectName;
	public Sprite buildImage;
	public string shortcut;
    public int cost, commandCost, sellValue;
	public float hitPoints, maxHitPoints;

    public Slider healthSlider; //in battalions, this is for total average hp
	public string desc;
	public float spawnTime;

	public bool dead; //if you syncVar this, you can sync all dead units (if you ever need to)
	private float deathTimer = 5f; //time in ms the object will remain on the field... (gives you chance to revive it)

    public Commander owningCommander; //only if owned by a commander

	public void TakeDamage(int amount){
		if (!dead) {
			hitPoints -= amount;
			healthSlider.value = hitPoints;
			if (hitPoints <= 0) {
				deathComes ();
			}
		}
	}

	private void deathComes(){
		dead = true;
		Destroy (GetComponent<SelectableObject> ());	                    //no longer selectable, remove Selectable object component
		GetComponent<Animator>().SetBool("Dead", true);
    }

	void OnMouseEnter(){
		if (healthSlider) {
			healthSlider.gameObject.SetActive (true);
		}
	}
	void OnMouseExit(){
		if (healthSlider) {
			healthSlider.gameObject.SetActive (false);
		}
	}

	// Use this for initialization
	void Start () {
		dead = false;
		healthSlider = this.gameObject.GetComponentInChildren <Slider>(); //get slider from children

		if (healthSlider){ //if world object has a health bar
			hitPoints = maxHitPoints;
			healthSlider.maxValue = maxHitPoints;
			healthSlider.value = maxHitPoints;
			healthSlider.gameObject.SetActive (false); //health sliders start not active
		}
	}

	// Update is called once per frame
	void Update () {
		if (dead) {
			deathTimer -= Time.deltaTime;
			if (deathTimer < 0){
                //don't need to sync deaths across network, as things will die when they drop to 0 hp

				if (GetComponent<Buildings> ()) { //if its a building, we need to update A*
					Bounds bounds = GetComponent<Collider>().bounds; //update A* graph
					this.gameObject.SetActive(false); 				//hide object
					AstarPath.active.UpdateGraphs(bounds); 			//update local A* graph
					Destroy (this.gameObject);						//then destroy itself
				}else{
					Destroy (this.gameObject);
				}
			}
		}
	}
}
