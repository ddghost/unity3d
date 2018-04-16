using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
