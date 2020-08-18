using System;
using System.Windows.Forms;

namespace OcularisDragonfruitPlugin.WinFormNet
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
		}

		/// <summary>
		/// Form Load Event Handler, initializes the Auga video engine and loads a 1x1 layout.
		/// </summary>
		private void MainForm_Load(object sender, EventArgs e)
		{
			// initialize the Auga Video Engine
			//axAuga.Init("0");
        }

		/// <summary>
		/// Form Closing Event Handler, shutdown the Auga video engine, log out of the adapter.
		/// </summary>
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
            if (axAuga != null)
            {
                axAuga.ShutDown();
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            //var winForm = new ExportVideo(txtIP.Text, txtUserName.Text, txtPassword.Text);
            //winForm.Show();
            //winForm.Focus();
            var adapter = new OCAdapter.OCAdapter();
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
    }
}
