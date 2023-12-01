using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitScript : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = (GameObject)Resources.Load("Sprites/MiniGame/Fishing/BattleObject");
        GameObject obj2 = GameObject.Find("Uki(Clone)");
        StartCoroutine(action(obj, obj2));
    }

    // Update is called once per frame
    IEnumerator action(GameObject obj,GameObject obj2)
    {
        Destroy(obj2.gameObject);
        yield return new WaitForSeconds(2);
        Instantiate(obj, new Vector3(-0.3f, 3f, 0f), Quaternion.identity);
        Destroy(this.gameObject);
    }
}
