using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskFactory: MonoBehaviour {
	private int allDiskNum = 0;
	private List<diskInfo> used = new List<diskInfo>();
	private List<diskInfo> free = new List<diskInfo>();

	public diskInfo getDisk(int lever ){
		diskInfo nowDisk = null;
		if (free.Count > 0) {
			nowDisk = free [0];
			nowDisk.reset (lever);
			used.Add (free [0]);
			free.Remove (free [0]);

		} else {
			allDiskNum++;
			nowDisk = new diskInfo (allDiskNum , lever);
			used.Add (nowDisk);
		}
			
		return nowDisk;

	}	

	public void reset(){
		foreach (diskInfo temp in used) {
			temp.disk.SetActive (false);
			free.Add (temp);
		}
		used.Clear ();
	}

	public int getNowUsedDisk(){
		return used.Count;
	}

	public void freeDisk(diskInfo diskinfo){
		if (used.Contains (diskinfo)) {
			diskinfo.disk.SetActive (false);
			used.Remove (diskinfo);
			free.Add (diskinfo);
		}

	}

	public diskInfo getHitDisk(GameObject disk){
		foreach (diskInfo i in used) {
			if (i.disk == disk) {
				return i;
			}

		}
		return null;
	} 
}
