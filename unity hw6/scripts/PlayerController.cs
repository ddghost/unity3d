using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController{
	GameObject player;
	Vector3 defaultLoc = Vector3.zero;
	public PlayerController(){
		player = Object.Instantiate (Resources.Load ("Prefabs/player"), defaultLoc , Quaternion.identity) as GameObject;
		player.AddComponent<playerMono> ();
		player.GetComponent<Animator>().SetBool ("ifStop", false);
		player.name = "player";
	}

	public GameObject getPlayer(){
		return player;
	}

	public void reset(){
		player.transform.position = defaultLoc;
		player.GetComponent<Animator>().SetBool ("ifStop", false);

	}
}
