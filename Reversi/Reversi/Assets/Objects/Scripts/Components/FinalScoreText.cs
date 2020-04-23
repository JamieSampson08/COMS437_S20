using System;
using UnityEngine;
using TMPro;

namespace Objects.Scripts
{
    public class FinalScoreText : MonoBehaviour
    {
        private TMP_Text scoreText;
        
        void Start()
        {
            //Fetch the Text component from the GameObject
            scoreText = GetComponent<TMP_Text>();

            String results = "";
            if (Settings.ComputerScore == Settings.PlayerScore)
            {
                results = "Game Was A Tie!";
            }
            else
            {
                string winner = Settings.PlayerScore > Settings.ComputerScore ? "Player Wins!" : "Computer Wins!";
                results = winner;
            }

            // Final Scoreboard
            results += "\n\nPlayer Score: " + Settings.PlayerScore + "\nComputer Score: " + Settings.ComputerScore;
            scoreText.text = results;
        }
    }
}