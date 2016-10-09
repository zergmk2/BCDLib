// Copyright (c) 2016 Lu Cao
//
// This software is released under the MIT License.
// http://opensource.org/licenses/mit-license.php

using System;
using System.Collections.Generic;
using System.Management;
using System.Reflection;

namespace CSharpBCDLib
{
    public class BcdStore
    {
        protected static ManagementObject Store { get; set; }
        protected static ManagementClass BcdCls { get; set; }
        protected static string FilePath { get; set; }

        public BcdStore(string bcdPath = "")
        {
            Log.Logger.Info("Initializing a BcdStore object.");
            ConnectionOptions connectionOptions = new ConnectionOptions();
            connectionOptions.Impersonation = ImpersonationLevel.Impersonate;
            connectionOptions.EnablePrivileges = true;
            if (!string.IsNullOrEmpty(bcdPath))
                FilePath = bcdPath;
            else
                FilePath = "";
            OpenStore();
        }
        /** Open the BCD store for read
         */
        protected void OpenStore()
        {
            try
            {
                BcdCls = new ManagementClass(@"root\WMI", "BcdStore", null);
                ManagementBaseObject moParams = BcdCls.GetMethodParameters("OpenStore");
                moParams["File"] = FilePath;
                ManagementBaseObject res = BcdCls.InvokeMethod("OpenStore", moParams, null);
                if (Convert.ToBoolean(res["ReturnValue"]) == false)
                {
                    Log.Logger.Error("OpenStore failed. BCD Path:" + FilePath);
                    return;
                }
                Store = (ManagementObject)typeof(ManagementBaseObject)
                    .GetMethod("GetBaseObject", BindingFlags.Static | BindingFlags.NonPublic)
                    .Invoke(null, new object[]
                            {
                                typeof(ManagementBaseObject)
                                    .GetField("_wbemObject", BindingFlags.Instance | BindingFlags.NonPublic)
                                    .GetValue(res["Store"]),
                                BcdCls.Scope
                            }
                 );
            }
            catch (Exception ex)
            {
                Log.Logger.Error(string.Format("Exception on OpenStore: {0}", ex.Message));
            }
        }

        public ManagementObject OpenObject(string Id)
        {
            if (Store == null || BcdCls == null || string.IsNullOrEmpty(Id))
                return null;
            ManagementObject bcdObj = null;

            try
            {
                ManagementBaseObject ooParams = Store.GetMethodParameters("OpenObject");
                ooParams["Id"] = Id;
                ManagementBaseObject ooRes = Store.InvokeMethod("OpenObject", ooParams, null);
                if (Convert.ToBoolean(ooRes["ReturnValue"]) == false)
                {
                    Log.Logger.Error("OpenObject failed. BCD Path:" + FilePath + "\nPress any key to continue.");
                    return null;
                }
                bcdObj = (ManagementObject)typeof(ManagementBaseObject)
                    .GetMethod("GetBaseObject", BindingFlags.Static | BindingFlags.NonPublic)
                    .Invoke(null, new object[]
                    {
                        typeof(ManagementBaseObject)
                            .GetField("_wbemObject", BindingFlags.Instance | BindingFlags.NonPublic)
                            .GetValue(ooRes["Object"]),
                        BcdCls.Scope
                    }
                 );
            }
            catch (Exception ex)
            {
                Log.Logger.Error(string.Format("Exception on OpenObject: {0}", ex.Message));
                return null;
            }
            return bcdObj;
        }

        public ManagementBaseObject GetElement(ManagementObject bcdObj, BuiltinElementType ty)
        {
            if (bcdObj == null || BcdCls == null)
                return null;
            ManagementBaseObject ele = null;

            try
            {
                ManagementBaseObject geParams = bcdObj.GetMethodParameters("GetElement");
                geParams["Type"] = ty;
                ManagementBaseObject geObjRes = bcdObj.InvokeMethod("GetElement", geParams, null);
                if (Convert.ToBoolean(geObjRes["ReturnValue"]) == false)
                {
                    Log.Logger.Error("GetElement failed. BCD Path:" + FilePath);
                    return null;
                }
                ele = ((ManagementBaseObject)(geObjRes.Properties["Element"].Value));
            }
            catch (Exception ex)
            {
                Log.Logger.Error(string.Format("Exception on GetElement: {0}", ex.Message));
                return null;
            }
            return ele;
        }

        public BcdObject CreateObject(string Id, uint Type)
        {
            if (Store == null || BcdCls == null || string.IsNullOrEmpty(Id))
                return null;
            ManagementObject bcdObj = null;

            try
            {
                ManagementBaseObject ooParams = Store.GetMethodParameters("CreateObject");
                ooParams["Id"] = Id;
                ooParams["Type"] = Type;
                ManagementBaseObject ooRes = Store.InvokeMethod("CreateObject", ooParams, null);
                if (Convert.ToBoolean(ooRes["ReturnValue"]) == false)
                {
                    Log.Logger.Error("CreateObject return false. BCD Path:" + FilePath);
                    return null;
                }
                return ToBCDObject((ManagementBaseObject)ooRes.Properties["Object"].Value);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(string.Format("Exception on CreateObject: {0}", ex.Message));
            }
            return null;
        }

        public bool DeleteObject(string Id)
        {
            if (Store == null || BcdCls == null || string.IsNullOrEmpty(Id))
                return false;

            try
            {
                ManagementBaseObject ooParams = Store.GetMethodParameters("DeleteObject");
                ooParams["Id"] = Id;
                ManagementBaseObject ooRes = Store.InvokeMethod("DeleteObject", ooParams, null);
                if (Convert.ToBoolean(ooRes["ReturnValue"]) == false)
                {
                    Log.Logger.Error("DeleteObject return false. BCD Path:" + FilePath);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(string.Format("Exception on DeleteObject: {0}", ex.Message));
            }
            return false;
        }

        public BcdObject[] EnumerateObjects(BCDObjectType Type)
        {
            if (Store == null || BcdCls == null)
                return null;
            BcdObject[] bcdObjs = null;

            Log.Logger.Info("WmiBcdstore.EnumerateObjects started with Type = " + Type.ToString());
            try
            {
                ManagementBaseObject ooParams = Store.GetMethodParameters("EnumerateObjects");
                ooParams["Type"] = Type;
                ManagementBaseObject ooRes = Store.InvokeMethod("EnumerateObjects", ooParams, null);
                if (Convert.ToBoolean(ooRes["ReturnValue"]) == false)
                {
                    Log.Logger.Error("EnumerateObjects return false. BCD Path:" + FilePath);
                    return null;
                }


                if (ooRes.Properties["Objects"] != null)
                {
                    bcdObjs = ToBCDObject((ManagementBaseObject[])ooRes.Properties["Objects"].Value);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(string.Format("Exception on WmiBcdstore.EnumerateObjects: {0}", ex.Message));
                return null;
            }
            Log.Logger.Info("WmiBcdstore.EnumerateObjects left with " + bcdObjs.Length + " Objects");
            return bcdObjs;
        }

        private BcdObject[] ToBCDObject(ManagementBaseObject[] MObjects)
        {
            if (MObjects == null)
            {
                return null;
            }
            List<BcdObject> objs = new List<BcdObject>();
            foreach (ManagementBaseObject obj in MObjects)
            {
                try
                {
                    BcdObject bcdObj = ToBCDObject(obj);
                    if (bcdObj != null)
                        objs.Add(bcdObj);
                }
                catch (Exception ex)
                {
                    Log.Logger.Fatal(string.Format("Exception on WmiBcdstore.CovertMObjectToBCDObject: {0}", ex.Message));
                }
            }
            return objs.ToArray();
        }

        public BcdObject ToBCDObject(ManagementBaseObject MObject)
        {
            if (MObject == null)
            {
                return null;
            }

            try
            {
                var bcdObj = (ManagementObject)typeof(ManagementBaseObject)
                    .GetMethod("GetBaseObject", BindingFlags.Static | BindingFlags.NonPublic)
                    .Invoke(null, new object[]
                    {
                        typeof(ManagementBaseObject)
                            .GetField("_wbemObject", BindingFlags.Instance | BindingFlags.NonPublic)
                            .GetValue(MObject),
                        BcdCls.Scope
                    }
                 );
                return new BcdObject(bcdObj);
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(string.Format("Exception on WmiBcdstore.CovertMObjectToBCDObject: {0}", ex.Message));
            }
            return null;

        }
    }
}
