using System.Collections;
using TMPro;
using UnityEngine;

namespace Objects.Scripts
{
    public class AlertPanelController : MonoBehaviour
    {
        public TMP_Text alertText;
        public GameObject skipPanel;
        private IEnumerator coroutine;
       

        void Start()
        {
            skipPanel.SetActive(false);
            alertText.enabled = false;
        }

        private IEnumerator WaitAndClear(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            skipPanel.SetActive(false);
            alertText.enabled = false;
        }

        private void EnableAlertPanel(string newText)
        {
            alertText.text = newText;
            skipPanel.SetActive(true);
            alertText.enabled = true;
            StartCoroutine(coroutine);
        }

        private void Update()
        {
            // coroutine is destroyed after every frame
            coroutine = WaitAndClear(3.0f);
            
            if (Settings.playerSkippedTurn)
            {
                EnableAlertPanel("Player\nSkipped Turn");
                Settings.playerSkippedTurn = false;
            }

            if (Settings.computerSkippedTurn)
            {
                EnableAlertPanel("Computer\nSkipped Turn");
                Settings.computerSkippedTurn = false;
            }

            if (Settings.isInvalidMove)
            {
                EnableAlertPanel("Invalid Move");
                Settings.isInvalidMove = false;
            }
        }
    }
}