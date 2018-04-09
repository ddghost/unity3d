# unity hw3

## 1.操作与总结 
> * 参考 Fantasy Skybox FREE 构建自己的游戏场景
> #### 答：
![此处输入图片的描述][1]


> * 写一个简单的总结，总结游戏对象的使用
> #### 答：游戏对象有
> * 空（empty）：在设计牧师与魔鬼的时候，由于要求动态生成场景，因此空对象可以用于挂载脚本来加载资源。
> * 摄像机（camera）：屏幕左下角是（0，0），右上角是（1，1），摄像机可以通过设定不同的深度进行分屏显示，例如可以用两个摄像机来实现主视角和角落的小地图的显示。
> * 光线（light）：没怎么使用过，调整过平行光线的角度，让场景看起来有些不同。
> * 一般的3d物体：立方体（cube），球（sphere），可以通过增加组件来添加脚本、添加材质等等，是构成场景的基本物体。

## 2.编程实践
### 实现牧师与魔鬼的动作分离
#### 本次作业只修改了代码，游戏的逻辑和ui没有改变，因此视频还是上一次作业的。
刚开始对动作的分离还不是太清晰，现在想想，动作分离应该就是把游戏里的动作实现从场记那里抽离出来，另外写一个动作管理器来提供接口进行对象动作的增删。
最开始的动作实现（仅以船为例，由于牧师与魔鬼是直接到船上的，所以没有位移）是通过在船上挂载一个处理鼠标点击的脚本(toSolveClick)和一个运动脚本(updateBoatMove)。
```
public boatController(string size){
		boat = Object.Instantiate (Resources.Load ("prefabs/boat", typeof(GameObject))
			, rightPos , Quaternion.identity, null) as GameObject ;
		boat.name = "boat";
		//给船对象添加两个脚本
		toSolveClick = boat.AddComponent (typeof(firstScenceSolveClick)) as firstScenceSolveClick;
		toSolveClick.setName (boat.name);
		updateBoatMove = boat.AddComponent (typeof(boatMoveBeahave)) as boatMoveBeahave;
		//
		defaultSize = size;
		this.size = defaultSize;
	}
```


当鼠标点击后，脚本通知场记，然后场记进行处理。
```
firstScenceUserAction action ;
void OnMouseDown(){
		if (action.getStatus() != "playing") {
			return;
		}
		else{
			if (characterName == "boat") {
				action.boatMove ();
			}
			else {
				action.getBoatOrGetShore (name);
			}
		}
	}
```

若符合开船条件就修改运动脚本中的目的地来实现运动。
```
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
			myBoat.move ( );
		}
    
    //myboat里的move函数，两端代码在不同的类中
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
```
这样的话，运动的代码是在场记中实现的，场记做的东西就有点太多了。


现在有了动作管理器，可以通过动作管理器给对象添加动作删除动作，而不是像之前那样一直挂着一个脚本，这样看起来也就更科学了。
逻辑依旧是点击船后，船上的脚本通知场记，场记判断是否能开船后通知船的控制器。
代码与之前不同的是不用再添加两个脚本到船上。
```
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
```
还修改了船的控制器中的move函数。
```
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
		//交由动作管理器添加动作
		actionManager.RunAction (boat , boatMove , null );
	}
```

动作和动作管理器的基类都是照搬老师代码，虽然这次的动作只有船的左右移动，并且也没有进行动作回调（ISSActionCallback），但是也还是能帮助加深对动作管理器的理解。
```
//动作基类
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

//动作管理器基类
public class SSActionManager : MonoBehaviour {
	private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
	private List<SSAction> waitingAdd = new List<SSAction>();
	private List<int> waitingDelete = new List<int>();

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	protected void Update()
	{
		foreach (SSAction ac in waitingAdd) actions[ac.GetInstanceID()] = ac;
		waitingAdd.Clear();

		foreach (KeyValuePair<int, SSAction> kv in actions)
		{
			SSAction ac = kv.Value;
			if (ac.destroy)
			{
				waitingDelete.Add(ac.GetInstanceID());
			}
			else if (ac.enable)
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

 [1]: https://github.com/ddghost/unity3d/blob/master/unity%20hw3/%E6%88%AA%E5%9B%BE/1.jpg
