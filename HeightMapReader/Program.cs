using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Text;

namespace HeightMapReader
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Console.Write("HGT file path: ");
                    string path = Console.ReadLine();
                    Console.Write("Sea level (-32767 to 32767): ");
                    short sealevel = Convert.ToInt16(Console.ReadLine());
                    Console.WriteLine("Processing...");
                    HeightMap hm = HeightMap.Parse(@path, sealevel);
                    Console.Clear();
                    Console.WriteLine("Processing done.");
                    Console.Write("Output filename: ");
                    string filename = Console.ReadLine();
                    hm.Save(filename);
                }
                catch (Exception ex)
                {
                    if (ex is FormatException || ex is OverflowException)
                    {
                        Console.WriteLine("Invalid format!");
                        Console.ReadKey();
                        continue;
                    }
                    else if (ex is FileNotFoundException)
                    {
                        Console.WriteLine("File not found!");
                        Console.ReadKey();
                        continue;
                    }
                    else Console.WriteLine(ex.StackTrace);
                }
                break;
            }
        }
    }

    class HeightMap
    {
        short[] value { get; }
        short sealevel { get; }

        public HeightMap(short[] values, short sealevel)
        {
            value = new short[values.Length];
            Array.Copy(values, value, values.Length);
        }

        public short[] Value
        {
            get { return value; }
        }

        public short SeaLevel
        {
            get { return sealevel; }
        }

        public static HeightMap Parse(string file_path, short sealevel)
        {
            try
            {
                StringBuilder ext = new StringBuilder();
                for (int i = 1; i <= 4; i++) ext.Insert(0, file_path[file_path.Length - i]);
                BinaryReader br = new BinaryReader(File.Open(@file_path + (ext.ToString().Equals(".hgt") ? "" : ".hgt"), FileMode.Open));
                List<short> bytes = new List<short>();
                while (br.BaseStream.Position != br.BaseStream.Length) bytes.Add(br.ReadInt16());
                br.Close();
                return new HeightMap(bytes.ToArray(), sealevel);
            }
            catch (FileNotFoundException)
            {
                throw;
            }
        }

        public void Save(string filename)
        {
            Bitmap bmp = new Bitmap((int)Math.Sqrt(value.Length), (int)Math.Sqrt(value.Length));
            for(int i = 0; i < value.Length; i++)
            {
                Color c;
                if (value[i] > sealevel) c = Color.FromArgb(0, Math.Abs(value[i] / 256), 0);
                else c = Color.FromArgb(0, 0, Math.Abs(value[i] / 256));
                bmp.SetPixel(i / bmp.Width, i % bmp.Width, c);
            }
            bmp.Save(filename + ".bmp");
        }

        //public void Save(string dir_path, string filename)
        //{
        //    Bitmap bmp = new Bitmap((int)Math.Sqrt(value.Length), (int)Math.Sqrt(value.Length));
        //    for (int i = 0; i < value.Length; i++)
        //    {
        //        Color c;
        //        if (value[i] > sealevel) c = Color.FromArgb(0, Math.Abs(value[i] % 255), 0);
        //        else c = Color.FromArgb(0, 0, Math.Abs(value[i] % 255));
        //        bmp.SetPixel(i / bmp.Width, i % bmp.Width, c);
        //    }
        //    bmp.Save(@dir_path + (dir_path[dir_path.Length-1] == '\\' ? "" : "\\") + filename + ".bmp");
        //}
    }
}
