using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel;

namespace Лаба
{

    class V2MainCollection : IEnumerable<V2Data>
    {

        private List<V2Data> Main_Data { get; set; }
        public int number;
        public V2MainCollection()
        {
            Main_Data = new List<V2Data>();
            number = 0;
        }

        public V2Data this[int index]
        {
            get
            {
                return Main_Data[index];
            }
            set
            {
                value.PropertyChanged += PC;
                Main_Data[index] = value;             
                
                OnDataChanged(this, new DataChangedEventArgs(ChangeInfo.Replace,
                              Main_Data[index].Freq));
            }
        }
        public event DataChangedEventHandler DataChanged;
        public void OnDataChanged(object source, DataChangedEventArgs args)
        {
            DataChanged?.Invoke(source, args);
        }

        public void PC(object sender, PropertyChangedEventArgs args)
        {
            OnDataChanged(this, new DataChangedEventArgs(ChangeInfo.ItemChanged,
                          Main_Data[number-1].Freq));
        }



        public int Count
        {
            set { }

            get
            {
                return (Main_Data.Count);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return Main_Data.GetEnumerator();
        }

        IEnumerator<V2Data> IEnumerable<V2Data>.GetEnumerator()
        {
            return Main_Data.GetEnumerator();
        }


        public double Mid_Value
        {
            get
            {
                var val = from data in Main_Data from i in data select i.Compl;
                var res = (from x in val select x.Magnitude).Average();

                return res;
            }                      
        }

        public IEnumerable<DataItem> Max_Far_Away
        {
            get
            {       
                var united = Main_Data.SelectMany(x => x);              
                double m_v = Mid_Value;          

                double max = united.Max(v => v.Compl.Magnitude);
                double min = united.Min(v => v.Compl.Magnitude);
              
                var total =  (Math.Abs(m_v - max) > Math.Abs(m_v - min)) ||((Math.Abs(m_v - max) < Math.Abs(m_v - min))) ?
                            ((Math.Abs(m_v - max) > Math.Abs(m_v - min)) ? max : min ):min;
       
                IEnumerable<V2DataOnGrid> V2 = from V2Data x in Main_Data where x.Indef == "A" select (V2DataOnGrid)x;
                //var V22 = from DataItem x in Main_Data where x.Compl.Magnitude == total select x;
                IEnumerable<V2DataCollection> C2 = from V2Data x in Main_Data where x.Indef == "B" select (V2DataCollection)x; 

                var V2_res = V2.SelectMany(x => x).Where(y => y.Compl.Magnitude == total);           
                var C2_res = C2.SelectMany(x => x).Where(y => y.Compl.Magnitude == total);
          
                IEnumerable<DataItem> Res = V2_res.Concat(C2_res);

                return Res;
            }
        }
        
        public IEnumerable<Vector2> More_then_one
        {
            get
            {
                IEnumerable<Vector2> vec_set = from x in Main_Data from item in x select item.Vect2;   
                var result = (from x in vec_set where (vec_set.Count() > 1) select x);

                var res = from x in Main_Data from i in x
                          group i by i.Vect2 into h where h.Count() > 1 select h.Key;                   

                return res;
            }
        }
        
        public void Add(V2Data item)
        {
            item.PropertyChanged += PC;         
            Main_Data.Add(item);
            
            OnDataChanged(this, new DataChangedEventArgs(ChangeInfo.Add, 
                          Main_Data[number].Freq));
            number++;

        }

        public bool Remove(string id, double w)
        {
            Console.WriteLine('\n' + "Before Removing:" + '\n');
            foreach (var item in Main_Data.ToArray())
            {
                Console.WriteLine(item);
            }

            foreach (var item in Main_Data.ToArray())
            {
                if ((item.Indef == id) && (item.Freq == w))
                {
                    Main_Data.Remove(item);
                }
            }

            Console.WriteLine('\n' + "After Removing:" + '\n');
            foreach (var item in Main_Data.ToArray())
            {
                Console.WriteLine(item);
            }

            if(Main_Data.Count != 0)
            {
                Console.WriteLine("Main_Data.Length = " + Main_Data.Count);          
               
                OnDataChanged(this, new DataChangedEventArgs(ChangeInfo.Remove, 
                              Main_Data[number - Main_Data.Count - 1].Freq));
               
            }
            number--;

            return Main_Data.Count > 0;   

        }

        public void AddDefaults()
        {
            Random rnd = new Random();

            Grid1D d1 = new Grid1D(1, 2);
            Grid1D d2 = new Grid1D(2, 2);
            V2DataOnGrid New_Grid = new V2DataOnGrid("A", 3, d1, d1);
            V2DataOnGrid New_Grid_1 = new V2DataOnGrid("A", 3, d2, d2);
            New_Grid.InitRandom(3, 7);
            New_Grid_1.InitRandom(4, 8);

            V2DataCollection New_Coll = new V2DataCollection("B", 5);
            New_Coll.InitRandom(4, 4, 6, 4, 6);

            

            Main_Data.Add(New_Grid);
            Main_Data.Add(New_Grid_1);
            Main_Data.Add(New_Grid_1);
            Main_Data.Add(New_Coll);            
           
        }

        public override string ToString()
        {
            foreach (var item in Main_Data)
            {
                Console.WriteLine(item.ToString());
            }
            return null;
        }

        public string ToLongString()
        {
            foreach (var item in Main_Data)
            {
                Console.WriteLine(item.ToLongString());
            }
            return null;
        }

        public string ToLongString(string format)
        {
            foreach (var item in Main_Data)
            {
                Console.WriteLine(item.ToLongString(format));
            }
            return null;
        }

    }

}
