using TMPro;
using UnityEngine;

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

        // (3,4)        (3,2)
        // 2 tabs between
        // ....... for more than 13 moves
        private void SetPossibleMoves()
        {
            possibleMovesText.text = "Possible Moves\n";
            string listOfMoves = "";
            int count = 1;
            foreach (Move m in Settings.possibleMoves)
            {
                if (count > 6)
                {
                    // TODO - make a "show more" button, store index in possibleMoves to start at that point
                    listOfMoves += "..........";
                    break;
                }
                listOfMoves += "(" + m.Row + "," + m.Col + ")\n";
                count++;
            }
            
            possibleMovesText.text += listOfMoves;
            print(possibleMovesText.text);
        }

        private void Update()
        {
            SetPossibleMoves();
        }
    }
}