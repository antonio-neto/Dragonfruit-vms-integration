using DragonfruitPlugin.Client.Contracts.Abstract;
using DragonfruitPlugin.Client.Contracts.Model;
using DragonfruitPlugin.Client.Helpers;
using Ocularis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OcularisDragonfruitPlugin.Domain
{
    public class OcularisPluginClientPlatform : IPluginClientPlatform
    {
        private string _ip;
        private string _userName;
        private string _password;
        private OCAdapter.OCAdapter _adapter;
        private List<ICamera> _cameras;

        public async Task GetAuth(string userName, string password, string[] metadata)
        {
            _adapter = new OCAdapter.OCAdapter();

            _userName = userName;
            _password = password;
            _ip = metadata[0];

            GetAdapterAndLogin();

            await Task.CompletedTask;
        }

        public async Task GetAuth()
        {
            await Task.CompletedTask;
        }

        public async Task GetChannelGroupAndChannelData(string jsonPath)
        {
            GetAdapterAndLogin();

            try
            {
                var channelHierarchy = new ChannelHierarchy();
                var channelGroup = new ChannelGroup
                {
                    ObjectID = "_",
                    ChannelGroupName = string.Empty
                };
                _cameras = _adapter.Cameras().ToList();
                foreach (var camera in _cameras)
                {
                    channelGroup.Channels.Add(new Channel
                    {
                        Name = camera.GetDisplayName(),
                        ObjectID = camera.GetID()
                    });
                }
                channelHierarchy.ChannelGroups.Add(channelGroup);

                FileHelper.Serialize(channelHierarchy, jsonPath);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            await Task.CompletedTask;
        }

        public async Task GetChannelStream(string channelID, DateTime startTime, DateTime endTime, string path)
        {
            await GetChannelStream(channelID, startTime, endTime, path);
        }

        public async Task GetChannelStream(string channelID, DateTime startTime, DateTime endTime, string path, AxHeimdallLib.AxAuga axAuga = null)
        {
            GetAdapterAndLogin();

            var camera = _cameras.FirstOrDefault(cam => cam.GetID() == channelID);
            if (string.IsNullOrWhiteSpace(channelID))
            {
                camera = _cameras.FirstOrDefault();
            }

            if (camera == null)
            {
                throw new Exception($"Channel: {channelID} - doesn't exist.");
            }

            try
            {
                //if (axAuga == null)
                //{
                //    axAuga = new AxHeimdallLib.AxAuga();
                //    axAuga.Enabled = true;
                //    axAuga.Location = new System.Drawing.Point(258, 12);
                //    axAuga.Name = "axAuga";
                //    axAuga.Size = new System.Drawing.Size(579, 535);
                //    axAuga.TriggerSelected += new AxHeimdallLib._IAugaEvents_TriggerSelectedEventHandler(axAuga_TriggerSelected);
                //}

                // times passed into Auga should be in UTC
                var dtStart = startTime.ToUniversalTime();
                var sStartYear = (short)dtStart.Year;
                var sStartMonth = (short)dtStart.Month;
                var sStartDay = (short)dtStart.Day;
                var sStartHour = (short)dtStart.Hour;
                var sStartMinute = (short)dtStart.Minute;
                var sStartSecond = (short)dtStart.Second;
                var sStartMillisecond = (short)dtStart.Millisecond;

                // times passed into Auga should be in UTC
                var dtEnd = endTime.ToUniversalTime();
                var sStopYear = (short)dtEnd.Year;
                var sStopMonth = (short)dtEnd.Month;
                var sStopDay = (short)dtEnd.Day;
                var sStopHour = (short)dtEnd.Hour;
                var sStopMinute = (short)dtEnd.Minute;
                var sStopSecond = (short)dtEnd.Second;
                var sStopMillisecond = (short)dtEnd.Millisecond;

                var cameraLiveUrl = camera.GetLiveUrl();

                // validate there is something to export
                //var sFrameTime = axAuga.ProbeFrameTime(cameraLiveUrl,                       // BSTR Url
                //                                    sStartYear,                                 // SHORT Year
                //                                    sStartMonth,                                // SHORT Month
                //                                    sStartDay,                                  // SHORT Day
                //                                    sStartHour,                                 // SHORT Hour
                //                                    sStartMinute,                               // SHORT Minute
                //                                    sStartSecond,                               // SHORT Second
                //                                    sStartMillisecond,                          // SHORT Millisecond
                //                                    2000                                        // LONG TimeoutMS
                //                                    );


                //if (sFrameTime == null)
                //{
                //    throw new Exception("No frames to export!");
                //}

                //axAuga.StartAVIExport(cameraLiveUrl,                        // BSTR CameraURL
                //                            string.Empty,                                         // BSTR MetaDataURL - not used
                //                            string.Empty,                                         // BSTR AudioURL
                //                            0,                                          // LONG CodecFourCC - index to the available codecs
                //                            string.Empty,                              // BSTR Preamble
                //                            1,                                          // BOOL bShowTimestamp
                //                            1,                                          // BOOL bShowCameraName
                //                            path,                                  // BSTR Filename
                //                            sStartYear,                                 // SHORT StartYearUTC
                //                            sStartMonth,                                // SHORT StartMonthUTC
                //                            sStartDay,                                  // SHORT StartDayUTC
                //                            sStartHour,                                 // SHORT StartHourUTC
                //                            sStartMinute,                               // SHORT StartMinuteUTC
                //                            sStartSecond,                               // SHORT StartSecondUTC
                //                            sStartMillisecond,                          // SHORT StartMilisecondUTC
                //                            sStopYear,                                  // SHORT StopYearUTC
                //                            sStopMonth,                                 // SHORT StopMonthUTC
                //                            sStopDay,                                   // SHORT StopDayUTC
                //                            sStopHour,                                  // SHORT StopHourUTC
                //                            sStopMinute,                                // SHORT StopMinuteUTC
                //                            sStopSecond,                                // SHORT StopSecondUTC
                //                            sStopMillisecond,                           // SHORT StopMilisecondUTC
                //                            0,                                          // LONG x  - zoom rectangle
                //                            0,                                          // LONG y  - zoom rectangle
                //                            0,                                          // LONG x2 - zoom rectangle
                //                            0,                                          // LONG y2 - zoom rectangle
                //                            4000                                        // LONG lQuality
                //                          );

            }
            catch (Exception ex)
            {
                throw ex;
            }

            await Task.CompletedTask;
        }

        private void GetAdapterAndLogin()
        {
            if (_adapter != null)
            {
                return;
            }

            LoginResult result;
            try
            {
                _adapter = new OCAdapter.OCAdapter();

                result = _adapter.Login(_ip, _userName, _password, Authentication.BASIC, TimeSpan.FromSeconds(60));
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Trace.WriteLine("Exception in ExportVideo, ProbeFrameTime: " + ex.Message);
                throw ex;
            }

            if (result == LoginResult.Failure)
            {
                throw new Exception("Login failed, wrong credentials.");
            }
        }

        private void axAuga_TriggerSelected(object sender, AxHeimdallLib._IAugaEvents_TriggerSelectedEvent e)
        {
            // 'header' to look for in regards to exporting
            if (e.triggerURL.StartsWith("exportprogress:"))
            {
                //exportprogress:
                //guid: b275c2-8125-4334-b5a4-b17854b6a540
                //start: 130304902030000000
                //stop: 130304902632580000
                //label: 11.66 Axis Q1755
                //type: avi
                //state: 1
                //progress: 0
                //filelist:

                // peel off the 'header'
                var sInfo = e.triggerURL.Substring("exportprogress:".Length);

                // get the progress 'element' of the export
                var iIndex1 = sInfo.IndexOf("progress:");

                var sProgress = sInfo.Substring(iIndex1);

                var iIndex2 = sProgress.IndexOf("\n");

                // get the 'progress' string value
                sProgress = sProgress.Substring("progress:".Length, (iIndex2 - "progress:".Length));

                // the value range is 0 - 1, so use as a double value
                var dProgress = Convert.ToDouble(sProgress);

                // make the value meaningful for the progress bar
                //_progressBar.Value = Convert.ToInt32(dProgress * 100);

                // get the state 'element' of the export
                iIndex1 = sInfo.IndexOf("state:");

                var sState = sInfo.Substring(iIndex1);

                iIndex2 = sState.IndexOf("\n");

                // get the 'progress' string value
                sState = sState.Substring("state:".Length, (iIndex2 - "state:".Length));

                var iState = Convert.ToInt16(sState);

                if (iState == 2)
                {
                    // TODO
                    // send signal/event
                }
            }

        }
    }
}
