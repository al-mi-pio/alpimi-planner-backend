using System;
using System.IO;
using System.Reflection.Metadata;

namespace alpimi_planner_backend.Collisions.CollisionUtils
{
    public class LogMaker
    {
        private string logFilePath;
        private string logFolderPath;
        private bool logEnabled;

        public LogMaker()
        {
            logEnabled = false;
            logFolderPath = "Logs";
            if (!Directory.Exists(logFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(logFolderPath);
                    logEnabled = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("-- Creating Log folder failed --");
                    Console.WriteLine(ex.Message);
                }

                try
                {
                    string[] logFiles = Directory.GetFiles(logFolderPath, "*.log");
                    foreach (string file in logFiles)
                    {
                        File.Delete(file);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("-- Deleting log files failed --");
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                logEnabled = true;
                try
                {
                    string[] logFiles = Directory.GetFiles(logFolderPath, "*.log");
                    foreach (string file in logFiles)
                    {
                        File.Delete(file);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("-- Deleting log files failed --");
                    Console.WriteLine(ex.Message);
                }
            }

            logFilePath = "Logs/log_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm") + ".log";
        }

        public void writeLog(string lineContent)
        {
            if (logEnabled)
            {
                using (StreamWriter logWriter = new StreamWriter(logFilePath, append: true))
                {
                    logWriter.WriteLine(lineContent);
                }
            }
        }

        public void disableLog()
        {
            logEnabled = false;
        }

        public bool isLogEnabled()
        {
            return logEnabled;
        }
    }
}
