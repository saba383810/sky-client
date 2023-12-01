using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour
{
    private bool _isGenerateCommonScene = true;
    private void Start()
    {
        //CommonSceneが存在してなければ生成
        _isGenerateCommonScene = true;
        for (var i = 0; i < SceneManager.sceneCount ; i++) 
        {
            if (SceneManager.GetSceneAt(i).name == "Common") _isGenerateCommonScene = false;
        }
        if (_isGenerateCommonScene)
        {
            SceneManager.LoadScene("Common", LoadSceneMode.Additive);
        }
        Destroy(this.gameObject);
    }
}
