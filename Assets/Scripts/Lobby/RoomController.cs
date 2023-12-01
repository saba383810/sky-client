using Common.Loading;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;
using UniRx;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] private CommonButton backButton;
    [SerializeField] private UserInformationView userInformationView;
    　void Start()
    {
        BGMManager.Instance.Play(BGMPath.ROOM);
        Bind();
        PlayerGenerate();
        userInformationView.Setup();
      
    }

     private void Bind()
     {
         backButton.OnClickOnce.TakeUntilDestroy(gameObject)
             .Subscribe(_ => RoomDisconnect());
     }

    private void PlayerGenerate()
    {
       GameObject.Find("BasicSpawner").GetComponent<BasicSpawner>().GeneratePlayer();
    }

    private async void RoomDisconnect()
    {
        var basicSpawner = GameObject.Find("BasicSpawner").GetComponent<BasicSpawner>();
        basicSpawner.LocalRunner.Despawn(basicSpawner.LocalPlayerObject);
        
        await basicSpawner.LocalRunner.Shutdown();
        LoadingManager.ChangeScene("OutGame");
    }

}
