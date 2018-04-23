using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class physicActionManager : SSActionManager,ISSActionCallback {

	public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
		int intParam = 0, string strParam = null, Object objectParam = null){

	}
	//清楚所有action，
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
