using System;
using System.IO;

namespace FormModeller
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            FormModel form = new FormModel();
            form.Title = "My New Form";
            FormField f = new FormField();
            f.Name = "First Name";
            f.Value = "Franklin";
            f.Key = "1";
            f.Table = "userdata";
            f.KeyName = "userID";
            f.KeyTable = "users";
            f.FieldType = "String";
            //f.DBType = "varchar(64)";
            f.Column = "firstname";
            f.X = 100;
            f.Y = 100;
            f.Width = 128;
            f.Height = 28;
            f.BackgroundColor = "ffffff";
            f.ForegroundColor = "000000";
            f.BorderColor = "454545";

            form.Fields.Add(f);

            //FormIO io = new FormIO();
            FormIO.Form = form;
            FormIO.Filepath = "C:\\Forms\\";
            FormIO.save();

            Console.ReadKey(true);
            string filename = FormIO.Filepath + form.Title.Replace(" ", "_") + ".json";
            Console.Write(readFile(filename));
            Console.ReadKey(true);
            FormIO.load();

            form = FormIO.Form;
            Console.Write(form.ToString());
            Console.ReadKey(true);
            Console.WriteLine("Execution Complete");
            Console.ReadKey(true);
        }

        public static string readFile(string filename)
        {
            string result = "";
            using (StreamReader fs = new StreamReader(filename))
            {
                result = fs.ReadToEnd();
            }
            return result;
        }
    }
}