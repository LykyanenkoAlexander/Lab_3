using Лаба;
using System;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace Лаба
{    
    class V2DataCollection : V2Data, IEnumerable<DataItem>
    {
        public List<DataItem> Data { get; set; }

        public override IEnumerator<DataItem> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new DataEnumerator(Data);
        }



        public V2DataCollection(string a, double b) : base(a, b)
        {
            Data = new List<DataItem>();
        }

        
        public V2DataCollection(string filename) : base("abc", 0)
        {

            FileStream f = null; string f_data;
            List<DataItem> Data_file = new List<DataItem>();
   

            try
            {
               
                if (!File.Exists(filename))
                {
                    throw new Exception("No such a filename\n");
                }
              
                f = new FileStream(filename, FileMode.Open);
                StreamReader file = new StreamReader(f);
                f_data = file.ReadLine();
                string[] base_sep = f_data.Split(' ');

                if(base_sep.Length != 2)
                {
                    Console.WriteLine(base_sep.Length);
                    throw new Exception("Wrong file input\n");
                }

                Indef = base_sep[0];
                Freq = Convert.ToDouble(base_sep[1]);

                f_data = file.ReadLine();

                while (f_data != null)
                {
                    string[] sep = f_data.Split(' ');
                    if (sep.Length != 4) 
                    {
                        throw new Exception("Wrong file input\n");
                    }                  
                  
                    DataItem Data_Coll = new DataItem();
                    Data_Coll.Vect2 = new Vector2(float.Parse(sep[0]), float.Parse(sep[1]));
                    Data_Coll.Compl = new Complex(Convert.ToDouble(sep[2]), Convert.ToDouble(sep[3]));
                    Data_file.Add(Data_Coll);

                    f_data = file.ReadLine();            
                }

                Data = new List<DataItem>(); 
                Data = Data_file;

            }

            catch (Exception e)
            {
                System.Console.WriteLine("Parse error");
                System.Console.WriteLine(e.Message);
            }

            finally
            {
                if (f != null)
                {
                    f.Close();
                }
            }
        }


        private class DataEnumerator : IEnumerator<DataItem>
        {         
            private List<DataItem> Data_new;
            public int position = -1;
            object IEnumerator.Current => Current;
            public DataItem Current
            {
                get
                {
                    DataItem D = new DataItem();
                    D = Data_new.ElementAt(position);
                    return D;
                }
            }
            public DataEnumerator(List<DataItem> Data)
            {
                Data_new = Data;
            }
            public bool MoveNext()
            {
                position += 1;
                return position < Data_new.Count;
            }
            public void Reset()
            {
                position = -1;
            }
            public void Dispose()
            {
                Data_new = null;
            }
        }

        public void InitRandom(int nItems, float xmax, float ymax, double minValue, double maxValue)
        {
            var rnd = new Random();
            for (int i = 0; i < nItems; i++)
            {
                var X = rnd.NextDouble() * xmax;
                var Y = rnd.NextDouble() * ymax;

                var X_f = (float)X;
                var Y_f = (float)Y;

                var next_Re = rnd.NextDouble();
                var next_Im = rnd.NextDouble();

                DataItem D = new DataItem();

                D.Vect2 = new Vector2(X_f, Y_f);
                D.Compl = new Complex(minValue + (next_Re * (maxValue - minValue)),
                                      minValue + (next_Im * (maxValue - minValue)));

  

                Data.Add(D);
            }
            Console.WriteLine("\n");
        }

        public override Complex[] NearAverage(float eps)
        {
            Complex mid_value = 0;
            int ind = 0;

            foreach (var item in Data)
            {
                mid_value += item.Compl;
            }

            mid_value = mid_value.Real / Data.Count;

            foreach (var item in Data)
            {
                if (Math.Abs(mid_value.Real - item.Compl.Real) <= (double)eps)
                {
                    ind++;
                }
            }

            Complex[] NearAverage = new Complex[ind];
            ind = 0;

            foreach (var item in Data)
            {
                if (Math.Abs(mid_value.Real - item.Compl.Real) <= (double)eps)
                {
                    NearAverage[ind] = item.Compl;
                    Console.WriteLine(item.Compl);
                    ind++;
                }
            }
            return NearAverage;
        }

        public override string ToString()
        {
            string res = "Type is V2DataCollection, " + "Base class values:" + '\n' +
                         "Indeficator = " + Indef + '\n' + "Frequency = " + Freq + '\n' +
                         "List size = " + Data.Count + '\n';
            return res;
        }

        public override string ToLongString()
        {
            string long_res = "Type is V2DataCollection, " + "Base class values:" + '\n' +
                         "Indeficator = " + Indef + '\n' + "Frequency = " + Freq + '\n' +
                         "List size = " + Data.Count;

            foreach (var item in Data)
            {
                long_res = long_res + '\n' + item.Vect2 + " = " + item.Compl + '\n' +
                    "module value = " + item.Compl.Magnitude;
            }
            
            return long_res;
        }

        public override string ToLongString(string format)
        {
            /*string long_res = "Type is V2DataCollection, " + "Base class values:" + '\n' +
                         "Indeficator = " + Indef + '\n' + "Frequency = " + Freq + '\n' +
                         "List size = " + Data.Count;
            */
            string long_res = "";
            foreach (var item in Data)
            {
                long_res = long_res + '\n' + item.Vect2 + " = " + item.Compl.ToString(format) + '\n' +
                    "module value = " + item.Compl.Magnitude;
            }

            return "s";
        }

    }

}
