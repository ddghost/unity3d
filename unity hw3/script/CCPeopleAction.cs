using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCPeopleMoveing : SSAction {

	public Vector3 aim;


	public override void Start(){
		
	}

	public static CCPeopleMoveing GetSSAction(Vector3 aim  ){
		CCPeopleMoveing action = ScriptableObject.CreateInstance<CCPeopleMoveing> ();
		action.aim = aim;

		return action;
	}

	public override void Update(){
		gameobject.transform.localPosition =  aim ;	
		this.destroy = true;
			//this.callback.SSActionEvent (this);
	
	}

	public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
		int intParam = 0, string strParam = null, Object objectParam = null){

	}
}
