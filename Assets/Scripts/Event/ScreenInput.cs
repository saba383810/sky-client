using UnityEngine;

namespace Event.Hakoniwa
{
    public class ScreenInput : MonoBehaviour
    {
        // フリック最小移動距離
        [SerializeField]
        private Vector2 FlickMinRange = new Vector2(30.0f,30.0f);
        // スワイプ最小移動距離
        [SerializeField]
        private Vector2 SwipeMinRange = new Vector2(50.0f,50.0f);
        // TAPをNONEに戻すまでのカウント
        [SerializeField]
        private int NoneCountMax = 2;
        private int NoneCountNow = 0;
        // スワイプ入力距離
        private Vector2 SwipeRange;
        // 入力方向記録用
        private Vector2 InputSTART;
        private Vector2 InputMOVE;
        private Vector2 InputEND;
        // フリックの方向
        public enum FlickDirection
        {
            NONE,
            TAP,
            UP,
            RIGHT,
            DOWN,
            LEFT,
            UP_LEFT,
            UP_RIGHT,
            DOWN_LEFT,
            DOWN_RIGHT
        }
        private FlickDirection NowFlick = FlickDirection.NONE;
        // スワイプの方向
        public enum SwipeDirection
        {
            NONE,
            TAP,
            UP,
            RIGHT,
            DOWN,
            LEFT,
            UP_LEFT,
            UP_RIGHT,
            DOWN_LEFT,
            DOWN_RIGHT
        }
        private SwipeDirection NowSwipe = SwipeDirection.NONE;


        // Update is called once per frame
        void Update()
        {
            GetInputVector();
            CheckPinch();
        }

        // 入力の取得
        private void GetInputVector()
        {
            // Unity上での操作取得
            if (Application.isEditor)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    InputSTART = Input.mousePosition;
                }
                else if (Input.GetMouseButton(0))
                {
                    InputMOVE = Input.mousePosition;
                    SwipeCLC();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    InputEND = Input.mousePosition;
                    FlickCLC();
                }
                else if(NowFlick != FlickDirection.NONE || NowSwipe != SwipeDirection.NONE)
                {
                    ResetParameter();
                }
            }
            // 端末上での操作取得
            else
            {
                if (Input.touchCount == 1)
                {
                    Touch touch = Input.touches[0];
                    if (touch.phase == TouchPhase.Began)
                    {
                        InputSTART = touch.position;
                    }
                    else if (touch.phase == TouchPhase.Moved)
                    {
                        InputMOVE = Input.mousePosition;
                        SwipeCLC();
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        InputEND = touch.position;
                        FlickCLC();
                    }
                }
                else if (NowFlick != FlickDirection.NONE || NowSwipe != SwipeDirection.NONE)
                {
                    ResetParameter();
                }
            }
        }

        // 入力内容からフリック方向を計算
        private void FlickCLC()
        {
            Vector2 _work = new Vector2((new Vector3(InputEND.x, 0, 0) - new Vector3(InputSTART.x, 0, 0)).magnitude, (new Vector3(0, InputEND.y, 0) - new Vector3(0, InputSTART.y, 0)).magnitude);

            if (_work.x <= FlickMinRange.x && _work.y <= FlickMinRange.y)
            {
                NowFlick = FlickDirection.TAP;
            }
            else
            {
                float _angle = Mathf.Atan2(InputEND.y - InputSTART.y, InputEND.x - InputSTART.x) * Mathf.Rad2Deg;

                if (-22.5f <= _angle && _angle < 22.5f) NowFlick = FlickDirection.RIGHT;
                else if (22.5f <= _angle && _angle < 67.5f) NowFlick = FlickDirection.UP_RIGHT;
                else if (67.5f <= _angle && _angle < 112.5f) NowFlick = FlickDirection.UP;
                else if (112.5f <= _angle && _angle < 157.5f) NowFlick = FlickDirection.UP_LEFT;
                else if (157.5f <= _angle || _angle < -157.5f) NowFlick = FlickDirection.LEFT;
                else if (-157.5f <= _angle && _angle < -112.5f) NowFlick = FlickDirection.DOWN_LEFT;
                else if (-112.5f <= _angle && _angle < -67.5f) NowFlick = FlickDirection.DOWN;
                else if (-67.5f <= _angle && _angle < -22.5f) NowFlick = FlickDirection.DOWN_RIGHT;
            }
        }

        // 入力内容からスワイプ方向を計算
        private void SwipeCLC()
        {
            SwipeRange = new Vector2((new Vector3(InputMOVE.x, 0, 0) - new Vector3(InputSTART.x, 0, 0)).magnitude, (new Vector3(0, InputMOVE.y, 0) - new Vector3(0, InputSTART.y, 0)).magnitude);

            if (SwipeRange.x <= SwipeMinRange.x && SwipeRange.y <= SwipeMinRange.y)
            {
                NowSwipe = SwipeDirection.TAP;
            }
            else 
            {
                float _angle = Mathf.Atan2(InputMOVE.y - InputSTART.y, InputMOVE.x - InputSTART.x) * Mathf.Rad2Deg;

                if(-22.5f <= _angle && _angle < 22.5f) NowSwipe = SwipeDirection.RIGHT;
                else if(22.5f <= _angle && _angle < 67.5f) NowSwipe = SwipeDirection.UP_RIGHT;
                else if (67.5f <= _angle && _angle < 112.5f) NowSwipe = SwipeDirection.UP;
                else if (112.5f <= _angle && _angle < 157.5f) NowSwipe = SwipeDirection.UP_LEFT;
                else if (157.5f <= _angle || _angle < -157.5f) NowSwipe = SwipeDirection.LEFT;
                else if (-157.5f <= _angle && _angle < -112.5f) NowSwipe = SwipeDirection.DOWN_LEFT;
                else if (-112.5f <= _angle && _angle < -67.5f) NowSwipe = SwipeDirection.DOWN;
                else if (-67.5f <= _angle && _angle < -22.5f) NowSwipe = SwipeDirection.DOWN_RIGHT;
            }
        }

        // NONEにリセット
        private void ResetParameter()
        {
            NoneCountNow++;
            if (NoneCountNow >= NoneCountMax)
            {
                NoneCountNow = 0;
                NowFlick = FlickDirection.NONE;
                NowSwipe = SwipeDirection.NONE;
                SwipeRange = new Vector2(0, 0);
            }
        }

        // フリック方向の取得
        public FlickDirection GetNowFlick()
        {
            return NowFlick;
        }

        // スワイプ方向の取得
        public SwipeDirection GetNowSwipe()
        {
            return NowSwipe;
        }

        // スワイプ量の取得
        public float GetSwipeRange()
        {
            if (SwipeRange.x > SwipeRange.y)
            {
                return SwipeRange.x;
            }
            else
            {
                return SwipeRange.y;
            }
        }

        // スワイプ量の取得
        public Vector2 GetSwipeRangeVec()
        {
            if (NowSwipe != SwipeDirection.NONE)
            {
                return new Vector2(InputMOVE.x - InputSTART.x, InputMOVE.y - InputSTART.y);
            }
            else
            {
                return new Vector2(0, 0);
            }
        }
    
        // ピンチイン・ピンチアウト
        private bool PinchFlg;
        private float PinchDistance;
        private float PinchSpeed = 0.03f;
        private float PinchMin = -20;
        private float PinchMax = 20;
        private Vector3 UnityPinchStartPos;
        private float StartDistance;
        private float PinchAdj;
        private void CheckPinch()
        {
            if (Input.touchCount == 2 || Input.GetMouseButtonDown(1))
            {
                Debug.Log("Click!");
                // Unity上での操作取得
                if (Application.isEditor)
                {
                    Debug.Log("Editor");
                    if (!PinchFlg) UnityPinchStartPos = Input.mousePosition;
                    PinchFlg = true;
                    if (UnityPinchStartPos.y < Input.mousePosition.y) PinchAdj = 1.0f;
                    else PinchAdj = -1.0f;
                    PinchDistance += Vector3.Distance(UnityPinchStartPos, Input.mousePosition) * PinchAdj * PinchSpeed * Time.deltaTime;
                    if (PinchDistance < PinchMin) PinchDistance = PinchMin;
                    else if (PinchMax < PinchDistance) PinchDistance = PinchMax;
                }
                // 端末上での操作取得
                else
                {
                    Debug.Log("iOS");
                    if (!PinchFlg) StartDistance = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
                    PinchFlg = true;
                    if (StartDistance < Vector2.Distance(Input.touches[0].position, Input.touches[1].position)) PinchAdj = 1.0f;
                    else PinchAdj = -1.0f;
                    PinchDistance += Vector2.Distance(Input.touches[0].position, Input.touches[1].position) * PinchAdj * PinchSpeed * Time.deltaTime;
                    if (PinchDistance < PinchMin) PinchDistance = PinchMin;
                    else if (PinchMax < PinchDistance) PinchDistance = PinchMax;
                }
            }
            else
            {
                PinchFlg = false;
            }
        }

// 制限設定
        public void SetPinchLimits(float _min, float _max, float _speed)
        {
            PinchMin = _min;
            PinchMax = _max;
            PinchSpeed = _speed;
        }

// フラグ取得
        public bool GetPinchFlg()
        {
            return PinchFlg;
        }

// 距離取得
        public float GetPinchDistance()
        {
            return PinchDistance;
        }
    
    }
}
