using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;
using log4net;
namespace SyncDataJob
{
    public static class Logger
    {
        /// <summary>
        /// Gets or sets the log.
        /// </summary>
        /// <value>
        /// The log.
        /// </value>
        private static log4net.ILog Log { get; set; }

        /// <summary>
        /// Initializes static members of the <see cref="Logger"/> class.
        /// </summary>
        static Logger()
        {
            Log = LogManager.GetLogger(typeof(Logger));
            XmlConfigurator.Configure();
        }

        /// <summary>
        /// Errors the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public static void Error(object msg)
        {
            Log.Error(msg);
        }

        /// <summary>
        /// Errors the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <param name="ex">The ex.</param>
        public static void Error(object msg, Exception ex)
        {
            Log.Error(msg, ex);
        }

        /// <summary>
        /// Errors the specified ex.
        /// </summary>
        /// <param name="ex">The ex.</param>
        public static void Error(Exception ex)
        {
            if (ex != null)
            {
                Log.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// Informations the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <param name="values">The values.</param>
        public static void Info(string msg)
        {
            Log.Info(msg);
        }
    }
}
