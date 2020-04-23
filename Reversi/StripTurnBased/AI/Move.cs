using System;

namespace AI
{
    public class Move
    {
        public int Col { get; }
        public int Row { get; }
        
        public Move(int newRow, int newCol)
        {
            Col = newCol;
            Row = newRow;
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