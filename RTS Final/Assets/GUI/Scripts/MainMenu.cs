using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
	public GameObject SinglePlayerMenu;
	public GameObject MultiplayerMenu;

	public void QuitGame(){
		Application.Quit();
	}

	public void QuickPlay(){
		SceneManager.LoadScene("Level1");
	}
		

}
