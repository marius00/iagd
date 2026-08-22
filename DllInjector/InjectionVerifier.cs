using log4net;
using EvilsoftCommons.Exceptions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace EvilsoftCommons.DllInjector {
    /// <summary>
    /// Runs the Microsoft "Listdlls.exe" to verify that the DLL injection was successful.
    /// Sometimes the injection reports as successful, but the DLL does not persist. (unloaded by anti virus?)
    /// </summary>
    public class InjectionVerifier {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(InjectionVerifier));


        /// <summary>
        /// Remove nag screens on running ListDLLs
        /// </summary>
        public static void FixRegistryNagOnListDlls() {
            try {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);


                key.CreateSubKey("Sysinternals");
                key = key.OpenSubKey("Sysinternals", true);

                key.CreateSubKey("ListDLLs");
                key = key.OpenSubKey("ListDLLs", true);

                key.SetValue("EulaAccepted", 1);

                key.Close();
            }
            catch (Exception ex) {
                Logger.Warn("Error trying to create registry keys, this is not critical.");
                Logger.Warn(ex.Message);
            }
        }

        /// <summary>
        /// Wine/Proton alternative: checks for a PID file written by the injected DLL.
        /// Also cleans up stale PID files (wrong pid or older than 1 day).
        /// </summary>
        public static bool VerifyInjectionViaFile(long pid, string linuxHackPath) {
            try {
                Directory.CreateDirectory(linuxHackPath);
                foreach (var file in Directory.GetFiles(linuxHackPath, "*.PID")) {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var fileAge = DateTime.Now - File.GetLastWriteTime(file);

                    bool isCurrentPid = long.TryParse(fileName, out long filePid) && filePid == pid;
                    bool isStale = fileAge.TotalDays > 1;

                    if (!isCurrentPid || isStale) {
                        try {
                            File.Delete(file);
                            Logger.Info($"Deleted stale PID file: {file}");
                        }
                        catch (IOException ex) {
                            Logger.Warn($"Could not delete stale PID file {file}: {ex.Message}");
                        }
                    }
                }

                var pidFile = Path.Combine(linuxHackPath, $"{pid}.PID");
                if (File.Exists(pidFile)) {
                    var age = DateTime.Now - File.GetLastWriteTime(pidFile);
                    if (age.TotalDays <= 1) {
                        Logger.Info($"Verified injection via PID file for process {pid}");
                        return true;
                    }
                }
            }
            catch (Exception ex) {
                Logger.Warn($"Error verifying injection via PID file: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// Wine/Proton: true if the injected DLL reported that it deliberately aborted the attach, meaning the
        /// game is not ready to be hooked yet (still loading, or in the character select menu -- injecting there
        /// tends to crash the game). Not a failure, just "try again on the next poll".
        ///
        /// The marker is consumed, so each aborted attach is reported exactly once. It has to be read here rather
        /// than taken from the DLL's .msg stream: an abort unloads the DLL, which removes the .PID file this class
        /// verifies against, and the .msg is drained asynchronously by the UI poll timer -- long after the injector
        /// has already concluded the injection failed.
        /// </summary>
        public static bool ConsumeInjectionAbortedMarker(long pid, string linuxHackPath) {
            try {
                Directory.CreateDirectory(linuxHackPath);

                foreach (var file in Directory.GetFiles(linuxHackPath, "*.ABORTED")) {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    bool isCurrentPid = long.TryParse(fileName, out long filePid) && filePid == pid;
                    bool isStale = (DateTime.Now - File.GetLastWriteTime(file)).TotalDays > 1;

                    // Consume our own marker, and clean up anything left behind by a previous game process.
                    try {
                        File.Delete(file);
                    }
                    catch (IOException ex) {
                        Logger.Warn($"Could not delete injection aborted marker {file}: {ex.Message}");
                        continue;
                    }

                    if (isCurrentPid && !isStale) {
                        return true;
                    }
                }
            }
            catch (Exception ex) {
                Logger.Warn($"Error checking for an injection aborted marker: {ex.Message}");
            }

            return false;
        }

        public static bool VerifyInjection(long pid, string dll) {
            FixRegistryNagOnListDlls();

            Logger.Info("Running Listdlls...");
            List<string> output = new List<string>();

            // Against the install folder, not the working directory: see the note in InjectionHelper.InjectXBit.
            // A miss here is quieter than most -- verification is simply skipped, and a successful injection is
            // then reported as a failure.
            var listDlls = Path.Combine(AppContext.BaseDirectory, "Listdlls.exe");
            if (File.Exists(listDlls)) {
                ProcessStartInfo startInfo = new ProcessStartInfo {
                    FileName = listDlls,
                    WorkingDirectory = AppContext.BaseDirectory,
                    Arguments = $"{pid}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process processTemp = new Process();
                processTemp.StartInfo = startInfo;
                processTemp.EnableRaisingEvents = true;
                try {
                    string spid = pid.ToString();
                    processTemp.Start();
                    processTemp.WaitForExit(3000);


                    while (!processTemp.StandardOutput.EndOfStream) {
                        string line = processTemp.StandardOutput.ReadLine();
                        output.Add(line);
                        if (line.Contains(dll))
                            return true;
                    }
                }
                catch (Exception ex) {
                    Logger.Warn("Exception while attempting to verify injection.. " + ex.Message + ex.StackTrace);
                }
            }
            else {
                Logger.Warn($"Could not find {listDlls}, unable to verify successful injection.");
            }
            return false;
        }

        /// <summary>
        /// Detects a playtest build, which needs its own hook DLL.
        ///
        /// Currently always false: the previous playtest has been merged into the live game, so there is nothing
        /// to tell apart right now. Kept wired up because the next playtest will need it again -- fill in an
        /// export that only the playtest build has and match it against <see cref="GetDllExports"/>, e.g.
        /// <c>GetDllExports(dll)?.Contains("??0AscendantAltar@GAME@@QEAA@XZ") == true</c>.
        ///
        /// Whatever goes here must be a *positive* test for the playtest, never an absence test. Absence is
        /// indistinguishable from an export table we failed to read, and the fallback has to stay on the default
        /// DLL: injecting a mismatched hook DLL crashes the game, whereas retrying costs a poll interval.
        /// </summary>
        public static bool IsPlaytest(string dll) {
            // TODO: The previous playtest is now the live game, a new export is needed to detect the current playtest.
            return false;
        }

        /// <summary>
        /// Reads the export name table straight out of the PE file.
        /// This used to shell out to dumpbin.exe, but that is only a thin wrapper around link.exe,
        /// which in turn needs a whole set of MSVC runtime DLLs that are only present on dev machines.
        /// Returns null if the file could not be parsed.
        /// </summary>
        private static HashSet<string>? GetDllExports(string dll) {
            try {
                if (!File.Exists(dll)) {
                    Logger.Warn($"The file \"{dll}\" does not exist.");
                    return null;
                }

                byte[] image = File.ReadAllBytes(dll);

                if (image.Length < 0x40 || BitConverter.ToUInt16(image, 0) != 0x5A4D) // "MZ"
                    return null;

                int peOffset = BitConverter.ToInt32(image, 0x3C);
                if (peOffset <= 0 || peOffset + 0x18 > image.Length || BitConverter.ToUInt32(image, peOffset) != 0x00004550) // "PE\0\0"
                    return null;

                int coffOffset = peOffset + 4;
                int numberOfSections = BitConverter.ToUInt16(image, coffOffset + 2);
                int sizeOfOptionalHeader = BitConverter.ToUInt16(image, coffOffset + 16);
                int optionalHeaderOffset = coffOffset + 20;

                ushort magic = BitConverter.ToUInt16(image, optionalHeaderOffset);
                // The export directory is the first entry in the data directory, which sits right after the
                // version-specific part of the optional header (96 bytes for PE32, 112 for PE32+).
                int dataDirectoryOffset;
                if (magic == 0x20B) // PE32+
                    dataDirectoryOffset = optionalHeaderOffset + 112;
                else if (magic == 0x10B) // PE32
                    dataDirectoryOffset = optionalHeaderOffset + 96;
                else
                    return null;

                uint exportDirRva = BitConverter.ToUInt32(image, dataDirectoryOffset);
                if (exportDirRva == 0)
                    return new HashSet<string>(); // Valid PE, simply no exports

                int sectionHeadersOffset = optionalHeaderOffset + sizeOfOptionalHeader;
                var sections = new List<(uint VirtualAddress, uint VirtualSize, uint RawAddress, uint RawSize)>();
                for (int i = 0; i < numberOfSections; i++) {
                    int s = sectionHeadersOffset + i * 40;
                    if (s + 40 > image.Length)
                        return null;

                    sections.Add((
                        BitConverter.ToUInt32(image, s + 12),
                        BitConverter.ToUInt32(image, s + 8),
                        BitConverter.ToUInt32(image, s + 20),
                        BitConverter.ToUInt32(image, s + 16)
                    ));
                }

                int RvaToOffset(uint rva) {
                    foreach (var section in sections) {
                        if (rva >= section.VirtualAddress && rva < section.VirtualAddress + Math.Max(section.VirtualSize, section.RawSize)) {
                            long offset = section.RawAddress + (rva - section.VirtualAddress);
                            return offset >= 0 && offset < image.Length ? (int) offset : -1;
                        }
                    }

                    return -1;
                }

                int exportDirOffset = RvaToOffset(exportDirRva);
                if (exportDirOffset < 0 || exportDirOffset + 40 > image.Length)
                    return null;

                uint numberOfNames = BitConverter.ToUInt32(image, exportDirOffset + 24);
                uint namePointerRva = BitConverter.ToUInt32(image, exportDirOffset + 32);

                int namePointerOffset = RvaToOffset(namePointerRva);
                if (namePointerOffset < 0)
                    return null;

                var exports = new HashSet<string>(StringComparer.Ordinal);
                for (uint i = 0; i < numberOfNames; i++) {
                    long entry = namePointerOffset + i * 4L;
                    if (entry + 4 > image.Length)
                        return null;

                    int nameOffset = RvaToOffset(BitConverter.ToUInt32(image, (int) entry));
                    if (nameOffset < 0)
                        continue;

                    int end = nameOffset;
                    while (end < image.Length && image[end] != 0)
                        end++;

                    exports.Add(Encoding.ASCII.GetString(image, nameOffset, end - nameOffset));
                }

                Logger.Info($"Read {exports.Count} exports from \"{dll}\".");
                return exports;
            }
            catch (Exception ex) {
                Logger.Warn($"Error reading the export table of \"{dll}\".. " + ex.Message + ex.StackTrace);
                return null;
            }
        }

    }
}
