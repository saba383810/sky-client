using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiniGame.AutoRun
{
    public class ReStartScript : MonoBehaviour
    {
        public void AutoRunReStartButtonOnClick()
        {
            SceneManager.LoadScene($"AutoRun");
        }
    }
}
