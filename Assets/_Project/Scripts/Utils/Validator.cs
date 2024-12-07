using System.Net;
using System.Text.RegularExpressions;

namespace _Project.Scripts.Utils
{
    /// <summary>
    /// Contains some static methods to validate user input.
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// Validates an IPv4 address
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsValidIP(string ip)
        {
            return IPAddress.TryParse(ip, out IPAddress address) && address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork;
        }

        /// <summary>
        /// Validates a port number (0 to 65535)
        /// </summary>
        /// <param name="portStr"></param>
        /// <returns></returns>
        public static bool IsValidPort(string portStr)
        {
            if (int.TryParse(portStr, out int port))
            {
                return port >= 0 && port <= 65535;
            }
            return false;
        }

        /// <summary>
        /// Validates a name (alphanumeric, underscores, max length 16)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsValidName(string name)
        {
            string pattern = @"^[a-zA-Z0-9_]{3,16}$";
            return Regex.IsMatch(name, pattern);
        }
    }
}