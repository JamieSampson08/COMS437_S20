using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Objects.Scripts
{
    public static class Helpers
    {
        public static (int, Move) Minimax(Board board, string rootPlayer, int maxDepth, int currentDepth, int alpha,
                int beta)
            {
                Move bestMove = new Move(-1, -1);
                int currentScore = 0;
                Move currentMove = null;
        
                // check terminal state
                if (board.IsTerminal() || currentDepth == maxDepth)
                {
                    return (board.Evaluate(rootPlayer), null);
                }
        
                // decide if minimizing or maximizing
                int bestScore = (board.CurrentPlayer == rootPlayer) ? int.MinValue : int.MaxValue;
        
                foreach (Move m in board.GetPossibleMoves())
                {
                    // copy board so we can change values
                    Board newBoard = board.Copy();
                    newBoard.MakeMove(m, true);
        
                    (currentScore, currentMove) = Minimax(newBoard, rootPlayer, maxDepth, currentDepth + 1, alpha, beta);
        
                    // check for the player to see if should be maximizing or minimizing
                    if (board.CurrentPlayer == rootPlayer)
                    {
                        // maximizing
                        if (currentScore > bestScore)
                        {
                            bestMove = m;
                        }
        
                        bestScore = Math.Max(bestScore, currentScore);
                        alpha = Math.Max(alpha, bestScore);
                    }
                    else
                    {
                        // minimizing
                        if (currentScore < bestScore)
                        {
                            bestMove = m;
                        }
        
                        bestScore = Math.Min(bestScore, currentScore);
                        beta = Math.Min(beta, bestScore);
                    }
        
                    // prune branches
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
        
                return (bestScore, bestMove);
            }
        
        public static void EndGame()
        {
            SceneManager.LoadScene("GameOver");
        }
        
        public static void CreateDisc(GameObject givenGamePiece, int gridRow, int gridCol, bool flip = false)
        {
            GameObject gamePiece = GameObject.Instantiate(givenGamePiece);
            gamePiece.name = gridRow + "," + gridCol; // so we can query later to flip

            if (flip)
            {
                gamePiece.GetComponent<DiscScript>().Flip(DiscScript.PieceColor.WHITE);
            }
        
            gamePiece.GetComponent<Rigidbody>().position =
                new Vector3(gridCol, 8, gridRow * -1);
        }
        
        public static void InitBoard(GameObject givenGamePiece)
        {
            for (var r = 1; r <= 8; r++)
            {
                for (var c = 1; c <= 8; c++)
                {
                    // logic to setup init Piece locations
                    if ((r == 4 && c == 4) || (r == 5 && c == 5))
                    {
                        CreateDisc(givenGamePiece, r - 1, c - 1, true);
                    }
                    else if ((r == 4 && c == 5) || (r == 5 && c == 4))
                    {
                        CreateDisc(givenGamePiece, r - 1, c - 1);
                    }
                }
            }
        }
    }
    
   
}