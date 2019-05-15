using System;
using System.IO;
using System.Threading;

namespace HellMail {

    public static class Logger {

        public static string LOGFILE;

        private static object locker = new Object();

        public static void Log(string logMessage) {

            lock (locker) {

                DateTime dt = DateTime.Now;

                FileStream fileStream = new FileStream(LOGFILE, FileMode.Create | FileMode.Append, FileAccess.Write, FileShare.Read);

                using (StreamWriter sr = new StreamWriter(fileStream))
                {
                    // File writing as usual
                    sr.WriteLine("[" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "]: " + logMessage);
                }

                fileStream.Close();
            }
        }

    }

}
