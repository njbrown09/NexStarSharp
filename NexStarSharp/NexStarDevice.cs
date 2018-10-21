using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using NexStarSharp.Enums;
using NexStarSharp.Exceptions;

namespace NexStarSharp
{
    public class NexStarDevice
    {
        private const long ROTOR_CONSTANT = 4294967296;


        private SerialPort _SerialPort;


        public bool IsConnected => _SerialPort != null && _SerialPort.IsOpen;


        /// <summary>
        /// Create a new instance of a nexstar device
        /// </summary>
        public NexStarDevice()
        {
            //Nothing here ATM
        }


        #region Connection Management
        
        /// <summary>
        /// Connect to the specified NexStar device.
        /// </summary>
        /// <param name="comPort">Com Port the NexStar device is running on.</param>
        /// <returns>If the connection was successful.</returns>
        public bool Connect(string comPort)
        {
            //Setup the serial port with the parameters given by NexStar
            _SerialPort = new SerialPort
            {
                PortName = comPort,
                BaudRate = 9600,
                Parity = Parity.None,
                StopBits = StopBits.One,
                ReadTimeout = 3500,
                WriteTimeout = 3500
            };
            
            _SerialPort.Open();

            return _SerialPort.IsOpen;
        }

        /// <summary>
        /// Disconnect from the current NexStar device.
        /// </summary>
        public void Disconnect()
        {
            _SerialPort.Close();
            _SerialPort.Dispose();
        }
        
        #endregion

        #region Telescope Movement

        /// <summary>
        /// Go to the specified azimuth and elevation
        /// </summary>
        /// <param name="azimuth">Azimuth in decimal degrees</param>
        /// <param name="elevation">Elevation in decimal degrees</param>
        public void GoToAzElev(double azimuth, double elevation)
        {
            if (!IsConnected) throw new NexStarException("Telescope Not Connected.");
            
            //Convert degrees into fractions of rotation
            double azimuthFraction = azimuth * 0.00277777778; //0.00277777778 is 1/360
            double elevationFraction = elevation * 0.00277777778; //0.00277777778 is 1/360

            //Convert the fractions into the numbers NexStar accepts
            long finalAzimuth = (long) (azimuthFraction * ROTOR_CONSTANT);
            long finalElevation = (long) (elevationFraction * ROTOR_CONSTANT);
            
            //Convert the final values into hexdecimal
            string azimuthHex = finalAzimuth.ToString("X8"); //X8 means hexdecimal but forces 8 digits.
            string elevationHex = finalElevation.ToString("X8"); //X8 means hexdecimal but forces 8 digits.
            
            //Write the data to the NexStar device.
            _SerialPort.WriteLine($"b{azimuthHex},{elevationHex}");
            
            //Clear the read buffer so it doesnt mess up any other commands.
            _SerialPort.DiscardInBuffer();
        }

        /// <summary>
        /// Immediately cancel the current goto operation.
        /// </summary>
        /// <exception cref="NexStarException"></exception>
        public void CancleGoto()
        {
            if (!IsConnected) throw new NexStarException("Telescope Not Connected.");
            
            _SerialPort.WriteLine("M");
            
            //Clear the read buffer so it doesnt mess up any other commands.
            _SerialPort.DiscardInBuffer();
        }
        
        /// <summary>
        /// Is the telescope currently being moved by GOTO?
        /// </summary>
        /// <returns>True of the telescope is currently being moved by goto.</returns>
        /// <exception cref="NexStarException"></exception>
        public bool IsMoving()
        {
            if (!IsConnected) throw new NexStarException("Telescope Not Connected.");
            
            _SerialPort.WriteLine("L"); //L is the command to check if GOTO is currently moving the scope.
            
            int moving = _SerialPort.ReadByte();
            
            _SerialPort.DiscardInBuffer();

            return moving == 49; //Its in ascii for some reason, and the ascii code for '1' is 49.
        }

        #endregion

        #region Telescope Information
        
        /// <summary>
        /// Get the model of the current telescope.
        /// </summary>
        /// <returns>The model of the connected telescope.</returns>
        /// <exception cref="NexStarException"></exception>
        public TelescopeModel GetTelescopeModel()
        {
            if (!IsConnected) throw new NexStarException("Telescope Not Connected.");
            
            _SerialPort.WriteLine("m"); //m (It MUST be lowercase!) is the command to get the model of the telescope.

            //Read the response from serial.
            TelescopeModel model = (TelescopeModel) _SerialPort.ReadChar();

            //Clear the read buffer so it doesnt mess up any other commands.
            _SerialPort.DiscardInBuffer();

            return model;
        }

        /// <summary>
        /// Is the telescope aligned?
        /// </summary>
        /// <returns>True if aligned, false if not aligned</returns>
        /// <exception cref="NexStarException"></exception>
        public bool IsAligned()
        {
            if (!IsConnected) throw new NexStarException("Telescope Not Connected.");
            
            _SerialPort.WriteLine("J"); //J is the command for checking alignment.

            int aligned = _SerialPort.ReadChar();
            
            _SerialPort.DiscardInBuffer();

            return aligned == 1;
        }

        #endregion
    }
}
