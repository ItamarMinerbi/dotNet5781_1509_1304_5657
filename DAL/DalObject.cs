using System;
using System.Collections.Generic;
using System.Linq;

using DO;
using DalExceptions;

namespace DAL
{
    sealed class DalObject : DalApi.IDAL
    {
        private static DS.DataSource ds = new DS.DataSource();

        #region Singleton
        static readonly DalObject instance = new DalObject();
        static DalObject() { }
        DalObject() { }
        public static DalObject Instance => instance;
        #endregion

        #region User
        public IEnumerable<User> GetUsers()
        {
            return from user in ds.Users
                   where user.IsActive
                   select new User            // select the user without the password
                   {
                       Username = user.Username,
                       Email = user.Email,
                       IsAdmin = user.IsAdmin,
                       IsActive = user.IsActive
                   };
        }

        public int UsersCount()
        {
            return (from user in ds.Users
                    where user.IsActive
                    select user).ToList().Count;
        }

        public void CreateUser(User user)
        {
            int searchIndex = ds.Users.FindIndex(i =>
                              (i.Username == user.Username || i.Email == user.Email) &&
                              i.IsActive);

            if (searchIndex != -1)          // User already exist
                throw new UserAlreadyExistException($"An existing user name or email on the system.");
            ds.Users.Add(user.Clone());
        }

        public void RemoveUser(User user)
        {
            int searchIndex = ds.Users.FindIndex(i =>
                              (i.Username == user.Username ||
                                i.Email == user.Email) &&
                               i.Password == user.Password &&
                               i.IsActive);

            if (searchIndex == -1)          // User does not exist
                throw new UserDoesNotExistException($"Incorrect username or password.");
            ds.Users[searchIndex].IsActive = false;
        }

        public User CheckUser(User user)
        {
            int searchIndex = ds.Users.FindIndex(i =>
                              (i.Username == user.Username || i.Email == user.Email) &&
                              i.Password == user.Password &&
                              i.IsActive);

            if (searchIndex == -1)          // User does not exist
                throw new UserDoesNotExistException($"Incorrect username or password.");
            User userReply = ds.Users[searchIndex].Clone();
            userReply.Password = "";        // We don't want to return the password
            return userReply;
        }
        #endregion

        #region Files
        public IEnumerable<string> GetPathes() => new List<string>();
        #endregion

        #region Bus
        public IEnumerable<Bus> GetBuses()
        {
            return from bus in ds.Buses
                   where bus.IsActive
                   select bus.Clone();
        }

        public int BusesCount()
        {
            return (from bus in ds.Buses
                    where bus.IsActive
                    select bus).ToList().Count;
        }

        public void CreateBus(Bus bus)
        {
            int searchIndex = ds.Buses.FindIndex(i =>
                              i.LicenseNumber == bus.LicenseNumber &&
                              i.IsActive);

            if (searchIndex != -1)       // Bus already exist
                throw new BusAlreadyExistException($"A bus with same license number" +
                    $" was found in the system. License number: {bus.LicenseNumber}.");
            ds.Buses.Add(bus.Clone());
        }

        public void UpdateBus(Bus bus)
        {
            int searchIndex = ds.Buses.FindIndex(i =>
                              i.LicenseNumber == bus.LicenseNumber &&
                              i.IsActive);

            if (searchIndex == -1)       // Bus does not exist
                throw new BusDoesNotExistException($"A bus with license number: {bus.LicenseNumber}" +
                    $" was not found in the system.");
            ds.Buses[searchIndex] = bus.Clone();
        }

        public Bus RequestBus(string licenseNumber)
        {
            int searchIndex = ds.Buses.FindIndex(i =>
                              i.LicenseNumber == licenseNumber &&
                              i.IsActive);

            if (searchIndex == -1)       // Bus does not exist
                throw new BusDoesNotExistException($"A bus with license number: {licenseNumber}" +
                    $" was not found in the system.");
            return ds.Buses[searchIndex].Clone();
        }

        public void RemoveBus(string licenseNumber)
        {
            int searchIndex = ds.Buses.FindIndex(i =>
                              i.LicenseNumber == licenseNumber &&
                              i.IsActive);

            if (searchIndex == -1)       // Bus does not exist
                throw new BusDoesNotExistException($"A bus with license number: {licenseNumber}" +
                    $" was not found in the system.");
            ds.Buses[searchIndex].IsActive = false;
        }
        #endregion

        #region Station
        public IEnumerable<Station> GetStations()
        {
            return from station in ds.Stations
                   where station.IsActive
                   select station.Clone();
        }

        public int StationsCount()
        {
            return (from station in ds.Stations
                    where station.IsActive
                    select station).ToList().Count;
        }

        public void CreateStation(Station station)
        {
            int searchIndex = ds.Stations.FindIndex(i =>
                              i.StationCode == station.StationCode &&
                              i.IsActive);

            if (searchIndex != -1)       // Station already exist
                throw new StationAlreadyExistException($"A station with the same code " +
                    $"was found in the system." +
                    $" Station code: {station.StationCode}.");
            ds.Stations.Add(station.Clone());
        }

        public void UpdateStation(Station station)
        {
            int searchIndex = ds.Stations.FindIndex(i =>
                              i.StationCode == station.StationCode &&
                              i.IsActive);

            if (searchIndex == -1)           // Station does not exist
                throw new StationDoesNotExistException($"A station with code: {station.StationCode}" +
                    $" was not found in the system.");
            ds.Stations[searchIndex] = station.Clone();
        }

        public Station RequestStation(int stationCode)
        {
            int searchIndex = ds.Stations.FindIndex(i =>
                              i.StationCode == stationCode &&
                              i.IsActive);

            if (searchIndex == -1)       // Station does not exist
                throw new StationDoesNotExistException($"A station with code: {stationCode}" +
                    $" was not found in the system.");
            return ds.Stations[searchIndex].Clone();
        }

        public void RemoveStation(int stationCode)
        {
            int searchIndex = ds.Stations.FindIndex(i =>
                              i.StationCode == stationCode &&
                              i.IsActive);

            if (searchIndex == -1)       // Station does not exist
                throw new StationDoesNotExistException($"A station with code: {stationCode}" +
                    $" was not found in the system.");
            ds.Stations[searchIndex].IsActive = false;
        }
        #endregion

        #region Line
        public IEnumerable<Line> GetLines()
        {
            return from line in ds.Lines
                   where line.IsActive
                   select line.Clone();
        }

        public IEnumerable<Line> GetLine(int lineNumber)
        {
            return (from line in ds.Lines
                    where line.IsActive && line.LineNumber == lineNumber
                    select line.Clone()).ToList();  // take linq action for the cloning
        }

        public int LinesCount()
        {
            return (from line in ds.Lines
                    where line.IsActive
                    select line).ToList().Count;
        }

        public int CreateLine(Line line)
        {
            int Id = DS.Config.GetLineID();
            line.ID = Id;
            int searchIndex = ds.Lines.FindIndex(i =>
                              i.ID == line.ID &&
                              i.IsActive);

            if (searchIndex != -1)       // Line already exist
                throw new LineAlreadyExistException($"A line with same id " +
                    $"was found in the system. Line id: {line.ID}.");
            ds.Lines.Add(line.Clone());
            return Id;
        }

        public void UpdateLine(Line line)
        {
            int searchIndex = ds.Lines.FindIndex(i =>
                              i.ID == line.ID &&
                              i.IsActive);

            if (searchIndex == -1)       // Line does not exist
                throw new LineDoesNotExistException($"A line with id: {line.ID}" +
                    $" was not found in the system.");
            ds.Lines[searchIndex] = line.Clone();
        }

        public Line RequestLine(int Id)
        {
            int searchIndex = ds.Lines.FindIndex(i =>
                              i.ID == Id &&
                              i.IsActive);

            if (searchIndex == -1)       // Line does not exist
                throw new LineDoesNotExistException($"A line with id: {Id}" +
                    $" was not found in the system.");
            return ds.Lines[searchIndex].Clone();
        }

        public void RemoveLine(int Id)
        {
            int searchIndex = ds.Lines.FindIndex(i =>
                              i.ID == Id &&
                              i.IsActive);

            if (searchIndex == -1)       // Line does not exist
                throw new LineDoesNotExistException($"A line with id: {Id}" +
                    $" was not found in the system.");
            ds.Lines[searchIndex].IsActive = false;
        }
        #endregion

        #region AdjStation
        public IEnumerable<AdjStations> GetAdjStations()
        {
            return from adjStations in ds.AdjStations
                   where adjStations.IsActive
                   select adjStations.Clone();
        }

        public int AdjStationsCount()
        {
            return (from adjStations in ds.AdjStations
                    where adjStations.IsActive
                    select adjStations).ToList().Count;
        }

        public void CreateAdjStations(AdjStations adjStations)
        {
            int searchIndex = ds.AdjStations.FindIndex(i =>
                              i.StationCode1 == adjStations.StationCode1 &&
                              i.StationCode2 == adjStations.StationCode2 &&
                              i.IsActive);

            if (searchIndex != -1)       // AdjStations already exist
                throw new AdjStationsAlreadyExistException($"An AdjStation with same station codes " +
                    $"was found in the system. " +
                    $"Code Station1: {adjStations.StationCode1}, Code Station2: {adjStations.StationCode2}.");
            ds.AdjStations.Add(adjStations.Clone());
        }

        public void UpdateAdjStations(AdjStations adjStations)
        {
            int searchIndex = ds.AdjStations.FindIndex(i =>
                              i.StationCode1 == adjStations.StationCode1 &&
                              i.StationCode2 == adjStations.StationCode2 &&
                              i.IsActive);

            if (searchIndex == -1)       // AdjStations does not exist
                throw new AdjStationsDoesNotExistException($"An AdjStation with codes:" +
                    $" ({adjStations.StationCode1}, {adjStations.StationCode2})" +
                    $" was not found in the system.");
            ds.AdjStations[searchIndex] = adjStations.Clone();
        }

        public AdjStations RequestAdjStations(int stationCode1, int stationCode2)
        {
            int searchIndex = ds.AdjStations.FindIndex(i =>
                              i.StationCode1 == stationCode1 &&
                              i.StationCode2 == stationCode2 &&
                              i.IsActive);

            if (searchIndex == -1)       // AdjStations does not exist
                throw new AdjStationsDoesNotExistException($"An AdjStation with codes:" +
                    $" ({stationCode1}, {stationCode2}) was not found in the system.");
            return ds.AdjStations[searchIndex].Clone();
        }

        public void RemoveAdjStations(int stationCode1, int stationCode2)
        {
            int searchIndex = ds.AdjStations.FindIndex(i =>
                              i.StationCode1 == stationCode1 &&
                              i.StationCode2 == stationCode2 &&
                              i.IsActive);

            if (searchIndex == -1)       // AdjStations does not exist
                throw new AdjStationsDoesNotExistException($"An AdjStation with codes:" +
                    $" ({stationCode1}, {stationCode2}) was not found in the system.");
            ds.AdjStations[searchIndex].IsActive = false;
        }
        #endregion

        #region StationLine
        public IEnumerable<StationLine> GetStationLines()
        {
            return from stationLine in ds.StationLines
                   where stationLine.IsActive
                   select stationLine.Clone();
        }

        public IEnumerable<StationLine> GetStationLinesInStation(int stationCode)
        {
            return from stationLine in ds.StationLines
                   where stationLine.IsActive &&
                         stationLine.StationCode == stationCode
                   select stationLine.Clone();
        }

        public int StationsLineCount()
        {
            return (from stationLine in ds.StationLines
                    where stationLine.IsActive
                    select stationLine).ToList().Count;
        }

        public void CreateStationLine(StationLine stationLine)
        {
            int searchIndex = ds.StationLines.FindIndex(i =>
                              i.ID == stationLine.ID &&
                              i.StationCode == stationLine.StationCode &&
                              i.IsActive);

            if (searchIndex != -1)       // StationLine already exist
                throw new StationLineAlreadyExistException($"A StationLine with same ID " +
                    $"and StationCode was found in the system. " +
                    $"ID: {stationLine.ID}, StationCode: {stationLine.StationCode}.");
            ds.StationLines.Add(stationLine.Clone());
        }

        public void UpdateStationLine(StationLine stationLine)
        {
            int searchIndex = ds.StationLines.FindIndex(i =>
                              i.ID == stationLine.ID &&
                              i.StationCode == stationLine.StationCode &&
                              i.IsActive);

            if (searchIndex == -1)       // StationLine does not exist
                throw new StationLineDoesNotExistException($"A StationLine with ID: {stationLine.ID} " +
                    $"and Stationcode: {stationLine.StationCode} was not found in the system.");
            ds.StationLines[searchIndex] = stationLine.Clone();
        }

        public StationLine RequestStationLine(int Id, int stationCode)
        {
            int searchIndex = ds.StationLines.FindIndex(i =>
                             i.ID == Id &&
                             i.StationCode == stationCode &&
                             i.IsActive);

            if (searchIndex == -1)       // StationLine does not exist
                throw new StationLineDoesNotExistException($"A StationLine with ID: {Id} " +
                    $"and Stationcode: {stationCode} was not found in the system.");
            return ds.StationLines[searchIndex].Clone();
        }

        public StationLine RequestStationLineByIndex(int Id, int numberInLine)
        {
            int searchIndex = ds.StationLines.FindIndex(i =>
                             i.ID == Id &&
                             i.StationNumberInLine == numberInLine &&
                             i.IsActive);

            if (searchIndex == -1)       // StationLine does not exist
                throw new StationLineDoesNotExistException($"A StationLine with ID: {Id} " +
                    $"in the: {numberInLine} index, was not found in the system.");
            return ds.StationLines[searchIndex].Clone();
        }

        public void RemoveStationLine(int Id, int stationCode)
        {
            int searchIndex = ds.StationLines.FindIndex(i =>
                             i.ID == Id &&
                             i.StationCode == stationCode &&
                             i.IsActive);

            if (searchIndex == -1)       // StationLine does not exist
                throw new StationLineDoesNotExistException($"A StationLine with ID: {Id} " +
                    $"and Stationcode: {stationCode} was not found in the system.");
            ds.StationLines[searchIndex].IsActive = false;
        }
        #endregion

        #region LineTrip
        public IEnumerable<LineTrip> GetLineTrips()
        {
            return from lineTrip in ds.LineTrips
                   where lineTrip.IsActive
                   select lineTrip.Clone();
        }

        public IEnumerable<LineTrip> GetLineTripsByPredicate(Predicate<LineTrip> predicate)
        {
            return from lineTrip in ds.LineTrips
                   where lineTrip.IsActive &&
                         predicate(lineTrip)
                   select lineTrip.Clone();
        }

        public int LineTripsCount()
        {
            return (from lineTrip in ds.LineTrips
                    where lineTrip.IsActive
                    select lineTrip).ToList().Count;
        }

        public void CreateLineTrip(LineTrip lineTrip)
        {
            int searchIndex = ds.LineTrips.FindIndex(i =>
                                i.ID == lineTrip.ID &&
                                i.StartTime == lineTrip.StartTime &&
                                i.IsActive);
            if (searchIndex != -1)   // line trip already exist
                throw new LineTripAlreadyExistException($"A line with ID: {lineTrip.ID} and " +
                    $"start time: {lineTrip.StartTime} was found in the system.");
            ds.LineTrips.Add(lineTrip.Clone());
        }

        public void UpdateLineTrip(LineTrip lineTrip)
        {
            int searchIndex = ds.LineTrips.FindIndex(i =>
                                i.ID == lineTrip.ID &&
                                i.StartTime == lineTrip.StartTime &&
                                i.IsActive);
            if (searchIndex == -1)   // line trip does not exist
                throw new LineTripDoesNotExistException($"The line with ID: {lineTrip.ID} has no " +
                    $"trips that start at {lineTrip.StartTime}.");
            ds.LineTrips[searchIndex] = lineTrip.Clone();
        }

        public LineTrip RequestLineTrip(int Id, TimeSpan startTime)
        {
            int searchIndex = ds.LineTrips.FindIndex(i =>
                                i.ID == Id &&
                                i.StartTime == startTime &&
                                i.IsActive);
            if (searchIndex == -1)   // line trip does not exist
                throw new LineTripDoesNotExistException($"The line with ID: {Id} has no " +
                    $"trips that start at {startTime}.");
            return ds.LineTrips[searchIndex].Clone();
        }

        public LineTrip RequestLineTripByPredicate(Predicate<LineTrip> predicate)
        {
            var res = ds.LineTrips.Find(predicate);
            if (res == null)
                throw new LineTripDoesNotExistException($"No line was match with the predicate.");
            return res.Clone();
        }

        public void RemoveLineTrip(int Id, TimeSpan startTime)
        {
            int searchIndex = ds.LineTrips.FindIndex(i =>
                                i.ID == Id &&
                                i.StartTime == startTime &&
                                i.IsActive);
            if (searchIndex == -1)   // line trip does not exist
                throw new LineTripDoesNotExistException($"The line with ID: {Id} has no " +
                    $"trips that start at {startTime}.");
            ds.LineTrips[searchIndex].IsActive = false;
        }
        #endregion

        #region LineTiming
        public IEnumerable<LineTiming> GetLineTimings()
        {
            return from line in ds.LineTimings
                   select line.Clone();
        }

        public int CreateLineTiming(LineTiming line)
        {
            int Id = DS.Config.GetLineTimingID();
            line.ID = Id;
            int searchIndex = ds.LineTimings.FindIndex(i =>
                              i.ID == line.ID &&
                              i.License == line.License &&
                              i.LineID == line.LineID &&
                              i.StartTime == line.StartTime);

            if (searchIndex != -1)
                throw new LineTimingAlreadyExistException($"A line timing with same values" +
                    $" was found in the system.");
            ds.LineTimings.Add(line.Clone());
            return Id;
        }

        public LineTiming RequestLineTiming(int Id)
        {
            int searchIndex = ds.LineTimings.FindIndex(i => i.ID == Id);

            if (searchIndex == -1)
                throw new LineTimingDoesNotExistException($"A line timing with this ID" +
                    $" was not found in the system.");
            return ds.LineTimings[searchIndex].Clone();
        }

        public void RemoveLineTiming(int Id)
        {
            int searchIndex = ds.LineTimings.FindIndex(i => i.ID == Id);

            if (searchIndex == -1)
                throw new LineTimingDoesNotExistException($"A line timing with this ID" +
                    $" was not found in the system.");
            ds.LineTimings.RemoveAt(searchIndex);
        }
        #endregion
    }
}