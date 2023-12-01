using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemesChanger : MonoBehaviour
{
    public Color NormalColor;
    public Color HighlightColor;

    public List<GameObject> ThemesGOs;
    public List<Image> ThemesButtonImages;


    //Chooses the first theme as the default one
    private void Start()
    {
        ChooseTheme(0);
    }

    //Changes the displayed theme based on the clicked button
    public void ChooseTheme(int theme)
    {
        for (int i = 0; i < ThemesGOs.Count; i++)
        {
            ThemesGOs[i].SetActive(false);
            ThemesButtonImages[i].color = NormalColor;
        }
        ThemesGOs[theme].SetActive(true);
        ThemesButtonImages[theme].color = HighlightColor;
    }
}
