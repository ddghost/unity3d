# unity hw4
## 1、编写一个简单的鼠标打飞碟（Hit UFO）游戏
游戏的演示视频地址：http://v.youku.com/v_show/id_XMzU0Mjg0NDg3Mg==.html?spm=a2h3j.8428770.3416059.1
根据老师课件中的uml图，进行游戏的设计。
![这里写图片描述](https://img-blog.csdn.net/20180416185709167?watermark/2/text/aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L0REZ2hzb3Q=/font/5a6L5L2T/fontsize/400/fill/I0JBQkFCMA==/dissolve/70)
游戏的简要介绍：本次的飞碟游戏一共三关，并且没有中断，有三种飞碟，速度大小和颜色不同，第一关刷第一级别的飞碟，第二关会随机刷第一、二级别的飞碟，第三关会随机刷第一、二、三级别的飞碟。每关有10个左右的飞碟，飞碟级别越高得分也越高，暂停关卡的话点击飞碟是不会有任何效果的。

开始先预先做好飞碟的预设，飞碟只是一个高度很矮的圆柱，不同级别的飞碟会用脚本直接调节飞碟的组件属性，因此只有一个飞碟预设。
![这里写图片描述](https://img-blog.csdn.net/2018041618572244?watermark/2/text/aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L0REZ2hzb3Q=/font/5a6L5L2T/fontsize/400/fill/I0JBQkFCMA==/dissolve/70)
首先是飞碟类的代码编写,该类有一个私有的游戏对象飞碟，以便直接对飞碟的颜色，大小等等进行调整，由于一个飞碟由工厂生产出来后就会不停的使用以节省对象生成和销毁的开销，因此飞碟重新使用时会有一个reset（）方法来重新设置飞碟的参数，reset（）方法通过传递该飞碟的等级，来进行大小、速度等等调整。
```
public class diskInfo{
	public GameObject disk;
	public int diskId;
	public int lever;
	public diskInfo(int id , int lever){
		this.diskId = id;
		disk = GameObject.Instantiate(Resources.Load<GameObject>("prefabs/disk" ), Vector3.zero , Quaternion.identity);
		disk.name = "disk" + id.ToString ();
		reset (lever);
	}

	public void reset(int lever){
		disk.transform.position = new Vector3 (Random.Range (-10f, 10f), Random.Range (-10f, 10f), Random.Range (0f, 2f));
		switch (lever) {
		case 1:
			disk.GetComponent<Renderer> ().material.color = Color.red;
			disk.transform.localScale = new Vector3 (3f, 0.3f, 3f);
			break;
		case 2:
			disk.GetComponent<Renderer> ().material.color = Color.yellow;
			disk.transform.localScale = new Vector3 (2f, 0.2f, 2f);
			break;
		default:
			disk.GetComponent<Renderer> ().material.color = Color.grey;
			disk.transform.localScale = new Vector3 (1f, 0.1f, 1f);
			break;
		}
		this.lever = lever;
		disk.SetActive(true);
	}
}

```
接下来就是写飞碟工厂,used队列是存放在使用的飞碟，free是队列是存放没有在飞的飞碟，场记找飞碟工厂要飞碟的时候，飞碟工厂就看free队列有没有飞碟，没有才新建，尽可能减少对象的新建，还有别的一些方法，reset（）用于游戏重新开始的重置，getNowUsedDisk（）用于获得当前在飞的飞碟的个数，getHitDisk（）用于获得被点击的飞碟的所属于的类，freeDisk()用于在飞碟被点击到后将飞碟类放到free队列。
```
public class DiskFactory: MonoBehaviour {
	private int allDiskNum = 0;
	private List<diskInfo> used = new List<diskInfo>();
	private List<diskInfo> free = new List<diskInfo>();

	public diskInfo getDisk(int lever ){
		diskInfo nowDisk = null;
		if (free.Count > 0) {
			nowDisk = free [0];
			nowDisk.reset (lever);
			used.Add (free [0]);
			free.Remove (free [0]);

		} else {
			allDiskNum++;
			nowDisk = new diskInfo (allDiskNum , lever);
			used.Add (nowDisk);
		}
			
		return nowDisk;

	}	

	public void reset(){
		foreach (diskInfo temp in used) {
			temp.disk.SetActive (false);
			free.Add (temp);
		}
		used.Clear ();
	}

	public int getNowUsedDisk(){
		return used.Count;
	}

	public void freeDisk(diskInfo diskinfo){
		if (used.Contains (diskinfo)) {
			diskinfo.disk.SetActive (false);
			used.Remove (diskinfo);
			free.Add (diskinfo);
		}

	}

	public diskInfo getHitDisk(GameObject disk){
		foreach (diskInfo i in used) {
			if (i.disk == disk) {
				return i;
			}
		}
		return null;
	} 
}
```
动作的基类与之前相同。
```
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

```

然后是飞碟运动的类，飞碟的运动终点是一个随机的区域（与飞碟初始的随机位置区域不重叠），速度则是根据该飞碟的等级来调。
```
public class diskMove : SSAction {
	public Vector3 aim;
	public float speed;
	public diskInfo thisDisk;
	public float time = 0;
	// Use this for initialization
	public override void Start () {
		
	}

	public static diskMove getDiskMove( diskInfo disk , int lever ){
		diskMove action = ScriptableObject.CreateInstance<diskMove> ();
		switch (lever) {
		case 1:
			action.speed = 6f;
			break;
		case 2:
			action.speed = 8f;
			break;
		case 3:
			action.speed = 10f;
			break;
		}
		action.thisDisk = disk;
		action.aim = new Vector3(Random.Range(-2f, 2f) , Random.Range(-2f, 2f) , Random.Range(4f, 10f) );
		return action;

	}

	// Update is called once per frame
	public override void Update () {
		gameobject.transform.position = Vector3.MoveTowards (gameobject.transform.position, aim, speed * Time.deltaTime);

		if (this.transform.position == aim) {
			this.destroy = true;
			this.enable = false;
			Singleton<DiskFactory>.Instance.freeDisk (thisDisk);
			thisDisk.disk.SetActive(false) ;
		}
	}
}
```
写完飞碟的运动类，接下来就是动作管理器的基类，在这里基本上也是照搬前面的，唯一有点不同的就是暂停游戏的时候就要停止更新动作队列里的动作，还有要是飞碟已经被打中了，active为false时，那么该动作就移除了（避免飞碟工厂重新用这飞碟的时候飞碟有两个动作，引起bug）。
```
public class SSActionManager : MonoBehaviour {
	protected Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
	protected List<SSAction> waitingAdd = new List<SSAction>();
	protected List<int> waitingDelete = new List<int>();
	protected roundController roundCtrl ;

	void Start()
	{
		
	}

	public SSActionManager(){
		roundCtrl = (roundController)Director.getInstance ().currentSceneController;

	}
	protected void Update()
	{
		if (roundCtrl.status == "pause") //停止玩就不更新action了
			return;
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

	public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager)
	{
		action.gameobject = gameobject;
		action.transform = gameobject.transform;
		action.callback = manager;
		waitingAdd.Add(action);
		action.Start();
	}
}
```
动作管理器的子类很简单，只多了个reset方法，用于重置游戏的时候清空动作队列。
```
public class CCActionManager : SSActionManager,ISSActionCallback {

	public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
		int intParam = 0, string strParam = null, Object objectParam = null){

	}
	//清楚所有action，
	public void reset(){
		actions.Clear ();
	}
}
```

然后就是gui的编写，在这里主要实现一些与用户交互的按钮，以及鼠标点击了飞碟发射了射线后的处理。
```
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
```
根据老师ppt写了Singleton类来获取单例
```
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

然后就是回合场记的编写,回合场记主要就是决定要不要放飞碟，要放多少个等等的方法，当然还要记录下现在的回合数，还有游戏的当前状态等等。
```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using baseCode;
public class roundController : MonoBehaviour ,sceneController , UserAction {
	public string status = "running"; // running , pause , gameover
	DiskFactory diskFac ;
	int roundLever = 1;
	int numOfDiskAlredySend = 0;
	float time = 0;
	float sendDiskTime = 1;

	CCActionManager actionManager;
	public ScoreController scoreCtrl = new ScoreController();

	void Awake(){
		Director director = Director.getInstance ();
		director.currentSceneController = this;
		this.gameObject.AddComponent<DiskFactory>();
		this.gameObject.AddComponent<CCActionManager> ();
		this.gameObject.AddComponent<UserGui> ();

	}
	// Use this for initialization
	void Start () {
	    diskFac = Singleton<DiskFactory>.Instance;
		actionManager = Singleton<CCActionManager>.Instance;
	}
	
	// Update is called once per frame
	void Update () {
		if (status == "pause" || status == "gameover") {
			return;
		} 
		else if (roundLever > 3) {
			if(diskFac.getNowUsedDisk() == 0)
				status = "gameover";
			return;
		}
		else if (numOfDiskAlredySend >= 10) {
			numOfDiskAlredySend = 0;
			roundLever++;
		}
		time += Time.deltaTime ;
		checkIfSendDisk ();

	}


	public void hitDisk(GameObject disk){
		diskInfo temp = diskFac.getHitDisk (disk);
		if (temp == null) {
			Debug.Log ("the disk of clicked is null? ");
		} else {
			scoreCtrl.addScore (temp.lever);
			diskFac.freeDisk (temp);
		}

	
	}

	private void checkIfSendDisk(){
		if(time > sendDiskTime){
			float randomNumOfSend = Random.Range (0f, roundLever);
			int numOfSendDisk;
			//决定要送的碟数
			if (randomNumOfSend <= 1) {
				numOfSendDisk = 1;
			} else if (randomNumOfSend <= 2) {
				numOfSendDisk = 2;
			} else {
				numOfSendDisk = 3;
			}
			sendSomeDisks (numOfSendDisk);
			time = 0;
		}
	}

	private void sendSomeDisks(int num){
		for (int loop = 0; loop < num; loop++) {
			float randomNumOfLever = Random.Range (0f, roundLever);
			int thisDiskLever;
			//决定要送的碟的lever
			if (randomNumOfLever <= 1) {
				thisDiskLever = 1;
			} else if (randomNumOfLever <= 2) {
				thisDiskLever = 2;
			} else {
				thisDiskLever = 3;
			}
			sendOneDisk (thisDiskLever);
		}
	}

	private void sendOneDisk(int sendLever){
		numOfDiskAlredySend++;
		diskInfo oneDisk = diskFac.getDisk ( sendLever );
		diskMove moveAction = diskMove.getDiskMove(oneDisk , sendLever );
		actionManager.RunAction (oneDisk.disk , moveAction , null);
	}

	public void loadResources(){
		
	}

	public void reset(){
		actionManager.reset ();
		diskFac.reset ();
		scoreCtrl.reset ();
		roundLever = 1;
		numOfDiskAlredySend = 0;
		status = "running";
	}

}

```
当然也还有记分员，但记分员比较简单，就负责记分还有重置分数
```
public class ScoreController{
	int score = 0 ;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void addScore(int lever ){
		score += lever;
	}

	public int getScore(){
		return score;
	}

	public void reset(){
		score = 0;
	}
}
```
