using System;
using System.Collections;
using System.Collections.Generic;

namespace AI
{
    public class Board
    {
        private const string BlackPiece = "O"; // player pieces
        private const string WhitePiece = "Q"; // computer pieces
        private const string BlankSpace = ".";
        private const string PossiblePiece = "?";

        public bool PlayerSkippedTurn = false;
        public bool ComputerSkippedTurn = false;

        public string CurrentPlayer { get; set; }

        // score tracking
        public int NumWhite = 2;
        public int NumBlack = 2;

        private string[,] _board;
        private readonly int _size;

        // evaluation const values
        private const int CornerWeights = 4;
        private const int EdgeWeights = 2;

        // possible moves to be played on board
        private ArrayList _moves;

        // key, value = Row, Col
        // used to check all directions from x position
        private readonly Dictionary<string, int[]> _checkDirections = new Dictionary<string, int[]>()
        {
            {"left", new[] {0, -1}}, // left
            {"right", new[] {0, 1}}, // right
            {"up", new[] {-1, 0}}, // up
            {"down", new[] {1, 0}}, // down
            {"topRight", new[] {-1, 1}}, // topRight
            {"topLeft", new[] {-1, -1}}, // topLeft
            {"botRight", new[] {1, 1}}, // botRight
            {"botLeft", new[] {1, -1}}, // botLeft
        };

        private string GetPlayerPiece(string player)
        {
            if (player == ReversiAi.ComputerName)
            {
                return WhitePiece;
            }

            return BlackPiece;
        }

        /// <summary>
        /// Board constructor  
        /// </summary>
        /// <param name="size">size X size board to create</param>
        /// <param name="player">root player on the board</param>
        public Board(int size, string player)
        {
            if (player == null)
            {
                var message = "Board failed to be initialized.";
                var expected = "Expecting a non null player.";
                Exceptions.BaseError(message, expected);
                Environment.Exit(1);
            }

            _size = size;
            InitBoard();
            CurrentPlayer = player;
            _moves = GetPossibleMoves();
        }


        private void InitBoard()
        {
            _board = new string[_size, _size];
            for (var r = 1; r <= _size; r++)
            {
                for (var c = 1; c <= _size; c++)
                {
                    // logic to setup init Piece locations
                    if ((r == 4 && c == 4) || (r == 5 && c == 5))
                    {
                        _board[r - 1, c - 1] = BlackPiece;
                    }
                    else if ((r == 4 && c == 5) || (r == 5 && c == 4))
                    {
                        _board[r - 1, c - 1] = WhitePiece;
                    }
                    else
                    {
                        _board[r - 1, c - 1] = BlankSpace;
                    }
                }
            }
        }// CONVERTED


        public void ShowBoard()
        {
            Console.WriteLine();
            // adds the possible move "?" icons
            ApplyPossibleMoves();


            int[] numbers = {0, 1, 2, 3, 4, 5, 6, 7};
            for (var r = 0; r <= _size; r++)
            {
                for (var c = 0; c <= _size; c++)
                {
                    // some super wonky code to add numbers to the rows and columns
                    if (r == 0 && c == 0)
                    {
                        Console.Write("  ");
                    }
                    else if (r == 0 && c != _size)
                    {
                        Console.Write(numbers[c - 1] + " ");
                    }
                    else if (c == 0)
                    {
                        Console.Write(numbers[r - 1] + " ");
                    }
                    else if (r == 0)
                    {
                        Console.Write(numbers[c - 1] + " ");
                    }
                    else
                    {
                        // print the actual value of the board
                        string piece = _board[r - 1, c - 1];
                        Console.Write(piece + " ");
                    }
                }

                Console.WriteLine();
            }
        }


        public Board Copy()
        {
            Board newBoard = new Board(_size, CurrentPlayer);
            newBoard._board = (string[,]) _board.Clone();
            newBoard.NumBlack = NumBlack;
            newBoard.NumWhite = NumWhite;
            newBoard._moves = _moves;
            return newBoard;
        }

        public bool IsTerminal()
        {
            // max number of pieces OR no possible moves left
            if ((NumBlack + NumWhite) == 64 || NumBlack == 0 || NumWhite == 0 ||
                (PlayerSkippedTurn && ComputerSkippedTurn))
            {
                return true;
            }

            return false;
        }


        private void ApplyPossibleMoves(bool remove = false)
        {
            foreach (Move m in _moves)
            {
                if (_board[m.Row, m.Col] == PossiblePiece || _board[m.Row, m.Col] == BlankSpace)
                {
                    if (remove)
                    {
                        _board[m.Row, m.Col] = BlankSpace;
                    }
                    else
                    {
                        _board[m.Row, m.Col] = PossiblePiece;
                    }
                }
            }

            ;
        }


        public int Evaluate(string player)
        {
            string playerPiece = GetPlayerPiece(player);
            string opponent = WhitePiece == playerPiece ? BlackPiece : WhitePiece;

            int weightedScore = 0;
            int opponentScore = 0;
            for (var r = 0; r < _size; r++)
            {
                for (var c = 0; c < _size; c++)
                {
                    string currentPiece = _board[r, c];
                    if (currentPiece == playerPiece)
                    {
                        if ((c == 0 && r == 0) || (c == (_size - 1) && r == (_size - 1)))
                        {
                            weightedScore += CornerWeights;
                        }
                        else if (r == 0 || r == (_size - 1) || c == 0 || c == (_size - 1))
                        {
                            weightedScore += EdgeWeights;
                        }
                        else
                        {
                            weightedScore++;
                        }
                    }
                    else if (currentPiece == opponent)
                    {
                        opponentScore++;
                    }
                }
            }

            return weightedScore - opponentScore;
        }

        public bool PrintPossibleMoves()
        {
            // if no moves are possible
            if (_moves.Count == 0)
            {
                return true;
            }

            foreach (Move m in _moves)
            {
                m.ToString();
            }

            return false;
        }


        public string MakeMove(Move move, bool skipValidation = false)
        {
            // don't need to validate if computer is attempting to make move
            if (!skipValidation)
            {
                string message = IsValidMove(move);
                if (message != null)
                {
                    return message;
                }
            }

            // set piece for move
            string piece = GetPlayerPiece(CurrentPlayer);
            _board[move.Row, move.Col] = piece;

            // handle increase in num of pieces
            if (piece == WhitePiece)
            {
                NumWhite++;
            }
            else
            {
                NumBlack++;
            }

            // flip pieces
            FlipPieces(move);

            SetupForNewTurn();

            // no errors
            return null;
        }

        public void SetupForNewTurn()
        {
            // switch players
            CurrentPlayer = CurrentPlayer == ReversiAi.PlayerName ? ReversiAi.ComputerName : ReversiAi.PlayerName;

            // remove possible moves
            ApplyPossibleMoves(true);

            // get new possible moves
            _moves = GetPossibleMoves();
        }

        private string IsValidMove(Move move)
        {
            // check out of bounds
            if (move.Col < 0 || move.Col > 7 || move.Row < 0 || move.Row > 7)
            {
                return "Col or Row is out of bounds.";
            }

            // check empty location
            if (_board[move.Row, move.Col] == BlackPiece || _board[move.Row, move.Col] == WhitePiece)
            {
                return "Location of the desired move is not empty.";
            }

            // check given move exists in possible moves on the board
            foreach (Move m in _moves)
            {
                if (m.Col == move.Col && m.Row == move.Row)
                {
                    // move found in possible moves
                    return null;
                }
            }

            return "Invalid Move.";
        }

        private void FlipPieces(Move move)
        {
            // grossly duplicate logic, but atm it works...
            foreach (var item in _checkDirections)
            {
                int[] dir = item.Value;
                int tempRow = dir[0];
                int tempCol = dir[1];
                // calls the same method as before, but passes in flip = true
                GetMovesInDirection(move, tempRow, tempCol, null, true);
            }
        }


        public ArrayList GetPossibleMoves()
        {
            ArrayList boardMoves = new ArrayList();

            // traverse all positions on board
            for (int r = 0; r < _size; r++)
            {
                for (int c = 0; c < _size; c++)
                {
                    string currentLocation = _board[r, c];
                    if (currentLocation != GetPlayerPiece(CurrentPlayer))
                    {
                        // only look at locations that have the player's piece 
                        continue;
                    }

                    // make a move out of player's piece location
                    Move currentMove = new Move(r, c);

                    // look in all 8 directions for a valid move
                    foreach (var item in _checkDirections)
                    {
                        int[] dir = item.Value;
                        int tempRow = dir[0];
                        int tempCol = dir[1];

                        Move moveToAdd = GetMovesInDirection(currentMove, tempRow, tempCol, boardMoves);

                        if (moveToAdd != null)
                        {
                            // if a move was found, add to list
                            boardMoves.Add(moveToAdd);
                        }
                    }
                }
            }

            return boardMoves;
        }


        private Move GetMovesInDirection(Move move, int rowDir, int colDir, ArrayList foundMoves, bool flip = false)
        {
            Move possibleMove = null;

            // new possible board move space
            int tempRow = move.Row + rowDir;
            int tempCol = move.Col + colDir;

            bool foundOpponent = false;
            ArrayList movesToFlip = new ArrayList();

            string opponentPiece = GetPlayerPiece(CurrentPlayer) == WhitePiece ? BlackPiece : WhitePiece;

            string locPiece = "";

            while (locPiece != BlankSpace && locPiece != PossiblePiece)
            {
                // check out of bounds
                if (tempCol < 0 || tempCol > 7 || tempRow < 0 || tempRow > 7)
                {
                    return null;
                }

                // get current Piece
                locPiece = _board[tempRow, tempCol];

                // if the Piece is the player's Piece, it will be evaluated later
                if (locPiece == GetPlayerPiece(CurrentPlayer))
                {
                    // if you are flipping pieces and you've found an opponent's piece
                    if (flip && foundOpponent)
                    {
                        // flip all the moves
                        foreach (Move m in movesToFlip)
                        {
                            _board[m.Row, m.Col] = GetPlayerPiece(CurrentPlayer);
                        }

                        // handle num pieces on board changes
                        if (opponentPiece == WhitePiece)
                        {
                            NumWhite -= movesToFlip.Count;
                            NumBlack += movesToFlip.Count;
                        }
                        else
                        {
                            NumBlack -= movesToFlip.Count;
                            NumWhite += movesToFlip.Count;
                        }
                    }

                    return null;
                }

                if (locPiece != BlankSpace && locPiece != PossiblePiece)
                {
                    // must find opponent's piece to be valid possible move
                    foundOpponent = true;

                    // if we will eventually be flipping pieces, add to list
                    if (flip)
                    {
                        movesToFlip.Add(new Move(tempRow, tempCol));
                    }

                    // increment Row Col 
                    tempRow += rowDir;
                    tempCol += colDir;
                }
            }

            if (foundOpponent)
            {
                // logic so that a duplicate move isn't added to the list
                bool moveExists = false;
                if (foundMoves != null)
                {
                    foreach (Move m in foundMoves)
                    {
                        if (m.Col == tempCol && m.Row == tempRow)
                        {
                            moveExists = true;
                            break;
                        }
                    }
                }

                // if the move wasn't found in the existing moves, then add to returned list
                if (!moveExists)
                {
                    possibleMove = new Move(tempRow, tempCol);
                }
            }

            return possibleMove;
        }
    }
}