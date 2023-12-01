using Event;
using Event.Hakoniwa;
using UnityEngine;

namespace Hakoniwa
{
    public class CameraMove : MonoBehaviour
    {
        [SerializeField]
        private ScreenInput _input;
        private float Speed = 0.02f;
        private bool MoveFlg;
        private Vector3 pos;
        private Camera _camera;
        private float StartFieldOfView;

        private void Start()
        {
            _input.SetPinchLimits(-20, 40, 0.03f);
            _camera = this.transform.GetComponent<Camera>();
            StartFieldOfView = _camera.fieldOfView;
        }

        // カメラ位置の更新
        void Update()
        {
            if (_input.GetNowSwipe() != ScreenInput.SwipeDirection.NONE && !_input.GetPinchFlg())
            {
                if (!MoveFlg) pos = this.transform.localPosition;
                MoveFlg = true;

                this.transform.localPosition = new Vector3(
                    pos.x - _input.GetSwipeRangeVec().x * Speed,
                    pos.y - _input.GetSwipeRangeVec().y * Speed,
                    pos.z);
                var changedPos = transform.localPosition;
                if (changedPos.x > 3) changedPos.x = 3;
                if (changedPos.x < -3) changedPos.x = -3;
                if (changedPos.y > 3) changedPos.y = 3;
                if (changedPos.y <-3) changedPos.y = -3;
                transform.localPosition = changedPos;
            }
            else
            {
                MoveFlg = false;
            }

            if (_input.GetPinchFlg())
            {
                _camera.fieldOfView = StartFieldOfView - _input.GetPinchDistance();
            }

        }
    }
}