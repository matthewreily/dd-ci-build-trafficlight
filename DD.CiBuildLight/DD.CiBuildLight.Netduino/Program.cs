using System;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DD.CiBuildLight.Netduino.IO;
using DD.CiBuildLight.Netduino.Model;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace DD.CiBuildLight.Netduino
{
    public class Program
    {
        #region Public Methods

        public static void Main()
        {
            AllOff();
            StartUp();
            _timer = new Timer(CheckBuildStatus, null, 0, 10*1000);

            Thread.Sleep(Timeout.Infinite);
        }

        static Program()
        {
            GreenlightRelayPort = new OutputPort(Pins.GPIO_PIN_D2, Off);
            YellowlightRelayPort = new OutputPort(Pins.GPIO_PIN_D3, Off);
            RedlightRelayPort = new OutputPort(Pins.GPIO_PIN_D4, Off);
        }

        

        

        private static Timer _timer;
        private static BuildStatus _status;
        private const bool Off = true;
        private const bool On = false;

        private static OutputPort RedlightRelayPort { get; set; }

        private static OutputPort YellowlightRelayPort { get; set; }

        private static OutputPort GreenlightRelayPort { get; set; }

        #endregion Private Fields

        #region Private Methods

        private static void AllOff()
        {
            RedlightRelayPort.Write(Off);
            GreenlightRelayPort.Write(Off);
            YellowlightRelayPort.Write(Off);
        }

        private static void BuildFail()
        {
            AllOff();
            RedlightRelayPort.Write(On);
        }

        private static void BuildSuccess()
        {
            AllOff();
            GreenlightRelayPort.Write(On);
        }

        private static void CheckBuildStatus(object state)
        {
            _status = BuildStatusManager.GetBuildStatus();

            if (_status == null || _status.CommunicationError)
            {
                CommunicationError();
            }
            else if (_status.Status)
            {
                BuildSuccess();
            }
            else
            {
                BuildFail();
            }
        }

        private static void CommunicationError()
        {
            AllOff();
            YellowlightRelayPort.Write(On);
        }

        private static void StartUp()
        {
            AllOff();
            for (var i = 0; i < 5; i++)
            {
                AllOn();
                Thread.Sleep(1000);
                AllOff();
                Thread.Sleep(1000);
            }
        }

        private static void AllOn()
        {
            RedlightRelayPort.Write(On);
            GreenlightRelayPort.Write(On);
            YellowlightRelayPort.Write(On);
        }

        #endregion
    }
}
