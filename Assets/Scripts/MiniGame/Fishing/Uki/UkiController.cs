using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UkiController : MonoBehaviour
{
    private Canvas _fish_failed_result;
    enum FishStatus
    {
        NomalState,
        HitState,
    }

    enum UkiStatus
    {
        succes,
        failed,
    }

    
    //FishStatus _correntStatus = FishStatus.NomalStatus;
    FishStatus _correntStatus = FishStatus.HitState;
    UkiStatus _ukiStatus = UkiStatus.succes;

    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = (GameObject)Resources.Load("Sprites/MiniGame/Fishing/UkiBikkuri");
        _fish_failed_result = (Canvas)Resources.Load("Sprites/MiniGame/Fishing/Canvas_Results_Failed", typeof(Canvas));
        StartCoroutine(HitStatus(obj));
        StartCoroutine("BadTiming");

    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Fish")
        {
            _correntStatus = FishStatus.HitState;
        }
            
    }
    IEnumerator BadTiming()
    {
        yield return new WaitUntil(() => _ukiStatus == UkiStatus.failed && Input.GetMouseButtonDown(0));
        Instantiate(_fish_failed_result, new Vector3(0.0f, 0.0f, 0f), Quaternion.identity);    
        Destroy(this.gameObject);
    }

    IEnumerator HitStatus(GameObject obj)
    {

        yield return new WaitUntil(() => _correntStatus == FishStatus.HitState);

        while(_correntStatus == FishStatus.HitState) 
        {
            _ukiStatus = UkiStatus.failed;
            int rnd = Random.Range(0,10);
            Debug.Log(rnd);
            if(rnd > 4)
            {
                Instantiate(obj, new Vector3(-7f, 3f, 0f), Quaternion.identity);
                _ukiStatus = UkiStatus.succes;
            }

            yield return new WaitForSeconds(1f);

        }
        
    }

}
