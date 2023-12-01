using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class Gyoei_Script : MonoBehaviour
{
    [SerializeField]bool ClearFlag = false;
    private bool FishingFlag = false,isFailed;
    private bool power = false;
    private Canvas _fish_failed_result;
    private float ran_x, ran_y;
    Rigidbody2D rbody;
    private int damage = 0,enemyHP= 0;
    private  bool isFishing = false;
    private string _point;
    [SerializeField] Slider _hpSplider;
    [SerializeField] Canvas _fish_result;
    // Start is called before the first frame update
    void Start()
    {
        rbody = this.GetComponent<Rigidbody2D>();
        _fish_result = (Canvas)Resources.Load("Sprites/MiniGame/Fishing/Canvas_Results", typeof(Canvas));
        _fish_failed_result = (Canvas)Resources.Load("Sprites/MiniGame/Fishing/Canvas_Results_Failed", typeof(Canvas));
        _hpSplider = GameObject.Find("/FishingDataCanvas/Slider").GetComponent<Slider>(); 
        StartCoroutine(RanHensu(ran_x, ran_y));
    }

    // Update is called once per frame

    IEnumerator RanHensu(float ran_x,float ran_y)
    {
        while (!(isFishing))
        {
            transform.DOMove(new Vector3(ran_x = Random.Range(-7, 8), ran_y = Random.Range(-7, 8), 1), 1.0f).SetLink(transform.parent.gameObject);
            yield return new WaitForSeconds(0.5f);
        }
        transform.DOMove(new Vector2(0, 20),
            1f).SetRelative(true);
        ClearFlag = true;
        yield return new WaitForSeconds(1.0f);
        Instantiate(_fish_result, new Vector3(0.0f, 0.0f, 0f), Quaternion.identity);
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        _point = collision.gameObject.tag;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            switch(_point)
            {
                case "SkyBluePoint":
                    damage = 100;
                    break;
                case "RedPoint":
                    damage = -30;
                    break;
                default:
                    break;
            }
            _hpSplider.value += damage;
        }
        if (_hpSplider.value >= 300)
        {
            // TODO
            if(!ClearFlag)
            {
                isFishing = true;
            }

        }else if(_hpSplider.value <= -100)
        {
            GameFailed();
        }
        else
        {
            _hpSplider.value -= 0.2f;
        }
    }    
    void GameFailed()
    {
        if (!isFailed)
        {
            Instantiate(_fish_failed_result, new Vector3(0.0f, 0.0f, 0f), Quaternion.identity); 
            Destroy(transform.parent.gameObject);
            isFailed = true;
        }
    }
}
