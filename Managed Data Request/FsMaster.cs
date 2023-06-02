using System;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Managed_Data_Request
{
    class FsMaster
    {
        SerialPort _serialPort;
        Thread _detectSerialPortsThread;
        bool _continueDetectSerialPortsThread = false;
        int _serialPortCount = 0;

        public static bool _serialPortIsConnected = false;

        public delegate void SerialPortChangedHandler(object source, EventArgs e);
        public event SerialPortChangedHandler SerialPortChanged;
        protected virtual void OnSerialPortChanged()
        {
            SerialPortChanged?.Invoke(this, null);
        }

        public void Start()
        {
            // Create a new SerialPort object with default settings.
            _serialPort = new SerialPort();
            _detectSerialPortsThread = new Thread(DetectSerialPorts);
            _detectSerialPortsThread.Start();
            _continueDetectSerialPortsThread = true;
        }

        public void Stop()
        {
            _continueDetectSerialPortsThread = false;
            _detectSerialPortsThread.Join();
            _serialPort.Close();
        }

        /// <summary>
        /// Continuously detects the presence of new or removed serial ports in the system using the SerialPort class.
        /// If a new port is detected, sets the default port to the last one in the list of port names and opens the new port for communication.
        /// If a port is lost, sets the default port to the last one in the list of port names if any, or sets the serialPortsExists flag to False and prints a warning message.
        /// The function runs in a separate thread until _continueDetectSerialPortsThread is set to False.
        /// </summary>
        private void DetectSerialPorts()
        {
            while (_continueDetectSerialPortsThread)
            {
                string[] ports = SerialPort.GetPortNames();

                if (_serialPortCount < ports.Length && ports.Length > 0)
                {
                    // First run or a new serial port has been detected.
                    // The serial port should only be set when the first port is detected.
                    // By default, select the highest serial port number.

                    _serialPortCount = ports.Length;
                    if (!ports.Contains(_serialPort.PortName))
                    {
                        _serialPort.Close();
                        _serialPort.PortName = ports[ports.Length - 1];
                        _serialPort.Open();   
                    }
                    OnSerialPortChanged();
                }
                else if (_serialPortCount > ports.Length)
                {
                    // One or more serial ports have been removed
                    // If a serial port remains and the serial port in use has been removed, switch to the next highest available port.
                    _serialPortCount = ports.Length;
                    if (ports.Length > 0 && !ports.Contains(_serialPort.PortName))
                    {
                        _serialPort.Close();
                        _serialPort.PortName = ports[ports.Length - 1];
                        _serialPort.Open();
                    }
                    else
                    {
                        _serialPort.Close();
                    }
                    OnSerialPortChanged();
                }
                Thread.Sleep(1000);
            }
        }

        // Get current port name
        public string GetPortName()
        {
            return _serialPort.PortName;
        }

        public void SetPortName(string portName)
        {
            bool portOpen = _serialPort.IsOpen;
            if (portName == _serialPort.PortName)
            {
                return;
            }

            _serialPort.Close();
            _serialPort.PortName = portName;
            if (portOpen)
            {
                _serialPort.Open();
            }
        }

        // Get current baud rate
        public int GetBaudRate()
        {
            return _serialPort.BaudRate;
        }

        public void SetBaudRate(string strBaudRate)
        {
            bool portOpen = _serialPort.IsOpen;
            int baudRate = int.Parse(strBaudRate);
            if (baudRate == _serialPort.BaudRate)
            {
                return;
            }
            
            _serialPort.Close();
            _serialPort.BaudRate = baudRate;
            if (portOpen)
            {
                _serialPort.Open();
            }
        }

        public bool SendMessage(int gauge_code, int gauge_value)
        {
            if (!_serialPort.IsOpen) return false;
            try
            {
                byte[] msg = GenerateMessage(ValueToGaugeCode(gauge_code), ValueToGaugeValue(gauge_value));
                _serialPort.Write(msg, 0, 8);
            }
            catch (TimeoutException) { }
            return true;

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

        private static byte[] ValueToGaugeCode(int value)
        {
            byte[] message_gauge_code = new byte[1];
            message_gauge_code = BitConverter.GetBytes(value);
            return message_gauge_code;
        }

        private static byte[] ValueToGaugeValue(int value)
        {
            byte[] message_gauge_value = new byte[4];
            message_gauge_value[0] = (byte)(value >> 24);
            message_gauge_value[1] = (byte)(value >> 16);
            message_gauge_value[2] = (byte)(value >> 8);
            message_gauge_value[3] = (byte)value;
            return message_gauge_value;
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
    }
}
