using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterDialogInit : Dialog {

    public int stars;
    public int score;
    public int time;
    public int level;
    public Text levelText;
    public Text scoreText;



    override public void OpenComplete()
    {
        base.OpenComplete();
    }

    override public void Open()
    {
        if (_isOpened)
            return;
        base.Open();

    }

}