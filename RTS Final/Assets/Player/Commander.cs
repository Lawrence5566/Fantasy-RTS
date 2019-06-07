using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Commander : NetworkBehaviour {
	public int team;
	public string playerColor;
	public string username;
	public int Resources;
	public int CommandAmount;
	public int CommandMax;

	public GameObject KnightBattalionPrefab;
	public GameObject BarracksPrefab;
	public GameObject FarmPrefab;

	[SerializeField]
	Behaviour[] componentsToDisable;
	[SerializeField]
	GameObject[] gameObjectsToDisable;

	private NetworkIdentity thisId;
	private Vector3 unitFlagStart; //for the current spawning unit
	private Vector3 unitFlagEnd;

	void Start(){
		Resources = 1000;
		CommandAmount = 0;
		CommandMax = 100;

		thisId = GetComponent<NetworkIdentity> ();
	}

    void Update(){
		if (!isLocalPlayer) { 										//if this player isn't controlled by the system
			for(int i = 0; i < componentsToDisable.Length; i++){ 	//loop through all components that is not yours and disable them
				componentsToDisable [i].enabled = false;
			}
			for(int i = 0; i < gameObjectsToDisable.Length; i++){ 
				gameObjectsToDisable [i].SetActive(false);
			}

			return; //then return, since none of this should run execpt local players version
		}
    }

    public void updateResourcesAmount(int amount){ //need to make these multiplayer versions when database is set up, but keep method name
		Resources += amount;
		HUD.HUDResourcesAmount.text = Resources.ToString();
	}
	public void updateCommandAmount(int amount, int max){ //pass in 0 to either if you don't want to update it
		CommandAmount += amount;
		CommandMax += max;

		HUD.HUDCommandAmount.text = CommandAmount.ToString () + "/" + CommandMax.ToString ();
	}

	public void moveUnitOnSpawn(NetworkInstanceId id){ //moves your new battalion out of spawn
		GameObject obj = ClientScene.FindLocalObject(id);
		Battalion battalion = obj.GetComponent<Battalion> ();
		battalion.spawnMove (unitFlagStart, unitFlagEnd);
		//move new spawned unit to unit flag start, then get into formation & give player control
		//once in formation(and only when in formation) move to unit flag end (but player can just select them and move them at this point)
	}

	public void SpawnUnit(string name, Vector3 unitFlagSpawn, Vector3 start, Vector3 end, int cost) {
		Debug.Log("spawn");

		unitFlagStart = start; //these need to be set for moving the battalion of out of its spawn
		unitFlagEnd = end;

		updateResourcesAmount(-cost);
		//a client that passes the prefab in gives a null object?
		CmdSpawnUnit(name, unitFlagSpawn, GetComponent<NetworkIdentity>());
	}
		
	public void Build(string name, Vector3 location, Quaternion rotation, int cost) {
		Debug.Log("build");
		updateResourcesAmount(-cost);
		//a client that passes the prefab in gives a null object?
		CmdSpawnBuilding(name, location, rotation, GetComponent<NetworkIdentity>());
	}

    // multiplayer scrpts //
	//calls on server when you call commands

	[Command]
	void CmdSpawnUnit(string name, Vector3 unitFlagSpawn, NetworkIdentity owningPlayer){
		//determine unit to spawn
		//spawn it at unit flag spawn
		//move to unit flag start, then get into formation
		//once in formation(and only when in formation) move to unit flag end

		GameObject obj = new GameObject ();
		switch (name) {
		case "Knight Battalion":
			obj = KnightBattalionPrefab;
			break;
		default:
			Debug.Log ("no prefab for that unit or unit name is wrong");
			break;
		}

		GameObject instance = Instantiate(obj, unitFlagSpawn, Quaternion.identity, owningPlayer.transform);
		//object now exists on the server

		//propagate to clients:
		NetworkServer.SpawnWithClientAuthority(instance, connectionToClient);
		// This should work, if not, you'll maybe have to get the component from the spawned object directly, rather than the instance.
		RpcOnUnitSpawned(instance.GetComponent<NetworkIdentity>().netId, owningPlayer);
	}

	[ClientRpc]
	void RpcOnUnitSpawned(NetworkInstanceId id, NetworkIdentity owningPlayer){
		GameObject obj = ClientScene.FindLocalObject(id);
		obj.transform.SetParent(owningPlayer.transform);

		Debug.Log("The GameObject that spawned was " + obj.name);

		if (owningPlayer = thisId) { //if you own this object
			moveUnitOnSpawn(id); //move the unit out of the building
		}
	}
		

    [Command]
	void CmdSpawnBuilding(string name, Vector3 location, Quaternion rotation, NetworkIdentity owningPlayer)
    {
		//running on the server
		//decide which object to spawn using objName of worldobject scripts
		GameObject obj = new GameObject();

		switch (name){
			case "Barracks":
				obj = BarracksPrefab;
				break;
			case "Farm":
				obj = FarmPrefab;
				break;
			default:
				Debug.Log ("no prefab for that building or object name is wrong");
				break;
		}
			

		GameObject instance = Instantiate(obj, location, rotation, owningPlayer.transform);
		//object now exists on the server

		//propagate to clients:
		NetworkServer.SpawnWithClientAuthority(instance, connectionToClient);
        // This should work, if not, you'll maybe have to get the component from the spawned object directly, rather than the instance.
		RpcOnObjectSpawned(instance.GetComponent<NetworkIdentity>().netId, owningPlayer);
        
    }

    [ClientRpc]
	void RpcOnObjectSpawned(NetworkInstanceId id, NetworkIdentity owningPlayer) //setting owner of a object spawned
    {
        GameObject obj = ClientScene.FindLocalObject(id);
		obj.transform.SetParent(owningPlayer.transform);

        Debug.Log("The GameObject that spawned was " + obj.name);
    }

    /* [Command]
    void CmdDestroyObject(NetworkInstanceId id)
    {
        GameObject obj = NetworkServer.FindLocalObject(id);

        NetworkServer.Destroy(obj);

        // This should work, if not, you'll maybe have to get the component from the spawned object directly, rather than the instance.
        RpcOnObjDestroyed(obj.GetComponent<NetworkIdentity>().netId); //call this to check it was destroyed
    }
    */

    /* [ClientRPC]
    void RpcOnObjDestroyed(NetworkInstanceId id) //might not be needed? (you can check an object is spawned)
    {
        GameObject obj = ClientScene.FindLocalObject(id);

        Debug.Log("The GameObject that was destroyed was " + obj.name);
    }*/

	//called by BuildMenu:
    public bool canAfford(WorldObject objToBuy) //need to make this a command so server runs it
    {
        if (objToBuy.cost <= Resources)
        {
            //can buy, try command points
            if (objToBuy.commandCost + CommandAmount <= CommandMax)
            {
                return true;    //can buy!
            }
            else
            {//can't buy, give not ennough command points message
                return false;
            }
        }
        else
        {
            return false; //can't buy, send can't afford message
        }
    }



}
