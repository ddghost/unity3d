using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMono : MonoBehaviour {
	private float mosveSpeed = 5f;
	private int nowDirection = 0;
	private bool ifRun = false;
	private bool ifAlreadyTurn = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Director.getInstance ().CurrentSceneController.getGameStatus () == "running") {
			MoveControl ();
		}
	}

	void MoveControl(){        
		nowDirection = getDirection ();
		this.transform.eulerAngles = new Vector3 (0, nowDirection, 0);
		if (ifRun) {nowDirection = getDirection ();
			this.transform.Translate (0, 0, mosveSpeed * Time.deltaTime);
		}
	}

	int getDirection(){
		int direction = 0;
		if (Input.GetKey(KeyCode.W))  
		{  
			if (Input.GetKey (KeyCode.A)) {  
				direction = -45;
			}
			else if (Input.GetKey (KeyCode.D)) {
				direction = 45;
			}
			else {
				direction = 360;
			}
		}  
		else if (Input.GetKey(KeyCode.S))  
		{  
			if (Input.GetKey (KeyCode.A)) {  
				direction = -135;
			}
			else if (Input.GetKey (KeyCode.D)) {
				direction = 135;
			}
			else {
				direction = 180;
			}
		}  
		else if (Input.GetKey(KeyCode.D))  
		{  
			direction = 90;
		}  
		else if (Input.GetKey(KeyCode.A))  
		{  
			direction = -90;
		}  

		if (direction != 0) {
			ifRun = true;
			return direction;
		}
		else {
			ifRun = false;
			return nowDirection;
		}
	}

	void OnCollisionEnter(Collision collision){
		string name = collision.gameObject.name;
		if (name == "chaser") {
			Singleton<EventManager>.Instance.setGameOver ();
			gameObject.GetComponent<Animator> ().SetBool ("ifStop", true);
		}
		else if (name.Length > 5 && name.Substring(0 , 5) == "score") {
			Singleton<EventManager>.Instance.addScore (collision.gameObject.name);
			collision.gameObject.SetActive (false);
		}
	}
}
