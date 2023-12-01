using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class FishingDataScript : MonoBehaviour
{
    [SerializeReference] private TextMeshProUGUI score; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Fish_Result_Data._fishScoreSum<= 0)
        {
            score.text = "Score:0";
        }
        else
        {
            score.text = "Score:" + Fish_Result_Data._fishScoreSum.ToString(); ;
        }
    }
}
