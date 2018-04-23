using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using baseCode;
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

	public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager)
	{
		action.gameobject = gameobject;
		action.transform = gameobject.transform;
		action.callback = manager;
		waitingAdd.Add(action);
		action.Start();
	}

	public void reset(){
		actions.Clear ();
	}
}