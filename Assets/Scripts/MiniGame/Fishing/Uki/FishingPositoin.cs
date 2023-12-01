using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FishingPositoin : MonoBehaviour
{
    private GameObject obj;
    private bool _ukiFlag;
    private GameObject target1;
    // Start is called before the first frame update
    void Start()
    {
        obj = (GameObject)Resources.Load("Sprites/MiniGame/Fishing/Uki");
        obj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!_ukiFlag)
            {
                UkiController();
                _ukiFlag = true;
            }
        }
    }
    void UkiController()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10.0f;
        var uki_obj = Instantiate(obj, Camera.main.ScreenToWorldPoint(mousePosition), Quaternion.identity); 
    }
}
