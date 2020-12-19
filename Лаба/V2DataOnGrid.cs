using Лаба;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;



namespace Лаба
{
    class V2DataOnGrid : V2Data, IEnumerable<DataItem>
    {
        public Grid1D[] Elem { get; set; }
        public Complex[,] Node_Val { get; set; }

        public V2DataOnGrid(string a, double b, Grid1D x, Grid1D y) : base(a, b)
        {
            Elem = new Grid1D[2];
            Elem[0] = x;
            Elem[1] = y;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new DataEnumerator(Node_Val, Elem[0], Elem[1]);
        }

        public override IEnumerator<DataItem> GetEnumerator()
        {            
            return new DataEnumerator(Node_Val, Elem[0], Elem[1]);
        }

        private class DataEnumerator : IEnumerator<DataItem>
        {
            private readonly Grid1D elem1, elem2;
            private Complex[,] Node_Val_a;
            private int position_x = -1;
            private int position_y = 0;

            object IEnumerator.Current => Current;

            public DataItem Current
            {
                get
                {
                    DataItem DI = new DataItem();
                    DI.Vect2 = new Vector2(elem1.GetOXCoord(position_x), elem2.GetOYCoord(position_y));
                    DI.Compl = Node_Val_a[position_x, position_y];
                    return DI;
                }
            }

            public DataEnumerator(Complex[,] values, Grid1D elem1, Grid1D elem2)
            {
                this.Node_Val_a = values;
                this.elem1 = elem1;
                this.elem1 = elem1;
            }

            public bool MoveNext()
            {
                if (position_x == Node_Val_a.GetLength(1) - 1)
                {
                    position_y++;
                    position_x = 0;
                }
                else
                {
                    position_x++;
                }

                return position_y < Node_Val_a.GetLength(0);
            }

            public void Reset()
            {
                position_x = -1;
                position_y = 0;
            }

            public void Dispose()
            {
                Node_Val_a = null;
            }
        }


        public void InitRandom(double minValue, double maxValue)
        {
            Random rnd = new Random();
            Node_Val = new Complex[Elem[0].Node, Elem[1].Node];

            for (int i = 0; i < Elem[0].Node; i++)
            {
                for (int j = 0; j < Elem[1].Node; j++)
                {

                    var next_Re = rnd.NextDouble();
                    var next_Im = rnd.NextDouble();

                    Node_Val[i, j] = new Complex(minValue + (next_Re * (maxValue - minValue)),
                                                 minValue + (next_Im * (maxValue - minValue)));

                    
                }
            }
        }

        public static implicit operator V2DataCollection(V2DataOnGrid a)
        {
            V2DataCollection V2 = new V2DataCollection(a.Indef, a.Freq);

            for (int i = 0; i < a.Node_Val.GetLength(0); i++)
            {
                for (int j = 0; j < a.Node_Val.GetLength(1); j++)
                {
                    DataItem DI = new DataItem();
                    DI.Vect2 = new Vector2(a.Elem[0].Step * i,
                                           a.Elem[1].Step * j);

                    DI.Compl = new Complex(a.Node_Val[i, j].Real,
                                           a.Node_Val[i, j].Imaginary);

                    V2.Data.Add(DI);
                }
            }

            return V2;
        }

        public override Complex[] NearAverage(float eps)
        {
            Complex mid_value = 0;
            int ind = 0;

            for (int i = 0; i < Elem[0].Node; i++)
            {
                for (int j = 0; j < Elem[1].Node; j++)
                {
                    mid_value += Node_Val[i, j];
                }
            }

            mid_value = mid_value.Real / (Elem[0].Node * Elem[1].Node);

            for (int i = 0; i < Elem[0].Node; i++)
            {
                for (int j = 0; j < Elem[1].Node; j++)
                {
                    if (Math.Abs(mid_value.Real - Node_Val[i, j].Real) <= (double)eps)
                    {
                        ind++;
                    }
                }
            }

            Complex[] NearAverage = new Complex[ind];
            ind = 0;

            for (int i = 0; i < Elem[0].Node; i++)
            {
                for (int j = 0; j < Elem[1].Node; j++)
                {
                    if (Math.Abs(mid_value.Real - Node_Val[i, j].Real) <= (double)eps)
                    {
                        NearAverage[ind] = Node_Val[i, j];
                        Console.WriteLine(NearAverage[ind]);
                        ind++;
                    }
                }
            }
            return NearAverage;
        }


        public override string ToString()
        {
            string res = "Type is V2DataOnGrid, " + '\n' + "Base class values:" + '\n' +
                    "Indeficator = " + Indef + '\n' + "Frequency = " + Freq + '\n' +
                    "Grid data:" + '\n' + "Ox: " + '\n' + "Number of nodes = " + Elem[0].Node +
                    ", Number of steps = " + Elem[0].Step + '\n' + '\n' +
                    "Oy: " + '\n' + "Number of nodes = " + Elem[1].Node + ", Number of steps = " +
                    Elem[1].Step + '\n';

            return res;
        }

        public override string ToLongString()
        {
            string long_res = "Type is V2DataOnGrid, " + "Base class values:" + '\n' +
                    "Indeficator = " + Indef + '\n' + "Frequency = " + Freq + '\n' +
                    "Grid data:" + '\n' + "Ox: " + "  " + "Number of nodes = " + Elem[0].Node +
                    ", Number of steps = " + Elem[0].Step + '\n' +
                    "Oy: " + "  " + "Number of nodes = " + Elem[1].Node + ", Number of steps = " +
                    Elem[1].Step + '\n';

            for (int i = 0; i < Elem[0].Node; i++)
            {
                for (int j = 0; j < Elem[1].Node; j++)
                {
                    long_res = long_res + '\n' + "[" + i * Elem[0].Step + ", " + j * Elem[1].Step +
                               "] " + " = " + Node_Val[i, j] + '\n' +
                    "module value = " + Node_Val[i, j].Magnitude;
                }
            }

            long_res = long_res + '\n';
            return long_res;
        }

        public override string ToLongString(string format)
        {
            string long_res = "Type is V2DataOnGrid, " + "Base class values:" + '\n' +
                    "Indeficator = " + Indef + '\n' + "Frequency = " + Freq + '\n' +
                    "Grid data:" + '\n' + "Ox: " + "  " + "Number of nodes = " + Elem[0].Node +
                    ", Number of steps = " + Elem[0].Step + '\n' +
                    "Oy: " + "  " + "Number of nodes = " + Elem[1].Node + ", Number of steps = " +
                    Elem[1].Step + '\n';

            for (int i = 0; i < Elem[0].Node; i++)
            {
                for (int j = 0; j < Elem[1].Node; j++)
                {
                    long_res = long_res + '\n' + "[" + i * Elem[0].Step + ", " + j * Elem[1].Step +
                               "] " + " = " + Node_Val[i, j].ToString(format) + '\n' +
                    "module value = " + Node_Val[i,j].Magnitude;
                }
            }

            long_res = long_res + '\n';

            return long_res;
        }


    }
}
