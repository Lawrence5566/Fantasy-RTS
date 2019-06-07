using UnityEngine;
using UnityEngine.Networking;

//multiplayer network player script, to allow communications with the server
public class MultiPlayer : NetworkBehaviour {
	
	[SyncVar] //if value changes on the server, all clients are automatically informed (care, a hook will not updated it on one client)
	public string PlayerName = "Player"; //multiplayer name, must always be unique

	public GameObject KnightPrefab;
	public GameObject BarracksPrefab;
	public GameObject FarmPrefab;
	
	[SerializeField]
	Behaviour[] componentsToDisable;
	[SerializeField]
	GameObject[] gameObjectsToDisable;

	void Start(){
		if (!isLocalPlayer) { 										//if this player isn't controlled by the system
			for(int i = 0; i < componentsToDisable.Length; i++){ 	//loop through all components that is not yours and disable them
				componentsToDisable [i].enabled = false;
			}
			for(int i = 0; i < gameObjectsToDisable.Length; i++){ 
				gameObjectsToDisable [i].SetActive(false);
			}
		}

	}

	/*void RegisterPlayer(){ //register player for this client, naming it its unique network ID
		string _ID = GetComponent<NetworkIdentity> ().netId; //set name to network ID
		transform.name = _ID;
	}*/

	public void Build(string name, Vector3 location, Quaternion rotation){ 		//called by player.cs
		CmdBuild (name, location, rotation, gameObject); 	//ask server to build object for you
	}

	public void ChangePlayerName(string n){ 				//request player name change
		CmdChangePlayerName (n);
	}

	/// COMMANDS - only server runs these commands ///

	[Command]
	void CmdBuild(string name, Vector3 location, Quaternion rotation, GameObject parent){
		if (name == "Barracks") {
			GameObject obj = Instantiate (BarracksPrefab, location, rotation, transform);
			NetworkServer.SpawnWithClientAuthority (obj, connectionToClient); //spawn and give authority to player who ownes it 
			//currently, server knows who owns what (obj is given to correct player)
			//but the clients just throw it in the scene, so we do a rpc callback to assign the object to the player
			RpcAssignToPlayer (obj, parent);

		} else if (name == "Farm") {
			GameObject obj = Instantiate (FarmPrefab, location, rotation, transform);
			NetworkServer.SpawnWithClientAuthority (obj, connectionToClient); //spawn and give authority to player who ownes it 
			RpcAssignToPlayer (obj, parent);
		} else if (name == "Knight") {
			GameObject obj = Instantiate (KnightPrefab, location, rotation, transform);
			NetworkServer.SpawnWithClientAuthority (obj, connectionToClient); //spawn and give authority to player who ownes it 
			RpcAssignToPlayer (obj, parent);
		}
			
	}

	[Command]
	void CmdChangePlayerName(string n){
		PlayerName = n;
	}

	//Client Rpc callbacks

	[ClientRpc]
	void RpcAssignToPlayer(GameObject obj, GameObject parent){ //assign parent of object
		obj.transform.SetParent (parent.transform);
	}
		

}
