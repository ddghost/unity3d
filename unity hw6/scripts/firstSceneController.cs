using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class firstSceneController : MonoBehaviour ,  sceneController {
	Vector3[] checkerLoc = 
					{new Vector3(10 , 0 , -10 ) ,new Vector3(10 , 0 , 0 ) , new Vector3(10 , 0 , 10) ,
					new Vector3(0 , 0 , -10 )  ,new Vector3(0 , 0 , 10 ) ,
					new Vector3(-10 , 0 , -10 ) ,new Vector3(-10 , 0 , 0 ) ,new Vector3(-10 , 0 , 10 ) ,};
	CheckerController[] checkerCtrl;
	PlayerController playerCtrl;
	string gameStatus = "running";
	ScoreController scoreCtrl;
	// Use this for initialization
	void Start () {
		init ();
	}
	
	// Update is called once per frame
	void Update () {
		if (scoreCtrl.getScore () == checkerLoc.Length) {
			gameStatus = "gameover";
		}
	}

	void OnEnable(){
		EventManager.gameover += setGameOver;
	}

	void OnDisable(){
		EventManager.gameover -= setGameOver;
	}

	void setGameOver(){
		gameStatus = "gameover";
	}

	void init(){
		this.gameObject.AddComponent<ChaserFactory>();
		this.gameObject.AddComponent<CCActionManager>();
		this.gameObject.AddComponent<EventManager>();
		this.gameObject.AddComponent<UserGui>();
		scoreCtrl = this.gameObject.AddComponent<ScoreController>();
		Director director = Director.getInstance ();
		director.CurrentSceneController = this;
		loadResources ();
	}

	public void loadResources(){
		Object.Instantiate (Resources.Load ("Prefabs/map"), Vector3.zero, Quaternion.identity);
		checkerCtrl = new CheckerController[checkerLoc.Length];
		playerCtrl = new PlayerController ();
		for (int loop = 0 ; loop < checkerLoc.Length ; loop++) {
			GameObject checker = Object.Instantiate (Resources.Load ("Prefabs/checker")
				, checkerLoc[loop] , Quaternion.identity) as GameObject;
			checker.name = "checker" + loop.ToString ();
			checkerCtrl[loop] = checker.AddComponent(typeof (CheckerController)) as CheckerController;
		
		}
	}

	public void reset(){
		gameStatus = "running";
		for (int loop = 0 ; loop < checkerLoc.Length ; loop++) {
			checkerCtrl [loop].reset ();
		}
		playerCtrl.reset ();
		scoreCtrl.reset ();
	}

	public string getGameStatus(){
		return gameStatus;
	}

	public int getScore(){
		return scoreCtrl.getScore ();
	}
}
