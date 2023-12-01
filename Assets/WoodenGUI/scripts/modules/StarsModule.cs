using UnityEngine;
using System.Collections;

public class StarsModule : MonoBehaviour {

	public GameObject glowing;

	public void Show(int starsAmount){
		SwitchBlock[] stars = gameObject.GetComponentsInChildren<SwitchBlock> ();

		StartCoroutine (MyCoroutine (stars, starsAmount));

	}

	IEnumerator MyCoroutine(SwitchBlock[] star, int starsAmount)
	{
	for (int i = 0; i < starsAmount; i++) {
		star [i].enabled = true;
		yield return new WaitForSeconds(1);    
	}
		if (starsAmount == 3) glowing.SetActive (true);
	}
	public void Hide(){
		SwitchBlock[] stars = gameObject.GetComponentsInChildren<SwitchBlock> ();
		for (int i = 0; i < stars.Length; i++) {
			stars [i].enabled = false;
			if (stars.Length == 3) glowing.SetActive (false);
		}
	}
}
