using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommandLineParser.Arguments;
using CommandLineParser.Exceptions;
using System.Windows.Forms;

namespace GetLatestApplicationVersion
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var parser = new CommandLineParser.CommandLineParser
            {
                ShowUsageOnEmptyCommandline = true,
                AcceptSlash = true
            };

            //-p "C:\Program Files" -n "notepad++.exe"

            var pathArgument = new ValueArgument<string>(
                    'p',
                    "path",
                    "Root path to scan for applications (searches recursively)")
            { Optional = false };
            parser.Arguments.Add(pathArgument);

            var nameArgument = new ValueArgument<string>(
                    'n',
                    "name",
                    "Name or name file search pattern to use for scanning")
            { Optional = false };
            parser.Arguments.Add(nameArgument);

            try
            {
                parser.ParseCommandLine(args);
            }
            catch (CommandLineException cle)
            {
                Console.WriteLine($"CommandLineException: {cle.Message}");
                return;
            }

            try
            {
                var scanDirectory = pathArgument.Value;

                if (!Directory.Exists(scanDirectory))
                    return;

                var highestVersion = new Version("0.0.0.0");

                foreach (var fileName in SafeFileEnumerator.EnumerateFiles(scanDirectory,
                    "*" + nameArgument.Value, SearchOption.AllDirectories))
                {
                    if (!File.Exists(fileName))
                        continue;

                    var foundProductVersion = new Version(FileVersionInfo.GetVersionInfo(fileName).FileVersion);

                    if (highestVersion >= foundProductVersion)
                        continue;

                    highestVersion = foundProductVersion;
                }

                MessageBox.Show("here");
                if (highestVersion != new Version("0.0.0.0"))
                {
                MessageBox.Show("here1");
                    Clipboard.SetText(highestVersion.ToString());
                }
                MessageBox.Show("here2");
            }
            catch (Exception ex)
            {
                MessageBox.Show("here3");
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}
