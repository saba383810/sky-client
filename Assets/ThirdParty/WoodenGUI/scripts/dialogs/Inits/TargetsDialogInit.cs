using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TargetsDialogInit : Dialog {

	public int level;
	public string targets;
	public int bonusA, bonusB, bonusC;
	public Text levelText;
	public Text targetsText;
	public Text bonusAValue, bonusBValue, bonusCValue;
	public Image bonusAIcon, bonusBIcon, bonusCIcon;
	public Bonus[] activeBonuses;


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
