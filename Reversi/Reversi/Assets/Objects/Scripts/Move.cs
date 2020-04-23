using System;

namespace Objects.Scripts
{
    public class Move
    {
        public int Col { get; }
        public int Row { get; }

        public int RenderedRow { get; set; }
        public int RenderedCol { get; set; }
        
        public DiscScript.PieceColor Color;
        
        public Move(int newRow, int newCol)
        {
            Col = newCol;
            Row = newRow;
        }

        public void SetRenderedPosition(int renRow, int renCol)
        {
            RenderedCol = renCol;
            RenderedRow = renRow;
        }

        /// <summary>
        /// Formats a move to show row, col values
        /// </summary>
        public void ToString()
        {
            Console.WriteLine("Move: (" + Row + "," + Col + ")");
        }
    }
}