using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//battalions should be no more that 15 to a unit (I don't want the game gettin too clustered)
//they pick one unit to be the leader, and center

public class Battalion : MonoBehaviour {
    public WorldObject battalionStats;
	public GameObject UnitPrefab;
	public int startingUnitSize;
	public List<BUnit> units; //list of all units, might be easier to just search through children? (check performance in profiler)
    public List<WorldObject> unitStats;
    private List<Vector3> unitPositions; //displacements relative to center
	//for center of battalion, use units[0].gameobject.transform.position (0 unit is center)

	//for spawningg battalion:
	private Vector3 unitFlagStart;
	private Vector3 unitFlagEnd;
	private bool spawning = false;

	void Awake () { //runs before everything else
		units = new List<BUnit> ();
		unitPositions = new List<Vector3>();

		createBattalion (startingUnitSize); //default size is 10, this will be changed later for upgrades etc
        battalionStats = GetComponent<WorldObject>();
    }

	public void createBattalion(int unitAmount){
		for (int x = 0; x < unitAmount; x++) {
			//spawn all units on top of one another
			GameObject newUnit = Instantiate (UnitPrefab, this.transform);
			newUnit.name = "unit " + x;
			newUnit.transform.localPosition = Vector3.zero;	//set local position after instantiating
			newUnit.transform.rotation = Quaternion.identity;

			units.Add (newUnit.GetComponent<BUnit>());
            unitStats.Add(newUnit.GetComponent<WorldObject>());
			unitPositions.Add (Vector3.zero);

			//turn off collisions untill they are properly spawned (spawnMove turns them back on)
            newUnit.GetComponent<CharacterController>().detectCollisions = false;
			newUnit.GetComponent<SelectableObject> ().enabled = false;
		}

	}

	public void changeFormation(string formationName){  //not for moving formation, only sets up the unitPositions list
		if (formationName == "skirmish") {
			//need to do a skirmish formation later
		} else { //standard lines
			int coloums = 5;
			int rows = 1; //starts default as 1 row

			if (units.Count > 10) { //3 rows
				rows = 3;
			} else if (units.Count > 5) { //2 rows
				rows = 2;
			} 
				
			int i = 0;
			//don't inlcude first unit, as this is the center unit
			for (float z = 0; z < rows; z++) {
				for (float x = 0; x < coloums; x++) {
					if (i >= units.Count) { //we've run out of units
						return; 
					}

                    //subtract half rows and coloums max to center it
                    //units[i].transform.position = transform.position + new Vector3((x - coloums/2)*1.5f, 0f, (z - rows/2)*1.5f); //this jumps the unit
					unitPositions[i] = new Vector3((x - coloums / 2) * 1.5f, 0f, (z - rows / 2) * 1.5f); //add local offset vector
					i++;
				}
			}
		}
	}
	public void Select(GameObject selectionCirclePrefab){
		foreach (BUnit unit in units) {
			SelectableObject unitSelect = unit.GetComponent<SelectableObject> ();
			unit.selectUnit(true);
			unitSelect.selectionCircle = Instantiate (selectionCirclePrefab);
			unitSelect.selectionCircle.transform.SetParent (unitSelect.transform, false);
		}
	}

	public void Deselect(){
		foreach (BUnit unit in units) {
			SelectableObject selectable = unit.GetComponent<SelectableObject> (); 
			Destroy (selectable.selectionCircle);
			selectable.selectionCircle = null;
			unit.selectUnit (false);
		}
	}

	public void spawnMove(Vector3 start, Vector3 end){ //initial unselectable spawn moving out of building
		//move new spawned unit to unit flag start, then get into formation & give player control
		//once in formation(and only when in formation) move to unit flag end (but player can just select them and move them at this point)

		changeFormation (""); //set up formation
		unitFlagStart = start;
		unitFlagEnd = end;
		spawning = true;
	}

	public void moveBattalion(Vector3 dest){
		changeFormation ("");

		//unit position in group rotation
		Vector3 relativePos = dest - units [0].gameObject.transform.position;
		Quaternion rotation = Quaternion.LookRotation (relativePos);

        int i = 0;
		foreach (BUnit unit in units){
            //calculate units offest from center (units[0] is leader, that is the center/anchor)
            //move to destination + offset
            //Vector3 offset = unit.transform.position - units[0].transform.position;

            //move to desination + localposition in the group
			unit.Move (dest + (rotation * unitPositions[i] ));
            i++;
		}
	}

	/*IEnumerator spawnWait(){
		yield return new WaitForSeconds (0.5f);
	}*/
	
	// Update is called once per frame
	void Update () {
		if (spawning) {
			int i = 0;
			foreach (BUnit unit in units) {
				//usually we would change these after the unit has left the building...
				unit.GetComponent<CharacterController>().detectCollisions = true;
				unit.GetComponent<SelectableObject> ().enabled = true;
				unit.transform.position = unitFlagStart + unitPositions [i];
				i++;
			}
			moveBattalion (unitFlagEnd);
			spawning = false;
		}

        foreach (WorldObject unit in unitStats){
            float hitPoints = 0f;
            hitPoints += unit.hitPoints;
            battalionStats.hitPoints = hitPoints / unitStats.Count;
        }

	}
}
