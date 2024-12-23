// © 2024 led-mirage. All rights reserved.

using System.Net;
using System.Net.Sockets;

public static class DnsHelper
{
    public static string? GetHostAddress(string hostName, AddressFamilyPreference preference = AddressFamilyPreference.Auto)
    {
        if (IPAddress.TryParse(hostName, out IPAddress? _))
        {
            return hostName;
        }

        IPAddress[] addresses = Dns.GetHostAddresses(hostName);
        foreach (IPAddress address in addresses)
        {
            if (preference == AddressFamilyPreference.Auto)
            {
                return address.ToString();
            }
            else if (preference == AddressFamilyPreference.IPv4 && address.AddressFamily == AddressFamily.InterNetwork)
            {
                return address.ToString();
            }
            else if (preference == AddressFamilyPreference.IPv6 && address.AddressFamily == AddressFamily.InterNetworkV6)
            {
                return address.ToString();
            }
        }

        if (addresses.Length > 0)
        {
            return addresses[0].ToString();
        }
        else
        {
            return null;
        }
    }
}