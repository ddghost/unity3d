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

	public CCActionManager actionManager ;

	void Awake(){
		Director.getInstance ().currentSceneController = this;
		guiCtrl = gameObject.AddComponent <FirstSceneGuiCtrl>() as FirstSceneGuiCtrl;
		actionManager = gameObject.AddComponent <CCActionManager>() as CCActionManager;
		loadResources();

	}
	// Use this for initialization
	void Start () {
		
	}

	//check Win Or Lost
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
			peopleCtrl [loop].reset(environment , actionManager);
		}
		myBoat.reset (actionManager);
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
		//判断船是否能开
		if (!myBoat.ifEmpty () && myBoat.getRunningState() != "running") {
			string toSize;
			string[] passengers = myBoat.getPassengerName();
			if (myBoat.size == "left") {
				toSize = "right";
			}
			else {
				toSize = "left";
			}

			// 船到另一岸了，因此船上的人物也要到另一岸，
			for (int loop = 0; loop < 2; loop++) {
				for (int loop1 = 0; loop1 < numOfPirestOrDevil * 2; loop1++) {
					if (peopleCtrl [loop1].getName () == passengers [loop]) {
						peopleCtrl [loop1].size = toSize;
					}
				}
			}

			//开船
			myBoat.move ( actionManager );
		}
	}

	public void getBoatOrGetShore(string name){
		if (myBoat.getRunningState()!= "waiting") {
			return;
		}
		int numberOfPeople = name [name.Length - 1] - '0';
		if (peopleCtrl [numberOfPeople].getStatus () == "shore") {
			
			if (myBoat.ifHaveSeat () && myBoat.size == peopleCtrl [numberOfPeople].size ) {
				peopleCtrl [numberOfPeople].getOnBoat (myBoat , actionManager);
			}
		}
		else {
			
			if ( myBoat.size == peopleCtrl [numberOfPeople].size) {
				peopleCtrl [numberOfPeople].getOffBoat (environment , actionManager);
				myBoat.outBoat (peopleCtrl [numberOfPeople].getName() );
			}
		}

	}
}


public class boatController{
	GameObject boat;

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
		//只有一个脚本了
		toSolveClick = boat.AddComponent (typeof(firstScenceSolveClick)) as firstScenceSolveClick;
		//
		toSolveClick.setName (boat.name);
		defaultSize = size;
		this.size = defaultSize;
	}

	public bool ifEmpty(){
		return nameOfPeopleOnBoat[0] == "" && nameOfPeopleOnBoat[1] == "";
	}
	public bool ifHaveSeat(){
		return nameOfPeopleOnBoat[0] == "" || nameOfPeopleOnBoat[1] == "";
	}

	public void move(CCActionManager actionManager){
		CCBoatMoveing boatMove = CCBoatMoveing.GetSSAction (10f);
		if (size == "right") {
			boatMove.aim = leftPos;
			size = "left";
		} 
		else {
			boatMove.aim = rightPos;
			size = "right";
		}
		actionManager.RunAction (boat , boatMove , null );
	}

	public string getRunningState(){
		if (boat.transform.position == rightPos || boat.transform.position == leftPos)
			return "waiting";
		else
			return "running";
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

	public void reset(CCActionManager actionManager){
		nameOfPeopleOnBoat [0] = nameOfPeopleOnBoat [1] = "";
		size = defaultSize;
		CCBoatMoveing boatMove = CCBoatMoveing.GetSSAction ();
		boatMove.aim = rightPos;
		actionManager.RunAction (boat , boatMove , null );
		size = "right";
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
		

	public void getOnBoat(boatController boatCtrl , CCActionManager actionManager){
		status = "boat";
		people.transform.parent = boatCtrl.getBoat().transform ;
		Vector3 aim = boatCtrl.getBoatPos (getName() );
		CCPeopleMoveing peopleMove = CCPeopleMoveing.GetSSAction ( aim );
		actionManager.RunAction (people , peopleMove , null );
	}

	public void getOffBoat(environmentController envCtrl , CCActionManager actionManager){
		status = "shore";
		people.transform.parent = null;
		Vector3 aim = envCtrl.getPosVec (size, number);
		CCPeopleMoveing peopleMove = CCPeopleMoveing.GetSSAction (aim );
		actionManager.RunAction (people , peopleMove , null );
	
	}

	public void reset( environmentController envCtrl , CCActionManager actionManager){
		size = defaultSize;
		status = "shore";
		people.transform.parent = null;
		CCPeopleMoveing peopleMove = CCPeopleMoveing.GetSSAction ( envCtrl.getPosVec(size , number ) );
		actionManager.RunAction (people , peopleMove , null );
	}

}

