// Copyright (c) 2016 Lu Cao
// 
// This software is released under the MIT License.
// http://opensource.org/licenses/mit-license.php

using CSharpBCDLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBCDLibDemo.Model
{
    internal class BcdModel
    {
        private BcdStore bcdStore;
        public BcdObject[] OSLoaderObjects;

        public BcdModel(string BCDStorePath = "")
        {
            InitBCDStore(BCDStorePath);
            OSLoaderObjects = bcdStore.EnumerateObjects(BCDObjectType.BCDAPPLICATION_OSLOADER);
        }

        private void InitBCDStore(string BCDStorePath)
        {
            bcdStore = new BcdStore(BCDStorePath);
        }

        internal void RefreshOSLoaderObjects()
        {
            OSLoaderObjects = bcdStore.EnumerateObjects(BCDObjectType.BCDAPPLICATION_OSLOADER);
        }
    }
}
