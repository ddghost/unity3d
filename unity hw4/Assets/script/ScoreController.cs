using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController{
	int score = 0 ;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void addScore(int lever ){
		score += lever;
	}

	public int getScore(){
		return score;
	}

	public void reset(){
		score = 0;
	}
}
