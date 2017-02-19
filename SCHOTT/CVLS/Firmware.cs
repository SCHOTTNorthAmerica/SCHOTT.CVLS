using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SCHOTT.CVLS
{
    /// <summary>
    /// Library of firmware versions for the CVLS
    /// </summary>
    public class CustomerFirmware
    {
        /// <summary>
        /// Dictionary of firmware streams.
        /// </summary>
        public static Dictionary<string, Stream> Streams = new Dictionary<string, Stream>();

        /// <summary>
        /// Current firmware version in the library.
        /// </summary>
        public static string CurrentVersion;

        /// <summary>
        /// Create the firmware dictionary.
        /// </summary>
        static CustomerFirmware()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var internalResources = assembly.GetManifestResourceNames()
                .Where(s => s.Contains("Firmware."))
                .OrderByDescending(o=>o).ToList();

            if (internalResources.Count <= 0)
                return;

            foreach (var name in internalResources)
            {
                var workingString = name.Substring(name.LastIndexOf("_", StringComparison.Ordinal) + 1);
                workingString = workingString.Substring(0, workingString.IndexOf(".bin", StringComparison.Ordinal));
                Streams.Add(workingString, assembly.GetManifestResourceStream(name));

                if (CurrentVersion == null)
                    CurrentVersion = workingString;
            }
        }
    }
}
