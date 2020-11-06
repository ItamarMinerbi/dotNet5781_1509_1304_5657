using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNet5781_01_1509_1304
{
    /// <summary>
    ///                                               -- IMPORTANT --
    /// <-- The class is based on the fact that the gas container is for 400L (for 1200 KM according to the exersise) -->
    /// 
    /// The first ctor get the license number (the user will enter the license number that is in the format) 
    /// and get the date of the bus that had started the drive.
    /// 
    /// The second ctor get all of the parameters and match them.
    /// 
    /// Parameters:
    /// licenseNumber - the license number of the bus (has a getter).
    /// startActivity - the date the bus had started to drive.
    /// lastTest - the date of the last test.
    /// km - the number of KM since last test (when equal to 20,000 KM the bus can't take a drive until doing a test).
    /// mileage - the number of KM the bus did in total.
    /// gas - the fuel container [400L for 1200 KM (based on wiki)].
    /// 
    /// Functions:
    /// Refuel - set the gas parameter to 400L (max).
    /// DoTheDrive - check if the bus can take the drive (check: lastTest parameter, km parameter, gas parameter)
    /// if its possible, the bus take the drive, else a message will be displayed.
    /// Test - set the lastTest parameter to DateTime.Now value.
    /// PrintDetails - print the license number, mileage parameter & the km parameter.
    /// </summary>
    class Bus
    {
        private string licenseNumber;
        public string LicenseNumber { get => licenseNumber; }

        private DateTime startActivity;
        private DateTime lastTest;

        private int km;
        private int mileage;
        private int gas;

        public Bus(string license, DateTime start) { licenseNumber = license;
            startActivity = start; 
            lastTest = start; km = 0;
            mileage = 0;
            gas = 400; 
        }
        public Bus(string license, DateTime start, DateTime Test, int KM, int Mileage, int Gas) { 
            licenseNumber = license;
            startActivity = start;
            lastTest = Test;
            km = KM;
            mileage = Mileage;
            gas = Gas % 401;
        }

        public void Reful() { gas = 400; Console.WriteLine("Gas refuled!"); }
        public void DoTheDrive(int Km)
        {
            if (DateTime.Now >= lastTest.AddYears(1)) Console.WriteLine("A year has passed! You have to take a test!");
            else if ((km + Km) > 20000) Console.WriteLine("Too many kilometers! take a test!");
            else if (((Km / 3) + 1) > gas) Console.WriteLine("You have to refuel the gas container!");
            else { km += Km; gas -= ((Km / 3) + 1); mileage += Km; }
        }
        public void Test() { km = 0; lastTest = DateTime.Now; Console.WriteLine("Test complete!"); }
        public void PrintDetails() { Console.WriteLine($"{licenseNumber}: {mileage}\\{km}."); }
    }
}