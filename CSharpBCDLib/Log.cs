// Copyright (c) 2016 Lu Cao
//
// This software is released under the MIT License.
// http://opensource.org/licenses/mit-license.php

using log4net;
using log4net.Config;

namespace CSharpBCDLib
{
    internal static class Log
    {
        public static readonly ILog Logger;
        static Log()
        {
            string assName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            string resXml = assName + ".log4net.xml";
            using (System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resXml))
            {
                log4net.Config.XmlConfigurator.Configure(stream);
            }

            Logger = LogManager.GetLogger("Logger");
        }
    }
}
