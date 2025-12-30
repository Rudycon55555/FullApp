using System;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace FullApp.Launcher
{
    internal static class Launcher
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: fullapp-launch <file.fullapp>");
                return;
            }

            string fullAppPath = args[0];

            if (!File.Exists(fullAppPath))
            {
                Console.WriteLine("Error: File not found.");
                return;
            }

            Console.WriteLine($"[FullApp] Launching: {fullAppPath}");

            using ZipArchive zip = ZipFile.OpenRead(fullAppPath);

            // Validate required files
            if (!HasEntry(zip, "Describe/FullAppFormat.txml") ||
                !HasEntry(zip, "Describe/FullApp.txml") ||
                !HasEntry(zip, "Describe/Metadata.json"))
            {
                Console.WriteLine("Error: Missing required FullApp descriptor files.");
                return;
            }

            // Detect OS + architecture
            string os = DetectOS();
            string arch = DetectArch();

            Console.WriteLine($"[FullApp] Host OS: {os}");
            Console.WriteLine($"[FullApp] Host Arch: {arch}");

            // Parse FullApp.txml
            string fullAppTxml = ReadEntry(zip, "Describe/FullApp.txml");
            var entrypoints = ParseEntrypoints(fullAppTxml);

            // Select entrypoint
            if (!entrypoints.TryGetValue((os, arch), out string? binaryPath))
            {
                Console.WriteLine("Error: No matching entrypoint for this OS/architecture.");
                return;
            }

            Console.WriteLine($"[FullApp] Selected binary: {binaryPath}");

            // Extract binary to temp folder
            string tempDir = Path.Combine(Path.GetTempPath(), "fullapp_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);

            string extractedBinary = Path.Combine(tempDir, Path.GetFileName(binaryPath));

            ExtractEntry(zip, binaryPath, extractedBinary);
            MakeExecutable(extractedBinary);

            Console.WriteLine($"[FullApp] Running: {extractedBinary}");

            // Launch
            var proc = new Process();
            proc.StartInfo.FileName = extractedBinary;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;

            proc.OutputDataReceived += (_, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
            proc.ErrorDataReceived += (_, e) => { if (e.Data != null) Console.WriteLine(e.Data); };

            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();
            proc.WaitForExit();

            Console.WriteLine($"[FullApp] Process exited with code {proc.ExitCode}");
        }

        // --- Utility Methods ---

        static bool HasEntry(ZipArchive zip, string path)
            => zip.GetEntry(path) != null;

        static string ReadEntry(ZipArchive zip, string path)
        {
            using var stream = zip.GetEntry(path)!.Open();
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        static void ExtractEntry(ZipArchive zip, string path, string output)
        {
            using var stream = zip.GetEntry(path)!.Open();
            using var fs = File.Create(output);
            stream.CopyTo(fs);
        }

        static void MakeExecutable(string path)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start("chmod", $"+x \"{path}\"")?.WaitForExit();
            }
        }

        static string DetectOS()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return "linux";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return "windows";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return "macos";
            return "unknown";
        }

        static string DetectArch()
        {
            return RuntimeInformation.OSArchitecture switch
            {
                Architecture.X64 => "x86_64",
                Architecture.Arm64 => "arm64",
                _ => "unknown"
            };
        }

        // Very simple TXML parser for entrypoints
        static Dictionary<(string os, string arch), string> ParseEntrypoints(string txml)
        {
            var map = new Dictionary<(string, string), string>();

            string? currentOS = null;
            string? currentArch = null;
            string? currentBinary = null;

            foreach (string rawLine in txml.Split('\n'))
            {
                string line = rawLine.Trim();

                if (line.StartsWith("os ="))
                    currentOS = ExtractValue(line);

                if (line.StartsWith("arch ="))
                    currentArch = ExtractValue(line);

                if (line.StartsWith("binary ="))
                    currentBinary = ExtractValue(line);

                if (line.StartsWith("</entry>"))
                {
                    if (currentOS != null && currentArch != null && currentBinary != null)
                        map[(currentOS, currentArch)] = currentBinary;

                    currentOS = currentArch = currentBinary = null;
                }
            }

            return map;
        }

        static string ExtractValue(string line)
        {
            int start = line.IndexOf('"') + 1;
            int end = line.LastIndexOf('"');
            return line[start..end];
        }
    }
}
