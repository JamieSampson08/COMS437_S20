using System;
using Objects.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardScript : MonoBehaviour
{
    public GameObject GamePiece;

    // ported variables
    private Board board;
    private String ROOT_PLAYER;
    int intPlayerTurn;
    bool playerTookTurn = false;
    bool computerTookTurn = false;
    private int maxDepth;

    // required row position values go from 0:-7, but read values go from 7:0
    private readonly int[] _invertNegateRow = {-7, -6, -5, -4, -3, -2, -1, 0};
    
    // Start is called before the first frame update
    void Start()
    {
        maxDepth = Settings.maxDepth;
        intPlayerTurn = Settings.turnOrder;

        ROOT_PLAYER = intPlayerTurn == 0 ? Settings.PlayerName : Settings.ComputerName;
        Settings.currentPlayer = ROOT_PLAYER;
        board = new Board(8, ROOT_PLAYER);
        InitBoard();
        board.ShowBoard();
    }
    private void Update()
    {
        if(Settings.makeComputerMove){
            print("Before Computer Move");
            board.ShowBoard();
            simulateHelper();
            MakeComputerMove();
            Settings.makeComputerMove = false;
            print("After Computer Move");
            board.ShowBoard();
        }
    }

    private void InitBoard()
    {
        for (var r = 1; r <= 8; r++)
        {
            for (var c = 1; c <= 8; c++)
            {
                // logic to setup init Piece locations
                if ((r == 4 && c == 4) || (r == 5 && c == 5))
                {
                    CreateDisc(r - 1, c - 1, true);
                }
                else if ((r == 4 && c == 5) || (r == 5 && c == 4))
                {
                    CreateDisc(r - 1, c - 1);
                }
            }
        }
    }

    private GameObject CreateDisc(int gridRow, int gridCol, bool flip = false)
    {
        GameObject gamePiece = Instantiate(GamePiece);
        gamePiece.name = gridRow + "," + gridCol; // so we can query later to flip

        if (flip)
        {
            gamePiece.GetComponent<DiscScript>().Flip(DiscScript.PieceColor.WHITE);
        }
        
        gamePiece.GetComponent<Rigidbody>().position =
            new Vector3(gridCol, 8, gridRow * -1);
        
        return gamePiece;
    }

    private void simulateHelper()
    {
        if (board.IsTerminal())
        {
            EndGame();
        }
        
        if (playerTookTurn && computerTookTurn)
        {
            playerTookTurn = false;
            computerTookTurn = false;
            board.PlayerSkippedTurn = false;
            board.ComputerSkippedTurn = false;
        }
    }
    
    private void OnMouseDown()
    {
        simulateHelper();

        if (board.CurrentPlayer != Settings.PlayerName)
        {
            return;  // clicking doesn't do anything
        }
        
        print("Before Player Movve");
        board.ShowBoard();
        MakePlayerMove();
        print("After Player Move");
        board.ShowBoard();
    }

    private void MakePlayerMove()
    {
        int previousComputerScore = board.NumWhite;
        playerTookTurn = true;
        
        // if no moves exist, set skip = true, else false
        board.PlayerSkippedTurn = board.PrintPossibleMoves();
        
        if (board.PlayerSkippedTurn)
        {
            print("No Moves Available. Skipping Turn.");
            board.SetupForNewTurn();
            computerTookTurn = false;
            return;
        }
        
        Vector3 mouse = Input.mousePosition;
        Ray castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit h;
        Physics.Raycast(castPoint, out h);
        
        int tempRow = (int)(h.point.z-transform.position.z + 4);
        int renderedRow = _invertNegateRow[tempRow];
        int renderedCol = (int)(h.point.x-transform.position.x + 4);
        
        print("renderedRow: " + renderedRow + " renderedCol: " + renderedCol);
        print("gridRow: " + renderedRow * -1 + " gridCol: " + renderedCol);
        
        // create a new move
        Move newMove = new Move( renderedRow * -1, renderedCol);

        string errorMessage = board.MakeMove(newMove);
        if (errorMessage != null)
        {
            // TODO - Post Error Message: "Invalid Move"
            return;
        }

        CreateDisc(newMove.Row, newMove.Col, true);
        
        if (previousComputerScore - board.NumWhite > 2)
        {
            Settings.playerGainsDiscs = true;
        }
    }

    private void EndGame()
    {
        SceneManager.LoadScene("GameOver"); // TODO - doesn't work???
    }
    
    private void MakeComputerMove()
    {
        int previousPlayerScore = board.NumBlack;
        computerTookTurn = true;

        int bestScore = 0;
        Move bestMove = null;
        
        // recursively call MinMax algorithm
        (bestScore, bestMove) = Minimax(board, Settings.ComputerName, maxDepth, 0, 
            int.MinValue, int.MaxValue);

        if (bestMove == null || bestMove.Col == -1 || bestMove.Row == -1)
        {
            print("No Moves Available. Skipping Turn.");
            board.ComputerSkippedTurn = true;
            playerTookTurn = false;
            board.SetupForNewTurn();
            return;
        }
        
        board.MakeMove(bestMove, true);
        CreateDisc(bestMove.Row, bestMove.Col);
        
        if (previousPlayerScore - board.NumBlack > 2)
        {
            Settings.playerLosesDiscs = true;
        }
    }
    
    private static (int, Move) Minimax(Board board, string rootPlayer, int maxDepth, int currentDepth, int alpha,
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
}
