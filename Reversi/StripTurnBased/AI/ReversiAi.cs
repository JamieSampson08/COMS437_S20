using System;
using System.Threading;

namespace AI
{
    class ReversiAi
    {
        public const string ComputerName = "COMPUTER"; // white pieces
        public const string PlayerName = "PLAYER"; // black pieces

        static void Main(string[] args)
        {
            int maxDepth = 0;

            while (true)
            {

                // request game difficulty level (ie. max depth of algorithm look ahead)
                Console.Write("Select Game Level Difficulty (1:easy - 10:hard): ");
                string strMaxDepth = Console.ReadLine();

                // error handling invalid number entered
                if (!int.TryParse(strMaxDepth, out maxDepth) || maxDepth < 1 || maxDepth > 10)
                {
                    var message = "Invalid level difficulty. Given: `" + strMaxDepth + "`";
                    var expected = "Expecting an integer between 1 and 10.";
                    Exceptions.BaseError(message, expected);
                    continue;
                    ;
                }

                break;
            }

            string ROOT_PLAYER = null;

            while (true)
            {

                // request if player wants to go first or second
                Console.Write("Would you like to go first (0) or second (1)?: ");
                string strTurnOrder = Console.ReadLine();

                // error handling invalid number entered
                if (!int.TryParse(strTurnOrder, out int intPlayerTurn) || (intPlayerTurn != 0 && intPlayerTurn != 1))
                {
                    var message = "Invalid turn selection. Given: `" + strTurnOrder + "`";
                    var expected = "Expecting an integer 0 or 1";
                    Exceptions.BaseError(message, expected);
                    continue;
                }

                // player is constant (ie. the top tier player)
                ROOT_PLAYER = intPlayerTurn == 0 ? PlayerName : ComputerName;
                break;
            }

            // create the game board
            Board board = new Board(8, ROOT_PLAYER);
            Console.WriteLine();

            bool playerTookTurn = false;
            bool computerTookTurn = false;

            // while loop for turn exchanges
            while (!board.IsTerminal())
            {
                if (playerTookTurn && computerTookTurn)
                {
                    playerTookTurn = false;
                    computerTookTurn = false;
                    board.PlayerSkippedTurn = false;
                    board.ComputerSkippedTurn = false;
                }

                Console.WriteLine("Computer: " + board.NumWhite);
                Console.WriteLine("Player: " + board.NumBlack);

                // reset values upon new turn pair
                int bestScore = 0;
                Move bestMove = null;

                // simulate player's turn
                if (board.CurrentPlayer == PlayerName)
                {
                    playerTookTurn = true;
                    Console.WriteLine("Player's Turn");

                    // if no moves exist, set skip = true, else false
                    board.PlayerSkippedTurn = board.PrintPossibleMoves();

                    if (board.PlayerSkippedTurn)
                    {
                        Console.WriteLine("No Moves Available. Skipping Turn.");
                        board.SetupForNewTurn();
                        computerTookTurn = false;
                        continue;
                    }

                    string strMove;

                    while (true)
                    {
                        Console.Write("Enter move in the form `Row,Col`: ");
                        strMove = Console.ReadLine();
                        string[] splitMove = strMove.Split(',');

                        if (splitMove.Length != 2 || !int.TryParse(splitMove[0], out int row) ||
                            !int.TryParse(splitMove[1], out int col))
                        {
                            var message = "Invalid move entered:";
                            var expected = "Expecting a move in the form `Row, Col`";
                            Exceptions.BaseError(message, expected);
                            continue;
                        }

                        // create a new move and attempt to make it
                        Move newMove = new Move(row, col);
                        string errorMessage = board.MakeMove(newMove);
                        if (errorMessage != null)
                        {
                            // if fails, posts a message and repeats this while loop until valid move is made
                            Exceptions.InvalidMove(newMove, errorMessage);
                            continue;
                        }

                        break;
                    }
                }
                // simulates computer's turn
                else
                {
                    computerTookTurn = true;

                    Console.WriteLine("Computer's Turn");

                    // recursively call MinMax algorithm
                    (bestScore, bestMove) = Minimax(board, ComputerName, maxDepth, 0, int.MinValue, int.MaxValue);

                    if (bestMove == null || bestMove.Col == -1 || bestMove.Row == -1)
                    {
                        Console.WriteLine("No Moves Available. Skipping Turn.");
                        board.ComputerSkippedTurn = true;
                        playerTookTurn = false;
                        board.SetupForNewTurn();
                        continue;
                    }

                    // mimic thinking for AI
                    Thread.Sleep(3000);
                    board.MakeMove(bestMove, true);
                }

                board.ShowBoard();
            }

            // winner handling
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            if (board.NumBlack == board.NumWhite)
            {
                Console.WriteLine("\nGame Was A Tie!");
            }
            else
            {
                string winner = board.NumBlack > board.NumWhite ? "Player Wins!" : "Computer Wins!";
                Console.WriteLine("\n" + winner);
            }

            // Final Scoreboard
            Console.WriteLine("Player Score: " + board.NumBlack);
            Console.WriteLine("Computer Score: " + board.NumWhite);
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
}