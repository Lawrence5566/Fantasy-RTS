using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSetter : MonoBehaviour {
	[SerializeField]
	private Texture2D baseCursor;
	[SerializeField]
	private Texture2D selectCursor;
	[SerializeField]
	private Texture2D attackCursor;
	[SerializeField]
	private Texture2D moveCursor;
	[SerializeField]
	private Texture2D buildCursor;

	public CursorMode cursorMode = CursorMode.Auto;

	public void setBaseCursor(){
		Cursor.SetCursor(baseCursor,Vector2.zero,cursorMode);
	}
	public void setSelectCursor(){
		Cursor.SetCursor(selectCursor,Vector2.zero,cursorMode);
	}
	public void setAttackCursor(){
		Cursor.SetCursor(attackCursor,Vector2.zero,cursorMode);
	}
	public void setMoveCursor(){
		Cursor.SetCursor(moveCursor,Vector2.zero,cursorMode);
	}
	public void setBuildCursorr(){
		Cursor.SetCursor(buildCursor,Vector2.zero,cursorMode);
	}

	// Use this for initialization
	void Start () {
		Cursor.SetCursor(baseCursor,Vector2.zero,cursorMode); //set cursor to base on game start
	}

	void Update(){
		/*Ray toMouse = Camera.main.ScreenPointToRay(Input.mousePosition); //uses main camera
		RaycastHit rayInfo;
		bool didHit = Physics.Raycast (toMouse, out rayInfo, 500.0f); //ray info going into rayinfo
	
		if(didHit){
			if (rayInfo.collider.GetComponent<SelectableObject> ()) {	//any selectable objects are owned by you, so you cant attack and can select them
				setSelectCursor ();
			} else if (rayInfo.collider.GetComponent<WorldObject> () && thisPlayer.team != rayInfo.transform.GetComponentInParent<Player>().team) { //you can attack any other world objects, as long as they are not on your team,
				if (HUD.currentlySelected.Count != 0 && HUD.currentlySelected [0].GetComponent<Unit> ()) { //if you have something selected and its a unit
						setAttackCursor ();
				}
			} else { //if not hovering over anything else, keep base cursor
				setBaseCursor ();
			}
		}*/
	}
	

}
