using Squirrel;
using System;
using System.Data;

namespace testSquirrels
{
    internal class Program
    {
        public static SquirrelDB sDB;

        private static void Main(string[] args)
        {
            string[] getTables = {"employees", };//"abilityscores", "ac_ok_test", "address", "allergen_severity", "allergies", "assignedhours", "assignedstaff", "children", "clientaddress", "clientphone", "clientschedule", "contact", "custody", "diagnosis", "emergencycontact", "familyhistory", "formsblank", "formscomplete", "formsneeded", "gender", "guardian", "healthcareproviders", "housingconcerns", "income", "initials", "insurance", "insurer", "livingarrangements", "locustest", "marital", "medicalconcerns", "medicalsupports", "med_management", "name", "orgaddress", "organization", "orgphones", "phone", "possible_assistance_sources", "presentingproblem", "providertypes", "referral", "releases", "ssn", "statefederalentitlements", "vocational", "vocationalfinancialneeds", "zipcodes", };
            foreach (string table in getTables)
            {
            sDB = new SquirrelDB();
            DataTable dt = new DataTable();
            
            sDB.Table = table;
            sDB.loadFactories();
            sDB.connectFactories();
            sDB.Basepath = "C:\\SquirrelDB\\Databases\\";
            sDB.Database = "hrDB";
            sDB.Username = "dev";
            sDB.Password = "dev";
            sDB.Host = "localhost";
            sDB.Extension = "sq3";
            Console.WriteLine("the current table is : "+table);
            sDB.setMySqlConnectionString();
            sDB.setSqliteConnectionString();
            sDB.setSelectStatement();
            
            System.Console.WriteLine(sDB.MySqlConnectionString);
            
            sDB.getColumnsFromMySqlTable();
            sDB.getVartypesFromRemoteTable();
            
            sDB.createLocalDatabase();
            sDB.createLocalTable();

            sDB.TranslateRemoteDB();

            //Console.ReadKey(true);
            Console.WriteLine(sDB.Log);
            sDB.loadFactories();
            sDB.connectFactories();
            sDB.Basepath = "C:\\SquirrelDB\\Databases\\";
            
            sDB.Username = "dev";
            sDB.Password = "dev";
            sDB.Host = "localhost";
            sDB.Extension = "sq3";
            
            
            ///Console.ReadKey(true);
            sDB.setMySqlConnectionString();
            sDB.setSqliteConnectionString();
            sDB.setSelectStatement();
            System.Console.WriteLine(sDB.MySqlConnectionString);
        
            sDB.getColumnsFromLocalTable();
            sDB.getVartypesFromLocalTable();
            dt = sDB.getLocalTable();

            for (int h = 0; h < dt.Columns.Count; h++)
            {

                Console.Write(dt.Columns[h].ColumnName + "\t");

            }
            Console.WriteLine();
            //Console.ReadKey(true);
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                for (int h = 0; h < dt.Columns.Count; h++)
                {

                    Console.Write(dt.Rows[i][h] + "\t");

                }
                Console.WriteLine();

                
            }
                }
            Console.ReadKey(true);
        }
    }
}