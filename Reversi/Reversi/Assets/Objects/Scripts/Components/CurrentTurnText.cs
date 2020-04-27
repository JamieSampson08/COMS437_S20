using TMPro;
using UnityEngine;

namespace Objects.Scripts
{
    public class CurrentTurnText : MonoBehaviour
    {
        private TMP_Text scoreText;

        void Start()
        {
            //Fetch the Text component from the GameObject
            scoreText = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            // turn arrow
            if (Settings.currentPlayer == Settings.ComputerName)
            {
                scoreText.text = "\n<-------";
            }
            else
            {
                scoreText.text = "<-------\n ";
            }
        }
    }
}