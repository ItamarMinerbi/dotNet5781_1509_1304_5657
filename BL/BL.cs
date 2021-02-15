using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using BlApi;
using BlExceptions;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace BL
{
    sealed class BL : IBL
    {
        internal static DalApi.IDAL dal = DalApi.DalFactory.GetDal(DalApi.Options.Xml);

        #region Singleton
        static readonly BL instance = new BL();
        static BL() { }
        BL() { }
        public static BL Instance => instance;
        #endregion

        #region User
        public UserDisplay TryLogin(string UsernameOrEmail, string Password)
        {
            try
            {
                DO.User userReply = dal.CheckUser(new DO.User() { Username = UsernameOrEmail, Email = UsernameOrEmail, Password = Password });
                UserDisplay userResult = userReply.Convert<DO.User, UserDisplay>();
                return userResult;
            }
            catch(Exception ex) { throw new UserDoesNotExistException(ex.Message, ex); }
        }

        internal bool IsValid(string emailaddress)
        {
            try { MailAddress m = new MailAddress(emailaddress); return true; }
            catch (Exception) { return false; }
        }

        public void CreateUser(string Username, string Email, string Password1, string Password2)
        {
            if (String.IsNullOrEmpty(Username) || String.IsNullOrWhiteSpace(Username) ||
                Username.Length < 4)
                throw new InvalidUsernameException("The name is too short!\nThe minimum length is 4 characters!");
            if (String.IsNullOrEmpty(Username) || String.IsNullOrWhiteSpace(Username) ||
                Username.Length > 16)
                throw new InvalidUsernameException("The name is too long!\nThe maximum length is 16 characters!");
            if (Password1.Length < 4)
                throw new InvalidPasswordException("The password is too short!\nThe minimum length is 4 characters!");
            if (Password1.Length > 256)
                throw new InvalidPasswordException("The password is too long!\nThe maximum length is 256 characters!");
            if (Password1 != Password2)
                throw new InvalidPasswordException("Both passwords are not the same!");
            if (String.IsNullOrEmpty(Email) || String.IsNullOrWhiteSpace(Email))
                throw new InvalidEmailException($"This email address is empty!");
            if (!IsValid(Email))
                throw new InvalidEmailException($"It looks like we can't send e-mails to this address: {Email}\nVerify that this address is correct!");
            try { 
                dal.CreateUser(new DO.User() 
                { 
                    Username = Username, 
                    Password = Password1, 
                    Email = Email, 
                    IsAdmin = false, 
                    IsActive = true 
                });
                // Send Mail
                string htmlBody = @"<!DOCTYPE html>

<html lang=""en"" xmlns=""http://www.w3.org/1999/xhtml"">
<head>  <meta charset=""utf-8"" />  </head>
<body style=""margin-top: 20px;"" dir=""ltr"">
    <table class=""body-wrap"" style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; width: 100%; background-color: #f6f6f6; margin: 0;"" bgcolor=""#f6f6f6"">
        <tbody>
            <tr style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;"">
                <td style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0;"" valign=""top""></td>
                <td class=""container"" width=""600"" style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; display: block !important; max-width: 600px !important; clear: both !important; margin: 0 auto;""
                    valign=""top"">
                    <div class=""content"" style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; max-width: 600px; display: block; margin: 0 auto; padding: 20px;"">
                        <table class=""main"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; border-radius: 3px; background-color: #fff; margin: 0; border: 1px solid #e9e9e9;""
                               bgcolor=""#fff"">
                            <tbody>
                                <tr style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;"">
                                    <td class="""" style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 16px; vertical-align: top; color: #fff; font-weight: 500; text-align: center; border-radius: 3px 3px 0 0; background-color: #38414a; margin: 0; padding: 20px;""
                                        align=""center"" bgcolor=""#71b6f9"" valign=""top"">
                                        <a href=""#"" style=""font-size:32px;color:#fff;"">dotNet5781 Project</a> <br>
                                        <span style=""margin-top: 10px;display: block;"">Hello {Username}, Welcome to our system!</span>
                                    </td>
                                </tr>
                                <tr style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;"">
                                    <td class=""content-wrap"" style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 20px;"" valign=""top"">
                                        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;"">
                                            <tbody>
                                                <tr style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;"">
                                                    <td class=""content-block"" style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;"" valign=""top"">
                                                        Hello {Username} <strong style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;"">
                                                            you enterd this email to your account
                                                        </strong> have fun with our system.
                                                    </td>
                                                </tr>
                                                <tr style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;"">
                                                    <td class=""content-block"" style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;"" valign=""top"">
                                                        If you need some help you can send us your question to this email address.
                                                    </td>
                                                </tr>
                                                <tr style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;"">
                                                    <td class=""content-block"" style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;"" valign=""top"">
                                                        <a href=""#"" class=""btn-primary"" style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; color: #FFF; text-decoration: none; line-height: 2em; font-weight: bold; text-align: center; cursor: pointer; display: inline-block; border-radius: 5px; text-transform: capitalize; background-color: #f1556c; margin: 0; border-color: #f1556c; border-style: solid; border-width: 8px 16px;"">
                                                            A little tutorial about the system.
                                                        </a>
                                                    </td>
                                                </tr>
                                                <tr style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;"">
                                                    <td class=""content-block"" style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0; padding: 0 0 20px;"" valign=""top"">
                                                        Thanks for joining <b>{Name}</b>.
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        <div class=""footer"" style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; width: 100%; clear: both; color: #999; margin: 0; padding: 20px;"">
                            <table width=""100%"" style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;"">
                                <tbody>
                                    <tr style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; margin: 0;"">
                                        <td class=""aligncenter content-block"" style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 12px; vertical-align: top; color: #999; text-align: center; margin: 0; padding: 0 0 20px;"" align=""center"" valign=""top"">
                                            &#169;All rights reserved <a href=""#"" style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 12px; color: #999; text-decoration: underline; margin: 0;"">{Name}</a>.
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </td>
                <td style=""font-family: 'Helvetica Neue',Helvetica,Arial,sans-serif; box-sizing: border-box; font-size: 14px; vertical-align: top; margin: 0;"" valign=""top""></td>
            </tr>
        </tbody>
    </table>
</body>
</html>";
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);    // gmail server
                smtp.EnableSsl = true;  // secure connection
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("dotnet5781.project.help@gmail.com", "5e16XPKxnlEukKuU");
                MailMessage mail = new MailMessage();
                mail.To.Add(Email);
                mail.From = new MailAddress("dotnet5781.project.help@gmail.com");
                mail.Subject = "Register";
                mail.Body = htmlBody.Replace("{Username}", Username).Replace("{Name}", "Itamar Minerbi & Eli Arazi Project");
                mail.IsBodyHtml = true;
                smtp.Send(mail);
            }
            catch(Exception ex) { throw new InvalidEmailException(ex.Message, ex); }
        }

        public void RemoveUser(string Username, string Email, string Password)
        {
            try { dal.RemoveUser(new DO.User() { Username = Username, Email = Email, Password = Password }); }
            catch (Exception) { return; }
        }

        public int UsersCount()
        {
            return dal.UsersCount();
        }
        #endregion

        #region File Manager Functions
        internal string BytesToStringSize(long size)
        {
            var kb = (double)size / 1024;
            var mb = (double)kb / 1024;
            var gb = (double)mb / 1024;
            var tb = (double)gb / 1024;
            string result;
            if (tb >= 1) result = String.Format("{0:0.## TB}", tb);
            else if (gb >= 1) result = String.Format("{0:0.## GB}", gb);
            else if (mb >= 1) result = String.Format("{0:0.## MB}", mb);
            else if (kb >= 1) result = String.Format("{0:0.## KB}", kb);
            else result = String.Format("{0} Bytes", size);
            return result;
        }

        public DisplayCounts GetCounts()
        {
            long size = 0;
            foreach (var filePath in dal.GetPathes())
                try { size += new FileInfo(filePath).Length; }
                catch (DalExceptions.XmlLoadException ex) { throw new XmlLoadException(ex.Message, ex); }
                catch (Exception ex) { throw new AnErrorOccurredException(ex.Message, ex); }
            string totalSize = BytesToStringSize(size);
            try
            {
                return new DisplayCounts
                {
                    UsersCount = dal.UsersCount(),
                    AdjStationsCount = dal.AdjStationsCount(),
                    LinesCount = dal.LinesCount(),
                    LineTripsCount = dal.LineTripsCount(),
                    StationLinesCount = dal.StationsLineCount(),
                    StationsCount = dal.StationsCount(),
                    TotalSpace = totalSize
                };
            }
            catch (DalExceptions.XmlLoadException ex) { throw new XmlLoadException(ex.Message, ex); }
            catch(DalExceptions.XmlParametersException ex) { throw new XmlParametersException(ex.Message, ex); }
            catch(Exception ex) { throw new AnErrorOccurredException(ex.Message, ex); }
        }

        public IEnumerable<DisplayFile> GetFiles()
        {
            foreach (var filePath in dal.GetPathes())
            {
                FileInfo file;
                string creationTime, lastMod;
                try
                { file = new FileInfo(filePath); creationTime = File.GetCreationTime(filePath).ToString();
                    lastMod = File.GetLastWriteTime(filePath).ToString(); }
                catch(Exception ex) { throw new XmlLoadException(ex.Message, ex); }
                yield return new DisplayFile
                {
                    Name = file.Name,
                    CreationDate = creationTime,
                    LastModifiedDate = lastMod,
                    SizeBytes = file.Length,
                    SizeString = BytesToStringSize(file.Length),
                    Path = filePath
                };
            }
        }
        #endregion

        #region Calculate Functions (Internal Functions)
        internal double CalculateDistance(DO.Station firstStation, DO.Station secondStation)
        {
            var lat1 = firstStation.Latitude;
            var lon1 = firstStation.Longitude;
            var lat2 = secondStation.Latitude;
            var lon2 = secondStation.Longitude;

            if ((lat1 == lat2) && (lon1 == lon2))
                return 0;
            var PI = Math.PI;
            var distance = ACos(Sin(PI * lat1 / 180.0) * Sin(PI * lat2 / 180.0) +
                Cos(PI * lat1 / 180.0) * Cos(PI * lat2 / 180.0) *
                Cos(PI * lon1 / 180.0 - PI * lon2 / 180.0)) * 6378;
            return double.Parse(String.Format("{0:0.##}", distance));   // Make the distance 2 digits
        }
        internal double Sin(double x) => Math.Sin(x);
        internal double Cos(double x) => Math.Cos(x);
        internal double ACos(double x) => Math.Acos(x);
        #endregion

        #region Build Functions (Internal functions)

        internal Station BuildStation(int stationCode)
        {
            // Request Station from DAL
            DO.Station stationReply;
            try { stationReply = dal.RequestStation(stationCode); }
            catch (DalExceptions.XmlLoadException ex) { throw new XmlLoadException(ex.Message, ex); }
            catch (Exception ex) { throw new StationDoesNotExistException(ex.Message, ex); }

            // Request Lines in the station from DAL
            IEnumerable<DO.StationLine> linesInTheStation;
            try { linesInTheStation = dal.GetStationLinesInStation(stationCode); }
            catch (DalExceptions.XmlLoadException ex) { throw new XmlLoadException(ex.Message, ex); }
            catch (Exception ex) { throw new AnErrorOccurredException(ex.Message, ex); }
            
            // Build a new BO station and update the fields
            Station stationResult = stationReply.Convert<DO.Station, Station>();    // update the lon, lat, name, stationcode etc

            // Convert the collection of StationLine to LinesInTheStation
            stationResult.Lines = from stationLine in linesInTheStation
                                  select BuildLinesInTheStation(stationLine.ID);
            List<LinesInTheStation> linesWithNoNull = stationResult.Lines.ToList();
            linesWithNoNull.RemoveAll(i => i == null);
            stationResult.Lines = from stationLine in linesWithNoNull
                                  select stationLine;
            // Return the new BO station
            return stationResult;
        }

        internal LinesInTheStation BuildLinesInTheStation(int Id)
        {
            // Request Line from DAL
            DO.Line lineReplay;
            try { lineReplay = dal.RequestLine(Id); }
            catch (DalExceptions.XmlLoadException ex) { throw new XmlLoadException(ex.Message, ex); }
            catch (Exception) { return null; }

            // Convert the result to BO LinesInTheStation
            LinesInTheStation linesInTheStationResult = lineReplay.Convert<DO.Line, LinesInTheStation>();

            // Get the name of the last station
            DO.Station station;
            try { station = dal.RequestStation(lineReplay.LastStation); }
            catch (DalExceptions.XmlLoadException ex) { throw new XmlLoadException(ex.Message, ex); }
            catch (Exception) { return null; }
            linesInTheStationResult.LastStationName = station.Name;

            // Return the new BO LinesInTheStation
            return linesInTheStationResult;
        }

        internal StationLine BuildStationLine(int stationCode)
        {
            // Request Station from DAL
            DO.Station stationReply;
            try { stationReply = dal.RequestStation(stationCode); }
            catch (DalExceptions.XmlLoadException ex) { throw new XmlLoadException(ex.Message, ex); }
            catch (Exception ex) { throw new StationDoesNotExistException(ex.Message, ex); }

            // Convert the result to BO StationLine
            StationLine stationLineResult = stationReply.Convert<DO.Station, StationLine>();

            // Return the new BO StationLine
            return stationLineResult;
        }

        internal Line BuildLine(int Id)
        {
            #region Get the data from DAL
            // Request Line from DAL
            DO.Line lineReply;
            try { lineReply = dal.RequestLine(Id); }
            catch (DalExceptions.XmlLoadException ex) { throw new XmlLoadException(ex.Message, ex); }
            catch (Exception ex) { throw new LineDoesNotExistException(ex.Message, ex); }

            // Get the first station index of the line
            DO.StationLine firstStationCode;
            try { firstStationCode = dal.RequestStationLine(Id, lineReply.FirstStation); }
            catch (DalExceptions.XmlLoadException ex) { throw new XmlLoadException(ex.Message, ex); }
            catch (Exception ex) { throw new StationLineDoesNotExistException(ex.Message, ex); }
            int firstStation = firstStationCode.StationNumberInLine;

            // Get the last station index of the line
            DO.StationLine lastStationCode;
            try { lastStationCode = dal.RequestStationLine(Id, lineReply.LastStation); }
            catch (DalExceptions.XmlLoadException ex) { throw new XmlLoadException(ex.Message, ex); }
            catch (Exception ex) { throw new StationLineDoesNotExistException(ex.Message, ex); }
            int lastStation = lastStationCode.StationNumberInLine;

            // For loop to get the stations in the line
            List<StationLine> stationLinesList = new List<StationLine>();
            for (int i = firstStation; i <= lastStation; i++)
            {
                // Request StationLine from DAL for the station code
                DO.StationLine stationLine;
                try { stationLine = dal.RequestStationLineByIndex(Id, i); }
                catch (DalExceptions.XmlLoadException ex) { throw new XmlLoadException(ex.Message, ex); }
                catch (Exception ex) { throw new StationLineDoesNotExistException(ex.Message, ex); }

                // Get the StationLine
                int stationCode = stationLine.StationCode;
                StationLine newStationLine = BuildStationLine(stationCode);  // If we will get an exception the above 'try' will catch it

                // Add the new StationLine to the list
                stationLinesList.Add(newStationLine);
            }

            // Get the trips of the line
            List<LineTrip> lineTrips = new List<LineTrip>();
            foreach (var trip in dal.GetLineTripsByPredicate(i => i.ID == Id))
                lineTrips.Add(trip.Convert<DO.LineTrip, LineTrip>());
            #endregion

            #region Create extra data for the line
            // Convert each 2 stations to AdjStations
            List<AdjStations> adjStationsList = new List<AdjStations>();
            adjStationsList.Add(new AdjStations()
            {
                StationCode1 = stationLinesList[lastStation - firstStation].StationCode,
                StationCode2 = 0,
                Distance = 0,
                Time = new TimeSpan(0, 0, 0)
            });
            TimeSpan totalTime = new TimeSpan(0, 0, 0);
            for (int i = 0; i <= lastStation - firstStation - 1; i++)
            {
                // Get the station codes
                int stationCode1 = stationLinesList[i].StationCode;
                int stationCode2 = stationLinesList[i + 1].StationCode;

                // Get the AdjStations
                AdjStations newAdjStations = BuildAdjStation(stationCode1, stationCode2);

                // Add to list
                adjStationsList.Add(newAdjStations);

                // Add the time tototalTime
                totalTime += newAdjStations.Time;
            }

            List<DisplayStationLine> displayStations = new List<DisplayStationLine>();
            for (int i = 0; i < stationLinesList.Count; i++)
            {
                displayStations.Add(new DisplayStationLine()
                {
                    StationCode = stationLinesList[i].StationCode,
                    Name = stationLinesList[i].Name,
                    Time = adjStationsList[i].Time,
                    Distance = adjStationsList[i].Distance
                });
            }
            #endregion

            #region Build the line from the data
            // Build a new BO Line
            Line lineResult = lineReply.Convert<DO.Line, Line>();      // Copy the ID and the line number
            Line.Areas area;
            if (Enum.TryParse(lineReply.Area.ToString(), true, out area))
                lineResult.Area = area;
            else
                lineResult.Area = Line.Areas.General;
            lineResult.Stations = displayStations;
            lineResult.TotalTime = totalTime;
            lineResult.Trips = from trip in lineTrips
                               orderby trip.StartTime
                               select trip;
            #endregion

            // Return the result
            return lineResult;
        }

        internal AdjStations BuildAdjStation(int stationCode1, int stationCode2)
        {
            // Check if the stations exist
            DO.Station station1, station2;
            try { station1 = dal.RequestStation(stationCode1); station2 = dal.RequestStation(stationCode2); }
            catch (DalExceptions.XmlLoadException ex) { throw new XmlLoadException(ex.Message, ex); }
            catch (Exception ex) { throw new StationDoesNotExistException(ex.Message, ex); }

            // Request AdjStations from DAL
            DO.AdjStations adjStationsReply;
            try { adjStationsReply = dal.RequestAdjStations(stationCode1, stationCode2); }
            catch (DalExceptions.XmlLoadException ex) { throw new XmlLoadException(ex.Message, ex); }
            catch (Exception) { 
                AddAdjStations(stationCode1, stationCode2);
                try { adjStationsReply = dal.RequestAdjStations(stationCode1, stationCode2); }
                catch (Exception ex) { throw new AdjStationsDoesNotExistException($"We couldn't calculate the distances between the stations Add this!", ex); }
            }

            // Convert to BO AdjStations
            AdjStations adjStationsResult = adjStationsReply.Convert<DO.AdjStations, AdjStations>();
            adjStationsResult.Station1Name = station1.Name;
            adjStationsResult.Station2Name = station2.Name;

            // Return the new AdjStations
            return adjStationsResult;
        }
        #endregion

        #region Add Functions (Public functions)
        public void AddStation(int stationCode, string name, double latitude, double longitude, string address)
        {
            DO.Station stationToAdd = new DO.Station()
            { 
                StationCode = stationCode,
                Name = name,
                Latitude = latitude,
                Longitude = longitude,
                Address = address,
                IsActive = true
            };
            try { dal.CreateStation(stationToAdd); }
            catch (DalExceptions.XmlLoadException ex) { throw new XmlLoadException(ex.Message, ex); }
            catch (DalExceptions.XmlParametersException ex) { throw new XmlParametersException(ex.Message, ex); }
            catch (DalExceptions.XmlWriteException ex) { throw new XmlWriteException(ex.Message, ex); }
            catch (Exception ex) { throw new StationAlreadyExistException(ex.Message, ex); }
        }

        public void AddLine(int lineNumber, int firstStationCode, int lastStationCode, Line.Areas area)
        {
            #region Check for stations with the first/lastStationCode
            try { dal.RequestStation(firstStationCode); dal.RequestStation(lastStationCode); }
            catch(Exception ex) { throw new StationDoesNotExistException(ex.Message, ex); }
            #endregion

            #region Create The Objects and Send them to DAL
            DO.Line.Areas DoArea;
            int Id;
            if (!Enum.TryParse(area.ToString(), true, out DoArea)) DoArea = DO.Line.Areas.General;
            DO.Line lineToAdd = new DO.Line()
            {
                LineNumber = lineNumber,
                FirstStation = firstStationCode,
                LastStation= lastStationCode,
                Area = DoArea,
                IsActive = true
            };
            
            try { Id = dal.CreateLine(lineToAdd); }
            catch (Exception ex) { throw new LineAlreadyExistException(ex.Message, ex); }

            DO.StationLine firstStation = new DO.StationLine()
            {
                ID = Id,
                StationCode = firstStationCode,
                StationNumberInLine = 0,
                IsActive = true
            };
            DO.StationLine lastStation = new DO.StationLine()
            {
                ID = Id,
                StationCode = lastStationCode,
                StationNumberInLine = 1,
                IsActive = true
            };

            try { dal.CreateStationLine(firstStation); }
            catch(Exception) { }    // If already exist it's fine...

            try { dal.CreateStationLine(lastStation); }
            catch (Exception) { }    // If already exist it's fine...
            #endregion
        }

        public void AddAdjStations(int stationCode1, int stationCode2, double distance = -1, TimeSpan time = new TimeSpan())
        {
            // Check if the stations are exist
            DO.Station station1;
            DO.Station station2;

            #region Check if the both of the stations are exist
            try { station1 = dal.RequestStation(stationCode1); station2 = dal.RequestStation(stationCode2); }
            catch(Exception ex) { throw new StationDoesNotExistException(ex.Message, ex); }
            #endregion

            #region Calculate distance and time
            if (distance < 0)
            {
                distance = CalculateDistance(station1, station2);
                time = TimeSpan.FromHours(distance / 60);  //  V*T=S  => T=S/V, avg speed ~ 60Kmh
            }
            if(time < new TimeSpan(0, 0, 10))    // The drive can not be less than 10 sec
                time = TimeSpan.FromHours(distance / 60);
            time = TimeSpan.Parse(time.ToString(@"hh\:mm\:ss"));

            DO.AdjStations adjStationsToAdd = new DO.AdjStations()
            {
                StationCode1 = stationCode1,
                StationCode2 = stationCode2,
                Distance = distance,
                Time = time,
                IsActive = true
            };
            #endregion

            #region Add to DAL
            try { dal.CreateAdjStations(adjStationsToAdd); }
            catch (Exception ex) { throw new AdjStationsAlreadyExistException(ex.Message, ex); }
            #endregion
        }

        public void AddStationLine(int Id, int stationCode, int stationIndex)
        {
            // Check if the station and the line are exist
            DO.Station station;
            DO.Line line;
            DO.StationLine firstStation;
            DO.StationLine lastStation;
            List<DO.StationLine> stationsInLine = new List<DO.StationLine>();
            List<DO.StationLine> updatedStationLine = new List<DO.StationLine>();

            #region Get data from DAL
            try { station = dal.RequestStation(stationCode); }
            catch (Exception ex) { throw new StationDoesNotExistException(ex.Message, ex); }

            try { line = dal.RequestLine(Id); }
            catch (Exception ex) { throw new LineDoesNotExistException(ex.Message, ex); }

            try { firstStation = dal.RequestStationLine(Id, line.FirstStation); lastStation = dal.RequestStationLine(Id, line.LastStation); }
            catch (Exception ex) { throw new StationLineDoesNotExistException(ex.Message, ex); }

            for (int i = firstStation.StationNumberInLine; i <= lastStation.StationNumberInLine; i++)
            {
                DO.StationLine stationLine;
                try { stationLine = dal.RequestStationLineByIndex(Id, i); }
                catch (Exception ex) { throw new StationLineDoesNotExistException(ex.Message, ex); } // Resources Error!
                stationsInLine.Add(stationLine);
                if (stationLine.StationCode == stationCode)  // Station already exist in the line
                    throw new StationLineAlreadyExistException($"This station line already in the line at index: {i}.");
            }
            #endregion

            #region Update The Indexes of Each Station
            stationIndex += firstStation.StationNumberInLine;
            for (int i = firstStation.StationNumberInLine; i < stationIndex; i++)
                updatedStationLine.Add(stationsInLine[i - firstStation.StationNumberInLine]);

            updatedStationLine.Add(new DO.StationLine()
            {
                ID = Id,
                StationCode = stationCode,
                StationNumberInLine = stationIndex,
                IsActive = true
            });

            for (int i = stationIndex; i <= lastStation.StationNumberInLine; i++)
            {
                DO.StationLine currenrStationLine = stationsInLine[i - firstStation.StationNumberInLine];
                updatedStationLine.Add(new DO.StationLine()
                {
                    ID = Id,
                    StationCode = currenrStationLine.StationCode,
                    StationNumberInLine = currenrStationLine.StationNumberInLine + 1,
                    IsActive = true
                });
            }
            #endregion

            #region Update In DAL
            foreach (var stationLine in updatedStationLine)
            {
                if(stationLine.StationCode != stationCode)
                {
                    try { dal.UpdateStationLine(stationLine); }
                    catch (Exception) { continue; }
                }
                else
                {
                    try { dal.CreateStationLine(stationLine); }
                    catch (Exception) { continue; }
                }
            }
            if(stationIndex == 0)
            {
                line.FirstStation = stationCode;
                try { dal.UpdateLine(line); }
                catch (Exception) { }
            }
            if(stationIndex > lastStation.StationNumberInLine)
            {
                line.LastStation = stationCode;
                try { dal.UpdateLine(line); }
                catch (Exception) { }
            }
            #endregion
        }
        
        public void AddTripLine(int Id, TimeSpan startTime, TimeSpan endTime, TimeSpan frequency)
        {
            DO.LineTrip lineTrip = null;
            DO.LineTrip firstTrip = null;
            DO.LineTrip lastTrip = null;
            List<DO.LineTrip> tripsToRemove = new List<DO.LineTrip>();

            #region Get data from DAL
            try { dal.GetLine(Id); }
            catch(Exception ex) { throw new LineDoesNotExistException(ex.Message, ex); }

            try { lineTrip = dal.RequestLineTrip(Id, startTime); }
            catch (Exception) { }
            if (lineTrip != null)      // lineTrip already exist
                throw new LineTripAlreadyExistException($"A line with ID: {Id} and " +
                    $"start time: {startTime.ToString(@"hh\:mm\:ss")} was found in the system.");

            try { firstTrip = dal.RequestLineTripByPredicate(i => i.ID == Id && i.IsActive
                              && i.StartTime < startTime && i.EndTime > startTime); }
            catch (Exception) { }

            try { lastTrip = dal.RequestLineTripByPredicate(i => i.ID == Id && i.IsActive
                                && i.StartTime < endTime && i.EndTime > endTime); }
            catch (Exception) { }

            tripsToRemove = dal.GetLineTripsByPredicate(i => i.ID == Id && i.IsActive &&
                                                    i.StartTime >= startTime && i.EndTime <= endTime).ToList();
            #endregion

            DO.LineTrip newLineTrip = new DO.LineTrip
            {
                ID = Id,
                StartTime = startTime,
                EndTime = endTime,
                Frequency = frequency,
                IsActive = true
            };

            /* 
             * Case1: The trip will be between trips
             * Case2: Only the start time will be while a trip
             * Case3: Only the end time will be while a trip
             * Case4: Both start time & end time are not in trips
             */
            #region Cases
            if (firstTrip != null && lastTrip != null)       // Case 1
            {
                tripsToRemove.Add(firstTrip.Convert<DO.LineTrip, DO.LineTrip>());
                tripsToRemove.Add(lastTrip.Convert<DO.LineTrip, DO.LineTrip>());
                
                firstTrip.EndTime = startTime;
                lastTrip.StartTime = endTime;

                foreach (var trip in tripsToRemove)
                    try { dal.RemoveLineTrip(trip.ID, trip.StartTime); }
                    catch (Exception) { }

                try { dal.CreateLineTrip(firstTrip); }
                catch (Exception) { }

                try { dal.CreateLineTrip(lastTrip); }
                catch (Exception) { }
            }
            else if(firstTrip != null && lastTrip == null)      // Case 2
            {
                firstTrip.EndTime = startTime;

                foreach (var trip in tripsToRemove)
                    try { dal.RemoveLineTrip(trip.ID, trip.StartTime); }
                    catch (Exception) { }

                try { dal.UpdateLineTrip(firstTrip); }
                catch (Exception) { }
            }
            else if(firstTrip == null && lastTrip != null)          // Case 3
            {
                tripsToRemove.Add(lastTrip.Convert<DO.LineTrip, DO.LineTrip>());
                lastTrip.StartTime = endTime;

                foreach (var trip in tripsToRemove)
                    try { dal.RemoveLineTrip(trip.ID, trip.StartTime); }
                    catch (Exception) { }

                try { dal.CreateLineTrip(lastTrip); }
                catch (Exception) { }
            }
            else if(firstTrip == null && lastTrip == null)              // Case 4
            {
                foreach (var trip in tripsToRemove)
                    try { dal.RemoveLineTrip(trip.ID, trip.StartTime); }
                    catch (Exception) { }
            }
            else { throw new AnErrorOccurredException("An unknown error occured. Please try again later."); }
            #endregion

            dal.CreateLineTrip(newLineTrip);
        }
        #endregion

        #region Update Functions (Public functions)
        public void UpdateStation(int stationCode, string name, double latitude, double longitude, string address)
        {
            // Create DO station
            DO.Station updatedStation = new DO.Station()
            {
                StationCode = stationCode,
                Name = name,
                Latitude = latitude,
                Longitude = longitude,
                Address = address,
                IsActive = true
            };

            // Send to DAL to update
            try { dal.UpdateStation(updatedStation); }
            catch (Exception ex) { throw new StationDoesNotExistException(ex.Message, ex); }
        }

        public void UpdateLine(int Id, int lineNumber, int newFirstStationCode, int newLastStationCode, Line.Areas area)
        {
            /* Request all data from DAL; line, stationLines, first and last station, 
                                          the new first/last Station*/

            DO.Line line;
            DO.StationLine firstStation;
            DO.StationLine lastStation;
            List<DO.StationLine> stationLines = new List<DO.StationLine>();
            List<int> stationLineToRemove = new List<int>();
            List<DO.StationLine> stationLineWithUpdatedIndexes = new List<DO.StationLine>();
            List<DO.AdjStations> adjStationsToAdd = new List<DO.AdjStations>();

            #region Get All Info From DAL
            try { line = dal.RequestLine(Id); }
            catch(Exception ex) { throw new LineDoesNotExistException(ex.Message, ex); } // Resources Error!

            try { firstStation = dal.RequestStationLine(Id, line.FirstStation); lastStation = dal.RequestStationLine(Id, line.LastStation); }
            catch (Exception ex) { throw new StationLineDoesNotExistException(ex.Message, ex); } // Resources Error!

            try { dal.RequestStation(newFirstStationCode); dal.RequestStation(newLastStationCode); }   // Check if the new first station exist
            catch (Exception ex) { throw new StationDoesNotExistException(ex.Message, ex); } // Resources Error!

            for (int i = firstStation.StationNumberInLine; i <= lastStation.StationNumberInLine; i++)
            {
                DO.StationLine stationLine;
                try { stationLine = dal.RequestStationLineByIndex(Id, i); }
                catch (Exception ex) { throw new StationLineDoesNotExistException(ex.Message, ex); } // Resources Error!
                stationLines.Add(stationLine);
            }
            #endregion

            #region Update the new first/last station
            // Get the newFirstStation and update it
            DO.StationLine newFirstStation;
            try
            {
                newFirstStation = dal.RequestStationLine(Id, newFirstStationCode);
                for (int i = firstStation.StationNumberInLine; i < newFirstStation.StationNumberInLine; i++)
                    stationLineToRemove.Add(stationLines[i - firstStation.StationNumberInLine].StationCode);
                for (int i = newFirstStation.StationNumberInLine, j = 0; i <= lastStation.StationNumberInLine; i++, j++)
                {
                    stationLineWithUpdatedIndexes.Add(new DO.StationLine()
                    {
                        ID = Id,
                        StationCode = stationLines[i - firstStation.StationNumberInLine].StationCode,
                        StationNumberInLine = j,
                        IsActive = true
                    });
                }
            }
            catch(Exception)    // The newFirstStation was not found in the system. Add new one
            {
                stationLineWithUpdatedIndexes.Add(new DO.StationLine()
                {
                    ID = Id,
                    StationCode = newFirstStationCode,
                    StationNumberInLine = firstStation.StationNumberInLine,
                    IsActive = true
                });
                for (int i = firstStation.StationNumberInLine; i <= lastStation.StationNumberInLine; i++)
                {
                    DO.StationLine currentStationLine = stationLines[i - firstStation.StationNumberInLine];
                    stationLineWithUpdatedIndexes.Add(new DO.StationLine()
                    {
                        ID = Id,
                        StationCode = currentStationLine.StationCode,
                        StationNumberInLine = currentStationLine.StationNumberInLine + 1,
                        IsActive = true
                    });
                }
            }

            // Get the newLastStation and update it
            DO.StationLine newLastStation;
            try
            {
                newLastStation = dal.RequestStationLine(Id, newLastStationCode);
                for (int i = newLastStation.StationNumberInLine + 1; i <= lastStation.StationNumberInLine; i++)
                    stationLineToRemove.Add(stationLines[i - firstStation.StationNumberInLine].StationNumberInLine);
            }
            catch(Exception)    // The newLastStation was not found in the system. Add new one
            {
                stationLineWithUpdatedIndexes.Add(new DO.StationLine()
                {
                    ID = Id,
                    StationCode = newLastStationCode,
                    StationNumberInLine = lastStation.StationNumberInLine + 1,
                    IsActive = true
                });
            }
            #endregion

            #region AdjStations
            // Convert each 2 stations to AdjStations
            for (int i = 0; i <= stationLineWithUpdatedIndexes.Count - 1; i++)
            {
                DO.Station station1, station2;
                // Get the both stations
                try {
                    station1 = dal.RequestStation(stationLineWithUpdatedIndexes[i].StationCode);
                    station2 = dal.RequestStation(stationLineWithUpdatedIndexes[i + 1].StationCode); 
                }
                catch(Exception ex) { throw new StationDoesNotExistException(ex.Message, ex); } // A station does not exist
                try {
                    double distance = CalculateDistance(station1, station2);
                    TimeSpan time = TimeSpan.FromHours(distance / 60);
                    adjStationsToAdd.Add(new DO.AdjStations()
                    {
                        StationCode1 = station1.StationCode,
                        StationCode2 = station2.StationCode,
                        Distance = distance,
                        Time = time
                    });
                }
                catch (Exception) { continue; }     // Already exist... It's fine...
            }
            #endregion

            #region Update and remove the values in DAL
            // Send the updated StationLines to DAL
            foreach (var updatedStationLine in stationLineWithUpdatedIndexes)
            {
                try { dal.UpdateStationLine(updatedStationLine); }
                catch(Exception) { continue; }
            }
            // Remove the unnecessary StationLines from DAL
            foreach (var unnecessaryStationLine in stationLineToRemove)
            {
                try { dal.RemoveStationLine(Id, unnecessaryStationLine); }
                catch (Exception) { continue; }
            }
            // Add the AdjStations (if does not exist...)
            foreach (var adjStations in adjStationsToAdd)
            {
                try { dal.CreateAdjStations(adjStations); }
                catch(Exception) { continue; } // If we are here that means the adjStations is already exist
            }

            DO.Line.Areas DoArea;
            if (!Enum.TryParse(area.ToString(), true, out DoArea)) DoArea = DO.Line.Areas.General;
            DO.Line updatedLine = new DO.Line() 
            { 
                ID = Id,
                LineNumber = lineNumber,
                FirstStation = newFirstStationCode,
                LastStation = newLastStationCode,
                Area = DoArea,
                IsActive = true
            };
            try { dal.UpdateLine(updatedLine); }
            catch(Exception ex) { throw new LineDoesNotExistException(ex.Message, ex); }
            #endregion
        }

        public void UpdateAdjStations(int stationCode1, int stationCode2, double distance, TimeSpan time)
        {
            distance = Math.Abs(distance);

            try { dal.RequestStation(stationCode1); dal.RequestStation(stationCode2); }
            catch(Exception ex) { throw new StationDoesNotExistException(ex.Message, ex); }

            DO.AdjStations adjStations;
            try { adjStations = dal.RequestAdjStations(stationCode1, stationCode2); }
            catch(Exception) { AddAdjStations(stationCode1, stationCode2, distance, time); return; }

            adjStations.Distance = distance;
            adjStations.Time = time;

            try { dal.UpdateAdjStations(adjStations); }
            catch (Exception) { AddAdjStations(stationCode1, stationCode2); }
        }

        public void UpdateStationLine(int Id, int stationCode, int stationNumberInLine)
        {
            RemoveStationLine(Id, stationCode);     // Remove the station from his index
            AddStationLine(Id, stationCode, stationNumberInLine); // Add the station in another index
        }
        #endregion

        #region Request Functions (Public Functions)
        public Station RequestStation(int stationCode) =>
            BuildStation(stationCode);

        public Line RequestLine(int Id) =>
            BuildLine(Id);

        public AdjStations RequestAdjStations(int StationCoed1, int StationCoed2) =>
            BuildAdjStation(StationCoed1, StationCoed2);

        public StationLine RequestStationLine(int Id, int stationCode) =>
            BuildStationLine(stationCode);
        #endregion

        #region Remove Functions (Public functions)
        public void RemoveStation(int StationCode)
        {
            #region Get the data from DAL
            // Check if the Station exists
            try { dal.RequestStation(StationCode); }
            catch(Exception) { return; }
            // Get AdjStations
            IEnumerable<DO.AdjStations> filteredAdjStations = (from station in dal.GetAdjStations()
                                                               where station.StationCode1 == StationCode ||
                                                               station.StationCode2 == StationCode
                                                               select station).ToList();    // Take linq action now because the foreach
            // Get StationLines
            IEnumerable<DO.StationLine> filteredStationLines = (from stationLine in dal.GetStationLines()
                                                                where stationLine.StationCode == StationCode
                                                                select stationLine).ToList();    // Take linq action now because the foreach
            #endregion

            #region Delete each kind of station
            try { dal.RemoveStation(StationCode); }
            catch(Exception) { return; }
            foreach (var item in filteredAdjStations)
                RemoveAdjStations(item.StationCode1, item.StationCode2);
            foreach (var item in filteredStationLines)
                try { RemoveStationLine(item.ID, item.StationCode); }
                catch(Exception) { continue; }
            #endregion
        }

        public void RemoveLine(int ID)
        {
            DO.Line line;
            DO.StationLine firstStation, lastStation;
            List<DO.StationLine> stationLines = new List<DO.StationLine>();
            List<DO.LineTrip> lineTrips = new List<DO.LineTrip>();
            
            #region Get the data from DAL
            // Get the line
            try { line = dal.RequestLine(ID); }
            catch(Exception) { return; }

            // Get the first/last stations
            try
            { firstStation = dal.RequestStationLine(ID, line.FirstStation);
                lastStation = dal.RequestStationLine(ID, line.LastStation); }
            catch (Exception) { dal.RemoveLine(ID); return; }

            // Get the stations of the line
            for (int i = firstStation.StationNumberInLine; i <= lastStation.StationNumberInLine; i++)
                try { stationLines.Add(dal.RequestStationLineByIndex(ID, i)); }
                catch (Exception) { dal.RemoveLine(ID); return; }

            // Get the linetrips of the line
            foreach (var trip in dal.GetLineTripsByPredicate(i => i.ID == ID))
                lineTrips.Add(trip);
            #endregion

            #region Remove the data from DAL
            // Remove each StationLine in the line
            foreach (var stationLine in stationLines)
            {
                try { dal.RemoveStationLine(ID, stationLine.StationCode); }
                catch (Exception) { continue; }
            }
            foreach (var trip in lineTrips)
            {
                try { dal.RemoveLineTrip(ID, trip.StartTime); }
                catch (Exception) { continue; }
            }
            // Remove the line
            try { dal.RemoveLine(ID); }
            catch(Exception) { return; }
            #endregion
        }

        public void RemoveAdjStations(int StationCode1, int StationCode2)
        {
            try { dal.RemoveAdjStations(StationCode1, StationCode2); }
            catch (Exception) { return; }
        }

        public void RemoveStationLine(int Id, int stationCode)
        {
            DO.Line line;
            DO.StationLine firstStation, lastStation;
            DO.StationLine stationLineToRemove;
            List<DO.StationLine> stationLines = new List<DO.StationLine>();

            #region Get the data from DAL
            // Get the StationLine
            try { stationLineToRemove = dal.RequestStationLine(Id, stationCode); }
            catch (Exception) { return; }

            // Get the line
            try { line = dal.RequestLine(Id); }
            catch (Exception) { dal.RemoveStationLine(Id, stationCode); return; }

            // Get the first/last station of the line
            try { firstStation = dal.RequestStationLine(Id, line.FirstStation);
              lastStation = dal.RequestStationLine(Id, line.LastStation); }
            catch (Exception) { dal.RemoveStationLine(Id, stationCode); return; }

            // Get the stations in the line
            for (int i = firstStation.StationNumberInLine; i <= lastStation.StationNumberInLine; i++)
                try { stationLines.Add(dal.RequestStationLineByIndex(Id, i)); }
                catch (Exception) { dal.RemoveStationLine(Id, stationCode); return; }
            #endregion

            #region Change the indexes
            // Change the indexes of each station
            for (int i = stationLineToRemove.StationNumberInLine + 1; i <= lastStation.StationNumberInLine; i++)
                stationLines[i - firstStation.StationNumberInLine].StationNumberInLine--;
            #endregion

            #region Update the data to DAL
            if (line.FirstStation == stationCode)
            {
                line.FirstStation = stationLines[stationLineToRemove.StationNumberInLine - firstStation.StationNumberInLine + 1].StationCode;
                try { dal.UpdateLine(line); }
                catch (Exception ex) { throw new LineDoesNotExistException(ex.Message, ex); }
            }
            else if (line.LastStation == stationCode)
            {
                line.LastStation = stationLines[stationLineToRemove.StationNumberInLine - firstStation.StationNumberInLine - 1].StationCode;
                try { dal.UpdateLine(line); }
                catch (Exception ex) { throw new LineDoesNotExistException(ex.Message, ex); }
            }
            // Update the new data to DAL
            foreach (var item in stationLines)
            {
                try { dal.UpdateStationLine(item); }
                catch (Exception) { continue; }
            }
            // Remove the StationLine from DAL
            try { dal.RemoveStationLine(Id, stationCode); }
            catch (Exception) { return; }
            #endregion
        }

        public void RemoveLineTrip(int Id, TimeSpan startTime)
        {
            try { dal.RemoveLineTrip(Id, startTime); }
            catch (Exception) { return; }
        }
        #endregion

        #region GetCount Functions (Public functions)
        public int GetLinesCount() => dal.LinesCount();

        public int GetStationsCount() => dal.StationsCount();

        public int GetStationLinesCount() => dal.StationsLineCount();

        public int GetAdjStationsCount() => dal.AdjStationsCount();
        #endregion

        #region GetAll Functions (Public functions)
        public IEnumerable<Line> GetLines()
        {
            Line Line;
            foreach (var line in dal.GetLines().OrderBy(x => x.ID))
            {
                try { Line = BuildLine(line.ID); }
                catch (XmlLoadException ex) { throw new XmlLoadException(ex.Message, ex); }
                catch (Exception) { continue; }
                yield return Line;
            }
        }

        public IEnumerable<Station> GetStations()
        {
            Station Station;
            foreach (var station in dal.GetStations().OrderBy(x => x.StationCode))
            {
                try { Station = BuildStation(station.StationCode); }
                catch (XmlLoadException ex) { throw new XmlLoadException(ex.Message, ex); }
                catch (Exception) { continue; }
                yield return Station;
            }
        }

        public IEnumerable<AdjStations> GetAdjStations()
        {
            AdjStations adjStations;
            foreach (var singleAdjStations in dal.GetAdjStations().OrderBy(x => x.StationCode1))
            {
                try { adjStations = BuildAdjStation(singleAdjStations.StationCode1, singleAdjStations.StationCode2); }
                catch (XmlLoadException ex) { throw new XmlLoadException(ex.Message, ex); }
                catch (Exception) { continue; }
                yield return adjStations;
            }
        }

        public IEnumerable<LineTrip> GetLineTrips()
        {
            LineTrip lineTrip;
            foreach (var singleTrip in dal.GetLineTrips().OrderBy(x => x.StartTime))
            { 
                lineTrip = singleTrip.Convert<DO.LineTrip, LineTrip>();
                yield return lineTrip;
            }
        }
        #endregion

        #region Simulator
        public void StartSimulator(TimeSpan startTime, int rate, Action<TimeSpan> updateTime)
        {
            Simulator.TickEvent += updateTime;
            Simulator.Start(startTime, rate);
        }

        public void StopSimulator()
        {
            TravelOperator.Stop();
            Simulator.Stop();
        }

        public void SetStationPanel(int station, Action<LineTiming> updateBus)
        {
            TravelOperator.Stop();
            if (station == -1) return;
            TravelOperator.StationCode = station;
            TravelOperator.UpdateEvent += updateBus;
            TravelOperator.Start();
        }
        #endregion
    }
}