using System;
using System.Data;
using System.Numerics;
using System.Collections.Generic;
using Лаба;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel;



struct DataItem
    {
        public Vector2 Vect2 { get; set; }
        public Complex Compl { get; set; }

        public DataItem(float x, float y)
        {
            Vect2 = new Vector2(x, y);
            Compl = new Complex(x, y);
        }

        public override string ToString()
        { return Vect2 + " , " + Compl; }

        public string ToString(string format)
        {
            string res = Vect2.ToString();
            res = res + " : " + Compl.ToString(format) + Math.Sqrt(Compl.Real*Compl.Real + Compl.Imaginary*Compl.Imaginary).ToString(format);
            return res;
        }

    }

    struct Grid1D
    {
        public float Step { get; set; }
        public int Node { get; set; }

        public Grid1D(float a, int b)
        {
            Step = a;
            Node = b;
        }

        public float GetOXCoord(int ox_coord)
        {
            
            return ox_coord*Step;
        }

        public float GetOYCoord(int oy_coord)
        {

            return oy_coord*Step;
        }

    public override string ToString()
        { return "Step = " + Step + ", " + "Node = " + Node; }

        public string ToString(string format)
        {
            string res;
            res = "Step = " +  Step.ToString(format) + " ,Node = " + Node.ToString(format);
            return res;
        }

}

    abstract class V2Data : IEnumerable<DataItem>, INotifyPropertyChanged
{
        public string Indef;
        public double Freq;
        public V2Data(string a, double b)
        {
            Indef = a;
            Freq = b;
        }
    
        public abstract Complex[] NearAverage(float eps);

        public abstract string ToLongString();

        public override string ToString()
        { return "V2Data: " + Indef + "," + Freq; }

        public abstract string ToLongString(string format);

        public abstract IEnumerator<DataItem> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void P_C(object source, string propertyName)
    {
        PropertyChanged?.Invoke(source, new PropertyChangedEventArgs(propertyName));
    }

    public string Ind
    {
        get => Indef;
        set
        {
            Indef = value;
            P_C(this, "Fr");
        }
    }

    public double Fr
    {
        get => Freq;
        set
        {
            Freq = value;
            P_C(this, "Fr");
        }
    }

}

   
class Res
{
    public static void Coll_Test(object data, DataChangedEventArgs args)
    {
        Console.WriteLine(args.ToString());
    }

    static void Main()
    {
        try
        {
            //file data      
            V2DataCollection Lab_2_Data_Coll = new V2DataCollection("data_1.txt");
            Console.WriteLine(Lab_2_Data_Coll.ToLongString());

            //Linq testing
            V2MainCollection Lab_2_Main_Coll = new V2MainCollection();
            Lab_2_Main_Coll.AddDefaults();
            Lab_2_Main_Coll.ToLongString();

            Console.WriteLine("\nMiddle module value:");
            Console.WriteLine(Lab_2_Main_Coll.Mid_Value);
            Console.WriteLine("\nMaxFarAway values:");

            foreach (DataItem item in Lab_2_Main_Coll.Max_Far_Away)
            {
                Console.WriteLine(item.ToString());
            }

            /*Console.WriteLine("\nMoreThenOne values:");
            foreach (Vector2 item in Lab_2_Main_Coll.More_then_one)
            {
                Console.WriteLine(item.ToString("f5"));
            }
            */

        }
        catch (Exception e) 
        {
            Console.WriteLine("{0} Exception caught.", e);
        }

        V2MainCollection Test_3 = new V2MainCollection();
        Test_3.DataChanged += Coll_Test;
        V2DataCollection File_Data = new V2DataCollection("data_1.txt");
        Test_3.Add(File_Data);
        Test_3.ToString();

        V2DataCollection new_test_1 = new V2DataCollection("new test 1", 123);
        V2DataCollection new_test_2 = new V2DataCollection("new test 2", 456);
        
        Test_3.Add(new_test_1);

        Test_3[0] = new_test_2;
        Test_3[0].Ind = "Changing_test" ;
        Test_3.Remove("Changing_test", 456);


      

    }


   



}
