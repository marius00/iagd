using System;
using System.IO;
using log4net;

namespace IAGrim.Utilities.Detection {
    /// <summary>
    /// The paths Proton hands to whatever it launches inside a game's prefix.
    ///
    /// Discovery elsewhere in IAGD starts at the registry, which is the right answer on Windows and an unreliable
    /// one inside a Proton prefix: Steam has usually not written the keys that <see cref="SteamDetection"/> looks
    /// for, and there is no Windows-side install for them to describe. Proton exports the same answers as
    /// environment variables instead -- as Unix paths, which the prefix reaches through its Z: drive.
    ///
    /// This is only consulted when running IAGD inside the game's own prefix, which is the supported Linux setup:
    /// the app and the game then share one wineserver, so injection, the window lookup and the file bridge are all
    /// the ordinary Windows code paths, unmodified.
    ///
    /// Every member returns null unless the corresponding variable is actually set, so nothing here can change
    /// what happens on Windows.
    /// </summary>
    internal static class ProtonPaths {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ProtonPaths));

        /// <summary>The Steam client installation, i.e. the folder holding config/libraryfolders.vdf.</summary>
        public static string? SteamRoot => Read("STEAM_COMPAT_CLIENT_INSTALL_PATH");

        /// <summary>The compatdata folder for the app being run, which holds the prefix itself in "pfx".</summary>
        public static string? CompatData => Read("STEAM_COMPAT_DATA_PATH");

        /// <summary>The game's own install folder, e.g. .../steamapps/common/Grim Dawn.</summary>
        public static string? GameInstallDir => Read("STEAM_COMPAT_INSTALL_PATH");

        /// <summary>
        /// True when at least one Proton variable is present, meaning IAGD was launched into a prefix rather than
        /// started on its own. Only used for reporting -- nothing branches on it.
        /// </summary>
        public static bool IsRunningUnderProton =>
            SteamRoot != null || CompatData != null || GameInstallDir != null;

        private static string? Read(string variable) {
            var value = Environment.GetEnvironmentVariable(variable);
            if (string.IsNullOrWhiteSpace(value)) {
                return null;
            }

            var converted = ToWindowsPath(value.Trim());
            if (!Directory.Exists(converted)) {
                Logger.Warn($"{variable} is set to \"{value}\", which does not resolve to a directory (\"{converted}\"). Ignoring it.");
                return null;
            }

            return converted;
        }

        /// <summary>
        /// Wine maps the Unix filesystem root onto the Z: drive, so an absolute Unix path is reachable from inside
        /// the prefix by prefixing it and flipping the separators.
        ///
        /// Anything already in Windows form is passed through untouched: Proton is not the only way to end up
        /// inside a prefix with these variables set, and a launcher that has already converted them should not
        /// have its work mangled.
        /// </summary>
        private static string ToWindowsPath(string path) {
            if (path.Length >= 2 && path[1] == ':') {
                return path;
            }

            if (!path.StartsWith("/", StringComparison.Ordinal)) {
                return path;
            }

            return "Z:" + path.Replace('/', '\\');
        }
    }
}
