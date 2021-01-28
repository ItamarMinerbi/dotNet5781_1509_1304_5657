using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using BlApi;
using BO;

namespace PlGuiTest
{
    class Program
    {
        static IBL BL = BlFactory.GetBL();
        static List<string> Admins = new List<string> { "Admin", "Test", "Dan Zilber" };
        static void Main(string[] args)
        {

        }

        public static void StartValuesFile()
        {
            StreamReader reader1;
            StreamReader reader2;
            StreamReader reader3;
            StreamReader reader4;
            StreamReader reader5;
            StreamWriter writer1;
            StreamWriter writer2;
            #region Users
            reader1 = new StreamReader(@"C:\Users\Itamar Minerbi\Desktop\Project codes\nameList.txt");
            reader2 = new StreamReader(@"C:\Users\Itamar Minerbi\Desktop\Project codes\passList.txt");
            writer1 = new StreamWriter(@"C:\Users\Itamar Minerbi\Desktop\Project codes\UsersCodeC#.txt");
            writer2 = new StreamWriter(@"C:\Users\Itamar Minerbi\Desktop\Project codes\Users&Passwords&MD5.txt");
            do
            {
                string name = reader1.ReadLine();
                string pass = reader2.ReadLine();
                string md5 = CreateMD5(pass);
                string IsAdmin = Admins.Contains(name) ? "true" : "false";
                writer1.WriteLine("Users.Add(new DO.User() { Username = \"" + name + "\", Password = \"" + md5 + "\", IsAdmin = " + IsAdmin + ", IsActive = true });");
                writer2.WriteLine($"Username: {name}, Password: {pass}, MD5: {md5}, IsAdmin: {IsAdmin}");
            } while (!reader1.EndOfStream && !reader2.EndOfStream);
            writer2.Close();
            writer1.Close();
            reader2.Close();
            reader1.Close();
            #endregion

            #region Stations
            reader1 = new StreamReader(@"C:\Users\Itamar Minerbi\Desktop\Project codes\stop_name.txt");
            reader2 = new StreamReader(@"C:\Users\Itamar Minerbi\Desktop\Project codes\stop_add.txt");
            reader3 = new StreamReader(@"C:\Users\Itamar Minerbi\Desktop\Project codes\stop_code.txt");
            reader4 = new StreamReader(@"C:\Users\Itamar Minerbi\Desktop\Project codes\stop_lat.txt");
            reader5 = new StreamReader(@"C:\Users\Itamar Minerbi\Desktop\Project codes\stop_lon.txt");
            writer1 = new StreamWriter(@"C:\Users\Itamar Minerbi\Desktop\Project codes\StationsCodeC#.txt");
            do
            {
                string code = reader3.ReadLine();
                string name = reader1.ReadLine();
                string add = reader2.ReadLine();
                string lat = reader4.ReadLine();
                string lon = reader5.ReadLine();

                writer1.WriteLine("Stations.Add(new DO.Station() { Name = @\"" + name + "\", StationCode = " + code + ", Address = @\"" + add + "\", Latitude = " + lat + ", Longitude = " + lon + ", IsActive = true });");

            } while (!reader1.EndOfStream && !reader2.EndOfStream && !reader3.EndOfStream && !reader4.EndOfStream && !reader5.EndOfStream);
            writer1.Close();
            reader5.Close();
            reader4.Close();
            reader3.Close();
            reader2.Close();
            reader1.Close();
            #endregion
        }

        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
