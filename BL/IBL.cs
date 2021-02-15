using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
    public interface IBL
    {
        #region User
        int UsersCount();
        void CreateUser(string Username, string Email, string Password1, string Password2);
        void RemoveUser(string Username, string Email, string Password);
        BO.UserDisplay TryLogin(string Username, string Password);
        #endregion

        #region File Manager Functions
        BO.DisplayCounts GetCounts();

        IEnumerable<BO.DisplayFile> GetFiles();
        #endregion

        #region Request Functions (Public Functions)
        BO.Station RequestStation(int stationCode);
        BO.Line RequestLine(int Id);
        BO.AdjStations RequestAdjStations(int StationCoed1, int StationCoed2);
        BO.StationLine RequestStationLine(int Id, int stationCode);
        #endregion

        #region Add Functions (Public functions)
        void AddStation(int stationCode, string name, double latitude, double longitude, string address);
        void AddLine(int lineNumber, int firstStationCode, int lastStationCode, BO.Line.Areas area);
        void AddAdjStations(int stationCode1, int stationCode2, double distance = -1, TimeSpan time = new TimeSpan());
        void AddStationLine(int Id, int stationCode, int stationNumberInLine);
        void AddTripLine(int Id, TimeSpan startTime, TimeSpan endTime, TimeSpan frequency);
        #endregion

        #region Update Functions (Public functions)
        void UpdateStation(int stationCode, string name, double latitude, double longitude, string address);
        void UpdateLine(int Id, int lineNumber, int firstStationCode, int lastStationCode, BO.Line.Areas area);
        void UpdateAdjStations(int stationCode1, int stationCode2, double distance, TimeSpan time);
        void UpdateStationLine(int Id, int stationCode, int stationNumberInLine);
        #endregion

        #region Remove Functions (Public functions)
        void RemoveStation(int StationCode);
        void RemoveLine(int ID);
        void RemoveAdjStations(int StationCode1, int StationCode2);
        void RemoveStationLine(int Id, int stationCode);
        void RemoveLineTrip(int Id, TimeSpan startTime);
        #endregion

        #region GetCount Functions (Public functions)
        int GetLinesCount();
        int GetStationsCount();
        int GetStationLinesCount();
        int GetAdjStationsCount();
        #endregion

        #region GetAll Functions (Public functions)
        IEnumerable<BO.Line> GetLines();
        IEnumerable<BO.Station> GetStations();
        IEnumerable<BO.AdjStations> GetAdjStations();
        IEnumerable<BO.LineTrip> GetLineTrips();
        #endregion

        #region Simulator
        void StartSimulator(TimeSpan startTime, int Rate, Action<TimeSpan> updateTime);
        void StopSimulator();
        void SetStationPanel(int station, Action<BO.LineTiming> updateBus);
        #endregion
    }
}