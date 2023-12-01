using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class NEO_fishing_quantity : MonoBehaviour
{
    private int ObjCount;
    private GameObject Medium_fish;
    void Start()
    {
       Medium_fish = (GameObject)Resources.Load("Prefabs/Fishing/MediumFish");
    }

    // Update is called once per frame
    void Update()
    {
        ObjCount = this.transform.childCount;
        if (ObjCount <= 3)
        {
            FishCount();  
        }

    }

    private async UniTask FishCount()
    {
        var Fishobj = Instantiate(Medium_fish, new Vector2(0f,0f), Quaternion.identity);
        Fishobj.transform.parent = transform;
        await UniTask.Delay(1500);

    }
}
