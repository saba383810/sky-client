using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WarningDialogInit : Dialog {

	public string warning;
	public Text warningText;

	override public void OpenComplete()
	{
		base.OpenComplete ();
	}

	override public void Open(){
		if (_isOpened)
			return;
		base.Open ();
		//warningText.text = warning;
	}

}
