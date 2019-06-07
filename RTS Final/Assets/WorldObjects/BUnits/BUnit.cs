using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BUnit : MonoBehaviour {
	public Animator animator;
	public float AttackSpeed, speed, AttackRange;
	public bool Attacking, Charging, Selected;

	private Vector3 dest;
	private Seeker seeker;
	private CharacterController charController;
	Path path;				//series of waypoints making up the path
	int currentWaypoint;	//current waypoint we are moving to
	float maxWaypointDistance = 0.5f;
	//private Vector3 rotDir;
	float currentSpeed;

    void Start () {
		dest = transform.position;
		seeker = GetComponent<Seeker> ();
		charController = GetComponent<CharacterController>();
	}

	public void selectUnit(bool mode){
		if (mode) {
			Selected = true;
			animator.SetBool ("Selected", true);
		} else {
			Selected = false;
			animator.SetBool ("Selected", false);
		}
	}

	public void attack(GameObject target){
		Attacking = true;
		if (Vector3.Distance (transform.position, target.transform.position) > AttackRange) {
			Charging = true;
		}
	}

	public void Move(Vector3 destination){
		dest = destination;
		seeker.StartPath (transform.position, dest, OnPathComplete); //where we are, where we want to go, function to call when it completes
		Attacking = false;
	}

	public void OnPathComplete(Path p){			//after path is calculated
		if (!p.error) {
			path = p; //save the path to start moving along
			currentWaypoint = 1; //always skip the first waypoint, as sometimes these are on the spot

			GraphNode node = AstarPath.active.GetNearest(dest).node;
			if (node.Walkable) { //set last waypoint to be the exact destination, as long as its walkable
				path.vectorPath[path.vectorPath.Count - 1] = dest;
			}
		} else { //path could fail if not reachable
			Debug.Log (p.error);
		}
	}
	
	// Update is called once per frame
	void Update () {
		// pathfinding //
		if (path == null) { //do nothing if we have no path
			return;
		}

		if (currentWaypoint >= path.vectorPath.Count) { //reached the last or greater waypoint (end)
			currentSpeed = 0;			 //you have reached destination, set speed to 0
			animator.SetFloat ("CurrentSpeed", 0f, .1f, Time.fixedDeltaTime);

			return;
		}

		Vector3 dir = (path.vectorPath [currentWaypoint] - transform.position).normalized * currentSpeed; //direction to move is next waypoint - position, scale by speed
		charController.SimpleMove(dir);		//simple move figures out fixedDeltaTime for us
		//set up rotation:
		Vector3 rotDir = (path.vectorPath [currentWaypoint] - transform.position).normalized * currentSpeed;
		float step = Time.fixedDeltaTime * speed;
		Quaternion target = Quaternion.identity;
		if (rotDir != Vector3.zero) {
			target = Quaternion.LookRotation (rotDir);
		}

		float speedPercent = charController.velocity.magnitude/speed;

		animator.SetFloat ("CurrentSpeed", speedPercent, .1f, Time.fixedDeltaTime); //set current animation motion to be speed/max speed 

		if (currentSpeed < speed) { 		//increase speed till at terminal velocity, this adds acceleration to units
			currentSpeed += speed / 30;		//second number is the no of frames out of 60 (60 = 1 second) it takes to reach max speed
		}

		//move waypoint forward along path only when we get to current waypoint
		if(Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < maxWaypointDistance){ //if we get close to the currentwaypoint, move to next one
			currentWaypoint++; //move to next waypoint
		}

		//rotate only after new waypoint has been selected(just incase we would have moved past it and would rotate to face backwards):
		//this way the unit is always looking ahead to next waypoint
		transform.rotation = Quaternion.Slerp (transform.rotation, target, step);
		
	}
}
