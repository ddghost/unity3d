using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using baseCode;
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
