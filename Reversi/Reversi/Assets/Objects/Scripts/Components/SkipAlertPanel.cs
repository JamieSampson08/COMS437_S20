using System;
using TMPro;
using UnityEngine;

namespace Objects.Scripts
{
    public class SkipAlertPanel : MonoBehaviour  // TODO 
    {
        private TMP_Text skippingPlayer;
        public GameObject alertPanel;

        private void Start()
        {
            alertPanel.SetActive(false);
            skippingPlayer.enabled = false;
        }

        private void Update()
        {
            alertPanel.SetActive(true);
            skippingPlayer.enabled = true;
            skippingPlayer.text = Settings.currentPlayer + " Skipping Turn";
        }
    }
}