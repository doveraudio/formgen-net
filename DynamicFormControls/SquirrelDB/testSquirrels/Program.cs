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
            string[] getTables = { "blind_spots", };//"block_types", "case_manager_log", "cell_providers", "cert_groups", "cert_packages", "certifications", "certint", "client_availability", "client_comp", "client_services", "clients", "comp_data_natural_supports", "comp_data_personal_barriers", "comp_data_personal_strengths", "comp_data_resource_barriers", "comp_data_resource_strengths", "comp_goals", "comp_natural_supports", "comp_personal_barriers", "comp_personal_strengths", "comp_resource_barriers", "comp_resource_strengths", "date_units", "education_levels", "employee_availability", "employee_injuries", "employees", "frequency", "full_icd9_2012", "hr_document_ids", "hr_file_types", "icd9_2012", "it_inv_assignment", "it_inv_lines", "it_inv_makes", "it_inv_models", "it_inv_types", "it_inventory", "it_os", "languages", "medications", "meeting_types", "messages", "organizations", "phone_calls", "phone_origin_types", "prescriptions", "releases", "s28", "severity", "shifts", "states", "tcm_clients", "timeoff_requests", "users", "vehicle_ins", "vehicle_reg", "vehicles", "writeupint", "writeups", "zipcodes" };
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
                Console.WriteLine("the current table is : " + table);
                
                sDB.createLocalTable();
                sDB.setMySqlConnectionString();
                sDB.setSqliteConnectionString();
                sDB.setSelectStatement();

                System.Console.WriteLine(sDB.MySqlConnectionString);

                sDB.getColumnsFromMySqlTable();
                sDB.getVartypesFromRemoteTable();

                sDB.createLocalDatabase();
                sDB.createLocalTable();

                sDB.TranslateRemoteDB();

                Console.ReadKey(true);
                Console.WriteLine(sDB.Log);
                sDB.loadFactories();
                sDB.connectFactories();
                sDB.Basepath = "C:\\SquirrelDB\\Databases\\";

                sDB.Username = "dev";
                sDB.Password = "dev";
                sDB.Host = "localhost";
                sDB.Extension = "sq3";

                Console.ReadKey(true);
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
                Console.ReadKey(true);
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