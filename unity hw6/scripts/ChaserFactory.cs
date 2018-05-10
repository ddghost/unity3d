using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserFactory: MonoBehaviour{

	public ChaserInfo getChaser(){
		return new ChaserInfo ();
	}
}
