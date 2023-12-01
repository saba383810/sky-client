using UnityEngine;
using Random = UnityEngine.Random;

namespace Gacha
{
    public class Gacha : MonoBehaviour
    {
        private Sprite[] _accImage;
        private Sprite[] _dressImages;
        private Sprite[] _eyesImage;
        
        private void Awake()
        {
            _accImage =  Resources.LoadAll<Sprite>("Sprites/DressUp/Acc");
            _dressImages =  Resources.LoadAll<Sprite>("Sprites/DressUp/Dress");
            _eyesImage = Resources.LoadAll<Sprite>("Sprites/DressUp/Eyes");
        }
        public Sprite DressGacha()
        {
            var randomValue = GetRandom((float)_dressImages.Length);
            Debug.Log("ガチャから入手したアイテムは"+_dressImages[randomValue]+"です！おめでとう！");
            return _dressImages[randomValue];
        }
        
        private int GetRandom(float max)
        {
            return (int)Random.Range(0, max);
        }
    }
}
