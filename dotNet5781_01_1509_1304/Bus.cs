using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNet5781_01_1509_1304
{
    /// <summary>
    ///                                               -- IMPORTANT --
    /// <-- The class based on that the gas container is for 400L (for 1200 KM according the exersise) -->
    /// 
    /// The first ctor get the license (the user will enter the license in the format) and get the date the bus started to drive.
    /// The second ctor get all of the parameters and match them.
    /// 
    /// Parameters:
    /// license number - the license of the bus.
    /// startActivity - the date the bus started to drive.
    /// lastTreatmrnt - the date of the last treatment of the bus.
    /// km - the number of KM since last treatment (when equal to 20,000 KM the bus can't do a drive until doing a treatment).
    /// mileage - the number of KM the bus did in total.
    /// gas - the fuel container [400L for 1200 KM (based on wiki)].
    /// 
    /// Functions:
    /// Refuling - set the gas parameter to 400L (max).
    /// DoTheDrive - check if the bus can do the drive (check: lastTreatment parameter, km parameter, gas parameter)
    /// if its possible, the bus do the drive, else a message will displayed.
    /// Treatment - set the lastTreatment parameter to DateTime.Now .
    /// PrintDetails - print the license, mileage parameter & the km parameter.
    /// </summary>
    class Bus
    {
        private string licenseNumber;
        public string LicenseNumber { get => licenseNumber; }

        private DateTime startActivity;
        private DateTime lastTreatment;

        private int km;
        private int mileage;
        private int gas;

        public Bus(string license, DateTime start) { licenseNumber = license;
            startActivity = start; 
            lastTreatment = start; km = 0;
            mileage = 0;
            gas = 400; 
        }
        public Bus(string license, DateTime start, DateTime Treatment, int KM, int Mileage, int Gas) { 
            licenseNumber = license;
            startActivity = start;
            lastTreatment = Treatment;
            km = KM;
            mileage = Mileage;
            gas = Gas % 401;
        }

        public void Refuling() { gas = 400; Console.WriteLine("Gas refuled!"); }
        public void DoTheDrive(int Km)
        {
            if (DateTime.Now >= lastTreatment.AddYears(1)) Console.WriteLine("You have to do a treatment!");
            else if ((km + Km) > 20000) Console.WriteLine("Too much killometers! do a treatment!");
            else if (((Km / 3) + 1) > gas) Console.WriteLine("You have to refuling the gas first!");
            else { km += Km; gas -= ((Km / 3) + 1); mileage += Km; }
        }
        public void Treatment() { km = 0; lastTreatment = DateTime.Now; Console.WriteLine("Treatment complete!"); }
        public void PrintDetails() { Console.WriteLine($"{licenseNumber}: {mileage}\\{km}."); }
    }
}