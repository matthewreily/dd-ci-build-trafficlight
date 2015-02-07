using System.Threading;
using DD.CiBuildLight.Netduino.IO;
using DD.CiBuildLight.Netduino.Model;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace DD.CiBuildLight.Netduino
{
    public class Program
    {
        
        private static BuildStatus _status;
        private const bool Off = true;
        private const bool On = false;

        private static readonly OutputPort RedlightRelayPort;
        private static readonly OutputPort YellowlightRelayPort;
        private static readonly OutputPort GreenlightRelayPort;

        static Program()
        {
            GreenlightRelayPort = new OutputPort(Pins.GPIO_PIN_D2, Off);
            YellowlightRelayPort = new OutputPort(Pins.GPIO_PIN_D3, Off);
            RedlightRelayPort = new OutputPort(Pins.GPIO_PIN_D4, Off);
        }
        
        public static void Main()
        {
            All(Off);
            StartUp();
            var timer = new Timer(CheckBuildStatus, null, 0, 10*1000);
            Thread.Sleep(Timeout.Infinite);
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

        private static void All(bool onOrOff)
        {
            RedlightRelayPort.Write(onOrOff);
            GreenlightRelayPort.Write(onOrOff);
            YellowlightRelayPort.Write(onOrOff);
        }

        private static void BuildFail()
        {
            All(Off);
            RedlightRelayPort.Write(On);
        }

        private static void BuildSuccess()
        {
            All(Off);
            GreenlightRelayPort.Write(On);
        }
        private static void CommunicationError()
        {
            All(Off);
            YellowlightRelayPort.Write(On);
        }

        private static void StartUp()
        {
            All(Off);
            for (var i = 0; i < 5; i++)
            {
                All(On);
                Thread.Sleep(1000);
                All(Off);
                Thread.Sleep(1000);
            }
        }
    }
}