using TMPro;
using UnityEngine;

namespace Objects.Scripts
{
    public class ScoreboardText : MonoBehaviour
    {
        private TMP_Text scoreText;

        void Start()
        {
            //Fetch the Text component from the GameObject
            scoreText = GetComponent<TMP_Text>();

            // current Scoreboard
            SetScoreText();
        }

        private void SetScoreText()
        {
            // current Scoreboard
            scoreText.text = "Player Score: " + Settings.PlayerScore + "\nComputer Score: " + Settings.ComputerScore;
        }

        private void Update()
        {
            SetScoreText();
        }
    }
}