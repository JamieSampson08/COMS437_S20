using System;
using System.IO;

namespace AI
{
    public class Exceptions
    {
        private static TextWriter errorWriter;
        private const ConsoleColor errorColor = ConsoleColor.DarkRed;

        public static void BaseError(string message, string expected)
        {
            Console.ForegroundColor = errorColor;
            errorWriter = Console.Error;
            errorWriter.WriteLine(message);
            errorWriter.WriteLine(expected);
        }

        public static void InvalidMove(Move move, string error)
        {
            errorWriter = Console.Error;
            Console.ForegroundColor = errorColor;
            errorWriter.WriteLine("Can not play the given move: " + move.Row + "," + move.Col);
            errorWriter.WriteLine(error);
        }
    }
}