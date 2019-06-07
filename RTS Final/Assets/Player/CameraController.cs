using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour {
	public float panSpeed = 20f;
	public float panBorderThickness = 10f;
	public Vector2 panLimit; //the limit you can pan, changes on map type etc
	//make sure to set this!

	public float scrollSpeed = 10f;
	public float panScrollMin = 20f;
	public float panScrollMax = 120f;

	void Awake(){
		transform.tag = "MainCamera"; //set this to be main cam by default
	}

	// Update is called once per frame
	void Update () {
		Vector3 pos = transform.position; //get x and y coords

		if (Input.mousePosition.x <= panBorderThickness || Input.GetKey("left")) {
			//pan left
			pos.x -= panSpeed * Time.deltaTime; //make sure we move relative to time not frame rate
		}
		if (Input.mousePosition.x >= Screen.width - panBorderThickness || Input.GetKey("right")) {
			//pan right
			pos.x += panSpeed * Time.deltaTime;
		}
		if (Input.mousePosition.y <= panBorderThickness || Input.GetKey("down")) {
			//pan down
			pos.z -= panSpeed * Time.deltaTime;
		}
		if (Input.mousePosition.y >= Screen.height - panBorderThickness || Input.GetKey("up")){
			//pan up
			pos.z += panSpeed * Time.deltaTime;
		}

		if(!EventSystem.current.IsPointerOverGameObject()){ //if player is not touching UI
			float scroll = Input.GetAxis ("Mouse ScrollWheel"); //get scrollwheel axis
			pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime; //100 hardcoded to give faster scroll speeds
			pos.y = Mathf.Clamp(pos.y, panScrollMin,panScrollMax);

			pos.x = Mathf.Clamp (pos.x, -20f, panLimit.x);	//stop camera going out of map
			pos.z = Mathf.Clamp (pos.z, -20f, panLimit.y);

			transform.position = pos; //set position to new position
		}

	}


}
