using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikkuriDelete : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Delete");
    }

    // Update is called once per frame
    IEnumerator Delete()
    {
        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }    
        
}
