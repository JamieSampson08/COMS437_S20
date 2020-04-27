using System.Collections;

namespace Objects.Scripts
{
    public class Settings
    {
        static public string ComputerName = "COMPUTER"; // white pieces
        static public string PlayerName = "PLAYER"; // black pieces
        
        static public int PlayerScore = 2;
        static public int ComputerScore = 2;

        static public int turnOrder = 0; // defaults to player going first
        static public int maxDepth = 1; // defaults to level 1
        static public string currentPlayer;

        // trigger character animations
        static public bool playerLosesDiscs;
        static public bool playerGainsDiscs;
        
        static public bool makeComputerMove;

        // needed for alert panel
        static public bool isInvalidMove;
        static public bool playerSkippedTurn;
        static public bool computerSkippedTurn;

        static public ArrayList possibleMoves;
    }
}