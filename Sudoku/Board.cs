using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku
{
    class Board
    {
        private Node[,] _board;
        private PriorityQueue<Node> _queue;

        public Board()
        {
            _board = new Node[9, 9];
            _queue = new PriorityQueue<Node>();
            ByOtherDetermineChanged = true;
        }

        public List<int[]> GetEffectivePositions(int x, int y)
        {
            var e = new List<int[]>();
            int cx = x;
            int cy = y;
            RewritePosition(ref cx, ref cy);

            //横ラインのサーチ
            for (int i = 0; i < 9; i += 3)
            {
                if (i == cx) continue;
                for (int j = 0; j < 3; j++)
                {
                    e.Add(new int[] { i + j, y });
                }
            }

            //縦ラインのサーチ
            for (int i = 0; i < 9; i += 3)
            {
                if (i == cy) continue;
                for (int j = 0; j < 3; j++)
                {
                    e.Add(new int[] { x, i + j });
                }
            }

            //ボックスのサーチ
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    if (cx + i == x && cy + j == y) continue;
                    e.Add(new int[] { cx + i, cy + j });
                }

            return e;
        }

        public List<int[]> GetHorizonalPositions(int x, int y)
        {
            var e = new List<int[]>();

            //横ラインのサーチ
            for (int i = 0; i < 9; i++)
            {
                if (i == x) continue;
                e.Add(new int[] { i, y });
            }

            return e;
        }

        public List<int[]> GetVerticalPositions(int x, int y)
        {
            var e = new List<int[]>();

            //縦ラインのサーチ
            for (int i = 0; i < 9; i++)
            {
                if (i == y) continue;
                e.Add(new int[] { x, i });
            }

            return e;
        }

        public List<int[]> GetBoxPositions(int x, int y)
        {
            var e = new List<int[]>();

            int cx = x;
            int cy = y;
            RewritePosition(ref cx, ref cy);

            //ボックスのサーチ
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    if (cx + i == x && cy + j == y) continue;
                    e.Add(new int[] { cx + i, cy + j });
                }

            return e;
        }

        private void RewritePosition(ref int x, ref int y)
        {
            x = (x / 3) * 3;
            y = (y / 3) * 3;
        }

        internal void InputNumbers(int i, string p)
        {
            var ca = p.ToCharArray();
            for (int j = 0; j < 9; j++)
            {
                _board[i, j] = new Node(ca[j]);
                _board[i, j].X = i;
                _board[i, j].Y = j;
                _queue.Push(_board[i, j]);
            }
        }

        public bool Accept
        {
            get { return _queue.Count == 0 || Solved; }
        }

        private bool Solved
        {
            get;
            set;
        }

        private bool ByOtherDetermineChanged
        {
            get;
            set;
        }

        //
        /// <summary>
        /// 1ステップ進める
        /// </summary>
        internal void SolveOnce()
        {
            Node n = _queue.Top; _queue.Pop();
            if (n.Priority <= 1)
            {
                var abint = n.DetermineNumber;
                var list = GetEffectivePositions(n.X, n.Y);
                foreach (var m in list)
                {
                    Node k = _board[m[0], m[1]];
                    k.RemoveAble(this, abint);
                }
                n.DetermineNumber = -30000;
                _board[n.X, n.Y] = n;
                requeue();
            }
            else
                if (n.Priority > 10000)
                {
                    Solved = true;
                }
                else
                    if (ByOtherDetermineChanged)
                    {
                        bool postflag = false;
                        for (int i = 0; i < 9; i++)
                            for (int j = 0; j < 9; j++)
                            {
                                postflag |= (SolveMiddleClass(i, j, GetHorizonalPositions(i, j)));
                                postflag |= (SolveMiddleClass(i, j, GetVerticalPositions(i, j)));
                                postflag |= (SolveMiddleClass(i, j, GetBoxPositions(i, j)));
                            }
                        ByOtherDetermineChanged = postflag;
                    }
            Step++;
        }

        public bool SolveMiddleClass(int i, int j, List<int[]> list)
        {
            var pl = list;
            var an = _board[i, j].AvailableNumbers;
            foreach (var val in an)
            {
                bool isDetermine = true;
                foreach (var pos in pl)
                {
                    if (_board[pos[0], pos[1]].AvailableNumbers.Contains(val))
                    {
                        isDetermine = false;
                        break;
                    }
                }
                if (isDetermine)
                {
                    //_board[i, j].DetermineNumber = val;
                    _board[i, j].RemoveOther(val);
                    _board[i, j].DetermineNumber = -30000;
                    //Console.WriteLine("({0}, {1}) が{2}に確定したよ～", i, j, val);
                    return true;
                }
            }

            return false;
        }

        private void requeue()
        {
            _queue = new PriorityQueue<Node>();
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    _queue.Push(_board[i, j]);
                }
        }

        internal void PutSolution(string prefix)
        {
            Console.WriteLine(prefix);
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Console.Write(_board[i, j]);
                    
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            Console.WriteLine("Available Counts:");
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Console.Write(string.Format("[{0}]", _board[i, j].AvailableNumbers.Count));

                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public int Step { get; set; }
    }
}
