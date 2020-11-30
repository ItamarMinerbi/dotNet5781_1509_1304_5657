using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNet5781_02_1509_1304
{
    class Program
    {
        static void Main(string[] args)
        {
            List<BusStationLine> stations = new List<BusStationLine>();
            BusLinesController controller = new BusLinesController();
            
            StartValues(stations, controller);
            
            string msgOptions = @"1) Add.
2) Remove.
3) Search.
4) Print.
5) Exit.
";
            bool Continue = true;
            while (Continue)
            {
                Console.WriteLine(msgOptions);
                int key;
                if (int.TryParse(Console.ReadKey(true).KeyChar.ToString(), out key))
                {
                    Console.WriteLine($"You chose: {key}\n\n");
                    try
                    {
                        switch (key)
                        {
                            case 1:
                                Option1(stations, controller);
                                break;
                            case 2:
                                Option2(controller);
                                break;
                            case 3:
                                Option3(controller);
                                break;
                            case 4:
                                Option4(stations, controller);
                                break;
                            case 5:
                                Continue = false;
                                break;
                            default:
                                break;
                        }
                    }catch(Exception ex) { Console.WriteLine(ex.Message); }
                }
                if (Continue)
                {
                    Console.Write("\n\nPress any key to continue . . .");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

        }

        static BusStationLine InputStation()
        {
            Console.Write("Please enter station code: ");
            string code = Console.ReadLine();
            Console.Write("\nPlease enter the latitude: ");
            string latitude = Console.ReadLine();
            Console.Write("\nPlease enter the longitude: ");
            string longitude = Console.ReadLine();
            Console.Write("\nPlease enter the distance from the last station: ");
            string distance = Console.ReadLine();
            Console.Write("\nPlease enter the time from the last station (in minutes): ");
            string time = Console.ReadLine();
            Console.Write("\nPlease enter the address of station: ");
            string address = Console.ReadLine();
            Console.WriteLine("\n\n");
            return new BusStationLine(code, double.Parse(latitude), double.Parse(longitude),
                double.Parse(distance), int.Parse(time), address);
        }
        static Line InputLine(List<BusStationLine> stations)
        {
            Console.Write("Please enter line number: ");
            string lineNumber = Console.ReadLine();
            Console.Write(@"
Enter the number of the area of the line: [0 - General, 
1 - North, 
2 - South, 
3 - Center, 
4 - Jerusalem]");
            string area = Console.ReadLine();
            Console.WriteLine("\nHow many stations do you want in the line? ");
            int count = int.Parse(Console.ReadLine());
            List<BusStationLine> list = new List<BusStationLine>();
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine("\nLine number {0}:\n", i);
                BusStationLine busStationLine = InputStation();
                stations.Add(busStationLine);
                list.Add(busStationLine);
                Console.WriteLine("\n");
            }
            return new Line(lineNumber, list, (Line.Areas)int.Parse(area));
        }
        static void StartValues(List<BusStationLine> stations, BusLinesController controller)
        {
            Random rand = new Random();
            for (int i = 0; i < 40; i++)
            {
                int stationCode = 0;
                do { stationCode = rand.Next(100000, 999999); }
                while (stations.Exists(x => x.StationCode == stationCode.ToString()));

                stations.Add(new BusStationLine(
                    StationCode: stationCode.ToString(),
                    Latitude: rand.NextDouble() * (35.5 - 34.3) + 34.3,
                    Longitude: rand.NextDouble() * (33.3 - 31.0) + 31.0,
                    Distance: rand.NextDouble() * 100.0,
                    Time: rand.Next(1, 1440),
                    Address: ""));
            }

            for (int i = 0; i < 10; i++)
            {
                int lineNumber = rand.Next(1, 999);
                do { lineNumber = rand.Next(1, 999); }
                while (controller.BusLines.Exists(x => x.BusLine == lineNumber.ToString()));

                List<BusStationLine> list = new List<BusStationLine>();

                List<int> indexes = new List<int>();
                int count = rand.Next(10, 20);
                for(int j = 0; j < count; j++)
                {
                    int randIndex = rand.Next(0, stations.Count);
                    do { randIndex = rand.Next(0, stations.Count); }
                    while (indexes.Contains(randIndex));
                    list.Add(stations[randIndex]);
                    indexes.Add(randIndex);
                }
                int area = rand.Next(0, 5);
                controller.AddLine(new Line("" + lineNumber, list, (Line.Areas)area));
            }
        }


        static void Option1(List<BusStationLine> stations, BusLinesController controller)
        { 
            Console.WriteLine("For new bus line enter: L.\n" +
                "For new station in line enter: S [default=L]: ");
            ConsoleKeyInfo input = Console.ReadKey();
            if(input.KeyChar == 'S')
            {
                Console.Write("\nPlease enter station code: ");
                string stationCode = Console.ReadLine();
                Console.WriteLine("\n");
                AddNewStation(stations, controller, stationCode);
            }
            else
            {
                AddNewLine(stations, controller);
            }
        }
        static void AddNewStation(List<BusStationLine> stations, 
            BusLinesController controller, string code)
        {
            BusStationLine busStationLine = InputStation();
            stations.Add(busStationLine);
            controller[code].AddStation(busStationLine);
        }
        static void AddNewLine(List<BusStationLine> stations, BusLinesController controller)
        {
            controller.AddLine(InputLine(stations));
        }


        static void Option2(BusLinesController controller)
        {
            Console.WriteLine("For remove line enter: L.\n" +
                "For remove station from line enter: S [default=L]: ");
            ConsoleKeyInfo input = Console.ReadKey();
            if (input.KeyChar == 'S')
            {
                Console.Write("\nEnter the line number: ");
                string lineNumber = Console.ReadLine();
                Console.Write("\nEnter the station code: ");
                string stationCode = Console.ReadLine();
                Console.WriteLine("\n");
                RemoveStation(controller, lineNumber, stationCode);
            }
            else
            {
                Console.Write("\nEnter the line number: ");
                string lineNumber = Console.ReadLine();
                Console.WriteLine("\n");
                RemoveLine(controller, lineNumber);
            }
        }
        static void RemoveLine(BusLinesController controller, string lineNumber)
        {
            controller.RemoveLine(lineNumber);
        }
        static void RemoveStation(BusLinesController controller, 
            string lineNumber, string stationCode)
        {
            controller[lineNumber].RemoveStation(stationCode);
        }


        static void Option3(BusLinesController controller)
        {
            Console.WriteLine("For search lines that pass through station enter: L\n" +
                "For search lines that pass through two stations enter: S [default=L]: ");
            ConsoleKeyInfo input = Console.ReadKey();
            if (input.KeyChar == 'S')
            {
                Console.Write("\nEnter the first station code: ");
                string stationCode1 = Console.ReadLine();
                Console.Write("\nEnter the second station code: ");
                string stationCode2 = Console.ReadLine();
                Console.WriteLine("\n");
                SearchOptionsBetweenTwoStations(controller, stationCode1, stationCode2);
            }
            else
            {
                Console.Write("\nEnter the station code: ");
                string stationCode = Console.ReadLine();
                Console.WriteLine("\n");
                SearchLines(controller, stationCode);
            }
        }
        static void SearchLines(BusLinesController controller, string stationCode)
        {
            PrintAllLines(controller.WhoPassesThroughMe(stationCode));
        }
        static void SearchOptionsBetweenTwoStations(BusLinesController controller, 
            string stationCode1, string stationCode2)
        {
            BusLinesController part1 = controller.WhoPassesThroughMe(stationCode1);
            BusLinesController part2 = controller.WhoPassesThroughMe(stationCode2);
            BusLinesController commonElements = controller.CommonElements(part1, part2);
            commonElements.SortLines();
            PrintAllLines(commonElements);
        }


        static void Option4(List<BusStationLine> stations, BusLinesController controller)
        {
            Console.WriteLine("For printing all lines enter: L\n" +
                "For printing all stations enter: S [default=L]: ");
            ConsoleKeyInfo input = Console.ReadKey();
            if (input.KeyChar == 'S')
            {
                Console.WriteLine("\n");
                PrintAllStationsAndLines(stations, controller);
            }
            else
            {
                Console.WriteLine("\n");
                PrintAllLines(controller);
            }
        }
        static void PrintAllLines(BusLinesController controller)
        {
            foreach (Line item in controller)
            {
                Console.WriteLine("{0}\n", item);
            }
        }
        static void PrintAllStationsAndLines(List<BusStationLine> stations, 
            BusLinesController controller)
        {
            stations = stations.Distinct().ToList();
            foreach (var item in stations)
            {
                try
                {
                    Console.WriteLine("{0}:", item);
                    PrintAllLines(controller.WhoPassesThroughMe(item.StationCode));
                    Console.WriteLine("\n");
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
        }
    }
}