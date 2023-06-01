//
//
// Managed Data Request sample
//
// Click on Connect to try and connect to a running version of FSX
// Click on Request Data any number of times
// Click on Disconnect to close the connection, and then you should
// be able to click on Connect and restart the process
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

// Add these two statements to all SimConnect clients
using Microsoft.FlightSimulator.SimConnect;
using System.Runtime.InteropServices;

namespace Managed_Data_Request
{
    public partial class Form1 : Form
    {

        // User-defined win32 event
        const int WM_USER_SIMCONNECT = 0x0403;

        // SimConnect object
        SimConnect simconnect = null;

        enum DEFINITIONS
        {
            SixPack,
            MakerFaire,
        }

        enum DATA_REQUESTS
        {
            REQUEST_1,
            REQUEST_2,
        };

        // this is how you declare a data structure so that
        // simconnect knows how to fill it/read it.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct SixPack
        {
            // this is how you declare a fixed size string
            // [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            // public String title;
            public double airspeed_indicated;
            public double attitude_pitch;
            public double attitude_bank;
            public double altimeter;
            public double vertical_speed;
            public double heading;
            public int turn_coodinator;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct MakerFaire
        {
            // this is how you declare a fixed size string
            // [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            // public String title;
            public int airspeed_indicated;
            public int altimeter;
            public int vertical_speed;
            public int rpm;
        };

        public Form1()
        {
            InitializeComponent();

            setButtons(true, false, false, false);
        }
        // Simconnect client will send a win32 message when there is 
        // a packet to process. ReceiveMessage must be called to
        // trigger the events. This model keeps simconnect processing on the main thread.

        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == WM_USER_SIMCONNECT)
            {
                if (simconnect != null)
                {
                    simconnect.ReceiveMessage();
                }
            }
            else
            {
                base.DefWndProc(ref m);
            }
        }

        private void setButtons(bool bConnect, bool bGetSixPack, bool bGetMakerFaire, bool bDisconnect)
        {
            buttonConnect.Enabled = bConnect;
            buttonRequestSixPackData.Enabled = bGetSixPack;
            buttonRequestMakeFaireData.Enabled = bGetMakerFaire;
            buttonDisconnect.Enabled = bDisconnect;
        }

        private void closeConnection()
        {
            if (simconnect != null)
            {
                // Dispose serves the same purpose as SimConnect_Close()
                simconnect.Dispose();
                simconnect = null;
                displayText("Connection closed");
            }
        }

        // Set up all the SimConnect related data definitions and event handlers
        private void initDataRequest()
        {
            try
            {
                // listen to connect and quit msgs
                simconnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(simconnect_OnRecvOpen);
                simconnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(simconnect_OnRecvQuit);

                // listen to exceptions
                simconnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(simconnect_OnRecvException);

                // define a data structure
                simconnect.AddToDataDefinition(DEFINITIONS.SixPack, "AIRSPEED INDICATED", "knots", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.SixPack, "ATTITUDE INDICATOR PITCH DEGREES", "radians", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.SixPack, "ATTITUDE INDICATOR BANK DEGREES", "radians", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.SixPack, "PLANE ALTITUDE", "feet", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.SixPack, "VERTICAL SPEED", "feet per second", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.SixPack, "HEADING INDICATOR", "radians", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.SixPack, "TURN COORDINATOR BALL", "position 128", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);

                simconnect.AddToDataDefinition(DEFINITIONS.MakerFaire, "AIRSPEED INDICATED", "knots", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.MakerFaire, "PLANE ALTITUDE", "feet", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.MakerFaire, "VERTICAL SPEED", "feet per second", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.AddToDataDefinition(DEFINITIONS.MakerFaire, "GENERAL ENG RPM:1", "rpm", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);

                // IMPORTANT: register it with the simconnect managed wrapper marshaller
                // if you skip this step, you will only receive a uint in the .dwData field.
                simconnect.RegisterDataDefineStruct<SixPack>(DEFINITIONS.SixPack);
                simconnect.RegisterDataDefineStruct<MakerFaire>(DEFINITIONS.MakerFaire);

                // catch a simobject data request
                simconnect.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(simconnect_OnRecvSimobjectData);
            }
            catch (COMException ex)
            {
                displayText(ex.Message);
            }
        }

        void simconnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            displayText("Connected to FSX");
        }

        // The case where the user closes FSX
        void simconnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            displayText("FSX has exited");
            closeConnection();
        }

        void simconnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            displayText("Exception received: " + data.dwException);
        }

        // The case where the user closes the client
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            closeConnection();
        }

        void simconnect_OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {

            switch ((DATA_REQUESTS)data.dwRequestID)
            {
                case DATA_REQUESTS.REQUEST_1:
                    SixPack s1 = (SixPack)data.dwData[0];
                    string display;
                    display = "Airspeed [knots]: " + s1.airspeed_indicated;
                    display += "\nAttitude pitch [radians]: " + s1.attitude_pitch;
                    display += "\nAttitude bank [radians]: " + s1.attitude_bank;
                    display += "\nAltimeter [feet]:      " + s1.altimeter;
                    display += "\nVertical speed [ft/s]: " + s1.vertical_speed;
                    display += "\nHeading [radians]: " + s1.heading;
                    display += "\nTurn coordinator [pos]: " + s1.turn_coodinator;
                    displayText(display);
                    break;

                case DATA_REQUESTS.REQUEST_2:
                    MakerFaire s2 = (MakerFaire)data.dwData[0];
                    string display_makerfaire;
                    display_makerfaire = "Airspeed [knots]: " + s2.airspeed_indicated;
                    display_makerfaire += "\nAltimeter [feet]:      " + s2.altimeter;
                    display_makerfaire += "\nVertical speed [ft/min]: " + s2.vertical_speed * 60;
                    display_makerfaire += "\nRPM [rpm]: " + s2.rpm;
                    displayText(display_makerfaire);
                    break;

                default:
                    displayText("Unknown request ID: " + data.dwRequestID);
                    break;
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (simconnect == null)
            {
                try
                {
                    // the constructor is similar to SimConnect_Open in the native API
                    simconnect = new SimConnect("Managed Data Request", this.Handle, WM_USER_SIMCONNECT, null, 0);

                    setButtons(false, true, true, true);

                    initDataRequest();

                }
                catch (COMException ex)
                {
                    displayText("Unable to connect to FSX");
                }
            }
            else
            {
                displayText("Error - try again");
                closeConnection();

                setButtons(true, false, false, false);
            }
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            closeConnection();
            setButtons(true, false, false, false);
        }

        private void buttonRequestSixPackData_Click(object sender, EventArgs e)
        {
            if (buttonRequestSixPackData.Text == "Request Aircraft SixPack Data")
            {
                setButtons(false, true, false, true);
                simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_1, DEFINITIONS.SixPack, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.VISUAL_FRAME, SIMCONNECT_DATA_REQUEST_FLAG.CHANGED, 0, 0, 0);
                buttonRequestSixPackData.Text = "Stop request";
                displayText("Request sent...");
            }
            else
            {
                setButtons(false, true, true, true);
                simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_1, DEFINITIONS.SixPack, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.VISUAL_FRAME, SIMCONNECT_DATA_REQUEST_FLAG.CHANGED, 0, int.MaxValue, 0);
                buttonRequestSixPackData.Text = "Request Aircraft SixPack Data";
                displayText("Request stopped...");
            }
        }

        private void buttonRequestMakeFaireData_Click(object sender, EventArgs e)
        {
            if (buttonRequestMakeFaireData.Text == "Request Aircraft MakerFaire Data")
            {
                setButtons(false, false, true, true);
                simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_2, DEFINITIONS.MakerFaire, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.VISUAL_FRAME, SIMCONNECT_DATA_REQUEST_FLAG.CHANGED, 0, 0, 0);
                buttonRequestMakeFaireData.Text = "Stop request";
                displayText("Request sent...");
            }
            else
            {
                setButtons(false, true, true, true);
                simconnect.RequestDataOnSimObject(DATA_REQUESTS.REQUEST_2, DEFINITIONS.MakerFaire, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.VISUAL_FRAME, SIMCONNECT_DATA_REQUEST_FLAG.CHANGED, 0, int.MaxValue, 0);
                buttonRequestMakeFaireData.Text = "Request Aircraft MakerFaire Data";
                displayText("Request stopped...");
            }

        }

        // Response number
        int response = 1;

        // Output text - display a maximum of 10 lines
        string output = "\n\n\n\n\n\n\n\n\n\n";

        void displayText(string s)
        {
            // remove first string from output
            // output = output.Substring(output.IndexOf("\n") + 1);

            // add the new string
            output = "Response #: " + response++ + "\n" + s;

            // display it
            richResponse.Text = output;
        }
    }
}
// End of sample
