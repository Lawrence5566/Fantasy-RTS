using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//attached to textObject

public class FloatingText : MonoBehaviour {
	public Animator animator;
	private Text textObj;

	void Awake(){ //needs to run first, since Start runs after setText at times 
		animator = GetComponentInChildren<Animator> ();
		textObj = animator.GetComponent<Text>();
	}

	void Start(){
		AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo (0);
		Destroy (gameObject, (clipInfo [0].clip.length - 0.1f)); //destroy this object just before end of clip
	}

	public void SetText(string text){
		textObj.text = text;
	}
	
	// Update is called once per frame
	void Update () {


	}
}
