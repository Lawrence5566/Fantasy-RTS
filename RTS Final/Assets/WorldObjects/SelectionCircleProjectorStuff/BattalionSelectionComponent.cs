using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

//remember that selection circles only project on terrain!
public class BattalionSelectionComponent : MonoBehaviour{
    public CommanderInput commanderInput;

	bool isSelecting = false;
	Vector3 mousePosition1;

	public GameObject selectionCirclePrefab;
	public GameObject selectionCircleBiggerPrefab;

    void Start(){
        commanderInput = GetComponent<CommanderInput>();
    }

    public void deselectAll(){
		//make clauses for all types of units, as they might have deslection animations, menus to close, etc

		foreach (var battalion in GetComponentsInChildren<Battalion>()) { //get all selectable objects in children of player
			battalion.Deselect();
		}

		foreach (Buildings Building in GetComponentsInChildren<Buildings>())
        {
			Building.closeMenu();
			Destroy(Building.GetComponent<SelectableObject>().selectionCircle);
        }

        commanderInput.updateSelectedObjects((WorldObject)null);

    }

	void Update()
	{
		// If we press the left mouse button, save mouse location and begin selection, also deselect all units (don't do this is ctrl is down?)
		if (Input.GetMouseButtonDown (0) && !EventSystem.current.IsPointerOverGameObject()) {
			isSelecting = true;
			mousePosition1 = Input.mousePosition;
			deselectAll ();
		}

		//If we let go of the left mouse button, end selection
		if (Input.GetMouseButtonUp (0)) {
			List<SelectableObject> trySelectedObjects = new List<SelectableObject> ();										//creates a new list of selectable object's

			foreach (var selectableObject in GetComponentsInChildren<SelectableObject>()) { 							//checking if we can select the unit
				if (IsWithinSelectionBounds (selectableObject.gameObject) && selectableObject.isActiveAndEnabled) { 	//group selection
					trySelectedObjects.Add (selectableObject);

				} else if (IsSingleSelected (selectableObject.gameObject)) {											//single select
					trySelectedObjects.Add (selectableObject);
				}
			}
				
			List<SelectableObject> selectedObjects = new List<SelectableObject> ();
		
			if (trySelectedObjects.Count > 1) { //deselect all buildings if selected count is greater than 1
				foreach (SelectableObject i in trySelectedObjects) {
					Buildings building = i.GetComponent<Buildings> ();
					if (building) {
						Destroy (i.selectionCircle);
					} else {
						selectedObjects.Add (i);
					}
				}
			} else if (trySelectedObjects.Count == 1) {
				selectedObjects.Add (trySelectedObjects [0]);
			}
				
			if (selectedObjects.Count > 0) { //update selection
                commanderInput.updateSelectedObjects(selectedObjects);
            }

			isSelecting = false;
		}

		if (isSelecting) { //creating circles dynamically during group selection
			foreach (var selectableObject in GetComponentsInChildren<SelectableObject>()) { //out of all selectable units
				if (selectableObject.isActiveAndEnabled && (IsWithinSelectionBounds (selectableObject.gameObject)) || IsSingleSelected(selectableObject.gameObject)) { 	//if in bounds,
					createCircle (selectableObject); 																													//create A cirlce
				} 
			}
		}
			

	}

	public void createCircle(SelectableObject selectableObject){ //cirlce prefabs should be set to 0.1 and 0.2 higher in y for projection to work
		if (selectableObject.selectionCircle == null) { //only creates if they haven't got one, this stops us setting all units many times
			if (selectableObject.gameObject.GetComponent<Buildings>()) { //if selected is a building, use bigger selection prefab
				selectableObject.selectionCircle = Instantiate (selectionCircleBiggerPrefab);
				selectableObject.selectionCircle.transform.SetParent (selectableObject.transform, false);
			} else { //its a unit or battalion, make sure to call select on the battalion and give the units circles in the battalion
				Battalion battalion = selectableObject.gameObject.GetComponentInParent<Battalion>(); //get battalion
				battalion.Select(selectionCirclePrefab);
			}
		}
	}

	public bool IsWithinSelectionBounds( GameObject gameObject ){  //checks if unit is within slection box, returns object in bounds
		if( !isSelecting )
			return false;
		//uses main camera, as this will always be the palyters thanks to controller
		var viewportBounds = Utils.GetViewportBounds(Camera.main, mousePosition1, Input.mousePosition );

		return viewportBounds.Contains(Camera.main.WorldToViewportPoint( gameObject.transform.position ) );
	}

	public bool IsSingleSelected(GameObject gameObject){
		//clicking single objects//
		Ray toMouse = Camera.main.ScreenPointToRay(Input.mousePosition); //uses main camera
		RaycastHit rayInfo;
		bool didHit = Physics.Raycast (toMouse, out rayInfo, 500.0f); //ray info going into rayinfo

		if (didHit) { 	//if hit something
			SelectableObject objectToSelect = rayInfo.collider.GetComponent<SelectableObject> (); //get world object script if it exists

			if (objectToSelect == gameObject.GetComponent<SelectableObject> ()) {		//if single unit that was hit, is equal to passed unit
				return true;
			} 

		}
		return false;
	}

	void OnGUI(){
		if( isSelecting ){
			// Create a rect from both mouse positions
			var rect = Utils.GetScreenRect( mousePosition1, Input.mousePosition );
			Utils.DrawScreenRect( rect, new Color( 0.8f, 0.8f, 0.95f, 0.25f ) );
			Utils.DrawScreenRectBorder( rect, 2, new Color( 0.8f, 0.8f, 0.95f ) );
		}
	}
}

public static class Utils //utility class
{
	static Texture2D _whiteTexture;
	public static Texture2D WhiteTexture
	{
		get
		{
			if( _whiteTexture == null )
			{
				_whiteTexture = new Texture2D( 1, 1 );
				_whiteTexture.SetPixel( 0, 0, Color.white );
				_whiteTexture.Apply();
			}

			return _whiteTexture;
		}
	}

	public static void DrawScreenRect( Rect rect, Color color ) //can only be accessed in OnGUI
	{
		GUI.color = color;
		GUI.DrawTexture( rect, WhiteTexture );
		GUI.color = Color.white;
	}

	public static void DrawScreenRectBorder( Rect rect, float thickness, Color color ) //draws rect border (empty inside)
	{
		// Top
		Utils.DrawScreenRect( new Rect( rect.xMin, rect.yMin, rect.width, thickness ), color );
		// Left
		Utils.DrawScreenRect( new Rect( rect.xMin, rect.yMin, thickness, rect.height ), color );
		// Right
		Utils.DrawScreenRect( new Rect( rect.xMax - thickness, rect.yMin, thickness, rect.height ), color);
		// Bottom
		Utils.DrawScreenRect( new Rect( rect.xMin, rect.yMax - thickness, rect.width, thickness ), color );
	}
	public static Rect GetScreenRect( Vector3 screenPosition1, Vector3 screenPosition2 )
	{
		// Move origin from bottom left to top left
		screenPosition1.y = Screen.height - screenPosition1.y;
		screenPosition2.y = Screen.height - screenPosition2.y;
		// Calculate corners
		var topLeft = Vector3.Min( screenPosition1, screenPosition2 );
		var bottomRight = Vector3.Max( screenPosition1, screenPosition2 );
		// Create Rect
		return Rect.MinMaxRect( topLeft.x, topLeft.y, bottomRight.x, bottomRight.y );
	}

	public static Bounds GetViewportBounds( Camera camera, Vector3 screenPosition1, Vector3 screenPosition2 )
	{
		var v1 = Camera.main.ScreenToViewportPoint( screenPosition1 );
		var v2 = Camera.main.ScreenToViewportPoint( screenPosition2 );
		var min = Vector3.Min( v1, v2 );
		var max = Vector3.Max( v1, v2 );
		min.z = camera.nearClipPlane;
		max.z = camera.farClipPlane;

		var bounds = new Bounds();
		bounds.SetMinMax( min, max );
		return bounds;
	}
}
