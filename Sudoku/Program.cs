using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Input:");

            var board = new Board();

            for (var i = 0; i < 9; i++)
            {
                board.InputNumbers(i, Console.ReadLine());
            }

            while (!board.Accept)
            {
                board.SolveOnce();
                board.PutSolution(string.Format("step {0}:", board.Step));

                //System.Threading.Thread.Sleep(500);
            }

            Console.WriteLine();

            board.PutSolution("Solution:");
        }
    }
}
