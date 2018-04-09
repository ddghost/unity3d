using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using baseCode;
public class CCActionManager : SSActionManager, ISSActionCallback
{
	//public firstSceneController sceneCtrl;

	void Awake(){
	
	}

	void Start()
	{

		//sceneCtrl = (firstSceneController)(Director.getInstance().CurrentScenceController);
		//sceneCtrl.actionManager = this;
	}
		

	public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
		int intParam = 0, string strParam = null, Object objectParam = null){

	}

}
