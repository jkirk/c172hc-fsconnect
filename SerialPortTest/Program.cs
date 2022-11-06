// Use this code inside a project created with the Visual C# > Windows Desktop > Console Application template.
// Replace the code in Program.cs with this code.

using System;
using System.IO.Ports;
using System.Threading;

public class PortChat
{
    static bool _continue;
    static SerialPort _serialPort;
    static string _defaultPortName;

    /// <summary>
    /// 
    /// </summary>
    public static void Main()
    {
        Thread readThread = new Thread(Read);

        // Create a new SerialPort object with default settings.
        _serialPort = new SerialPort();
        _defaultPortName = _serialPort.PortName;
        
        string[] ports = SerialPort.GetPortNames();
        if (ports.Length > 0) {
            _defaultPortName = ports[0];
            _serialPort.PortName = _defaultPortName;
        }
        // Set the read/write timeouts
        _serialPort.ReadTimeout = 500;
        _serialPort.WriteTimeout = 500;
        
        readThread.Start();

        bool showMenu = true;
        while (showMenu)
        {
            showMenu = MainMenu();
        }

        readThread.Join();
        _serialPort.Close();
    }

    private static bool MainMenu()
    {
        Console.Clear();
        Console.WriteLine("Welcome to c172hc-fsconnect SerialPortTest!\n");
        Console.WriteLine("1) Configure Serial Port");
        Console.WriteLine("2) Send Airspeed Demo Message");
        Console.WriteLine("3) Send start to target value in time interval");
        Console.WriteLine("4) Exit");

        Console.Write("\nSelect an option: ");

        switch (Console.ReadLine())
        {
            case "1":
                ConfigureSerialPort();
                return true;
            case "2":
                AirspeedDemoMessage();
                return true;
            case "3":
                return false;
            case "4":
                return false;
            default:
                return true;
        }
    }

    private static void ConfigureSerialPort()
    {
        _serialPort.Close();
        // Allow the user to set the appropriate properties.
        _serialPort.PortName = SetPortName(_defaultPortName);
        _serialPort.BaudRate = SetPortBaudRate(_serialPort.BaudRate);
        _serialPort.Parity = SetPortParity(_serialPort.Parity);
        _serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
        _serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
        _serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);
    }

    private static void AirspeedDemoMessage()
    {
        string message;
        Byte[] message_airspeed_demo = { 0x02, 0x20, 0x00, 0x00, 0x1D, 0x80, 0x42, 0x03 };
        StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
        Console.WriteLine("Type QUIT to exit and press ENTER to send Airspeed-Demo message");

        _serialPort.Open();
        _continue = true;
        while (_continue)
        {
            message = Console.ReadLine();

            if (stringComparer.Equals("quit", message))
            {
                _continue = false;
            }
            else
            {
                _serialPort.Write(message_airspeed_demo, 0, 8);
                Console.WriteLine("Message sent:  { 0x02, 0x20, 0x00, 0x00, 0x1D, 0x80, 0x42, 0x03 }");
            }
        }
        _serialPort.Close();
    }

    public static void Read()
    {
        while (_continue)
        {
            try
            {
                string message = _serialPort.ReadLine();
                // Console.WriteLine(message);
            }
            catch (TimeoutException) { }
        }
    }

    // Display Port values and prompt user to enter a port.
    public static string SetPortName(string defaultPortName)
    {
        string portName;

        Console.WriteLine("Available Ports:");
        foreach (string s in SerialPort.GetPortNames())
        {
            Console.WriteLine("   {0}", s);
        }

        Console.Write("Enter COM port value (Default: {0}): ", defaultPortName);
        portName = Console.ReadLine();

        if (portName == "" || !(portName.ToLower()).StartsWith("com"))
        {
            portName = defaultPortName;
        }
        return portName;
    }
    // Display BaudRate values and prompt user to enter a value.
    public static int SetPortBaudRate(int defaultPortBaudRate)
    {
        string baudRate;

        Console.Write("Baud Rate(default:{0}): ", defaultPortBaudRate);
        baudRate = Console.ReadLine();

        if (baudRate == "")
        {
            baudRate = defaultPortBaudRate.ToString();
        }

        return int.Parse(baudRate);
    }

    // Display PortParity values and prompt user to enter a value.
    public static Parity SetPortParity(Parity defaultPortParity)
    {
        string parity;

        Console.WriteLine("Available Parity options:");
        foreach (string s in Enum.GetNames(typeof(Parity)))
        {
            Console.WriteLine("   {0}", s);
        }

        Console.Write("Enter Parity value (Default: {0}):", defaultPortParity.ToString(), true);
        parity = Console.ReadLine();

        if (parity == "")
        {
            parity = defaultPortParity.ToString();
        }

        return (Parity)Enum.Parse(typeof(Parity), parity, true);
    }
    // Display DataBits values and prompt user to enter a value.
    public static int SetPortDataBits(int defaultPortDataBits)
    {
        string dataBits;

        Console.Write("Enter DataBits value (Default: {0}): ", defaultPortDataBits);
        dataBits = Console.ReadLine();

        if (dataBits == "")
        {
            dataBits = defaultPortDataBits.ToString();
        }

        return int.Parse(dataBits.ToUpperInvariant());
    }

    // Display StopBits values and prompt user to enter a value.
    public static StopBits SetPortStopBits(StopBits defaultPortStopBits)
    {
        string stopBits;

        Console.WriteLine("Available StopBits options:");
        foreach (string s in Enum.GetNames(typeof(StopBits)))
        {
            Console.WriteLine("   {0}", s);
        }

        Console.Write("Enter StopBits value (None is not supported and \n" +
         "raises an ArgumentOutOfRangeException. \n (Default: {0}):", defaultPortStopBits.ToString());
        stopBits = Console.ReadLine();

        if (stopBits == "")
        {
            stopBits = defaultPortStopBits.ToString();
        }

        return (StopBits)Enum.Parse(typeof(StopBits), stopBits, true);
    }
    public static Handshake SetPortHandshake(Handshake defaultPortHandshake)
    {
        string handshake;

        Console.WriteLine("Available Handshake options:");
        foreach (string s in Enum.GetNames(typeof(Handshake)))
        {
            Console.WriteLine("   {0}", s);
        }

        Console.Write("Enter Handshake value (Default: {0}):", defaultPortHandshake.ToString());
        handshake = Console.ReadLine();

        if (handshake == "")
        {
            handshake = defaultPortHandshake.ToString();
        }

        return (Handshake)Enum.Parse(typeof(Handshake), handshake, true);
    }
}