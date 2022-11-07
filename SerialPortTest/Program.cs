// Use this code inside a project created with the Visual C# > Windows Desktop > Console Application template.
// Replace the code in Program.cs with this code.

using System;
using System.IO.Ports;
using System.Linq;
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
                StartToTargetValue();
                return true;
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

    private static byte CalculateChecksum(byte[] msg)
    {
        byte checksum = 0xFF;
        for (int i = 0; i < msg.Length; i++)
        {
            checksum ^= msg[i]; // XOR
        }
        return checksum;
    }

    private static void StartToTargetValue()
    {
        int start_value = 0;
        int target_value = 0;
        int target_time_interval = 0;
        float message_interval = 0.05F;  // 50ms
        string message;
       

        start_value = ReadStartValue();
        target_value = ReadTargetValue(start_value);
        target_time_interval = ReadTargetTimeInterval();

        _serialPort.Open();
        _continue = true;
        while (_continue)
        {
            int step_value = Convert.ToInt32(Math.Ceiling((target_value - start_value) / target_time_interval * message_interval + 0.5));
            Console.WriteLine("step_value: {0}", step_value);

            if (step_value < 0)
            {
                for (int value = start_value; value >= target_value; value += step_value)
                {
                    byte[] message_gauge_stx = { 0x02 };
                    byte[] message_gauge_code = { 0x20 };
                    byte[] message_value = new byte[5];
                    Buffer.BlockCopy(message_gauge_code, 0, message_value, 0, 1);
                    Buffer.BlockCopy(BitConverter.GetBytes(value), 0, message_value, 1, 4);
                    byte message_checksum = CalculateChecksum(message_value);
                    byte[] msg = { 0x02, message_value[0], message_value[1], message_value[2], message_value[3], message_value[4], message_value[5], message_checksum, 0x03 };

                    _serialPort.Write(msg, 0, 8);
                    // Console.WriteLine("value sent:  {0}", value.ToString("X8"));
                    // https://stackoverflow.com/a/58708490
                    Console.WriteLine("value sent:  {0}", string.Join(", ", msg.Select(b => "0x" + b.ToString("X2"))));
                }
            }
            else
            {
                for (int value = start_value; value <= target_value; value += step_value)
                {
                    byte[] message_gauge_stx = { 0x02 };
                    byte[] message_gauge_code = { 0x20 };
                    byte[] message_value = new byte[5];
                    Buffer.BlockCopy(message_gauge_code, 0, message_value, 0, 1);
                    Buffer.BlockCopy(BitConverter.GetBytes(value), 0, message_value, 1, 4);
                    byte message_checksum = CalculateChecksum(message_value);

                    byte[] msg = { 0x02, message_value[0], message_value[4], message_value[3], message_value[2], message_value[1], message_checksum, 0x03 };
                    _serialPort.Write(msg, 0, 8);
                    // Console.WriteLine("value sent:  {0}", value.ToString("X8"));
                    // https://stackoverflow.com/a/58708490
                    Console.WriteLine("value sent:  {0} (checksum: {1})", string.Join(", ", msg.Select(b => "0x" + b.ToString("X2"))), message_checksum.ToString("X2"));
                }
            }

            Console.Write("Type QUIT (+ ENTER) to quit or press ENTER to enter new target value: ");
            message = Console.ReadLine();
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
            if (stringComparer.Equals("quit", message))
            {
                _continue = false;
            }
            else
            {
                start_value = target_value;
                target_value = ReadTargetValue(start_value);
            }
        }

        _serialPort.Close();
    }

    private static int ReadStartValue()
    {
        int _start_value = 0;
        bool valid_input = false;
        string input;
        while (!valid_input)
        {
            Console.Write("Start value (default: 0): ");
            input = Console.ReadLine();
            if (input == "")
            {
                valid_input = true;
            }
            else
            {
                valid_input = int.TryParse(input, out _start_value);
                if (!valid_input)
                {
                    Console.WriteLine("Given start value is invalid!");
                }
            }
        }
        return _start_value;
    }
    private static int ReadTargetValue(int _start_value)
    {
        int _target_value = 0;
        bool valid_input = false;
        string input;

        while (!valid_input)
        {
            Console.Write("Target value: ");
            input = Console.ReadLine();
            valid_input = int.TryParse(input, out _target_value);
            if (!valid_input)
            {
                Console.WriteLine("Given target value is invalid!");
            }
            else if (_target_value == _start_value)
            {
                valid_input = false;
                Console.WriteLine("Start value and target value can not be the same value!");
            }
        }
        return _target_value;
    }

    private static int ReadTargetTimeInterval()
    {
        
        int _target_time_interval = 0;
        bool valid_input = false;
        string input;

        while (!valid_input)
        {
            Console.Write("Target time interval: ");
            input = Console.ReadLine();
            valid_input = int.TryParse(input, out _target_time_interval);
            if (_target_time_interval < 1)
            {
                valid_input = false;
                Console.WriteLine("Target time interval needs to be greater 0!");
            }
        }
        return _target_time_interval;
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