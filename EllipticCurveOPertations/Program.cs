using System;
using System.Collections.Generic;


namespace EllipticCurveOPertations
{
    internal class Program
    {
        public struct ECurve
        {
            public int a, b,p;

        }
        public struct Point
        {
            public int x, y;
            public Point(int nx, int ny)
            {
                x = nx;
                y = ny;
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override string ToString()
            {
                return $"({x},{y})";
            }
        }

        private static int ModInverse(int a, int m)
        {
            a = a % m;
            for (int x = 1; x < m; x++)
            {
                if ((a * x) % m == 1)
                    return x;
            }
            throw new ArgumentException($" No {a}^(-1) по mod {m}");
        }
        public static Point SumOfTwoPts(Point a, Point b, ECurve curve)
        {
            int lambda;
            if (a.Equals(b))
            {

                int numerator = (3 * a.x * a.x + curve.a) % curve.p;
                int denominator = (2 * a.y) % curve.p;
                if (denominator < 0)
                    denominator += curve.p;

                int invDenominator = ModInverse(denominator, curve.p);
                lambda = (numerator * invDenominator) % curve.p;
            }
            else
            {
                int numerator = (b.y - a.y) % curve.p;
                int denominator = (b.x - a.x) % curve.p;
                if (denominator < 0)
                    denominator += curve.p;

                int invDenominator = ModInverse(denominator, curve.p);
                lambda = (numerator * invDenominator) % curve.p;
            }

            if (lambda < 0)
                lambda += curve.p;

            int x3 = (lambda * lambda - a.x - b.x) % curve.p;
            if (x3 < 0)
                x3 += curve.p;

            int y3 = (lambda * (a.x - x3) - a.y) % curve.p;
            if (y3 < 0)
                y3 += curve.p;

            return new Point(x3, y3);
        }
        public static bool[] IntToBin(int input)
        {
            if (input == 0)
            {
                return new bool[] { false };
            }

            List<bool> bits = new List<bool>();
            int n = input;

            while (n > 0)
            {
                bits.Add((n % 2) == 1);
                n /= 2;
            }

            return bits.ToArray();
        }
        public static string BoolArrayToString(bool[] input)
        {
            string res ="";
            foreach (bool b in input)
            {
                res += b ? 1:0;
            }
            return res;
        }
        public static Point MultPointByInt(Point a, int n, ECurve curve)
        {
            if (n == 0) return new Point(0, 0); // Точка на бесконечности

            bool[] nbin = IntToBin(n);
            var tmp = new List<Point> { a };

            for (int k = 1; k < nbin.Length; k++)
            {
                tmp.Add(SumOfTwoPts(tmp[k - 1], tmp[k - 1], curve));
            }

            Point res = new Point(0, 0);
            for (int i = 0; i < nbin.Length; i++)
            {
                if (nbin[i])
                {
                    if (res.x == 0 && res.y == 0) 
                        res = tmp[i];
                    else
                        res = SumOfTwoPts(res, tmp[i], curve);
                }
            }

            return res;
        }

        public static int GetOrd(Point a, ECurve curve, int N)
        {
            if (a.x == 0 && a.y == 0)
                return 1;

            Point current = a;
            for (int k = 2; k <= N; k++)
            {
                
                try
                {
                    current = SumOfTwoPts(current, a, curve);
                }
                catch (ArgumentException)
                {
               
                    return k;
                }

                if (current.x == 0 && current.y == 0)
                    return k;
            }
            return N; 
        }
        static void Main(string[] args)
        {
            ECurve curve = new ECurve { a = 3, b = 3, p = 7 };
            Point P = new Point(4, 4);
            //int n = 142;
            //string str = MultPointByInt(P,n,curve).ToString();
            Console.WriteLine(GetOrd(P,curve,32));
            //(156,704)

        }
    }
}
