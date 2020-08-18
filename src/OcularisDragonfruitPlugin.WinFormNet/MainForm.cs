using Ocularis;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OcularisDragonfruitPlugin.WinFormNet
{
    public partial class MainForm : Form
    {
        OCAdapter.OCAdapter _adapter;
		ICamera _camera;
		ulong _iStreamID = 0;
		uint _iCallbackID = 0;
        ProcessFrameDelegate _procFrame;
        IntPtr _procFrameCallbackPtr;
        Domain.OcularisPluginClientPlatform _plugin;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ProcessFrameDelegate(
        // We need to decode the start and end time from two 32 bit values
        // The low dword holds the low bits of the start time
        // The low 16 bits of the high dword hold the high bits of the start time
        // The high 16 bits of the high dword hold the difference between the end time and the start time
                IntPtr times, // NOTE****: This has changed since v4.2.

                uint Handle,
                uint PixelFormat,
                IntPtr buffer1,
                uint w,
                uint h,
                uint stride,
                IntPtr buffer2,
                uint w2,
                uint h2,
                uint stride2,
                IntPtr buffer3,
                uint w3,
                uint h3,
                uint stride3,
                uint dwTimeHigh,
                uint dwTimeLow);

        public MainForm()
        {
            InitializeComponent();
		}

		/// <summary>
		/// Form Load Event Handler, initializes the Auga video engine and loads a 1x1 layout.
		/// </summary>
		private void MainForm_Load(object sender, EventArgs e)
		{
            _procFrame += new ProcessFrameDelegate(ProcessFrameImpl);

            _procFrameCallbackPtr = Marshal.GetFunctionPointerForDelegate(_procFrame);

            // initialize the Auga Video Engine
            axAuga.Init("0");
            axAuga.SetSourcePrivilegeValue("videohooks", "deferredmode", 1);
        }

		/// <summary>
		/// Form Closing Event Handler, shutdown the Auga video engine, log out of the adapter.
		/// </summary>
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
            _procFrame -= new ProcessFrameDelegate(ProcessFrameImpl);

            try
            {
                axAuga.VideoHookPTZCommand2(_iStreamID, 101/*STOP_LIVE*/, 0, 0, 0);
                axAuga.StopStreamingToProcessor(_iStreamID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception on Stop Streaming: " + ex.Message);
            }

            try
            {
                if (axAuga != null)
                {
                    axAuga.ShutDown();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception in ShutDown: " + ex.Message);
            }

            try
			{
				if (_adapter != null)
				{
					_adapter.Logout();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception on Logout: " + ex.Message);
			}
        }

		private bool LoginAndSetDefaultCamera()
        {
			try
			{
				if (_adapter == null)
				{
					_adapter = new OCAdapter.OCAdapter();
			
					if (_adapter.Login(txtIP.Text, txtUserName.Text, txtPassword.Text, Authentication.BASIC, TimeSpan.FromSeconds(60)) == LoginResult.Failure)
					{
						MessageBox.Show("Unable to Log in " + _adapter.GetErrorString(), "OnSSI SDK Sample Solution - Image Export", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return false;
					}
				}

				_camera = _adapter.Cameras().FirstOrDefault();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception in Login: " + ex.Message);
                return false;
			}

            return true;

        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (!LoginAndSetDefaultCamera())
            {
                return;
            }

			axAuga.SetViewportSource(0, _camera.GetLiveUrl());

            axAuga.PlayBrowse(0, 0, 1 * 1000);
            foreach (ISpeaker speaker in _camera.Speakers())
            {
                axAuga.SetCameraIDispatch(speaker.GetID(), speaker);
            }

            foreach (IMicrophone microphone in _camera.Mics())
            {
                axAuga.SetCameraIDispatch(microphone.GetID(), microphone);
            }

            short sStartYear, sStartMonth, sStartDay, sStartHour, sStartMinute, sStartSecond, sStartMillisecond;
			short sStopYear, sStopMonth, sStopDay, sStopHour, sStopMinute, sStopSecond, sStopMillisecond;

			DateTime dtEnd = DateTime.Now;
			DateTime dtStart = dtEnd.AddMinutes(-30);

			// times passed into Auga should be in UTC
			dtStart = dtStart.ToUniversalTime();
			sStartYear = (short)dtStart.Year;
			sStartMonth = (short)dtStart.Month;
			sStartDay = (short)dtStart.Day;
			sStartHour = (short)dtStart.Hour;
			sStartMinute = (short)dtStart.Minute;
			sStartSecond = (short)dtStart.Second;
			sStartMillisecond = (short)dtStart.Millisecond;

			// times passed into Auga should be in UTC
			dtEnd = dtEnd.ToUniversalTime();
			sStopYear = (short)dtEnd.Year;
			sStopMonth = (short)dtEnd.Month;
			sStopDay = (short)dtEnd.Day;
			sStopHour = (short)dtEnd.Hour;
			sStopMinute = (short)dtEnd.Minute;
			sStopSecond = (short)dtEnd.Second;
			sStopMillisecond = (short)dtEnd.Millisecond;

			string sFrameTime = "";
			try
			{
				// validate there is something to export
				sFrameTime = axAuga.ProbeFrameTime(_camera.GetLiveUrl(),                       // BSTR Url
													sStartYear,                                 // SHORT Year
													sStartMonth,                                // SHORT Month
													sStartDay,                                  // SHORT Day
													sStartHour,                                 // SHORT Hour
													sStartMinute,                               // SHORT Minute
													sStartSecond,                               // SHORT Second
													sStartMillisecond,                          // SHORT Millisecond
													2000                                        // LONG TimeoutMS
													);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception in ExportVideo, ProbeFrameTime: " + ex.Message);
				return;
			}

			if (sFrameTime == null)
			{
				MessageBox.Show("No frames to export!");
				return;
			}

			try
			{
				// create an avi video file from the currently loaded camera
				axAuga.StartAVIExport(_camera.GetLiveUrl(),                        // BSTR CameraURL
										"",                                         // BSTR MetaDataURL - not used
										"",                                         // BSTR AudioURL
										0,                                          // LONG CodecFourCC - index to the available codecs
										"Howdy Doody",                              // BSTR Preamble
										1,                                          // BOOL bShowTimestamp
										1,                                          // BOOL bShowCameraName
                                        txtFileToExport.Text,                       // BSTR Filename
										sStartYear,                                 // SHORT StartYearUTC
										sStartMonth,                                // SHORT StartMonthUTC
										sStartDay,                                  // SHORT StartDayUTC
										sStartHour,                                 // SHORT StartHourUTC
										sStartMinute,                               // SHORT StartMinuteUTC
										sStartSecond,                               // SHORT StartSecondUTC
										sStartMillisecond,                          // SHORT StartMilisecondUTC
										sStopYear,                                  // SHORT StopYearUTC
										sStopMonth,                                 // SHORT StopMonthUTC
										sStopDay,                                   // SHORT StopDayUTC
										sStopHour,                                  // SHORT StopHourUTC
										sStopMinute,                                // SHORT StopMinuteUTC
										sStopSecond,                                // SHORT StopSecondUTC
										sStopMillisecond,                           // SHORT StopMilisecondUTC
										0,                                          // LONG x  - zoom rectangle
										0,                                          // LONG y  - zoom rectangle
										0,                                          // LONG x2 - zoom rectangle
										0,                                          // LONG y2 - zoom rectangle
										4000                                        // LONG lQuality
										);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception in ExportVideo, StartAVIExport: " + ex.Message);
			}
		}

		private void btnShowCamera_Click(object sender, EventArgs e)
        {
            if (!LoginAndSetDefaultCamera())
            {
                return;
            }

            axAuga.PlayBrowse(0, 0, 1 * 1000);
            foreach (ISpeaker speaker in _camera.Speakers())
            {
                axAuga.SetCameraIDispatch(speaker.GetID(), speaker);
            }

            foreach (IMicrophone microphone in _camera.Mics())
            {
                axAuga.SetCameraIDispatch(microphone.GetID(), microphone);
            }
        }

		private void btnVideoGraphic_Click(object sender, EventArgs e)
        {
            if (!LoginAndSetDefaultCamera())
            {
                return;
            }

            axAuga.PlayBrowse(0, 0, 1 * 1000);

            foreach (var cam in _adapter.Cameras())
            {
                // set the neccesary privileges to view video
                axAuga.SetCameraIDispatch(cam.GetID(), cam);
            }

            _iStreamID = axAuga.StartStreamingToProcessorCallback2((ulong)_procFrameCallbackPtr.ToInt64(), _camera.GetLiveUrl(), (ulong)_iCallbackID, 0, 0, 1);

            axAuga.VideoHookPTZCommand2(_iStreamID, 100/*START_LIVE*/, 0, 0, 0);
        }

        /// <summary>
        /// Callback function for frame data
        /// </summary>
        /// <param name="times">The start time and duration.</param> 
        /// <param name="Handle">The ID of the Handle returning the data.</param>
        /// <param name="PixelFormat">#define VIDEOFORMAT_BGR  (1)
        ///                           #define VIDEOFORMAT_YUV2 (2)
        ///                           #define VIDEOFORMAT_RGBA (4)
        ///                           #define VIDEOFORMAT_ABGR (8)
        ///                           #define VIDEOFORMAT_411  (16)
        ///                           #define VIDEOFORMAT_420  (16)
        ///                           #define VIDEOFORMAT_422  (32)
        ///                           #define VIDEOFORMAT_GRAY (64)</param>
        /// <param name="buffer1">frame data buffer</param>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        /// <param name="stride">number of bytes from one row of pixels in memory to the next</param>
        /// <param name="buffer2">frame data buffer(optional)</param>
        /// <param name="w2">width</param>
        /// <param name="h2">height</param>
        /// <param name="stride2">number of bytes from one row of pixels in memory to the next</param>
        /// <param name="buffer3">frame data buffer(optional)</param>
        /// <param name="w3">width</param>
        /// <param name="h3">height</param>
        /// <param name="stride3">number of bytes from one row of pixels in memory to the next</param>
        /// <param name="dwTimeHigh">frame time</param>
        /// <param name="dwTimeLow">frame time</param>
        public void ProcessFrameImpl(IntPtr times, // NOTE****: This has changed since v4.2.
                                        uint Handle,
                                        uint PixelFormat,
                                        IntPtr buffer1,
                                        uint w,
                                        uint h,
                                        uint stride,
                                        IntPtr buffer2,
                                        uint w2,
                                        uint h2,
                                        uint stride2,
                                        IntPtr buffer3,
                                        uint w3,
                                        uint h3,
                                        uint stride3,
                                        uint dwTimeHigh,
                                        uint dwTimeLow)
        {
            //timesArray[0] = start time for frame.
            //timesArray[1] = end time for frame.
            //timesArray[2] = start time for the next frame. 
            long[] timesArray = new long[3];
            Marshal.Copy(times, timesArray, 0, 3);

            ulong startTime = (ulong)timesArray[0];
            ulong endTime = (ulong)timesArray[1];
            ulong nextTime = (ulong)timesArray[2];

            System.Console.WriteLine("StartTime: {0}, EndTime: {1}, NextTime: {2}", startTime, endTime, nextTime);

            using (var g = pnlCamera.CreateGraphics())
            {
                if ((buffer1 != null) && (w != 0) && (h != 0))
                {
                    using (Bitmap bmp = new Bitmap((int)w, (int)h, (int)(w * 3), System.Drawing.Imaging.PixelFormat.Format24bppRgb, buffer1))
                    {
                        g.DrawImage(bmp, 0, 0, pnlCamera.Width, pnlCamera.Height);
                    }
                }
                else
                {
                    MessageBox.Show("Bad data from ProcessFrameImpl");
                }
            }
        }

        /// <summary>
        /// An Auga Event to monitor the progress of an export.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                string sInfo = e.triggerURL.Substring("exportprogress:".Length);

                // get the progress 'element' of the export
                int iIndex1 = sInfo.IndexOf("progress:");

                string sProgress = sInfo.Substring(iIndex1);

                int iIndex2 = sProgress.IndexOf("\n");

                // get the 'progress' string value
                sProgress = sProgress.Substring("progress:".Length, (iIndex2 - "progress:".Length));

                // the value range is 0 - 1, so use as a double value
                double dProgress = Convert.ToDouble(sProgress);

                // get the state 'element' of the export
                iIndex1 = sInfo.IndexOf("state:");

                string sState = sInfo.Substring(iIndex1);

                iIndex2 = sState.IndexOf("\n");

                // get the 'progress' string value
                sState = sState.Substring("state:".Length, (iIndex2 - "state:".Length));

                int iState = Convert.ToInt16(sState);

                switch (iState)
                {
                    case 0:
                        sState = "Not Initialized";
                        break;

                    case 1:
                        sState = "Running";
                        break;

                    case 2:
                        sState = "Done";
						MessageBox.Show("Finished Export Video");
						break;

                    case 3:
                        sState = "Done with errors???";
                        break;

                    case 4:
                        sState = "Failed";
                        break;

                    case 5:
                        sState = "Canceled";
                        break;
                }
            }
        }

        private async Task btnGetAuth_Click(object sender, EventArgs e)
        {
            try
            {
                _plugin = new Domain.OcularisPluginClientPlatform();
                await _plugin.GetAuth(txtUserName.Text, txtPassword.Text, new string[] {
                    txtIP.Text
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception on GetAuth: " + ex.Message);
            }
        }

        private async Task btnGetChannelGroupAndChannelData_Click(object sender, EventArgs e)
        {
            try
            {
                await _plugin.GetChannelGroupAndChannelData(txtFileToExport.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception on GetChannelGroupAndChannelData: " + ex.Message);
            }
        }

        private async Task btnGetChannelStream_Click(object sender, EventArgs e)
        {
            try
            {
                var endDate = DateTime.Now;
                var startDate = endDate.AddMinutes(-30);
                await _plugin.GetChannelStream(_camera.GetID(), startDate, endDate, txtFileToExport.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception on GetChannelStream: " + ex.Message);
            }
        }
    }
}
