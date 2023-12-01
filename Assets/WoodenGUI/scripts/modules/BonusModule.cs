using UnityEngine;
using System.Collections;

public class BonusModule : MonoBehaviour {

	public void Show(int starsAmount){
		
	}
	public void Hide(){
		SwitchBlock[] stars = gameObject.GetComponentsInChildren<SwitchBlock> ();
		for (int i = 0; i < stars.Length; i++) {
			stars [i].enabled = false;
		}
	}
		
}
