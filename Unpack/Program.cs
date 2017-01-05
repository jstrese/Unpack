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
            if (args.Length >= 2)
            {
                //
                // TODO: Check these paths
                //
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
                        // TODO: Check this path
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

                // TODO: Correct the LogMessage function
                //LogMessage("Test Message");

                Console.ReadLine();
            }
        }

        //
        // Used for a file is passed to the program and not a directory
        //
        private static void ExtractFile()
        {
            bool fileReady = false;
            DateTime endtime = DateTime.Now.AddSeconds(Convert.ToInt32(arguments["maxwait"]));


            while (!fileReady && (DateTime.Now.CompareTo(endtime) < 1))
            {
                fileReady = isFileReady();
            }

            //
            // TODO: Figure out why the while() exited (could be maxwait exceeded)
            //       Continue on with extraction if maxwait is OK
            //
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
                        //
                        // TODO: Log saying that file was ready but length was detected as 0
                        //
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
        // TODO: Clean up this dirty dirty function
        //
        private static void LogMessage(string message)
        {
            try
            {
                File.Create(String.Format("{0}\\log.txt", System.Reflection.Assembly.GetEntryAssembly().Location));
            }
            catch (Exception) { }

            using (FileStream logfile = File.OpenWrite(String.Format("{0}\\log.txt", System.Reflection.Assembly.GetEntryAssembly().Location)))
            {

                string DebugOutput = String.Format("\t=== Argument Output ===\r\n{0}\r\n\t========================\r\n\r\n",
                    String.Format("\t{0}\t=>\t{1}\r\n", "filePath", arguments["filePath"]),
                    String.Format("\t{0}\t=>\t{1}\r\n", "extractPath", arguments["extractPath"]),
                    String.Format("\t{0}\t=>\t{1}\r\n", "programPath", arguments["programPath"]),
                    String.Format("\t{0}\t=>\t{1}\r\n", "skipwait", arguments["skipwait"]),
                    String.Format("\t{0}\t\t=>\t{1}\r\n", "maxwait", arguments["maxwait"]),
                    String.Format("\t{0}\t\t=>\t{1}\r\n", "trash", arguments["trash"]),
                    String.Format("\t{0}\t\t=>\t{1}\r\n", "clean", arguments["clean"]),
                    String.Format("\t{0}\t\t=>\t{1}\r\n", "log", arguments["log"])
                );

                string LogMessage = String.Format("[{0} {1}] {2}\r\n{3}\r\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), message, DebugOutput);

                logfile.Write(ASCIIEncoding.Default.GetBytes(LogMessage), 0, ASCIIEncoding.Default.GetByteCount(LogMessage));
            }
        }
    }
}
