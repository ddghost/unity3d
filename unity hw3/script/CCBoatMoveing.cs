using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCBoatMoveing : SSAction {

	public Vector3 aim;
	public float speed;

	public override void Start(){
		
	}

	public static CCBoatMoveing GetSSAction(float speed = 10f){
		CCBoatMoveing action = ScriptableObject.CreateInstance<CCBoatMoveing> ();
		action.speed = speed;
		return action;
	}

	public override void Update(){
		gameobject.transform.position = Vector3.MoveTowards (gameobject.transform.position, aim , speed * Time.deltaTime);	
		if (this.transform.position == aim) {
			this.destroy = true;
			//this.callback.SSActionEvent (this);
		}
	}

	public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
		int intParam = 0, string strParam = null, Object objectParam = null){

	}
}
