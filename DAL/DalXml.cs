using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using DO;
using DalExceptions;

namespace DAL
{
    sealed class DalXml : DalApi.IDAL
    {
        internal XElement LoadXml(string path)
        {
            try { return XElement.Load(path); }
            catch(Exception ex) { throw new XmlLoadException($"Could not find '{path}'" +
                $" Please make sure that file is exist.", ex); }
        }

        #region Pathes
        internal static readonly string path = Directory.GetCurrentDirectory()
                        + @"\XML Files\{0}.xml";
        internal readonly string configPath = String.Format(path, "config");
        internal readonly string usersPath = String.Format(path, "Users");
        internal readonly string stationsPath = String.Format(path, "Stations");
        internal readonly string linesPath = String.Format(path, "Lines");
        internal readonly string adjStationsPath = String.Format(path, "Adjacent Stations");
        internal readonly string stationsLinePath = String.Format(path, "Stations Line");
        internal readonly string lineTripsPath = String.Format(path, "Line Trips");
        internal readonly string lineTimingsPath = String.Format(path, "Line Timing");
        #endregion

        #region Singleton
        static readonly DalXml instance = new DalXml();
        static DalXml() { }
        DalXml() { }
        public static DalXml Instance => instance;
        #endregion

        #region User
        internal IEnumerable<User> LoadUsersFromXML(XElement usersRoot)
        {
            List<User> users = new List<User>();
            foreach (var user in usersRoot.Elements())
            {
                try { users.Add(new User
                    {
                        Username = user.Element("Username").Value,
                        Password = user.Element("Password").Value,
                        Email = user.Element("Email").Value,
                        IsAdmin = bool.Parse(user.Element("IsAdmin").Value),
                        IsActive = bool.Parse(user.Element("IsActive").Value)
                    });
                }
                catch(ArgumentNullException ex) { throw new XmlParametersException("Argument is null", ex); }
                catch(ArgumentException ex) { throw new XmlParametersException("Argument is not valid!", ex); }
                catch (FormatException ex) { throw new XmlParametersException("The format does not match!", ex); }
                catch (OverflowException ex) { throw new XmlParametersException("Argument is not valid!", ex); }
                catch (Exception ex) { throw new AnErrorOccurredException(ex.Message, ex); }
            }
            return users;
        }

        public int UsersCount()
        {
            return (from user in LoadUsersFromXML(LoadXml(usersPath))
                    where user.IsActive
                    select user).ToList().Count;
        }

        public void CreateUser(User user)
        {
            XElement usersRoot = LoadXml(usersPath);
            int index = LoadUsersFromXML(usersRoot).ToList().FindIndex(i =>
                                            (i.Username == user.Username ||
                                             i.Email == user.Email) &&
                                             i.IsActive);
            if(index != -1)
                throw new UserAlreadyExistException($"An existing user name or email on the system.");
            try {
                usersRoot.Add(XmlFunctions.BuildElementToXml(user));
                usersRoot.Save(usersPath);
            }
            catch(Exception ex) { throw new XmlWriteException(ex.Message, ex); }
        }

        public void RemoveUser(User user)
        {

        }

        public User CheckUser(User user)
        {
            XElement usersRoot = LoadXml(usersPath);
            var usersList = LoadUsersFromXML(usersRoot).ToList();
            int index = usersList.FindIndex(i => (i.Username == user.Username ||
                                                  i.Email == user.Email) &&
                                                  i.Password == user.Password &&
                                                  i.IsActive);
            if (index == -1)
                throw new UserDoesNotExistException($"Incorrect username or password.");
            User userReply = usersList[index].Clone();  // I'm not sure if it's like shared memory so I cloned it...
            userReply.Password = "";        // We don't want to return the password
            return userReply;
        }
        #endregion

        #region Files
        public IEnumerable<string> GetPathes()
        {
            yield return configPath;
            yield return usersPath;
            yield return stationsPath;
            yield return linesPath;
            yield return adjStationsPath;
            yield return stationsLinePath;
            yield return lineTripsPath;
        }
        #endregion

        #region Station
        internal IEnumerable<Station> LoadStationsFromXML(XElement stationsRoot)
        {
            List<Station> stations = new List<Station>();
            foreach (var station in stationsRoot.Elements())
            {
                try {
                    stations.Add(new Station
                    {
                        StationCode = int.Parse(station.Element("StationCode").Value),
                        Name = station.Element("Name").Value,
                        Latitude = double.Parse(station.Element("Latitude").Value),
                        Longitude = double.Parse(station.Element("Longitude").Value),
                        Address = station.Element("Address").Value,
                        IsActive = bool.Parse(station.Element("IsActive").Value)
                    });
                }
                catch (ArgumentNullException ex) { throw new XmlParametersException("Argument is null", ex); }
                catch (ArgumentException ex) { throw new XmlParametersException("Argument is not valid!", ex); }
                catch (FormatException ex) { throw new XmlParametersException("The format does not match!", ex); }
                catch (OverflowException ex) { throw new XmlParametersException("Argument is not valid!", ex); }
                catch (Exception ex) { throw new AnErrorOccurredException(ex.Message, ex); }
            }
            return stations;
        }

        public IEnumerable<Station> GetStations()
        {
            return from station in LoadStationsFromXML(LoadXml(stationsPath))
                   where station.IsActive
                   select station.Clone();
        }

        public int StationsCount()
        {
            return (from station in LoadStationsFromXML(LoadXml(stationsPath))
                    where station.IsActive
                    select station).ToList().Count;
        }

        public void CreateStation(Station station)
        {
            XElement stationsRoot = LoadXml(stationsPath);
            var stationsList = LoadStationsFromXML(stationsRoot).ToList();
            int index = stationsList.FindIndex(i =>
                                               i.StationCode == station.StationCode &&
                                               i.IsActive);

            if (index != -1)       // Station already exist
                throw new StationAlreadyExistException($"A station with the same code " +
                    $"was found in the system." +
                    $" Station code: {station.StationCode}.");
            try
            {
                stationsRoot.Add(XmlFunctions.BuildElementToXml(station.Clone()));
                stationsRoot.Save(stationsPath);
            }
            catch (Exception ex) { throw new XmlWriteException(ex.Message, ex); }
        }

        public void UpdateStation(Station station)
        {
            XElement stationsRoot = LoadXml(stationsPath);
            var stationsList = LoadStationsFromXML(stationsRoot).ToList();
            int index = stationsList.FindIndex(i =>
                                               i.StationCode == station.StationCode &&
                                               i.IsActive);

            if (index == -1)           // Station does not exist
                throw new StationDoesNotExistException($"A station with code: {station.StationCode}" +
                    $" was not found in the system.");
            stationsList[index] = station.Clone();
            stationsList.SaveToXml(stationsPath, stationsRoot.Name.ToString());
        }

        public Station RequestStation(int stationCode)
        {
            XElement stationsRoot = LoadXml(stationsPath);
            var stationsList = LoadStationsFromXML(stationsRoot).ToList();
            int index = stationsList.FindIndex(i =>
                                               i.StationCode == stationCode &&
                                               i.IsActive);

            if (index == -1)       // Station does not exist
                throw new StationDoesNotExistException($"A station with code: {stationCode}" +
                    $" was not found in the system.");
            return stationsList[index].Clone();
        }

        public void RemoveStation(int stationCode)
        {
            XElement stationsRoot = LoadXml(stationsPath);
            var stationsList = LoadStationsFromXML(stationsRoot).ToList();
            int index = stationsList.FindIndex(i =>
                                               i.StationCode == stationCode &&
                                               i.IsActive);

            if (index == -1)       // Station does not exist
                throw new StationDoesNotExistException($"A station with code: {stationCode}" +
                    $" was not found in the system.");
            stationsList[index].IsActive = false;
            stationsList.SaveToXml(stationsPath, stationsRoot.Name.ToString());
        }
        #endregion

        #region Line
        internal IEnumerable<Line> LoadLinesFromXML(XElement linesRoot)
        {
            List<Line> lines = new List<Line>();
            foreach (var line in linesRoot.Elements())
            {
                try {
                    lines.Add(new Line
                    {
                        ID = int.Parse(line.Element("ID").Value),
                        LineNumber = int.Parse(line.Element("LineNumber").Value),
                        FirstStation = int.Parse(line.Element("FirstStation").Value),
                        LastStation = int.Parse(line.Element("LastStation").Value),
                        Area = (Line.Areas)Enum.Parse(typeof(Line.Areas),
                                      line.Element("Area").Value, true),
                        IsActive = bool.Parse(line.Element("IsActive").Value)
                    });
                }
                catch (ArgumentNullException ex) { throw new XmlParametersException("Argument is null", ex); }
                catch (ArgumentException ex) { throw new XmlParametersException("Argument is not valid!", ex); }
                catch (FormatException ex) { throw new XmlParametersException("The format does not match!", ex); }
                catch (OverflowException ex) { throw new XmlParametersException("Argument is not valid!", ex); }
                catch (Exception ex) { throw new AnErrorOccurredException(ex.Message, ex); }
            }
            return lines;
        }

        public IEnumerable<Line> GetLines()
        {
            return from line in LoadLinesFromXML(LoadXml(linesPath))
                   where line.IsActive
                   select line.Clone();
        }

        public IEnumerable<Line> GetLine(int lineNumber)
        {
            return from line in LoadLinesFromXML(LoadXml(linesPath))
                   where line.IsActive && line.LineNumber == lineNumber
                   select line.Clone();
        }

        public int LinesCount()
        {
            return (from line in LoadLinesFromXML(LoadXml(linesPath))
                    where line.IsActive
                    select line).ToList().Count;
        }

        public int CreateLine(Line line)
        {
            XElement configRoot = LoadXml(configPath);
            int Id = int.Parse(configRoot.Element("ID").Value);
            line.ID = ++Id;
            configRoot.SetElementValue("ID", Id);
            configRoot.Save(configPath);

            XElement linesRoot = LoadXml(linesPath);
            var linesList = LoadLinesFromXML(linesRoot).ToList();

            int index = linesList.FindIndex(i =>
                                            i.ID == line.ID &&
                                            i.IsActive);

            if (index != -1)       // Line already exist
                throw new LineAlreadyExistException($"A line with same id " +
                    $"was found in the system. Line id: {line.ID}.");
            try
            {
                linesRoot.Add(XmlFunctions.BuildElementToXml(line.Clone()));
                linesRoot.Save(linesPath);
            }
            catch (Exception ex) { throw new XmlWriteException(ex.Message, ex); }
            return Id;
        }

        public void UpdateLine(Line line)
        {
            XElement linesRoot = LoadXml(linesPath);
            var linesList = LoadLinesFromXML(linesRoot).ToList();
            int index = linesList.FindIndex(i =>
                                            i.ID == line.ID &&
                                            i.IsActive);

            if (index == -1)       // Line does not exist
                throw new LineDoesNotExistException($"A line with id: {line.ID}" +
                    $" was not found in the system.");
            linesList[index] = line.Clone();
            linesList.SaveToXml(linesPath, linesRoot.Name.ToString());
        }

        public Line RequestLine(int Id)
        {
            XElement linesRoot = LoadXml(linesPath);
            var linesList = LoadLinesFromXML(linesRoot).ToList();
            int index = linesList.FindIndex(i =>
                                            i.ID == Id &&
                                            i.IsActive);

            if (index == -1)       // Line does not exist
                throw new LineDoesNotExistException($"A line with id: {Id}" +
                    $" was not found in the system.");
            return linesList[index].Clone();
        }

        public void RemoveLine(int Id)
        {
            XElement linesRoot = LoadXml(linesPath);
            var linesList = LoadLinesFromXML(linesRoot).ToList();
            int index = linesList.FindIndex(i =>
                                            i.ID == Id &&
                                            i.IsActive);

            if (index == -1)       // Line does not exist
                throw new LineDoesNotExistException($"A line with id: {Id}" +
                    $" was not found in the system.");
            linesList[index].IsActive = false;
            linesList.SaveToXml(linesPath, linesRoot.Name.ToString());
        }
        #endregion

        #region AdjStation
        internal IEnumerable<AdjStations> LoadAdjStationsFromXML(XElement adjStationsRoot)
        {
            List<AdjStations> adjStations = new List<AdjStations>();
            foreach (var adjStation in adjStationsRoot.Elements())
            {
                try {
                    adjStations.Add(new AdjStations
                    {
                        StationCode1 = int.Parse(adjStation.Element("StationCode1").Value),
                        StationCode2 = int.Parse(adjStation.Element("StationCode2").Value),
                        Distance = double.Parse(adjStation.Element("Distance").Value),
                        Time = TimeSpan.Parse(adjStation.Element("Time").Value),
                        IsActive = bool.Parse(adjStation.Element("IsActive").Value)
                    });
                }
                catch (ArgumentNullException ex) { throw new XmlParametersException("Argument is null", ex); }
                catch (ArgumentException ex) { throw new XmlParametersException("Argument is not valid!", ex); }
                catch (FormatException ex) { throw new XmlParametersException("The format does not match!", ex); }
                catch (OverflowException ex) { throw new XmlParametersException("Argument is not valid!", ex); }
                catch (Exception ex) { throw new AnErrorOccurredException(ex.Message, ex); }
            }
            return adjStations;
        }

        public IEnumerable<AdjStations> GetAdjStations()
        {
            return from adjStations in LoadAdjStationsFromXML(LoadXml(adjStationsPath))
                   where adjStations.IsActive
                   select adjStations.Clone();
        }

        public int AdjStationsCount()
        {
            return (from adjStations in LoadAdjStationsFromXML(LoadXml(adjStationsPath))
                    where adjStations.IsActive
                    select adjStations).ToList().Count;
        }

        public void CreateAdjStations(AdjStations adjStations)
        {
            XElement adjStationsRoot = LoadXml(adjStationsPath);
            var adjStationsList = LoadAdjStationsFromXML(adjStationsRoot).ToList();
            int index = adjStationsList.FindIndex(i =>
                                                  i.StationCode1 == adjStations.StationCode1 &&
                                                  i.StationCode2 == adjStations.StationCode2 &&
                                                  i.IsActive);

            if (index != -1)       // AdjStations already exist
                throw new AdjStationsAlreadyExistException($"An AdjStation with same station codes " +
                    $"was found in the system. " +
                    $"Code Station1: {adjStations.StationCode1}, Code Station2: {adjStations.StationCode2}.");
            try
            {
                adjStationsRoot.Add(XmlFunctions.BuildElementToXml(adjStations.Clone()));
                adjStationsRoot.Save(adjStationsPath);
            }
            catch (Exception ex) { throw new XmlWriteException(ex.Message, ex); }
        }

        public void UpdateAdjStations(AdjStations adjStations)
        {
            XElement adjStationsRoot = LoadXml(adjStationsPath);
            var adjStationsList = LoadAdjStationsFromXML(adjStationsRoot).ToList();
            int index = adjStationsList.FindIndex(i =>
                                                  i.StationCode1 == adjStations.StationCode1 &&
                                                  i.StationCode2 == adjStations.StationCode2 &&
                                                  i.IsActive);

            if (index == -1)       // AdjStations does not exist
                throw new AdjStationsDoesNotExistException($"An AdjStation with codes:" +
                    $" ({adjStations.StationCode1}, {adjStations.StationCode2})" +
                    $" was not found in the system.");
            adjStationsList[index] = adjStations.Clone();
            adjStationsList.SaveToXml(adjStationsPath, adjStationsRoot.Name.ToString());
        }

        public AdjStations RequestAdjStations(int stationCode1, int stationCode2)
        {
            XElement adjStationsRoot = LoadXml(adjStationsPath);
            var adjStationsList = LoadAdjStationsFromXML(adjStationsRoot).ToList();
            int index = adjStationsList.FindIndex(i =>
                                                  i.StationCode1 == stationCode1 &&
                                                  i.StationCode2 == stationCode2 &&
                                                  i.IsActive);

            if (index == -1)       // AdjStations does not exist
                throw new AdjStationsDoesNotExistException($"An Adjacent Stations with codes:" +
                    $" ({stationCode1}, {stationCode2}) was not found in the system.");
            return adjStationsList[index].Clone();
        }

        public void RemoveAdjStations(int stationCode1, int stationCode2)
        {
            XElement adjStationsRoot = LoadXml(adjStationsPath);
            var adjStationsList = LoadAdjStationsFromXML(adjStationsRoot).ToList();
            int searchIndex = adjStationsList.FindIndex(i =>
                                                        i.StationCode1 == stationCode1 &&
                                                        i.StationCode2 == stationCode2 &&
                                                        i.IsActive);

            if (searchIndex == -1)       // AdjStations does not exist
                throw new AdjStationsDoesNotExistException($"An AdjStation with codes:" +
                    $" ({stationCode1}, {stationCode2}) was not found in the system.");
            adjStationsList[searchIndex].IsActive = false;
            adjStationsList.SaveToXml(adjStationsPath, adjStationsRoot.Name.ToString());
        }
        #endregion

        #region StationLine
        internal IEnumerable<StationLine> LoadStationLineFromXML(XElement stationLineRoot)
        {
            List<StationLine> stationLines = new List<StationLine>();
            foreach (var stationLine in stationLineRoot.Elements())
            {
                try {
                    stationLines.Add(new StationLine
                    {
                        ID = int.Parse(stationLine.Element("ID").Value),
                        StationCode = int.Parse(stationLine.Element("StationCode").Value),
                        StationNumberInLine = int.Parse(stationLine.Element("StationNumberInLine").Value),
                        IsActive = bool.Parse(stationLine.Element("IsActive").Value)
                    });
                }
                catch (ArgumentNullException ex) { throw new XmlParametersException("Argument is null", ex); }
                catch (ArgumentException ex) { throw new XmlParametersException("Argument is not valid!", ex); }
                catch (FormatException ex) { throw new XmlParametersException("The format does not match!", ex); }
                catch (OverflowException ex) { throw new XmlParametersException("Argument is not valid!", ex); }
                catch (Exception ex) { throw new AnErrorOccurredException(ex.Message, ex); }
            }
            return stationLines;
        }

        public IEnumerable<StationLine> GetStationLines()
        {
            return from stationLine in LoadStationLineFromXML(LoadXml(stationsLinePath))
                   where stationLine.IsActive
                   select stationLine.Clone();
        }

        public IEnumerable<StationLine> GetStationLinesInStation(int stationCode)
        {
            return from stationLine in LoadStationLineFromXML(LoadXml(stationsLinePath))
                   where stationLine.IsActive &&
                         stationLine.StationCode == stationCode
                   select stationLine.Clone();
        }

        public int StationsLineCount()
        {
            return (from stationLine in LoadStationLineFromXML(LoadXml(stationsLinePath))
                    where stationLine.IsActive
                    select stationLine).ToList().Count;
        }

        public void CreateStationLine(StationLine stationLine)
        {
            XElement stationLineRoot = LoadXml(stationsLinePath);
            var stationLineList = LoadStationLineFromXML(stationLineRoot).ToList();
            int index = stationLineList.FindIndex(i =>
                                                  i.ID == stationLine.ID &&
                                                  i.StationCode == stationLine.StationCode &&
                                                  i.IsActive);

            if (index != -1)       // StationLine already exist
                throw new StationLineAlreadyExistException($"A StationLine with same ID " +
                    $"and StationCode was found in the system. " +
                    $"ID: {stationLine.ID}, StationCode: {stationLine.StationCode}.");
            try
            {
                stationLineRoot.Add(XmlFunctions.BuildElementToXml(stationLine.Clone()));
                stationLineRoot.Save(stationsLinePath);
            }
            catch (Exception ex) { throw new XmlWriteException(ex.Message, ex); }
        }

        public void UpdateStationLine(StationLine stationLine)
        {
            XElement stationLineRoot = LoadXml(stationsLinePath);
            var stationLineList = LoadStationLineFromXML(stationLineRoot).ToList();
            int index = stationLineList.FindIndex(i =>
                                                  i.ID == stationLine.ID &&
                                                  i.StationCode == stationLine.StationCode &&
                                                  i.IsActive);

            if (index == -1)       // StationLine does not exist
                throw new StationLineDoesNotExistException($"A StationLine with ID: {stationLine.ID} " +
                    $"and Stationcode: {stationLine.StationCode} was not found in the system.");
            stationLineList[index] = stationLine.Clone();
            stationLineList.SaveToXml(stationsLinePath, stationLineRoot.Name.ToString());
        }

        public StationLine RequestStationLine(int Id, int stationCode)
        {
            XElement stationLineRoot = LoadXml(stationsLinePath);
            var stationLineList = LoadStationLineFromXML(stationLineRoot).ToList();
            int index = stationLineList.FindIndex(i =>
                                                  i.ID == Id &&
                                                  i.StationCode == stationCode &&
                                                  i.IsActive);

            if (index == -1)       // StationLine does not exist
                throw new StationLineDoesNotExistException($"A StationLine with ID: {Id} " +
                    $"and Stationcode: {stationCode} was not found in the system.");
            return stationLineList[index].Clone();
        }

        public StationLine RequestStationLineByIndex(int Id, int numberInLine)
        {
            XElement stationLineRoot = LoadXml(stationsLinePath);
            var stationLineList = LoadStationLineFromXML(stationLineRoot).ToList();
            int index = stationLineList.FindIndex(i =>
                                                  i.ID == Id &&
                                                  i.StationNumberInLine == numberInLine &&
                                                  i.IsActive);

            if (index == -1)       // StationLine does not exist
                throw new StationLineDoesNotExistException($"A StationLine with ID: {Id} " +
                    $"in the: {numberInLine} index, was not found in the system.");
            return stationLineList[index].Clone();
        }

        public void RemoveStationLine(int Id, int stationCode)
        {
            XElement stationLineRoot = LoadXml(stationsLinePath);
            var stationLineList = LoadStationLineFromXML(stationLineRoot).ToList();
            int index = stationLineList.FindIndex(i =>
                                                  i.ID == Id &&
                                                  i.StationCode == stationCode &&
                                                  i.IsActive);

            if (index == -1)       // StationLine does not exist
                throw new StationLineDoesNotExistException($"A StationLine with ID: {Id} " +
                    $"and Stationcode: {stationCode} was not found in the system.");
            stationLineList[index].IsActive = false;
            stationLineList.SaveToXml(stationsLinePath, stationLineRoot.Name.ToString());
        }
        #endregion

        #region LineTrip
        internal IEnumerable<LineTrip> LoadLineTripsFromXML(XElement lineTripsRoot)
        {
            List<LineTrip> lineTrips = new List<LineTrip>();
            foreach (var lineTrip in lineTripsRoot.Elements())
            {
                try {
                    lineTrips.Add(new LineTrip
                    {
                        ID = int.Parse(lineTrip.Element("ID").Value),
                        StartTime = TimeSpan.Parse(lineTrip.Element("StartTime").Value),
                        EndTime = TimeSpan.Parse(lineTrip.Element("EndTime").Value),
                        Frequency = TimeSpan.Parse(lineTrip.Element("Frequency").Value),
                        IsActive = bool.Parse(lineTrip.Element("IsActive").Value)
                    });
                }
                catch (ArgumentNullException ex) { throw new XmlParametersException("Argument is null", ex); }
                catch (ArgumentException ex) { throw new XmlParametersException("Argument is not valid!", ex); }
                catch (FormatException ex) { throw new XmlParametersException("The format does not match!", ex); }
                catch (OverflowException ex) { throw new XmlParametersException("Argument is not valid!", ex); }
                catch (Exception ex) { throw new AnErrorOccurredException(ex.Message, ex); }
            }
            return lineTrips;
        }

        public IEnumerable<LineTrip> GetLineTrips()
        {
            return from lineTrip in LoadLineTripsFromXML(LoadXml(lineTripsPath))
                   where lineTrip.IsActive
                   select lineTrip.Clone();
        }

        public IEnumerable<LineTrip> GetLineTripsByPredicate(Predicate<LineTrip> predicate)
        {
            return from lineTrip in LoadLineTripsFromXML(LoadXml(lineTripsPath))
                   where lineTrip.IsActive &&
                         predicate(lineTrip)
                   select lineTrip.Clone();
        }

        public int LineTripsCount()
        {
            return (from lineTrip in LoadLineTripsFromXML(LoadXml(lineTripsPath))
                    where lineTrip.IsActive
                    select lineTrip).ToList().Count;
        }

        public void CreateLineTrip(LineTrip lineTrip)
        {
            XElement lineTripsRoot = LoadXml(lineTripsPath);
            var lineTripsList = LoadLineTripsFromXML(lineTripsRoot).ToList();
            int index = lineTripsList.FindIndex(i =>
                                                i.ID == lineTrip.ID &&
                                                i.StartTime == lineTrip.StartTime &&
                                                i.IsActive);
            if (index != -1)   // line trip already exist
                throw new LineTripAlreadyExistException($"A line with ID: {lineTrip.ID} and " +
                    $"start time: {lineTrip.StartTime} was found in the system.");
            try
            {
                lineTripsRoot.Add(XmlFunctions.BuildElementToXml(lineTrip.Clone()));
                lineTripsRoot.Save(lineTripsPath);
            }
            catch (Exception ex) { throw new XmlWriteException(ex.Message, ex); }
        }

        public void UpdateLineTrip(LineTrip lineTrip)
        {
            XElement lineTripsRoot = LoadXml(lineTripsPath);
            var lineTripsList = LoadLineTripsFromXML(lineTripsRoot).ToList();
            int index = lineTripsList.FindIndex(i =>
                                                i.ID == lineTrip.ID &&
                                                i.StartTime == lineTrip.StartTime &&
                                                i.IsActive);
            if (index == -1)   // line trip does not exist
                throw new LineTripDoesNotExistException($"The line with ID: {lineTrip.ID} has no " +
                    $"trips that start at {lineTrip.StartTime}.");
            lineTripsList[index] = lineTrip.Clone();
            lineTripsList.SaveToXml(lineTripsPath, lineTripsRoot.Name.ToString());
        }

        public LineTrip RequestLineTrip(int Id, TimeSpan startTime)
        {
            XElement lineTripsRoot = LoadXml(lineTripsPath);
            var lineTripsList = LoadLineTripsFromXML(lineTripsRoot).ToList();
            int index = lineTripsList.FindIndex(i =>
                                                i.ID == Id &&
                                                i.StartTime == startTime &&
                                                i.IsActive);
            if (index == -1)   // line trip does not exist
                throw new LineTripDoesNotExistException($"The line with ID: {Id} has no " +
                    $"trips that start at {startTime}.");
            return lineTripsList[index].Clone();
        }

        public LineTrip RequestLineTripByPredicate(Predicate<LineTrip> predicate)
        {
            XElement lineTripsRoot = LoadXml(lineTripsPath);
            var lineTripsList = LoadLineTripsFromXML(lineTripsRoot).ToList();
            var res = lineTripsList.Find(predicate);
            if (res == null)
                throw new LineTripDoesNotExistException($"No line was match with the predicate.");
            return res.Clone();
        }

        public void RemoveLineTrip(int Id, TimeSpan startTime)
        {
            XElement lineTripsRoot = LoadXml(lineTripsPath);
            var lineTripsList = LoadLineTripsFromXML(lineTripsRoot).ToList();
            int index = lineTripsList.FindIndex(i =>
                                                i.ID == Id &&
                                                i.StartTime == startTime &&
                                                i.IsActive);
            if (index == -1)   // line trip does not exist
                throw new LineTripDoesNotExistException($"The line with ID: {Id} has no " +
                    $"trips that start at {startTime}.");
            lineTripsList[index].IsActive = false;
            lineTripsList.SaveToXml(lineTripsPath, lineTripsRoot.Name.ToString());
        }
        #endregion

        #region LineTiming
        internal IEnumerable<LineTiming> LoadLineTimingsFromXML(XElement lineTimingsRoot)
        {
            List<LineTiming> lineTimings = new List<LineTiming>();
            foreach (var lineTiming in lineTimingsRoot.Elements())
            {
                try
                {
                    lineTimings.Add(new LineTiming
                    {
                        ID = int.Parse(lineTiming.Element("ID").Value),
                        License = int.Parse(lineTiming.Element("License").Value),
                        LineID = int.Parse(lineTiming.Element("LineID").Value),
                        StartTime = TimeSpan.Parse(lineTiming.Element("StartTime").Value),
                        ActualStartTime = TimeSpan.Parse(lineTiming.Element("ActualStartTime").Value),
                        LastStation = int.Parse(lineTiming.Element("LastStation").Value),
                        LastStationTime = TimeSpan.Parse(lineTiming.Element("LastStationTime").Value),
                        ArrivalTime = TimeSpan.Parse(lineTiming.Element("ArrivalTime").Value),
                    });
                }
                catch (ArgumentNullException ex) { throw new XmlParametersException("Argument is null", ex); }
                catch (ArgumentException ex) { throw new XmlParametersException("Argument is not valid!", ex); }
                catch (FormatException ex) { throw new XmlParametersException("The format does not match!", ex); }
                catch (OverflowException ex) { throw new XmlParametersException("Argument is not valid!", ex); }
                catch (Exception ex) { throw new AnErrorOccurredException(ex.Message, ex); }
            }
            return lineTimings;
        }

        public IEnumerable<LineTiming> GetLineTimings()
        {
            return from lineTiming in LoadLineTimingsFromXML(LoadXml(lineTimingsPath))
                   select lineTiming.Clone();
        }

        public int CreateLineTiming(LineTiming line)
        {
            XElement configRoot = LoadXml(configPath);
            int Id = int.Parse(configRoot.Element("LineTimingID").Value);
            line.ID = ++Id;
            configRoot.SetElementValue("LineTimingID", Id);
            configRoot.Save(configPath);

            XElement lineTimingsRoot = LoadXml(lineTimingsPath);
            var lineTripsList = LoadLineTimingsFromXML(lineTimingsRoot).ToList();
            int index = lineTripsList.FindIndex(i =>
                                                i.ID == line.ID &&
                                                i.License == line.License &&
                                                i.LineID == line.LineID &&
                                                i.StartTime == line.StartTime);

            if (index != -1)
                throw new LineTimingAlreadyExistException($"A line timing with same values" +
                    $" was found in the system.");
            try
            {
                lineTimingsRoot.Add(XmlFunctions.BuildElementToXml(line.Clone()));
                lineTimingsRoot.Save(lineTimingsPath);
            }
            catch (Exception ex) { throw new XmlWriteException(ex.Message, ex); }
            return Id;
        }

        public LineTiming RequestLineTiming(int Id)
        {
            XElement lineTimingsRoot = LoadXml(lineTimingsPath);
            var lineTripsList = LoadLineTimingsFromXML(lineTimingsRoot).ToList();
            int index = lineTripsList.FindIndex(i => i.ID == Id);

            if (index == -1)
                throw new LineDoesNotExistException($"A line timing with this ID" +
                    $" was not found in the system.");
            return lineTripsList[index].Clone();
        }

        public void RemoveLineTiming(int Id)
        {
            XElement lineTimingsRoot = LoadXml(lineTimingsPath);
            var lineTripsList = LoadLineTimingsFromXML(lineTimingsRoot).ToList();
            int index = lineTripsList.FindIndex(i => i.ID == Id);

            if (index == -1)
                throw new LineDoesNotExistException($"A line timing with this ID" +
                    $" was not found in the system.");
            lineTripsList.RemoveAt(index);
            lineTripsList.SaveToXml(lineTimingsPath, lineTimingsRoot.Name.ToString());
        }
        #endregion
    }
}