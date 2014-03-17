using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku
{
    class Node : IComparable<Node>
    {
        // 0x1 = 1, 0x2 = 2, 0x4 = 3, 0x8 = 4
        // 0x10 = 5, 0x20 = 6, 0x40 = 7, 0x80 = 8
        // 0x100 = 9
        private int ables = 0;
        private int _prr = int.MaxValue;
        private bool changedPriority = true;
        private int determinedint = -10000;

        public int X
        {
            get;
            set;
        }

        public int Y
        {
            get;
            set;
        }

        public Node(char p)
        {
            if (p != '.')
            {
                int v = p - '0';
                determinedint = v;
                RemoveOther(v);
            }
        }

        public void RemoveOther(int v)
        {
            for (int i = 1; i <= 9; i++)
            {
                if (i != v)
                {
                    RemoveAble(i);
                }
            }
        }

        public void RemoveAble(Board board, int number)
        {
            RemoveAble(number);
        }

        public void RemoveAble(int number)
        {
            ables |= ( 1 << (number - 1) );
            changedPriority = true;
        }

        public int Priority
        {
            get {
                if (determinedint > 0) return 0;
                if (determinedint < -10000) return int.MaxValue;

                if (changedPriority)
                {
                    int c = 0;
                    for (var i = 0; i < 9; i++)
                    {
                        if ((ables & (0x1 << i)) == 0) //可能性があるほど0が多いので
                        {
                            c++; //cを加算して優先度を下げる
                        }
                    }
                    _prr = c;
                    return c;
                }
                return _prr;
            }
        }

        public List<int> AvailableNumbers
        {
            get {
                var l = new List<int>();
                for (int i = 0; i < 9; i++)
                {
                    if ((ables & (0x1 << i)) == 0)
                    {
                        l.Add(i+1);
                    }
                }
                //Console.WriteLine("({0}, {1}) Count: {2}", X, Y, l.Count);
                return l;
            }
        }

        public int DetermineNumber
        {
            get {
                if (determinedint > 0) return determinedint;

                List<int> c = AvailableNumbers;
                if (c.Count == 1) return c[0];
                return -10000;
            }
            set
            {
                determinedint = value;
            }
        }

        int IComparable<Node>.CompareTo(Node other)
        {
            return other.Priority.CompareTo(Priority);
        }

        public override string ToString()
        {
            if (AvailableNumbers.Count == 1)
                return "" + AvailableNumbers[0];
            else
                return ".";
        }
    }
}
