using UnityEngine;
using UnityEngine.UI;

namespace Objects.Scripts
{
    public class CurrentTurnText : MonoBehaviour
    {
        private Text scoreText;

        void Start()
        {
            //Fetch the Text component from the GameObject
            scoreText = GetComponent<Text>();
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
                scoreText.text = "<-------";
            }
        }
    }
}