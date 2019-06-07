using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//make sure to turn of static batching in the player settings!, otherwise meshes won't combine

public class MeshCombiner : MonoBehaviour {
	public void CombineMeshes(){

		MeshRenderer thisRenderer = gameObject.GetComponent<MeshRenderer> ();

		Quaternion oldRot = transform.rotation;
		Vector3 oldPos = transform.position;

		transform.rotation = Quaternion.identity;
		transform.position = Vector3.zero;


		// All our children (and us)
		MeshFilter[] filters = GetComponentsInChildren<MeshFilter> (false); //(false means don't include inactive objects)

		// All the meshes in our children (just a big list)
		List<Material> materials = new List<Material>();
		MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer> (false); // <-- you can optimize this
		foreach (MeshRenderer renderer in renderers)
		{
			if (renderer.transform == transform) //skip ourselves
				continue;

		    Material[] localMats = renderer.sharedMaterials;
			//populate our materials list with all materials in children:
			foreach (Material localMat in localMats) //find everything that can have materials, look at each material, if we dont have it, add it. 
				if(!materials.Contains(localMat))
					materials.Add (localMat);
		}
		thisRenderer.materials = materials.ToArray(); //add all our collected materials to the final mesh renderer

		// Each material will have a mesh for it.
		List<Mesh> submeshes = new List<Mesh>();
		foreach (Material material in materials) //creating a sub mesh for each material to go in 'submeshes'
		{
		// Make a combiner for each (sub)mesh that is mapped to the right material.
			List<CombineInstance> combiners = new List<CombineInstance> ();
		   	foreach (MeshFilter filter in filters)
		   	{
				if (filter.transform == transform) continue; //skip ourselves

				// The filter doesn't know what materials are involved, get the renderer.
				MeshRenderer renderer = filter.GetComponent<MeshRenderer> ();  // <-- (Easy optimization is possible here, give it a try!)
				if (renderer == null){
				Debug.LogError (filter.name + " has no MeshRenderer");
				continue;
				}

				// Let's see if their materials are the one we want right now.
				Material[] localMaterials = renderer.sharedMaterials;
				for (int materialIndex = 0; materialIndex < localMaterials.Length; materialIndex++)
				{
					if (localMaterials [materialIndex] != material)
						continue;
					// This submesh is the material we're looking for right now.
					CombineInstance ci = new CombineInstance();
					ci.mesh = filter.sharedMesh;
					ci.subMeshIndex = materialIndex;
					ci.transform = filter.transform.localToWorldMatrix;
					combiners.Add (ci);
				}
		  	}
			// Flatten into a single mesh.
			Mesh mesh = new Mesh ();
			mesh.CombineMeshes (combiners.ToArray(), true);
			submeshes.Add (mesh);
		}

		//so now we have a bunch of submeshes all corresponding to a material

		// The final mesh: combine all the material-specific meshes as independent submeshes.
		List<CombineInstance> finalCombiners = new List<CombineInstance>();
		foreach (Mesh mesh in submeshes)  //for each one of our material meshes
		{
		 	CombineInstance ci = new CombineInstance ();
			ci.mesh = mesh;
			ci.subMeshIndex = 0;
			ci.transform = Matrix4x4.identity;
			finalCombiners.Add (ci);
		}
		Mesh finalMesh = new Mesh();
		finalMesh.CombineMeshes (finalCombiners.ToArray(), false); //set to false cus we need the submeshes
		GetComponent<MeshFilter> ().sharedMesh = finalMesh;

		transform.rotation = oldRot;
		transform.position = oldPos;

		for (int i = 0; i < transform.childCount; i++) {
			transform.GetChild (i).gameObject.SetActive (false);
		}
	}

	/*public void changeColour(){ //change colour to blue

		// All our children (and us)
		MeshFilter[] filters = GetComponentsInChildren<MeshFilter> (false); //(false means don't include inactive objects)

		// All the meshes in our children (just a big list)
		List<Material> materials = new List<Material>();
		MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer> (false); // <-- you can optimize this
		foreach (MeshRenderer renderer in renderers)
		{
			if (renderer.transform == transform) //skip ourselves
				continue;

			Material[] localMats = renderer.sharedMaterials;
			//populate our materials list with all materials in children:
			foreach (Material localMat in localMats) //find everything that can have materials, look at each material, and change its colour
				localMat.color = Color.blue;
		}
	
	}*/
}
