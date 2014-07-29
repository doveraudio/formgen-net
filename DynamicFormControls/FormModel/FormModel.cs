using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace FormModeller
{
    [DataContractAttribute()]
    public class FormField
    {
        public FormField()
        {
            this.Name = "";
            this.Label = "";
            this.Column = "";
            this.Value = "";
            this.DBType = new List<string>();
            this.FieldType = "";
            this.Table = "";
            this.Key = "";
            this.KeyTable = "";
            this.KeyName = "";
            this.X = 0;
            this.Y = 0;
            this.Height = 0;
            this.Width = 0;
            this.BackgroundColor = "";
            this.BorderColor = "";
            this.ForegroundColor = "";
        }

        public FormField(String name, String label, String column, String value, string[] dbtype, String fieldtype, String table, String key, String keytable, String keyname, int x, int y, int height, int width, string backgroundColor, string borderColor, string foregroundColor)
        {
            this.Name = name;
            this.Label = label;
            this.Column = column;
            this.Value = value;
            this.DBType = new List<string>(dbtype);
            this.FieldType = fieldtype;
            this.Table = table;
            this.Key = key;
            this.KeyTable = keytable;
            this.KeyName = keyname;
            this.X = x;
            this.Y = y;
            this.Height = height;
            this.Width = width;
            this.BackgroundColor = backgroundColor;
            this.BorderColor = borderColor;
            this.ForegroundColor = foregroundColor;
        }

        [DataMember()]
        public String Table { get; set; }

        [DataMember()]
        public String Name { get; set; }

        [DataMember()]
        public String Column { get; set; }

        [DataMember()]
        public List<string> DBType { get; set; }

        [DataMember()]
        public String FieldType { get; set; }

        [DataMember()]
        public String KeyTable { get; set; }

        [DataMember()]
        public String KeyName { get; set; }

        [DataMember()]
        public String Key { get; set; }

        [DataMember()]
        public String Value { get; set; }

        [DataMember()]
        public int X { get; set; }

        [DataMember()]
        public int Y { get; set; }

        [DataMember()]
        public string BackgroundColor { get; set; }

        [DataMember()]
        public string BorderColor { get; set; }

        [DataMember()]
        public string ForegroundColor { get; set; }

        [DataMember()]
        public int Height { get; set; }

        [DataMember()]
        public int Width { get; set; }

        [DataMember()]
        public string Label { get; set; }
    }

    [DataContractAttribute()]
    public class FormModel
    {
        public FormModel()
        {
            Fields = new List<FormField>();
        }

        [DataMember()]
        public String Title { get; set; }

        [DataMember()]
        public List<FormField> Fields { get; set; }

        [DataMember()]
        public int Height { get; set; }

        [DataMember()]
        public int Width { get; set; }

        [DataMember()]
        public string BackgroundColor { get; set; }

        [DataMember()]
        public string ForegroundColor { get; set; }

        [DataMember()]
        public string BorderColor { get; set; }
    }

    public static class FormIO
    {
        public static string Filepath { get; set; }

        public static FormModel Form { get; set; }

        public static void save()
        {
            if (!Directory.Exists(Filepath))
            {
                Directory.CreateDirectory(Filepath);
            }
            string filename = Form.Title + ".json";
            filename = filename.Replace(" ", "_");
            filename = Filepath + filename;
            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                DataContractJsonSerializer formOut = new DataContractJsonSerializer(Form.GetType());
                formOut.WriteObject(fs, Form);
            }
        }

        public static void load()
        {
            string filename = Form.Title + ".json";
            filename = filename.Replace(" ", "_");
            filename = Filepath + filename;
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                DataContractJsonSerializer formIn = new DataContractJsonSerializer(typeof(FormModel));
                Form = (FormModel)formIn.ReadObject(fs);
            }
        }
    }
}