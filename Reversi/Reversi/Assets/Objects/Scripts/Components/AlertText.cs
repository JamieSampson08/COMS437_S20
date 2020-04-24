using TMPro;
using UnityEngine;

namespace Objects.Scripts
{
    public class AlertText : MonoBehaviour
    {
        private GameObject board;
        private TMP_Text alertText;

        void Start()
        {
            //Fetch the Text component from the GameObject
            alertText = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            if (Settings.playerSkippedTurn)
            {
                alertText.text = "Player\nSkipped Turn";
            }

            if (Settings.computerSkippedTurn)
            {
                alertText.text = "Computer\nSkipped Turn";
            }

            if (Settings.isInvalidMove)
            {
                alertText.text = "Invalid Move";
            }
        }
    }
}