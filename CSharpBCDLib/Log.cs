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
            XmlConfigurator.Configure();
            Logger = LogManager.GetLogger("CSharpBCDLib");
        }

    }
}
