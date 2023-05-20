// Use this code inside a project created with the Visual C# > Windows Desktop > Console Application template.
// Replace the code in Program.cs with this code.

using System;
using System.IO.Ports;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

public class SerialPortTest
{
    static bool _continueDetectSerialPortsThread = false;
    static bool _continue;
    static SerialPort _serialPort;
    static bool _serialPortsExists = false;
    static bool _serialPortIsConnected = false;
    static int _serialPortCount = 0;
    static string _defaultPortName = "COM1";

    /// <summary>
    /// Continuously detects the presence of new or removed serial ports in the system using the SerialPort class.
    /// If a new port is detected, sets the default port to the last one in the list of port names and opens the new port for communication.
    /// If a port is lost, sets the default port to the last one in the list of port names if any, or sets the serialPortsExists flag to False and prints a warning message.
    /// The function runs in a separate thread until _continueDetectSerialPortsThread is set to False.
    /// </summary>
    private static void DetectSerialPorts()
    {
        while (_continueDetectSerialPortsThread)
        {
            string[] ports = SerialPort.GetPortNames();
            if (_serialPortCount < ports.Length && ports.Length > 0)
            {
                _serialPortCount = ports.Length;
                _defaultPortName = ports[ports.Length - 1];
                _serialPort.PortName = _defaultPortName;
                _serialPortsExists = true;

                Console.WriteLine($"\nINFO: New port detected: {ports[ports.Length - 1]}");
            }
            else if (_serialPortCount > ports.Length)
            {
                _serialPortCount = ports.Length;
                Console.WriteLine("\nINFO: Port lost!");
        if (ports.Length > 0)
        {
            _defaultPortName = ports[ports.Length - 1];
            _serialPort.PortName = _defaultPortName;
                    _serialPortsExists = true;
        }
        else
        {
                    _serialPortsExists = false;
                    Console.WriteLine("\nWARNING: no COM ports found!");
                }
            }
            Thread.Sleep(1000);
        }
        }

    /// <summary>
    /// Initializes a SerialPort object, starts the Thread which detects new serial ports and and displays a menu to the user until the program is exited.
    /// </summary>
    public static void Main()
    {
        // Create a new SerialPort object with default settings.
        _serialPort = new SerialPort();
        Thread detectSerialPortsThread = new Thread(DetectSerialPorts);

        detectSerialPortsThread.Start();
        _continueDetectSerialPortsThread = true;

        bool showMenu = true;
        while (showMenu)
        {
            showMenu = MainMenu();
        }

        _continueDetectSerialPortsThread = false;
        Console.WriteLine("\nINFO: Shutting down serial interface...");
        detectSerialPortsThread.Join();
        _serialPort.Close();
    }

    /// <summary>
    /// Show main menu
    /// </summary>
    private static bool MainMenu()
    {
        Console.Clear();
        Console.WriteLine("Welcome to c172hc-fsconnect SerialPortTest!");
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine();
        if (_serialPortsExists)
        {
            Console.WriteLine("Current serial port settings: {0} ({1},{2},{3},{4},{5})", _serialPort.PortName, _serialPort.BaudRate, _serialPort.DataBits, _serialPort.Parity, _serialPort.StopBits, _serialPort.Handshake);
        }
        else
        {
            Console.WriteLine("No COM ports detected!");
        }
        
        Console.WriteLine();
        Console.WriteLine("1) Configure Serial Port");
        Console.WriteLine("2) Send Airspeed Demo Message (hardcoded checksum)");
        Console.WriteLine("3) Send Airspeed Demo Message (calculated checksum)");
        Console.WriteLine("4) Send (single) Message (Gauge Type, Gauge Value)");
        Console.WriteLine("5) Send (multiple) Messages (Gauge Type, Gauge Value)");
        Console.WriteLine("7) Generate + send messages from start to target value with custom step + message interval");
        Console.WriteLine("8) Latency Analysis (Send n messages @ m ms)");
        Console.WriteLine();
        Console.WriteLine("r) Reload main menu");
        Console.WriteLine("q) Quit");
        Console.WriteLine();
        Console.Write("Select an option: ");

        switch (Console.ReadKey().Key)
        {
            case ConsoleKey.D1:
                ConfigureSerialPort();
                return true;
            case ConsoleKey.D2:
                AirspeedDemoMessageHardcoded();
                return true;
            case ConsoleKey.D3:
                AirspeedDemoMessageCalculated();
                return true;
            case ConsoleKey.D4:
                SendMessage();
                return true;
            case ConsoleKey.D5:
                SendMessages();
                return true;
            case ConsoleKey.D7:
                StartToTargetMessageInterval();
                return true;
            case ConsoleKey.D8:
                LatencyAnalysis();
                return true;

            case ConsoleKey.Q:
                return false;
            default:
                return true;
        }
    }

    /// <summary>
    /// Let the user set serial port settings
    /// </summary>
    private static void ConfigureSerialPort()
    {
        _serialPort.Close();
        // Allow the user to set the appropriate properties.
        Console.WriteLine();
        _serialPort.PortName = SetPortName(_defaultPortName);
        _serialPort.BaudRate = SetPortBaudRate(_serialPort.BaudRate);
        _serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
        _serialPort.Parity = SetPortParity(_serialPort.Parity);
        _serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
        _serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);
    }

    /// <summary>
    /// Sends the Airspeed-Demo message to the serial port.
    /// The message consists of a start-of-text (STX) byte, a gauge code byte, a value byte array of length 4, a checksum byte, and an end-of-text (ETX) byte.
    /// The checksum is hard-coded.
    /// The message is sent to the serial port when the user presses ENTER.
    /// </summary>
    private static void AirspeedDemoMessageHardcoded()
    {
        Byte[] message_airspeed_demo = { 0x02, 0x20, 0x00, 0x00, 0x1D, 0x80, 0x43, 0x03 };

        _serialPort.Open();
        _continue = true;
        while (_continue)
        {
            Console.WriteLine("Press ENTER to send Airspeed-Demo message or press q to quit.");
            switch (Console.ReadLine())
            {
                case "q":
                    _continue = false;
                    break;
                default:
                    _serialPort.Write(message_airspeed_demo, 0, message_airspeed_demo.Length);
                    PrintMessage(message_airspeed_demo);
                    break;
            }
        }
        _serialPort.Close();
    }

    /// <summary>
    /// Sends an Airspeed-Demo message to the serial port.
    /// The message consists of a start-of-text (STX) byte, a gauge code byte, a value byte array of length 4, a checksum byte, and an end-of-text (ETX) byte.
    /// The checksum is calculated based on the gauge code byte and the value byte array using the CalculateChecksum() method.
    /// The message is sent to the serial port when the user presses ENTER.
    /// </summary>
    private static void AirspeedDemoMessageCalculated()
    {
        const byte message_stx = 0x02;
        byte[] message_gauge_code = new byte[1] { 0x20 };
        byte[] message_value = new byte[4] { 0x00, 0x00, 0x1D, 0x80 };
        byte message_checksum = CalculateChecksum(message_gauge_code.Concat(message_value).ToArray());
        const byte message_etx = 0x03;
        byte[] msg = { message_stx, message_gauge_code[0], message_value[0], message_value[1], message_value[2], message_value[3], message_checksum, message_etx };

        _serialPort.Open();
        _continue = true;
        while (_continue)
        {
            Console.WriteLine("Press ENTER to send Airspeed-Demo message or press q to quit.");
            switch (Console.ReadLine())
            {
                case "q":
                    _continue = false;
                    break;
                default:
                    _serialPort.Write(msg, 0, msg.Length);
                    PrintMessage(msg);
                    break;
            }
        }
        _serialPort.Close();
    }

    /// <summary>
    /// Prints a message to the console indicating that a message has been sent,
    /// along with the contents of the message in hexadecimal format.
    /// </summary>
    /// <param name="msg">The byte array containing the message to print.</param>
    private static void PrintMessage(byte[] msg)
    {
        Console.Write("Message sent: ");
        string message = string.Join(", ", msg.Select(b => $"0x{b:X2}"));
        Console.WriteLine(message);
    }

    /// <summary>
    /// Calculates the checksum of a byte array.
    /// </summary>
    /// <param name="msg">The byte array to calculate the checksum for.</param>
    /// <returns>The calculated checksum.</returns>
    private static byte CalculateChecksum(byte[] msg)
    {
        byte sum = 0;
        for (int i = 0; i < msg.Length; i++)
        {
            sum += msg[i];
        }
        return (byte)(~sum + 1);
    }

    private static byte[] ValueToGaugeCode(UInt32 value)
    {
        byte[] message_gauge_code = new byte[1];
        message_gauge_code = BitConverter.GetBytes(value);
        return message_gauge_code;
    }

    private static byte[] ValueToGaugeValue(UInt32 value)
    {
        byte[] message_gauge_value = new byte[4];
        message_gauge_value[0] = (byte)(value >> 24);
        message_gauge_value[1] = (byte)(value >> 16);
        message_gauge_value[2] = (byte)(value >> 8);
        message_gauge_value[3] = (byte)value;
        return message_gauge_value;
    }

    /// <summary>
    /// Reads a value from the console, ensuring that it falls within a specified range and is numeric.
    /// </summary>
    /// <param name="value_name">The name of the value being read.</param>
    /// <param name="value_default">The default value to be used if the user enters nothing.</param>
    /// <param name="value_min">The minimum acceptable value.</param>
    /// <param name="value_max">The maximum acceptable value.</param>
    /// <returns>The UInt32 value entered by the user.</returns>
    private static UInt32 ReadValue(string value_name, string value_default = "0", UInt32 value_min = 0, UInt32 value_max = UInt32.MaxValue)
    {
        UInt32 input_value = 0;
        UInt32 default_value = 0;
        UInt32.TryParse(value_default, out default_value);
        bool valid_input = false;

        while (!valid_input)
        {
            Console.Write(value_name + " (default: {0} (0x{1:X2})): ", value_default, default_value);
            string input = Console.ReadLine();
            if (input == "")
            {
                input_value = default_value;
                valid_input = true;
            }
            else
            {
                valid_input = UInt32.TryParse(input, out input_value);
                valid_input = (valid_input && input_value >= value_min && input_value <= value_max);
                if (!valid_input)
                {
                    Console.WriteLine("ERROR: {0} must be numeric and between {1} and {2}", value_name, value_min, value_max);
                }
            }
        }
        return input_value;
    }

    /// <summary>
    /// A wrapper around ReadValue, where the Gauge Value is the prompted, the default value is 0 and the code needs to be a value between 0 and UInt32.MaxValue.
    /// </summary>
    /// <param name="value_name">The name of the value being read.</param>
    /// <param name="value_default">The default value to be used if the user enters nothing.</param>
    /// <param name="value_min">The minimum acceptable value.</param>
    /// <param name="value_max">The maximum acceptable value.</param>
    /// <returns>The UInt32 value entered by the user.</returns>
    private static UInt32 ReadGaugeValue(string value_name = "Gauge Value", string value_default = "0", UInt32 value_min = 0, UInt32 value_max = UInt32.MaxValue)
    {
        return ReadValue(value_name, value_default, value_min, value_max);
    }
    /// <summary>
    /// A wrapper around ReadValue, where the Gauge Code is the prompt, default value is 32 (0x20) and the code needs to be a value between 0 and 255.
    /// </summary>
    /// <param name="value_name">The name of the value being read.</param>
    /// <param name="value_default">The default value to be used if the user enters nothing.</param>
    /// <param name="value_min">The minimum acceptable value.</param>
    /// <param name="value_max">The maximum acceptable value.</param>
    /// <returns>The UInt32 value entered by the user.</returns>
    private static UInt32 ReadGaugeCode(string value_name = "Gauge Code", string value_default = "32", UInt32 value_min = 0, UInt32 value_max = 0xFF)
    {
        return ReadValue(value_name, value_default, value_min, value_max);
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

    /// <summary>
    /// Generate the c172hc message to be sent to the serial console
    /// </summary>
    /// <param name="message_gauge_code">The gauge code.</param>
    /// <param name="message_gauge_value">The gauge value.</param>
    /// <returns>The c172hc message as byte array[].</returns>
    private static byte[] GenerateMessage(byte[] message_gauge_code, byte[] message_gauge_value)
    {
        const byte message_stx = 0x02;
        const byte message_etx = 0x03;
        byte message_checksum = CalculateChecksum(message_gauge_code.Concat(message_gauge_value).ToArray());
        return new byte[] { message_stx, message_gauge_code[0], message_gauge_value[0], message_gauge_value[1], message_gauge_value[2], message_gauge_value[3], message_checksum, message_etx };
    }
    /// <summary>
    /// Asks the user for Gauge Code + Gauge Vaule and sends a message to the serial port.
    /// The message consists of a start-of-text (STX) byte, a gauge code byte, a value byte array of length 4, a checksum byte, and an end-of-text (ETX) byte.
    /// The checksum is calculated based on the gauge code byte and the value byte array using the CalculateChecksum() method.
    /// </summary>
    private static void SendMessage()
    {
        Console.Clear();
        Console.WriteLine("Send (single) Message (Gauge Code, Gauge Value)");
        Console.WriteLine("-----------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("Please enter values in decimal numeral system.");
        Console.WriteLine();

        _serialPort.Open();

        byte[] msg = GenerateMessage(ValueToGaugeCode(ReadGaugeCode()), ValueToGaugeValue(ReadGaugeValue()));

        _serialPort.Write(msg, 0, msg.Length);
        PrintMessage(msg);

        _serialPort.Close();
        Console.Write("Press ENTER to return to main menu!");
        _ = Console.ReadLine();
    }

    /// <summary>
    /// Asks the user repeatedly for Gauge Code + Gauge Vaule and sends multiple message to the serial port.
    /// The message consists of a start-of-text (STX) byte, a gauge code byte, a value byte array of length 4, a checksum byte, and an end-of-text (ETX) byte.
    /// The checksum is calculated based on the gauge code byte and the value byte array using the CalculateChecksum() method.
    /// </summary>
    private static void SendMessages()
    {
        bool continue_send_messages = true;

        Console.Clear();
        Console.WriteLine("Send (multiple) Messages (Gauge Code, Gauge Value)");
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("Please enter values in decimal numeral system.");
        Console.WriteLine();

        _serialPort.Open();
        _continue = true;
        while (continue_send_messages)
        {
            byte[] msg = GenerateMessage(ValueToGaugeCode(ReadGaugeCode()), ValueToGaugeValue(ReadGaugeValue()));

            _serialPort.Write(msg, 0, msg.Length);
            PrintMessage(msg);

            Console.Write("Press ENTER to send another message or press q to quit: ");
            switch (Console.ReadLine())
            {
                case "q":
                    continue_send_messages = false;
                    _continue = false;
                    break;
                default:
                    break;
            }
        }
        _serialPort.Close();
    }

    private static void StartToTargetValue()
    {
        byte[] message_gauge_code = { 0x20 };
        UInt32 start_value = 0;
        UInt32 target_value = 0;
        int target_time_interval = 0;
        const int message_interval = 50;
        string message;

        start_value = ReadValue("Start value");
        target_value = ReadValue("Target value");
        target_time_interval = ReadTargetTimeInterval();

        _serialPort.Open();
        _continue = true;
        while (_continue)
        {
            Int32 step_value = Convert.ToInt32(Math.Ceiling((target_value - start_value) / target_time_interval * message_interval / 1000 + 0.5));
            Console.WriteLine("step_value: {0}", step_value);

            if (step_value < 0)
            {
                for (UInt32 value = start_value; value >= target_value; value += (UInt32)step_value)
                {
                    byte[] msg = GenerateMessage(message_gauge_code, ValueToGaugeValue(value));
                    _serialPort.Write(msg, 0, 8);
                    PrintMessage(msg);
                    Thread.Sleep(message_interval);
                }
            }
            else
            {
                for (UInt32 value = start_value; value <= target_value; value += (UInt32)step_value)
                {
                    byte[] msg = GenerateMessage(message_gauge_code, ValueToGaugeValue(value));
                    _serialPort.Write(msg, 0, 8);
                    PrintMessage(msg);
                    Thread.Sleep(message_interval);
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
                target_value = ReadValue("Target value");
            }
        }

        _serialPort.Close();
    }

    /// <summary>
    /// Asks the user for the Gauge Code, a start and a target value, a step value, a timeout and perform a latency analysis.
    /// </summary>
    private static void LatencyAnalysis()
    {
        Console.Clear();
        Console.WriteLine("Latency Analysis");
        Console.WriteLine("----------------");
        Console.WriteLine();

        bool continue_send_messages = true;
        UInt32 start_value = 0;

        //_serialPort.Open();
        
        // At a baud rate of 9600 the maximum number of 8 byte messages would be 150
        // We expect messages every 50ms, so we can try to time out after 100ms
        // _serialPort.ReadTimeout = 100;

        byte[] message_gauge_code = ValueToGaugeCode(ReadGaugeCode());
        start_value = ReadValue("Start value");

        while (continue_send_messages)
        {
            bool valid_values = false;
            UInt32 target_value = 100;
            string serialport_data_block = "";
            int messsage_sent_count = 0;

            while (!valid_values)
            {
                if (start_value == 100)
                {
                    target_value = ReadValue("Target value", "200");
                }
                else
                {
                    target_value = ReadValue("Target value", "100");
                }
                
                if (start_value == target_value)
                {
                    Console.WriteLine("ERROR: Start and target value must not be the same value!");
                }
                else
                {
                    valid_values = true;
                }
            }

            UInt32 step_value = ReadValue("Step value", "1", 1, 1000);
            UInt32 message_interval = ReadValue("Message interval [ms]", "50", 50, 1000);
            UInt32 timeout_value = ReadValue("No data timeout after [ms]", "10000", 1);
            _serialPort.Open();
            _serialPort.ReadTimeout = (int)timeout_value;

            DateTime latencyAnalysisStart = DateTime.Now;
            bool latency_analysis = true;

            Console.WriteLine("Waiting for Latency Analysis start code @ {0}...", latencyAnalysisStart.ToString("yyyy-MM-ddTHH:mm:sszzz", System.Globalization.CultureInfo.InvariantCulture));
            while (latency_analysis)
            {
                try
                {
                    // c172hc-fsmaster basically does the following to send the start code 0x55:
                    //
                    //   int startCode = 0x55;
                    //   Serial.println(startCode);
                    //
                    // What happens is, that the decimal value of startCode = 0x55 (= 85) gets converted to the string "85\r\n"
                    // see: https://www.arduino.cc/reference/en/language/functions/communication/serial/println/
                    //
                    // So we have to trim the received string and match it against the string "85"

                    string serialport_data = _serialPort.ReadLine();

                    if (serialport_data.Trim() == "85" && messsage_sent_count == 0)
                    {
                        Console.WriteLine("Start code detected!");
                        Console.WriteLine("Latency Analysis started at: " + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz", System.Globalization.CultureInfo.InvariantCulture));
                        serialport_data_block = "";

                        if (start_value < target_value)
                        {
                            for (UInt32 value = start_value; value <= target_value; value += step_value)
                            {
                                byte[] msg = GenerateMessage(message_gauge_code, ValueToGaugeValue(value));
                                _serialPort.Write(msg, 0, 8);
                                messsage_sent_count++;
                                //  PrintMessage(msg);
                                Thread.Sleep((int)message_interval);
                            }
                        }
                        else
                        {
                            for (UInt32 value = start_value; value >= target_value; value -= step_value)
                            {
                                byte[] msg = GenerateMessage(message_gauge_code, ValueToGaugeValue(value));
                                _serialPort.Write(msg, 0, 8);
                                messsage_sent_count++;
                                // PrintMessage(msg);
                                Thread.Sleep((int)message_interval);
                            }
                        }
                    }
                    else if (serialport_data.Trim() == "85" && messsage_sent_count > 0)
                    {
                        Console.WriteLine("Latency Analysis stopped at: " + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz", System.Globalization.CultureInfo.InvariantCulture));
                        _serialPort.Close();
                        Console.WriteLine("== Latency Analysis Summary ==");
                        Console.WriteLine();
                        Console.WriteLine(serialport_data_block);
                        Console.WriteLine("# messsages sent: {0}", messsage_sent_count);
                        latency_analysis = false;
                    }
                    else if (serialport_data.Trim() != "85" && messsage_sent_count > 0)
                    {
                        serialport_data_block = serialport_data_block + "\n" + serialport_data;
                    }
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("Timeout @ " + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz", System.Globalization.CultureInfo.InvariantCulture));
                    latency_analysis = false;
                }
            }
            Console.Write("Press ENTER to send another message or press q to quit: ");
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.Q:
                    continue_send_messages = false;
                    break;
                default:
                    start_value = target_value;
                    Console.WriteLine("\ncurrent (start) value: {0}", start_value);
                    break;
            }
        }
        _serialPort.Close();
    }

    /// <summary>
    /// Asks the user for the Gauge Code, a start and atarget value, a step value and a message interval.
    /// Messages are then generated with the specified step value and at the specified message interval.
    /// </summary>
    private static void StartToTargetMessageInterval()
    {
        bool continue_send_messages = true;

        Console.Clear();
        Console.WriteLine("Generate + send messages from start to target value with custom step + message interval");
        Console.WriteLine("---------------------------------------------------------------------------------------");
        Console.WriteLine();

        _serialPort.Open();
        _continue = true;

        UInt32 start_value = 0;
        UInt32 target_value = 100;
        bool valid_values = false;

        byte[] message_gauge_code = ValueToGaugeCode(ReadGaugeCode());
        start_value = ReadValue("Start value");
        
        while (continue_send_messages)
        {
            while (!valid_values)
            {

                target_value = ReadValue("Target value", "100");
                if (start_value == target_value)
                {
                    Console.WriteLine("ERROR: Start and target value must not be the same value!");
                }
                else
                {
                    valid_values = true;
                }
            }

            UInt32 step_value = ReadValue("Step value", "1", 1, 1000);
            UInt32 message_interval = ReadValue("Message interval [ms]", "50", 50, 1000);
            Console.WriteLine("Press ENTER to send messsages");
            _ = Console.ReadLine();

            if (start_value < target_value)
            {
                for (UInt32 value = start_value; value <= target_value; value += step_value)
                {
                    byte[] msg = GenerateMessage(message_gauge_code, ValueToGaugeValue(value));
                    _serialPort.Write(msg, 0, 8);
                    PrintMessage(msg);
                    Thread.Sleep((int)message_interval);
                }
            }
            else
            {
                for (UInt32 value = start_value; value >= target_value; value -= step_value)
                {
                    byte[] msg = GenerateMessage(message_gauge_code, ValueToGaugeValue(value));
                    _serialPort.Write(msg, 0, 8);
                    PrintMessage(msg);
                    Thread.Sleep((int)message_interval);
                }
            }
            Console.Write("Press ENTER to send another message or press q to quit: ");
            switch (Console.ReadLine())
            {
                case "q":
                    continue_send_messages = false;
                    _continue = false;
                    break;
                default:
                    start_value = target_value;
                    valid_values = false;
                    Console.WriteLine("current (start) value: {0}", start_value);
                    break;
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