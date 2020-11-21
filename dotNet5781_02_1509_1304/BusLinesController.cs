using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNet5781_02_1509_1304
{
    class BusLinesController : IEnumerable
    {
        private List<Line> busLines;
        public List<Line> BusLines { get => busLines; }

        public IEnumerator GetEnumerator() { return new MyEnumerator(this); }
        class MyEnumerator : IEnumerator
        {
            List<Line> buses;
            public MyEnumerator(BusLinesController controller) { buses = controller.busLines; }
            int position = -1;
            public object Current => buses[position];
            public bool MoveNext() { return (++position < buses.Count); }
            public void Reset() { }
        }

        public BusLinesController()
        {
            busLines = new List<Line>();
        }
        public BusLinesController(List<Line> lines)
        {
            busLines = lines;
        }

        public void AddLine(Line line)
        {
            if (busLines.Exists(i => i.BusLine == line.BusLine))
            {
                Line index = busLines.Find(i => i.BusLine == line.BusLine);
                index.Stations.Reverse();
                if (line.Stations == index.Stations && !busLines.Exists(i => i.Stations == index.Stations))
                {
                    busLines.Add(line);
                }
                else
                {
                    throw new ArgumentException("Bus already exist!");
                }
            }
            else
            {
                busLines.Add(line);
            }
        }
        public void RemoveLine(string lineNumber)
        {
            int index1 = busLines.FindIndex(i => i.BusLine == lineNumber);
            int index2 = busLines.FindLastIndex(i => i.BusLine == lineNumber);
            if (index1 >= 0)
            {
                if (index1 == index2)
                    busLines.RemoveAt(index1);
                else
                {
                    Console.WriteLine("Two lines found:\n{0}\n{1}\n" +
                        "Which one you want to remove? [1/2] (default 1): "
                        , busLines[index1], busLines[index2]);
                    ConsoleKeyInfo input = Console.ReadKey();
                    if (input.KeyChar == '2')
                        busLines.RemoveAt(index2);
                    else
                        busLines.RemoveAt(index1);
                }
            }
            else
            {
                throw new ArgumentException("Bus does not exist!");
            }
        }

        public BusLinesController WhoPassesThroughMe(string stationCode)
        {
            BusLinesController result = new BusLinesController();
            foreach (Line BusLine in busLines)
                foreach (BusStationLine station in BusLine.Stations)
                    if (station.StationCode == stationCode)
                        result.AddLine(BusLine);
            if(result.busLines.Count > 0)
                return result;
            throw new ArgumentException("No line was found!");
        }
        public BusLinesController CommonElements(BusLinesController controller1,
            BusLinesController controller2)
        {
            return new BusLinesController(controller1.busLines.FindAll(
                i => controller2.busLines.Exists(
                    x => x.StationCode == i.StationCode)));
        }

        public void SortLines()
        {
            busLines.Sort();
        }
        public Line this[string index]
        {
            get
            {
                foreach (var item in busLines)
                    if (item.BusLine == index)
                        return item;
                throw new ArgumentException("Bus does not exist");
            }
        }
    }
}