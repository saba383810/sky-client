using System;
using Common.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MiniGame.NeoAutoRun
{
    public class PauseController : MonoBehaviour
    {
        [SerializeField] private NeoAutoRunGameManager neoAutoRunGameManager;
        [SerializeField] private GameObject pauseObj;
        [SerializeField] private Button pauseOpenButton,restartButton,exitButton,pauseContinueButton,pauseStartButton,pauseExitButton;

        private void Awake()
        {
            pauseOpenButton.onClick.AddListener((() =>
            {
                var phase = neoAutoRunGameManager.GetAutoRunPhase();
                if(phase==NeoAutoRunGameManager.AutoRunPhase.NotStart) return;
                pauseObj.SetActive(true);
                neoAutoRunGameManager.PauseTime();
            }));
            pauseContinueButton.onClick.AddListener((() =>
            {
                pauseObj.SetActive(false);
                neoAutoRunGameManager.PauseTime();
            }));
            restartButton.onClick.AddListener((() => LoadingManager.ChangeScene($"NeoAutoRun")));
            exitButton.onClick.AddListener((() => LoadingManager.ChangeScene($"OutGame")));
            pauseStartButton.onClick.AddListener((() => LoadingManager.ChangeScene($"NeoAutoRun")));
            pauseExitButton.onClick.AddListener((() => LoadingManager.ChangeScene("OutGame")));
            
        }
    }
}
