using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 旋转 : MonoBehaviour {
	public GameObject mid;
	public float speed;
	public Vector3 angle;

	// Use this for initialization
	void Start () {
		speed = Random.Range (5f, 40f);
		angle = new Vector3 (0  , 1 , Random.Range(-0.3f , 0.3f) );
	}

	// Update is called once per frame
	void Update () {

		this.transform.RotateAround (mid.transform.position, angle, speed * Time.deltaTime);
	}
}
