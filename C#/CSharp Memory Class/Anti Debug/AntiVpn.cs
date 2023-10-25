using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_Memory_Class.Anti_Debug
{
    class AntiVpn
    {
        private static readonly string[] VpnIpRanges = {
        "10.0.0.",
        "172.16.0.",
        "192.168.0.",
    };

        public static bool IsVpnConnected()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface ni in networkInterfaces)
            {
                IPInterfaceProperties ipProps = ni.GetIPProperties();

                foreach (UnicastIPAddressInformation ipInfo in ipProps.UnicastAddresses)
                {
                    foreach (string vpnRange in VpnIpRanges)
                    {
                        if (ipInfo.Address.ToString().StartsWith(vpnRange))
                        {
                            // VPN IP range detected
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
