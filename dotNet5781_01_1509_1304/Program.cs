using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNet5781_01_1509_1304
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Bus> company = new List<Bus>();
            bool Continue = true;

            string msgOptions = @"1) Add a bus.
2) Choose a bus to take a drive.
3) Refuel the gas or do a test.
4) Print buses details.
5) Exit.
";
            while (Continue)
            {
                Console.WriteLine(msgOptions);
                int key;
                if (int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out key))
                {
                    Console.WriteLine($"You chose: {key}\n\n");
                    switch (key)
                    {
                        case 1:
                            AddABus(company);
                            break;
                        case 2:
                            DriveABus(company);
                            break;
                        case 3:
                            ReafulOrTreatmentABus(company);
                            break;
                        case 4:
                            PrintBusesDetails(company);
                            break;
                        case 5:
                            Continue = false;
                            break;
                        default:
                            break;
                    }
                }
                if (Continue)
                {
                    Console.Write("\n\nPress any key to continue . . .");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        /// <summary>
        /// Option 1.
        ///  Add a bus to the company.
        /// </summary>
        /// <param name="company"> our company </param>
        static void AddABus(List<Bus> company)
        {
            DateTime date = EnterDate();
            Console.WriteLine("Enter the licene number please: ");
            string license = EnterLicense(date);
            company.Add(new Bus(license, date));
            Console.WriteLine($"Bus: {license} added!");
        }

        /// <summary>
        /// Option 2.
        ///  Choose a bus for a drive. If the bus does not exist in the company, a message will be displayed.
        /// </summary>
        /// <param name="company"> our company </param>
        static void DriveABus(List<Bus> company)
        {
            Console.WriteLine("Enter the licene number please: ");
            string license = Console.ReadLine();
            int index = FindIndex(company, license);
            if (index >= 0)
                company[index].DoTheDrive(new Random().Next(0, 1200));
            else
                Console.WriteLine("Bus was not found!\n");
        }

        /// <summary>
        /// Option 3.
        ///  Take a test or refuel the gas. If the bus does not exist in the company, a message will be displayed.
        /// </summary>
        /// <param name="company"> our company </param>
        static void ReafulOrTreatmentABus(List<Bus> company)
        {
            Console.WriteLine("Enter the licene number please: ");
            string license = Console.ReadLine();
            int index = FindIndex(company, license);
            if (index >= 0)
            {
                Console.Write("refuel or test (deafault=refuel): ");
                string input = Console.ReadLine();
                if (input == "test")
                    company[index].Test();
                else
                    company[index].Reful();
            }
            else
                Console.WriteLine("Bus was not found!\n");
        }

        /// <summary>
        /// Option 4.
        ///  Print the details of each bus of the company.
        /// </summary>
        /// <param name="company"> our company </param>
        static void PrintBusesDetails(List<Bus> company)
        {
            for (int i = 0; i < company.Count; i++)
                company[i].PrintDetails();
        }

        /// <summary>
        /// Enter the license number in format:
        /// 2018+: NNN-NN-NNN
        /// 2018-: NN-NNN-NN
        /// </summary>
        /// <param name="date"> the date of the bus </param>
        /// <returns> the license number in the required format </returns>
        static string EnterLicense(DateTime date)
        {/*
            This function is very ugly. I didn't have enough time to think about something better.         
         */
            string result = "";
            if (date.Year < 2018)
            {
                for (int i = 0; i < 9;)
                {
                    ConsoleKeyInfo input = Console.ReadKey(true);
                    if (input.Key == ConsoleKey.Backspace)
                    {
                        if (i != 0 && i != 3 && i != 7)
                        {
                            Console.CursorLeft = i - 1;
                            Console.Write(' ');
                            Console.CursorLeft = i - 1;
                            result = result.Substring(0, i - 1);
                            i--;
                        }
                        else if (i == 7 || i == 3)
                        {
                            Console.CursorLeft = i - 2;
                            Console.Write("  ");
                            Console.CursorLeft = i - 2;
                            result = result.Substring(0, i - 2);
                            i -= 2;
                        }
                    }
                    else if (Char.IsDigit(input.KeyChar))
                    {
                        Console.Write(input.KeyChar);
                        result += input.KeyChar;
                        i++;
                    }
                    if (i == 2 || i == 6)
                    {
                        result += "-";
                        Console.Write("-");
                        i++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 10;)
                {
                    ConsoleKeyInfo input = Console.ReadKey(true);
                    if (input.Key == ConsoleKey.Backspace)
                    {
                        if (i != 0 && i != 4 && i != 7)
                        {
                            Console.CursorLeft = i - 1;
                            Console.Write(' ');
                            Console.CursorLeft = i - 1;
                            result = result.Substring(0, i - 1);
                            i--;
                        }
                        else if (i == 4 || i == 7)
                        {
                            Console.CursorLeft = i - 2;
                            Console.Write("  ");
                            Console.CursorLeft = i - 2;
                            result = result.Substring(0, i - 2);
                            i -= 2;
                        }
                    }
                    else if (Char.IsDigit(input.KeyChar))
                    {
                        Console.Write(input.KeyChar);
                        result += input.KeyChar;
                        i++;
                    }
                    if (i == 3 || i == 6)
                    {
                        result += "-";
                        Console.Write("-");
                        i++;
                    }
                }
            }
            Console.WriteLine("\n");
            return result;
        }

        /// <summary>
        /// User enter date in format:
        ///  "DayDay/MonthMonth/YearYearYearYear"
        ///  or: "dd/MM/yyyy"
        /// </summary>
        /// <returns> the date user entered </returns>
        static DateTime EnterDate()
        {
            Console.WriteLine("Enter date: (the format is 'dd/MM/yyyy')");
            DateTime result = new DateTime();
            if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", new System.Globalization.CultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out result))
            {
                Console.WriteLine("\nThe date you entered doesn't match the format.\nPlease enter again.\n");
                return EnterDate(); 
            }
            return result;
        }

        /// <summary>
        ///   Find the bus index of the company
        /// </summary>
        /// <param name="company"> the company of the buses </param>
        /// <param name="license"> the license number (bus) the user search </param>
        /// <returns> return the index in the company. if was not found, return -1 </returns>
        static int FindIndex(List<Bus> company, string license)
        {
            for (int i = 0; i < company.Count; i++)
                if (company[i].LicenseNumber == license)
                    return i;
            return -1;
        }
    }
}