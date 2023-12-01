using UnityEngine;
using UnityEngine.UI;

namespace Gacha
{
    public class GachaManager : MonoBehaviour
    {

        [SerializeField] private Transform gachaWindowParent;
        [SerializeField] private Button startGachaButton;
        private void Start()
        {
            var gachaAnimation = (GameObject)Resources.Load("Prefabs/Gacha/GachaAnimation");
            startGachaButton.onClick.AddListener((() => Instantiate(gachaAnimation, gachaWindowParent)));
        }
        
    }
}
