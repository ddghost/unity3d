using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UserGui : MonoBehaviour {
	bool ifShowWin = false;
	// Use this for initialization
	void Start () {
		
	}
	// Update is called once per frame
	void OnGUI(){
		if (Director.getInstance ().CurrentSceneController.getGameStatus() == "gameover") {
			int score = Director.getInstance ().CurrentSceneController.getScore();
			string msg = "your score is " + score + " click to restart";
			if (GUI.Button (new Rect (Screen.width / 2 - 100 , Screen.height / 2 - 20, 200, 40), msg)){
				Director.getInstance().CurrentSceneController.reset();
			}
		}

	}
}