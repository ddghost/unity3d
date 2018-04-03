using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using baseCode;


public class firstSceneController : MonoBehaviour , sceneController , firstScenceUserAction{
	environmentController environment;
	boatController myBoat ;
	const int numOfPirestOrDevil = 3;
	peopleController[] peopleCtrl  = new peopleController[numOfPirestOrDevil * 2];
	string oriSize = "right";
	FirstSceneGuiCtrl guiCtrl;
	Vector3 environmentPos = Vector3.zero; 
	Vector3 leftShorePos = new Vector3 (-6f, 2f, 0f);
	Vector3 rightShorePos = new Vector3 (6f, 2f, 0f);
	string gameStatus = "playing";
	void Awake(){
		Director.getInstance ().currentSceneController = this;
		guiCtrl = gameObject.AddComponent <FirstSceneGuiCtrl>() as FirstSceneGuiCtrl;
		loadResources();

	}
	// Use this for initialization
	void Start () {
		
	}

	void Update(){
		int leftDevil = 0, leftPriest = 0, rightDevil = 0, rightPriest = 0 , leftShorePeople = 0;
		for (int loop = 0; loop < numOfPirestOrDevil * 2; loop++) {
			if (peopleCtrl [loop].getStatus() == "shore" && peopleCtrl [loop].size == "left") {
				leftShorePeople++;
			} 
			if (peopleCtrl [loop].getName()[0] == 'd' && peopleCtrl [loop].size == "left") {
				leftDevil++;
			} else if (peopleCtrl [loop].getName()[0] == 'd' && peopleCtrl [loop].size == "right") {
				rightDevil++;
			} else if (peopleCtrl [loop].getName()[0] == 'p' && peopleCtrl [loop].size == "left") {
				leftPriest++;
			} else {
				rightPriest++;
			}
		}
		if ((leftDevil > leftPriest && leftPriest != 0) || (rightPriest != 0 && rightDevil > rightPriest)) {
			gameStatus = "lost";
		}
		else if (leftShorePeople == 6) {
			gameStatus = "win";
		}


	}

	public string getStatus(){
		return gameStatus;
	}

	public void reset(){
		gameStatus = "playing";
		for (int loop = 0; loop < numOfPirestOrDevil * 2; loop++) {
			peopleCtrl [loop].reset(environment);
		}
		myBoat.reset ();
	}


	public void loadResources(){
		environment = new environmentController();
		myBoat = new boatController ( oriSize );

		for (int loop = 0; loop < numOfPirestOrDevil; loop++) {
			peopleCtrl [loop] = new peopleController ("devil" , loop , environment.getPosVec(oriSize , loop) , "shore" , oriSize);
		}

		for (int loop = numOfPirestOrDevil; loop < 2 * numOfPirestOrDevil; loop++) {
			peopleCtrl [loop] = new peopleController ("priest" , loop , environment.getPosVec(oriSize , loop) , "shore" , oriSize );
		}
	}

	public void boatMove(){
		if (!myBoat.ifEmpty () && myBoat.getRunningState() != "running") {
			string toSize;
			string[] passengers = myBoat.getPassengerName();
			if (myBoat.size == "left") {
				toSize = "right";
			}
			else {
				toSize = "left";
			}

			for (int loop = 0; loop < 2; loop++) {
				for (int loop1 = 0; loop1 < numOfPirestOrDevil * 2; loop1++) {
					if (peopleCtrl [loop1].getName () == passengers [loop]) {
						peopleCtrl [loop1].size = toSize;
					}
				}
			}
			myBoat.move ();
		}

	}

	public void getBoatOrGetShore(string name){
		if (myBoat.getRunningState()!= "waiting") {
			return;
		}
		int numberOfPeople = name [name.Length - 1] - '0';
		if (peopleCtrl [numberOfPeople].getStatus () == "shore") {
			if (myBoat.ifHaveSeat () && myBoat.size == peopleCtrl [numberOfPeople].size ) {
				peopleCtrl [numberOfPeople].getOnBoat (myBoat);
			}
		}
		else {
			if ( myBoat.size == peopleCtrl [numberOfPeople].size) {
				peopleCtrl [numberOfPeople].getOffBoat (environment);
				myBoat.outBoat (peopleCtrl [numberOfPeople].getName() );
			}
		}

	}
}


public class boatController{
	GameObject boat;
	readonly boatMoveBeahave updateBoatMove;
	readonly firstScenceSolveClick toSolveClick;
	Vector3 leftPos = new Vector3 (-4f, 0.7f, 0f);
	Vector3 rightPos = new Vector3 (4f, 0.7f, 0f);
	string []nameOfPeopleOnBoat = {"" , ""};
	Vector3 []boatPos = { new Vector3( -0.25f , 1.5f , 0f ) , new Vector3( 0.25f , 1.5f , 0f ) };
	public string size ;
	private string defaultSize ;

	public boatController(string size){
		boat = Object.Instantiate (Resources.Load ("prefabs/boat", typeof(GameObject))
			, rightPos , Quaternion.identity, null) as GameObject ;
		boat.name = "boat";
		toSolveClick = boat.AddComponent (typeof(firstScenceSolveClick)) as firstScenceSolveClick;
		toSolveClick.setName (boat.name);
		updateBoatMove = boat.AddComponent (typeof(boatMoveBeahave)) as boatMoveBeahave;
		defaultSize = size;
		this.size = defaultSize;
	}

	public bool ifEmpty(){
		return nameOfPeopleOnBoat[0] == "" && nameOfPeopleOnBoat[1] == "";
	}
	public bool ifHaveSeat(){
		return nameOfPeopleOnBoat[0] == "" || nameOfPeopleOnBoat[1] == "";
	}

	public void move(){
		if (size == "right") {
			updateBoatMove.setAim (leftPos);
			size = "left";
		} 
		else {
			updateBoatMove.setAim (rightPos);
			size = "right";
		}
	}

	public string getRunningState(){
		return updateBoatMove.getState ();
	}

	public string[] getPassengerName(){
		return nameOfPeopleOnBoat;
	}

	public GameObject getBoat(){
		return boat;
	}

	public void outBoat(string name){
		if (nameOfPeopleOnBoat [0] == name) {
			nameOfPeopleOnBoat [0] = "";
		} 
		else if (nameOfPeopleOnBoat [1] == name) {
			nameOfPeopleOnBoat [1] = "";
		}
	}

	public Vector3 getBoatPos(string name ){
		Vector3 result =  Vector3.zero;
		for (int loop = 0; loop < 2; loop++) {
			if (nameOfPeopleOnBoat [loop].Length == 0) {
				nameOfPeopleOnBoat [loop] = name;
				result = boatPos [loop];
				break;
			}
		}
		return result;
	}

	public void reset(){
		nameOfPeopleOnBoat [0] = nameOfPeopleOnBoat [1] = "";
		size = defaultSize;
		updateBoatMove.setAim (rightPos);
	}
}


public class boatMoveBeahave: MonoBehaviour{
	Vector3 aim = new Vector3 (4f, 0.7f, 0f);
	float speed = 20.0f;
	string status = "waiting" ;

	void Update(){
		if (this.transform.position == aim) {
			status = "waiting";
		}
		else {
			this.transform.position = Vector3.MoveTowards (this.transform.position, aim , speed * Time.deltaTime);	
		}
	}

	public void setAim(Vector3 aim){
		this.aim = aim;
		status = "running"; 
	}

	public string getState(){
		return status;
	}
}



public class environmentController{
	GameObject environment;
	Vector3 environmentPos =  Vector3.zero ;
	Vector3 leftShorePos = new Vector3 (-6f, 2f, 0f);
	Vector3 rightShorePos = new Vector3 (6f, 2f, 0f);
	public environmentController(){
		environment = Object.Instantiate (Resources.Load ("prefabs/environment", typeof(GameObject))
			, environmentPos, Quaternion.identity, null) as GameObject;
	}

	public Vector3 getPosVec(string size , int number){

		Vector3 result = new Vector3(0 , 0 , 0);
		if (size == "right") {
			result = rightShorePos + number * Vector3.right;		
		}
		else {
			result = leftShorePos + number * Vector3.left;
		}
		return result;
	}

}
	

public class peopleController{

	GameObject people;
	private string status;
	public string size ;
	private string defaultSize ; 
	firstScenceSolveClick solveClick;
	int number ;

	public peopleController(string name , int number , Vector3 pos , string status , string size){
		people = Object.Instantiate (Resources.Load ("prefabs/" + name, typeof(GameObject))
			, pos, Quaternion.identity, null) as GameObject;
		people.name = name + number.ToString() ;
		solveClick = people.AddComponent (typeof(firstScenceSolveClick)) as firstScenceSolveClick;
		solveClick.setName (people.name);
		this.number = number;
		this.status = status;
		defaultSize = size;
		this.size = size;
	}



	public string getName(){
		return people.name;
	}

	public string getStatus(){
		return status;
	}
		

	public void getOnBoat(boatController boatCtrl){
		status = "boat";
		people.transform.parent = boatCtrl.getBoat().transform ;
		people.transform.localPosition = boatCtrl.getBoatPos( getName() ) ;

	}

	public void getOffBoat(environmentController envCtrl){
		status = "shore";
		people.transform.parent = null;
		people.transform.position = envCtrl.getPosVec(size , number );
	}

	public void reset( environmentController envCtrl ){
		status = "shore";
		size = defaultSize;
		people.transform.parent = null;
		people.transform.position = envCtrl.getPosVec(size , number );
	}

}

