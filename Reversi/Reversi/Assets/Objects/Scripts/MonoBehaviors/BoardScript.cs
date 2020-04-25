using Objects.Scripts;
using UnityEngine;

// #define DEBUG

public class BoardScript : MonoBehaviour
{
    public GameObject GamePiece;
    public GameObject OpponentAI;

    // ported variables
    private Board board;
            
    private bool playerTookTurn = false;
    private bool computerTookTurn = false;

    // required row position values go from 0:-7, but read values go from 7:0
    private readonly int[] _invertNegateRow = {-7, -6, -5, -4, -3, -2, -1, 0};
    
    // Start is called before the first frame update
    void Start()
    {
        Settings.currentPlayer = Settings.turnOrder == 0 ? Settings.PlayerName : Settings.ComputerName;
        board = new Board(8, Settings.currentPlayer);
        Helpers.InitBoard(GamePiece);
#if DEBUG
        board.ShowBoard();
#endif
    }
    private void Update()
    {
        if(Settings.makeComputerMove){
#if DEBUG
            print("Before Computer Move");
            board.ShowBoard();
#endif
            MakeComputerMove();
            Settings.makeComputerMove = false;
#if DEBUG
            print("After Computer Move");
            board.ShowBoard();
#endif
        }
    }

    private void OnMouseDown()
    {
        Settings.playerLosesDiscs = true;
        
        if (board.CurrentPlayer != Settings.PlayerName)
        {
            return;
        }
#if DEBUG
        print("Before Player Movve");
        board.ShowBoard();
#endif
        MakePlayerMove();
#if DEBUG
        print("After Player Move");
        board.ShowBoard();
#endif
    }

    private void MakePlayerMove()
    {
        ResetSkipping();

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
        Physics.Raycast(castPoint, out RaycastHit h);
        
        int tempRow = (int)(h.point.z-transform.position.z + 4);
        int renderedRow = _invertNegateRow[tempRow];
        int renderedCol = (int)(h.point.x-transform.position.x + 4);
        
#if DEBUG
        print("renderedRow: " + renderedRow + " renderedCol: " + renderedCol);
        print("gridRow: " + renderedRow * -1 + " gridCol: " + renderedCol);
#endif
        
        // create a new move
        Move newMove = new Move( renderedRow * -1, renderedCol);

        string errorMessage = board.MakeMove(newMove);
        if (errorMessage != null)
        {
            Settings.isInvalidMove = true;
            return;
        }

        Helpers.CreateDisc(GamePiece, newMove.Row, newMove.Col, true);
        board.UpdateUnityBoard();
        
        if (board.IsTerminal())
        {
            Helpers.EndGame();
        }
        
        if ((previousComputerScore - board.NumWhite) > 2)
        {
            Settings.playerGainsDiscs = true;
        }
    }

    private void ResetSkipping()
    {
        if (playerTookTurn && computerTookTurn)
        {
            playerTookTurn = false;
            computerTookTurn = false;
            board.PlayerSkippedTurn = false;
            board.ComputerSkippedTurn = false;
        }
    }

    private void MakeComputerMove()
    {
        ResetSkipping();
        
        int previousPlayerScore = board.NumBlack;
        computerTookTurn = true;

        int bestScore = 0;
        Move bestMove = null;
        
        // recursively call MinMax algorithm
        (bestScore, bestMove) = Helpers.Minimax(board, Settings.ComputerName, Settings.maxDepth, 0, 
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
        Helpers.CreateDisc(GamePiece, bestMove.Row, bestMove.Col);
        board.UpdateUnityBoard();
        
        if (board.IsTerminal())
        {
            Helpers.EndGame();
        }
        
        if ((previousPlayerScore - board.NumBlack) > 2)
        {
            Settings.playerLosesDiscs = true;
        }
    }
}
