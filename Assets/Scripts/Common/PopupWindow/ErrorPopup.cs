using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Common.PopupWindow
{
    public class ErrorPopup : Popup
    {
        [SerializeField] private TMP_Text errorText = default;
        public void Setup(string error)
        {
            base.Setup();
            errorText.text = error;
            Show();
        }
    }
}
