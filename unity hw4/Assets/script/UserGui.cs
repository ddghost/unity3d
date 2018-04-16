using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using baseCode;
public class UserGui : MonoBehaviour {
	roundController roundCtrl;
	bool ifShowWin = false;
	// Use this for initialization
	void Start () {
		roundCtrl = (roundController)Director.getInstance ().currentSceneController;
	}
	
	// Update is called once per frame
	void Update () {
		if (roundCtrl.status == "gameover" || roundCtrl.status == "pause") {
			return;
		}
		checkClick ();
	}

	void OnGUI(){
		string showButtonText;
		GUI.Box (new Rect (15, 15, 120, 50) ,"");
		GUI.Label (new Rect (15, 15, 120, 25), "status: " + roundCtrl.status);
		GUI.Label (new Rect (15, 40, 120, 25), "score: " + roundCtrl.scoreCtrl.getScore() );
		if (roundCtrl.status == "running") {
			showButtonText = "pause";
		} 
		else if(roundCtrl.status == "gameover" ) {
			showButtonText = "start";
		}
		else  { //status = pause
			showButtonText = "go on";
		}

		if (GUI.Button (new Rect (15, 70, 120, 30), showButtonText)) {
			if (showButtonText == "go on") {
				roundCtrl.status = "running";
			} else if (showButtonText == "start") {
				roundCtrl.status = "running";
				roundCtrl.reset ();
			} else { //showButtonText = pause
				roundCtrl.status = "pause";
			}
		}

		if (GUI.Button (new Rect (15, 110, 120, 30), "reset")) {
			
			roundCtrl.status = "running";
			roundCtrl.reset ();
		}
			
	}

	void checkClick(){
		if (Input.GetButtonDown ("Fire1")) {
			Vector3 mp = Input.mousePosition;
			Camera ca = Camera.main;
			Ray ray = ca.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				((roundController)Director.getInstance ().currentSceneController).hitDisk (hit.transform.gameObject);
			}
		}
	}
}
