using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace MimiSharp.Utility
{
    public static class IPEndpointChecker
    {

        public static bool IsPortInUse(int port)
        {
            // netstat -tan cmd 
            IPGlobalProperties iPGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var activeIpsEndpointTcpProtocol  = iPGlobalProperties.GetActiveTcpConnections();

            foreach (var ipEndpoint  in activeIpsEndpointTcpProtocol)
            {
                if (ipEndpoint.LocalEndPoint.Port == port)
                    return true;
               
            }
            return false;

        }

        public static int FindAvailablePort (int perferedPort , int noOfAttemps)
                => !IsPortInUse(perferedPort) ? perferedPort : FindAvaliableRandomPort( noOfAttemps);

        private static int FindAvaliableRandomPort( int noOfAttemps)
        {
            int maxPort = 65535;
            int minPort = 1024;
            Random random = new Random();

            for (int i = 0; i < noOfAttemps; i++)
            {
               int port =  random.Next(minPort , maxPort+1);
                if(IsPortInUse(port)) return port;
            }
            throw new InvalidOperationException($"Attempt {noOfAttemps}: No available ports found in the specified range.");


        }
    }
}
