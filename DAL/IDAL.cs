using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalApi
{
    // CRUD - Create, Update, Request, Delete

    public interface IDAL
    {
        #region User
        int UsersCount();
        void CreateUser(DO.User user);
        void RemoveUser(DO.User user);
        DO.User CheckUser(DO.User user);
        #endregion

        #region Files (Dal Object Will Return Empty Enumerable)
        IEnumerable<string> GetPathes();
        #endregion

        #region Bus
        //IEnumerable<DO.Bus> GetBuses();
        //int BusesCount();
        //void CreateBus(DO.Bus bus);
        //void UpdateBus(DO.Bus bus);
        //DO.Bus RequestBus(string licenseNumber);
        //void RemoveBus(string licenseNumber);
        #endregion

        #region Station
        IEnumerable<DO.Station> GetStations();
        int StationsCount();
        void CreateStation(DO.Station station);
        void UpdateStation(DO.Station station);
        DO.Station RequestStation(int stationCode);
        void RemoveStation(int stationCode);
        #endregion

        #region Line
        IEnumerable<DO.Line> GetLines();
        IEnumerable<DO.Line> GetLine(int lineNumber);
        int LinesCount();
        int CreateLine(DO.Line line);
        void UpdateLine(DO.Line line);
        DO.Line RequestLine(int Id);
        void RemoveLine(int Id);
        #endregion

        #region AdjStation
        IEnumerable<DO.AdjStations> GetAdjStations();
        int AdjStationsCount();
        void CreateAdjStations(DO.AdjStations adjStations);
        void UpdateAdjStations(DO.AdjStations adjStations);
        DO.AdjStations RequestAdjStations(int stationCode1, int stationCode2);
        void RemoveAdjStations(int stationCode1, int stationCode2);
        #endregion

        #region StationLine
        IEnumerable<DO.StationLine> GetStationLines();
        IEnumerable<DO.StationLine> GetStationLinesInStation(int stationCode);
        int StationsLineCount();
        void CreateStationLine(DO.StationLine stationLine);
        void UpdateStationLine(DO.StationLine stationLine);
        DO.StationLine RequestStationLine(int Id, int stationCode);
        DO.StationLine RequestStationLineByIndex(int Id, int numberInLine);
        void RemoveStationLine(int Id, int stationCode);
        #endregion

        #region LineTrip
        IEnumerable<DO.LineTrip> GetLineTrips();
        IEnumerable<DO.LineTrip> GetLineTripsByPredicate(Predicate<DO.LineTrip> predicate);
        int LineTripsCount();
        void CreateLineTrip(DO.LineTrip lineTrip);
        void UpdateLineTrip(DO.LineTrip lineTrip);
        DO.LineTrip RequestLineTrip(int Id, TimeSpan startTime);
        DO.LineTrip RequestLineTripByPredicate(Predicate<DO.LineTrip> predicate);
        void RemoveLineTrip(int Id, TimeSpan startTime);
        #endregion

        #region LineTiming
        IEnumerable<DO.LineTiming> GetLineTimings();
        int CreateLineTiming(DO.LineTiming line);
        DO.LineTiming RequestLineTiming(int Id);
        void RemoveLineTiming(int Id);
        #endregion
    }
}