# unity3d hw6
这次作业是巡逻兵游戏，简单的介绍一下游戏规则，一共有九个房间，玩家开始在正中间的房间，按wasd移动，玩家的目标是要把另外的八个房间里的红色方块全部捡完，但是这些房间都有巡逻兵看守，玩家进入房间后巡逻兵会追赶玩家，被追上就游戏失败，玩家离开房间后巡逻兵不会追出房间。下面来看一个gif图简单演示一下。


这次与上次作业不同的是这次作业要用到观察者模式，还有unity3d中的碰撞器，触发器，动画等等。多用了这么多东西，因此做起来也花了不少时间。

## 模型资源
首先先去找了一下模型资源，由于用到了动画，因此去unity的Assest store找了一下资源（自己不会做），资源链接如下，
https://www.assetstore.unity3d.com/cn/?stay#!/content/73611
这个模型有站立，走动，射击的动画，但是没有死亡，不过没关系，只要可以走一走就好了。然后就做将动画组合成Animator，毕竟玩家死了之后还一直走不是很科学，这个Animator就很简单，设置一个bool变量判断玩家是否死亡，死了就停止走动动画，换成站立不动的，重新开始后玩家又活了，因此把该bool变量改一下，玩家就又在走动了。Animator如下图
！！！！

## 预置资源
然后就是做预置，首先做了地图预置，这个就是比较枯燥的工作了，把墙和地板设置好位置大小。
然后再做巡逻兵和玩家的预置，注意这两个预置都要加上碰撞盒collider，手动设置好大小，并且玩家还要加上刚体，完成碰撞要两个物体都有碰撞盒且其中一个由刚体，因此给玩家加了即可，由于巡逻路线事先设好，巡逻兵没有加，考虑到巡逻兵的路线不会穿过墙，要是墙会在路线上，那就一定要加，否则由于墙和巡逻兵都没刚体，没有判断碰撞，那就会出现穿墙的情况。
然后还有要做玩家捡了得分的小块，这个小块就是个cube。还有就是一个检测器，检测玩家是否进入了房间，在空对象上加了个碰撞盒（box collider），调一下碰撞盒位置大小，最关键的一步就是要把碰撞盒中的isTrigger点了，如下图所示
！！！！！！

这样碰撞盒就变成了一个触发器，玩家进入的时候不会有碰撞效果，但要是在这个检测器挂一个脚本，那么玩家进入的时候脚本就会调用OnTriggerEnter()这个函数，我们就可以得知玩家进入了房间了，在这个函数里让房间里的巡逻兵把走向的目标设到玩家身上即可，离开触发器的话也会调用OnTriggerExit（）这个函数。试想要是不设置触发器的话，玩家进入房间就有碰撞效果，那么就被这个无形的墙挡住了，根本就进不去了。


## 代码
做完前期的准备工作后，就完事具备，只欠一个程序员了。这一次作业要用到观察者模式，这个模式可以更好的解耦，让程序更简洁易懂。虽然做了这次作业知道怎么用这个模式了，但其实我还是不太清楚什么时候该用什么时候不该用，例如游戏开始时该不该让场记发布一下游戏开始了呢，因此这次没有用太多，只在玩家被抓捕后发布游戏结束，还有就是玩家捡到红色方块后发布加分。
s
### 观察者模式
下面是有关这个模式的代码，专门有一个EventManager类管理事件。
```
public class EventManager : MonoBehaviour {
	public delegate void checkGameOver();
	public static event checkGameOver gameover;
	public delegate void ScoreChange(string name);
	public static event ScoreChange scoreChange;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setGameOver(){
		if (gameover != null) {
			gameover ();
		}
	}

	public void addScore(string name){
		if (scoreChange != null) {
			scoreChange (name);
		}
	}

}
```

然后在场记还有分数管理员那里订阅事件。
```
void OnEnable(){
	EventManager.gameover += setGameOver;
}

void OnDisable(){
    EventManager.gameover -= setGameOver;
}


void OnEnable(){
    EventManager.scoreChange += addScore;
}

void OnDisable(){
    EventManager.scoreChange -= addScore;
}
```

当玩家撞到巡逻者的时候，发布游戏失败事件，撞到红色块的时候，发布加分事件。这里由于玩家要有碰撞OnCollisionEnter，因此没有将玩家运动的脚本交给运动管理器管理。
```
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
```


### 房间检测器
接下来再展示比较关键的一部分代码，那就是房间检测器的控制器代码，这个脚本会被挂在检测器上，每个房间都有独立的检测器，并且为了方便，得分的红色方块也在这里加载，可以把它看成是每个房间的管理者，它管理了追捕者和红色方块，当检测到玩家进入或者离开的时候，设定追捕者是否追捕玩家。
```
public class CheckerController: MonoBehaviour{
	ChaserInfo chaser;
	GameObject score;
	Vector3 defaultLoc = Vector3.zero;
	void Awake(){
		ChaserFactory chaserFactory = Singleton<ChaserFactory>.Instance;
		chaser = chaserFactory.getChaser();
		chaser.setParent (this.gameObject, defaultLoc);
		chaser.setCheckerWorldLoc (this.transform.position);
		score = Object.Instantiate (Resources.Load ("Prefabs/score"), Vector3.zero, Quaternion.identity) as GameObject;
		score.transform.parent = this.gameObject.transform;
		score.transform.localPosition = Vector3.zero;
		score.name = "score" + this.gameObject.name.Substring (7);
	}

	public void reset(){
		chaser.reset ();
		score.SetActive (true);
		chaser.setParent (this.gameObject, defaultLoc);
	}

	void OnTriggerEnter(Collider collider){
		if(collider.gameObject.name == "player")
			chaser.runner = collider.gameObject;
	}

	void OnTriggerExit(Collider collider){
		if(collider.gameObject.name == "player")
			chaser.runner = null;
	}
}

```

### 追捕者
给每个追捕者弄了个ChaserInfo类来管理，在里面设置了追捕者相对每个房间里的检测器的默认巡逻路径，当房间检测到有人进来的时候，这个类里的runner就会设置玩家。追捕运动是PaserAction类，这个交由动作管理器管理，PaserAction里有对应的追捕者的信息ChaserInfo，可以直接调用来看是否要追捕玩家。
```
public class ChaserInfo{
	public GameObject runner;
	private GameObject chaser;
	private PatrolAction patrolAc;
	private int nowAim;
	Vector3 checkerWorldLoc ;

	Vector3[] patrolLoc = { new Vector3 (4, 0, 0) , new Vector3 (0, 0, 4) 
								, new Vector3 (-4, 0, 0) , new Vector3 (0 , 0, -4) };

	// Use this for initialization
	public ChaserInfo(){
		chaser = Object.Instantiate (Resources.Load ("Prefabs/chaser") 
			, Vector3.zero , Quaternion.identity) as GameObject;
		runner = null;
		nowAim = 0;
		chaser.name = "chaser";
	}

	public void setParent(GameObject parent , Vector3 loc){
		chaser.transform.parent = parent.transform;
		chaser.transform.localPosition = loc;
	}

	public void setNewAim(){
		nowAim = (nowAim + 1) % patrolLoc.Length;
		patrolAc.setAim(patrolLoc[nowAim] );
	}

	public void setCheckerWorldLoc(Vector3 loc){
		checkerWorldLoc = loc;
		setAction ();
	}

	void setAction(){
		patrolAc = PatrolAction.getAction (); 
		patrolAc.setChaserInfo (this);
		Singleton<CCActionManager>.Instance.RunAction(chaser , patrolAc , null );
	}

	public Vector3 getCheckerWorldLoc(){
		return checkerWorldLoc;
	}

	public void reset(){
		nowAim = 0;
		runner = null;
		setAction ();
		Singleton<CCActionManager>.Instance.RunAction(chaser , patrolAc , null );
	}
}


public class PatrolAction : SSAction {
	Vector3 aim;
	float speed = 5f;
	ChaserInfo chaserInfo = null;

	// Use this for initialization
	public override void Start () {
		aim = this.transform.localPosition;
	}

	public static PatrolAction getAction(){
		return new PatrolAction ();
	}
	
	// Update is called once per frame
	public override void Update () {
		if (chaserInfo != null) {
			if (chaserInfo.runner != null) {
				aim = chaserInfo.runner.transform.position - chaserInfo.getCheckerWorldLoc ();
			}

			gameobject.transform.localPosition = Vector3.MoveTowards (gameobject.transform.localPosition 
				, aim, speed * Time.deltaTime);
			gameobject.transform.LookAt (chaserInfo.getCheckerWorldLoc() + aim);
			if (aim == gameobject.transform.localPosition) {
				chaserInfo.setNewAim ();
			} 
		}

	}
		
	public void setAim(Vector3 newAim){
		aim = newAim;
	}

	public void setChaserInfo(ChaserInfo info){
		chaserInfo = info;
	}
}


```

### 玩家
关于玩家的主要由两个类，一个是玩家的控制器playerController，还有一个就是玩家运动的脚本playerMono，运动脚本主要就是设置玩家移动，还有就是设置碰撞红色小块或追捕者后发布事件。
```
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


```


### 场记
场记主要就是设置一下游戏状态，加载等等。
```
ublic class firstSceneController : MonoBehaviour ,  sceneController {
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
```

### 分数管理员
scoreController主要就是管理分数这一个整形变量，订阅了加分的事件，代码量不多。
```
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
```

### Gui
Gui只在游戏结束（失败或成功）后弹出一个框。
```
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
```

### 生产追捕者的工厂
这个工厂的代码很少，主要是因为生产了巡逻兵后要把巡逻兵的以房间的检测器为本地位置，而不是在该工厂设置位置。
```
public class ChaserFactory: MonoBehaviour{

	public ChaserInfo getChaser(){
		return new ChaserInfo ();
	}
}

```


### 导演 动作基类 动作管理器 生成单例的Singleton类
这些与之前的代码相差不大，基本上就是复制粘贴以前的代码。
！！！
```
public class Director : System.Object {
	private static Director _instance;

	public sceneController CurrentSceneController{ get; set; }
	public static Director getInstance() {
		if (_instance == null) {

			_instance = new Director ();
		}
		return _instance;
	}
}

public interface sceneController{
	void loadResources();
    string getGameStatus();
	void reset ();
	int getScore ();
}

public enum SSActionEventType : int { Started, Competeted }

public interface ISSActionCallback
{
	void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
		int intParam = 0, string strParam = null, Object objectParam = null);
}

public class SSAction : ScriptableObject
{
	public bool enable = true;
	public bool destroy = false;

	public GameObject gameobject { get; set; }
	public Transform transform { get; set; }
	public ISSActionCallback callback { get; set; }

	protected SSAction() { }

	public virtual void Start()
	{
		throw new System.NotImplementedException();
	}

	public virtual void Update()
	{
		throw new System.NotImplementedException();
	}
		
}


public class SSActionManager : MonoBehaviour {
	protected Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
	protected List<SSAction> waitingAdd = new List<SSAction>();
	protected List<int> waitingDelete = new List<int>();

	void Start()
	{
		
	}

	public SSActionManager(){
		
	}

	public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager)
	{
		action.gameobject = gameobject;
		action.transform = gameobject.transform;
		action.callback = manager;
		waitingAdd.Add(action);
		action.Start();
	}
}


public class CCActionManager : SSActionManager,ISSActionCallback {

	public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
		int intParam = 0, string strParam = null, Object objectParam = null){

	}
		
	protected void Update()
	{
		if (Director.getInstance().CurrentSceneController.getGameStatus() != "running") {
			actions.Clear ();
		}

		foreach (SSAction ac in waitingAdd) actions[ac.GetInstanceID()] = ac;
		waitingAdd.Clear();

		foreach (KeyValuePair<int, SSAction> kv in actions)
		{
			SSAction ac = kv.Value;
			if (ac.gameobject.active == false || ac.destroy)//gameobject的active是false就不更新action了
			{
				waitingDelete.Add(ac.GetInstanceID());
			}
			else if ( ac.enable)
			{
				ac.Update();
			}
		}
		foreach (int key in waitingDelete)
		{
			SSAction ac = actions[key]; actions.Remove(key); DestroyObject(ac);
		}
		waitingDelete.Clear();
	}
}

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour{

	protected static T instance;
	public static T Instance{
		get{ 
			if (instance == null) {
				instance = (T)FindObjectOfType (typeof(T));
				if (instance == null) {
					Debug.LogError ("An instance of " + typeof(T) +
						" is needed in the scene, but there is none.");
				}
			}
			return instance;
		}
	}

}

```