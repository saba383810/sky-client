using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClearDialogInit : Dialog {

	public int stars;
	public int time;
	public int level;
	public Text levelText;

	override public void OpenComplete()
	{
		base.OpenComplete ();
	}

	override public void Open(){
		if (_isOpened)
			return;
		base.Open ();

	}

}
