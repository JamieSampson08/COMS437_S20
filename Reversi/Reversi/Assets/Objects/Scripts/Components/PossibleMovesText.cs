using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Objects.Scripts
{
    public class PossibleMovesText : MonoBehaviour
    {
        private TMP_Text possibleMovesText;

        void Start()
        {
            //Fetch the Text component from the GameObject
            possibleMovesText = GetComponent<TMP_Text>();
        }

        private void SetPossibleMoves()
        {
            possibleMovesText.text = "Possible Moves";
            // (3,4)        (3,2)
            // 2 tabs between
            // ....... for more than 13 moves
        }
        
        private void Update()
        {
            SetPossibleMoves();
        }
    }
}