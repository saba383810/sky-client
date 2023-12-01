using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class Fish_Result_Data : MonoBehaviour
{
    private float _ranNum;
    public static int _fishScoreSum;
    [SerializeField]string FishName;
    [SerializeField]private TextMeshProUGUI _fishName;
    [SerializeField]private int FishScore;
    [SerializeField]private TextMeshProUGUI _fishScore;
    [SerializeField]private Image FishImage;

    // Start is called before the first frame update
    void Start()
    {
        _ranNum = Random.Range(0, 6);
        FishSelect();
    }

    // Update is called once per frame

    void FishSelect()
    {
        Debug.Log(FishImage);
        switch (_ranNum)//魚のデータ作成
        {
            case 1:
                FishImage.sprite = (Sprite)Resources.Load("Sprites/MiniGame/Fishing/Images/tai", typeof(Sprite));
                FishName = "タイ";
                FishScore = 5000;
                break;
            case 2:
                FishImage.sprite = (Sprite)Resources.Load("Sprites/MiniGame/Fishing/Images/isidai", typeof(Sprite));
                FishName = "イシダイ";
                FishScore = 2000;
                break;
            case 3:
                FishImage.sprite = (Sprite)Resources.Load("Sprites/MiniGame/Fishing/Images/ebi", typeof(Sprite));
                FishName = "エビ";
                FishScore = 3000;
                break;
            case 4:
                FishImage.sprite = (Sprite)Resources.Load("Sprites/MiniGame/Fishing/Images/amguro", typeof(Sprite));
                FishName = "マグロ";
                FishScore = 5000;
                break;
            default:
                FishImage.sprite = (Sprite)Resources.Load("Sprites/MiniGame/Fishing/Images/ika", typeof(Sprite));
                FishName = "イカ";
                FishScore = 500;
                break;
        }
        Debug.Log(FishImage.sprite);
        _fishName.text = FishName ;
        _fishScore.text = FishScore.ToString() ;
        _fishScoreSum += FishScore;

    }
    public void Retry()
    {
        SceneManager.LoadScene("Fishing");
    }
    
}

