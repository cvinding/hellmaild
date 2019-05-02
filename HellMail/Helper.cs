using System;
using System.Net;
using System.Net.NetworkInformation;

namespace HellMail {

    public static class Helper {

        public static string GetFQDN() {
            string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string hostName = Dns.GetHostName();

            domainName = "." + domainName;

            // if hostname does not already include domain name 
            if (!hostName.EndsWith(domainName, StringComparison.CurrentCultureIgnoreCase) && !domainName.Equals(".(none)")) {
                // add the domain name part
                hostName += domainName;   
            }

            // return the fully qualified name
            return hostName;                    
        }
    }
}
