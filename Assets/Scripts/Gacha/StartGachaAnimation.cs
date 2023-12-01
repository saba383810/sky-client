using Common.Loading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Gacha
{
    public class StartGachaAnimation: MonoBehaviour
    {
        [SerializeField] private Transform gachaWindowParent;
        [SerializeField] private Button startGachaButton,exitButton;
        private async void Start()
        {
            var gachaAnimation = (GameObject)Resources.Load("Prefabs/Gacha/GachaAnimation");
            startGachaButton.onClick.AddListener(() => Instantiate(gachaAnimation, gachaWindowParent));
            
            exitButton.onClick.AddListener(() =>LoadingManager.ChangeScene("OutGame"));
        }
    }
}
