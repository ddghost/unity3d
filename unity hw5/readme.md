# unity hw5
博客地址为https://blog.csdn.net/DDghsot/article/details/80053559

视频地址为http://v.youku.com/v_show/id_XMzU1OTMwNzA1Mg==.html?spm=a2hzp.8244740.0.0

根据老师的uml图，在之前的打飞碟游戏的代码的基础上进行修改

![这里写图片描述](https://img-blog.csdn.net/20180423172234981?watermark/2/text/aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L0REZ2hzb3Q=/font/5a6L5L2T/fontsize/400/fill/I0JBQkFCMA==/dissolve/70)

对场记的Start函数进行修改，场记中有一个bool变量决定是否使用物理系统，当为真的时候，动作管理器就使用物理系统，否则就使用原来的运动管理器。
```
// Use this for initialization
void Start () {
    diskFac = Singleton<DiskFactory>.Instance;
    if (ifPhysicManager) {
        actionManager = gameObject.AddComponent<physicActionManager>();
    } else {
        actionManager = gameObject.AddComponent<CCActionManager>();
    }
}
```
同时可以在场景中看到在main这个空对象可以选择是否使用物理运动

![这里写图片描述](https://img-blog.csdn.net/20180423172727274?watermark/2/text/aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L0REZ2hzb3Q=/font/5a6L5L2T/fontsize/400/fill/I0JBQkFCMA==/dissolve/70)

新添加一个物理运动的管理器,由于是物理运动，因此运动都是要使用FixedUpdate,那么物理运动的管理器也同样是在FixedUpdate的时候更新每一个运动，这一个物理运动的管理器与之前的运动管理器的代码基本上相似。
```
public class physicActionManager : SSActionManager,ISSActionCallback {

	public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
		int intParam = 0, string strParam = null, Object objectParam = null){

	}
	//清除所有action，
	public void reset(){
		actions.Clear ();
	}

	public void FixedUpdate(){
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
				ac.FixedUpdate();
			}
		}

		foreach (int key in waitingDelete)
		{
			SSAction ac = actions[key]; actions.Remove(key); DestroyObject(ac);
		}
		waitingDelete.Clear();
	}
}
```
没有直接新写一个物理运动类，只是改写了一下原来的运动类diskMove，当使用物理运动的时候，将随机一个运动的方向，同时新添加一个FixedUpdate的函数，用于物理运动，物理运动的时候，取消了重力，因为有重力的时候发现下坠太快了，尽可能把新增加的物理运动和原来的运动一样，因此取消了重力。而且因为原来的运动就是恒速的运动，因此没有给物体直接加一个恒力，而是给物体直接加一个速度的变化VelocityChange。
```
public class diskMove : SSAction {
	public Vector3 aim;
	public float speed;
	public diskInfo thisDisk;
	public float time = 0;
	public Vector3 dirction ;
	public override void Start () {
		
	}

	public static diskMove getDiskMove( diskInfo disk , int lever , bool ifPhysicManager){
		diskMove action = ScriptableObject.CreateInstance<diskMove> ();
		action.thisDisk = disk;
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

		if (!ifPhysicManager) {
			action.aim = new Vector3 (Random.Range (-2f, 2f), Random.Range (-2f, 2f), Random.Range (4f, 10f));

		} else {
			action.aim = Vector3.zero;
			float xPositionOfMax ,xPositionOfMin , yPositionOfMax , yPositionOfMin;
			if (disk.disk.transform.position.x > 0) {
				xPositionOfMax = 0.2f;
				xPositionOfMin = 0f;
			}
			else {
				xPositionOfMax = 0f;
				xPositionOfMin = -0.2f;
			}

			if (disk.disk.transform.position.y > 0) {
				yPositionOfMax = 0.2f;
				yPositionOfMin = 0f;
			} else {
				yPositionOfMax = 0f;
				yPositionOfMin = -0.2f;
			}
			action.dirction = new Vector3 (Random.Range (xPositionOfMin, xPositionOfMax)
				, Random.Range (yPositionOfMin, yPositionOfMax), Random.Range (0.2f, 1f));
			Rigidbody rigid = action.thisDisk.disk.GetComponent<Rigidbody> ();
			rigid.AddForce (action.dirction * action.speed , ForceMode.VelocityChange);
			rigid.useGravity = false;
		}
		return action;
	}

	// Update is called once per frame
	public override void Update () {
		gameobject.transform.position = Vector3.MoveTowards (gameobject.transform.position, aim, speed * Time.deltaTime);

		if (this.transform.position == aim) {
			this.destroy = true;
			this.enable = false;
			Singleton<DiskFactory>.Instance.freeDisk (thisDisk);

		}
	}
	//
	public override void FixedUpdate(){
		time += Time.fixedDeltaTime;
		if (time > 2) {
			time = 0;
			this.destroy = true;
			this.destroy = false;
			Singleton<DiskFactory>.Instance.freeDisk (thisDisk);
		}
	}
}
```
同时如果要用物理运动管理器的话，物理运动是要加刚体的，因此在飞碟的类diskInfo初始化要加刚体。
```
public class diskInfo{
	public GameObject disk;
	public int diskId;
	public int lever;

	public diskInfo(int id , int lever ,bool ifPhysicManager){
		this.diskId = id;
		disk = GameObject.Instantiate(Resources.Load<GameObject>("prefabs/disk" ), Vector3.zero , Quaternion.identity);
		disk.name = "disk" + id.ToString ();
		if (ifPhysicManager && disk.GetComponents<Rigidbody>() != null ) {
			disk.AddComponent<Rigidbody> ();
		}
		reset (lever);
	}

	public void reset(int lever){
		disk.transform.rotation = Quaternion.identity;
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
跟随者老师的模式设计学习后，再添加新的东西修改不用很多，也就修改几个类，以前写过的几个类也不用做大的修改，就能轻松添加新的功能，由此可见好的设计模式对于程序的扩展是非常重要的。
