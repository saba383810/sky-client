using Common.Loading;
using OutGame.Character;
using Title;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OutGame.WindowController
{
    public class MiniGameWindowController : MonoBehaviour
    {
        [SerializeField] private Button fishingButton = default;
        [SerializeField] private Button autoRunButton = default;
        [SerializeField] private Transform buttonGroupTransform = default;
        [SerializeField] private DressUpImage chara = default;
        [SerializeField] private FloatingIslandView floatingIslandView;
        private void Start()
        {
            chara.Setup();
            WindowControllerAnimation.SlideInAnimation(buttonGroupTransform);
            fishingButton.onClick.AddListener(()=>OnMiniGameButtonClicked(MiniGameScene.NeoFishing));
            autoRunButton.onClick.AddListener(()=>OnMiniGameButtonClicked(MiniGameScene.NeoAutoRun));
            floatingIslandView.Setup();
        }

        private enum MiniGameScene
        {
            NeoAutoRun,
            NeoFishing
        }
    
        private void OnMiniGameButtonClicked(MiniGameScene selectScene)
        {
            var sceneName = selectScene.ToString();
        
            //ロードアニメーション
            LoadingManager.ChangeScene(sceneName);
        }

    }
}
