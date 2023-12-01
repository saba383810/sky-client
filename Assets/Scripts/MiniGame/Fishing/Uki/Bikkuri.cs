using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bikkuri : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = (GameObject)Resources.Load("Sprites/MiniGame/Fishing/HitObject");
        StartCoroutine(ButtonTap(obj));
    }

    // Update is called once per frame
    IEnumerator ButtonTap(GameObject obj)
    {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        Instantiate(obj, new Vector3(0f, 0f, 0f), Quaternion.identity);
        Debug.Log("canvas!");
    }
    
}
