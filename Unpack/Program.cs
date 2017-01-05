using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unpack
{
    class Program
    {
        //
        //      Args        =>  Default Values
        //      ==============================
        //      (first)     =>  Path to file to extract
        //      (second)    =>  Path to extract file to
        //      programPath =>  Path to the program that we use for extracting
        //      skipwait    =>  false
        //      maxwait     =>  3600
        //      trash       =>  false
        //      clean       =>  (blank)
        //      log         =>  false
        //

        private static Dictionary<string, string> arguments = new Dictionary<string, string>()
        {
            ["filePath"] = "",
            ["extractPath"] = "",
            ["programPath"] = "C:\\Program Files\\7-Zip\\7z.exe",
            ["skipwait"] = "false",
            ["maxwait"] = "3600",
            ["trash"] = "false",
            ["clean"] = "",
            ["log"] = "false"
        };

        static void Main(string[] args)
        {
            //
            // TODO: Determine if filePath is a directory or archive
            //
            if (args.Length >= 2 && isFile(args[0]) && isDirectoryValid(args[1]))
            {
                arguments["filePath"] = args[0];
                arguments["extractPath"] = args[1];

                foreach (string arg in args)
                {
                    if (arg == "-skipwait")
                        arguments["skipwait"] = "true";
                    else if (arg == "-trash")
                        arguments["trash"] = "true";
                    else if (arg == "-log")
                        arguments["log"] = "true";

                    else if (arg.Contains("="))
                    {
                        string[] arg_split = arg.Split('=');

                        if (arg_split[0] == "-maxwait")
                        {
                            int maxwait;

                            if (Int32.TryParse(arg_split[1], out maxwait))
                            {
                                if (maxwait > 86400)
                                    arguments["maxwait"] = "86400";
                                else if (maxwait < 1)
                                    arguments["maxwait"] = "0";
                                else
                                    arguments["maxwait"] = maxwait.ToString();
                            }
                        }

                        if (arg_split[0] == "-clean")
                            arguments["clean"] = arg_split[1];
                        if (arg_split[0] == "-programPath")
                            arguments["programPath"] = arg_split[1];
                    }
                }

                Console.WriteLine(String.Format("{0}\t=>\t{1}", "filePath", arguments["filePath"]));
                Console.WriteLine(String.Format("{0}\t=>\t{1}", "extractPath", arguments["extractPath"]));
                Console.WriteLine(String.Format("{0}\t=>\t{1}", "programPath", arguments["programPath"]));
                Console.WriteLine(String.Format("{0}\t=>\t{1}", "skipwait", arguments["skipwait"]));
                Console.WriteLine(String.Format("{0}\t\t=>\t{1}", "maxwait", arguments["maxwait"]));
                Console.WriteLine(String.Format("{0}\t\t=>\t{1}", "trash", arguments["trash"]));
                Console.WriteLine(String.Format("{0}\t\t=>\t{1}", "clean", arguments["clean"]));
                Console.WriteLine(String.Format("{0}\t\t=>\t{1}", "log", arguments["log"]));
                Console.WriteLine(String.Format("Log Write Path\t=>\t{0}\\log.txt", Directory.GetParent(System.Reflection.Assembly.GetEntryAssembly().Location)));

                ExtractFile();

                Console.ReadLine();
            }
        }

        //
        // Used for a file is passed to the program and not a directory
        //
        private static void ExtractFile()
        {
            if (isFile(arguments["programPath"]))
            {
                bool fileReady = false;
                DateTime endtime = DateTime.Now.AddSeconds(Convert.ToInt32(arguments["maxwait"]));

                while (!fileReady && (DateTime.Now.CompareTo(endtime) < 1))
                {
                    Console.WriteLine("> Waiting for file to be available");
                    fileReady = isFileReady();
                }

                if (fileReady)
                {
                    using (Process Extractor = new Process())
                    {
                        Extractor.StartInfo.FileName = arguments["programPath"];

                        //
                        // If the username and password are not NULL and
                        // 'UseShellExecute' is not false, a window will appear
                        //
                        Extractor.StartInfo.CreateNoWindow = true;
                        Extractor.StartInfo.UseShellExecute = false;
                        Extractor.StartInfo.UserName = null;
                        Extractor.StartInfo.Password = null;

                        Extractor.StartInfo.Arguments = String.Format("e \"{0}\" -o\"{1}\" -r -y", arguments["filePath"], arguments["extractPath"]);

                        if (Extractor.Start())
                            LogMessage("Extraction executed successfully.");
                        else
                            LogMessage("Extraction process did not execute.");
                    }
                }
                else
                    LogMessage(String.Format("'maxwait' of '{0}' has been exceeded while waiting for the file to be available", arguments["maxwait"]));
            }
        }

        //
        // Checks to see if the supplied directory is valid
        //
        private static bool isDirectoryValid(string dir)
        {
            if (Directory.Exists(dir))
                return true;
            else
            {
                LogMessage(String.Format("Unable to locate directory: {0}", dir));
                return false;
            }
        }

        //
        // Checks to see if the supplied file is valid
        //
        private static bool isFile(string path)
        {
            if (File.Exists(path))
                return true;
            else
            {
                LogMessage(String.Format("Unable to locate file: {0}", path));
                return false;
            }
        }

        //
        // Attempts to access a file to see if the file is locked
        //
        private static bool isFileReady()
        {
            try
            {
                using (FileStream input = File.Open(arguments["filePath"], FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    if (input.Length > 0)
                        return true;
                    else
                    {
                        LogMessage("Zero byte file located");
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        //
        // Used when a directory is passed to the program and not a file
        // TODO: Implement recursive extracting
        //
        private static void ExtractDirectory()
        {

        }

        //
        // TODO: Implement cleaning
        //
        private static void CleanExtractedDirectory()
        {

        }

        //
        // Logs a message to our log file - file is created if it does not exist
        //
        private static void LogMessage(string message)
        {
            //
            // TODO: Graceful error handling when writing to the log file
            //
            try
            {
                using (FileStream logfile = new FileStream(String.Format("{0}\\log.txt", Directory.GetParent(System.Reflection.Assembly.GetEntryAssembly().Location)), FileMode.Append, FileAccess.Write))
                using (StreamWriter write = new StreamWriter(logfile))
                {
                    string DebugOutput = String.Format("Argument Output\r\n{0}{1}{2}{3}{4}{5}{6}{7}=============================\r\n",
                        String.Format("{0}\t=>\t{1}\r\n", "filePath", arguments["filePath"]),
                        String.Format("{0}\t=>\t{1}\r\n", "extractPath", arguments["extractPath"]),
                        String.Format("{0}\t=>\t{1}\r\n", "programPath", arguments["programPath"]),
                        String.Format("{0}\t=>\t{1}\r\n", "skipwait", arguments["skipwait"]),
                        String.Format("{0}\t\t=>\t{1}\r\n", "maxwait", arguments["maxwait"]),
                        String.Format("{0}\t\t=>\t{1}\r\n", "trash", arguments["trash"]),
                        String.Format("{0}\t\t=>\t{1}\r\n", "clean", arguments["clean"]),
                        String.Format("{0}\t\t\t=>\t{1}\r\n", "log", arguments["log"])
                    );

                    string LogMessage = String.Format("==== [{0} {1}] ====\r\nLog Message: {2}\r\n\r\n{3}\r\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), message, DebugOutput);

                    write.WriteAsync(LogMessage);
                }
            } catch (Exception) { }
        }
    }
}
