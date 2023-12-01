using UnityEngine;

public class DressUpSprite : MonoBehaviour
{
    [SerializeField] private SpriteRenderer hairImage = default;
    [SerializeField] private SpriteRenderer hairBackImage = default;
    [SerializeField] private SpriteRenderer dressImage = default;
    [SerializeField] private SpriteRenderer accImage = default;
    [Header("コードが自動でsetupをするかどうか")]
    [SerializeField] private bool isAutoSetUp = true;
    private const string DressUpPath = "Sprites/DressUp/";
    // Start is called before the first frame update
    private void Start()
    {
        if (!isAutoSetUp) return;
        //オートセットアップがオンなら
        var hairId = DataManager.Instance.PlayerDataManager.HairId;
        if (hairId == 0) hairId = 69;
        var dressId = DataManager.Instance.PlayerDataManager.DressId;
        if (dressId == 0) dressId = 83;
        var accId = DataManager.Instance.PlayerDataManager.AccId;
        if (accId == 0) accId = 51;
        HairChange(hairId);
        DressChange(dressId);
        AccChange(accId);
    }

    public void Setup(int hairId, int dressId, int accId)
    {
        HairChange(hairId);
        DressChange(dressId);
        AccChange(accId);
    }
    
    /// <summary>
    /// 髪型の変更を行う
    /// </summary>
    /// <param name="hairNum"></param>
    public void HairChange(int hairNum)
    {
        hairImage.sprite = Resources.Load<Sprite>($"{DressUpPath}Hair/{hairNum}");
        hairBackImage.sprite = Resources.Load<Sprite>($"{DressUpPath}Hair_Back/{hairNum}");
    }
    /// <summary>
    /// ドレスの変更を行う
    /// </summary>
    /// <param name="dressNum"></param>
    public void DressChange(int dressNum)
    {
        dressImage.sprite = Resources.Load<Sprite>($"{DressUpPath}Dress/{dressNum}");
    }
    /// <summary>
    /// アクセサリーの変更を行う
    /// </summary>
    /// <param name="accNum"></param>
    public void AccChange(int accNum)
    {
        accImage.sprite = Resources.Load<Sprite>($"{DressUpPath}Acc/{accNum}");
    }
}
