// Copyright (c) 2016 Lu Cao
// 
// This software is released under the MIT License.
// http://opensource.org/licenses/mit-license.php

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBCDLib
{
    public enum BCDObjectType : uint
    {
        BCDAPPLICATION_BOOTMGR = 0x10100002,
        BCDAPPLICATION_OSLOADER = 0x10200003,
        BOOTLOADERSETTINGS = 0x20200003,
        BCDDEVICE = 0x30000000,
    }

    public class BcdObject
    {
        private readonly ManagementObject InternalObject;
        private readonly Dictionary<uint, BcdElement> elements = new Dictionary<uint, BcdElement>();
        public readonly BcdElementDictionary elementsDict;

        public string Id { get; private set; }
        public uint TypeId { get; private set; }

        public BcdObject(ManagementObject BaseObject)
        {
            if (BaseObject != null)
            {
                InternalObject = BaseObject;
                Id = InternalObject.Properties["Id"].Value.ToString();
                TypeId = Convert.ToUInt32(InternalObject.Properties["Type"].Value.ToString());
                EnumerateElements();
                elementsDict = new BcdElementDictionary(this);
            }
        }

        private void EnumerateElements()
        {
            Log.Logger.Info("Entering EnumerateElements");
            try
            {
                ManagementBaseObject ooParams = InternalObject.GetMethodParameters("EnumerateElements");
                ManagementBaseObject ooRes = InternalObject.InvokeMethod("EnumerateElements", ooParams, null);
                if (Convert.ToBoolean(ooRes["ReturnValue"]) == false)
                {
                    Log.Logger.Error("EnumerateElements return false.");
                    Log.Logger.Info("Leaving EnumerateElements.");
                    return;
                }

                ManagementBaseObject[] RetElements = (ManagementBaseObject[])ooRes.Properties["Elements"].Value;
                foreach (ManagementBaseObject obj in RetElements)
                {
                    BcdElement bcdelement = BcdElement.BcdElementFactory(obj);
                    this.elements.Add(bcdelement.TypeId, bcdelement);
                }

            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(string.Format("Exception on WmiBcdstore.EnumerateObjects: {0}", ex.Message));
            }
            Log.Logger.Info("Leaving EnumerateElements.");
            return;
        }

        public bool SetObjectListElement(string[] Ids, uint TypeId)
        {
            Log.Logger.Info("Entering SetObjectListElement");
            Log.Logger.Info(string.Format("Type : {0:X}", TypeId));
            Log.Logger.Info("SetObjectListElement : " + Ids);
            System.Management.ManagementBaseObject inParams = InternalObject.GetMethodParameters("SetObjectListElement");
            inParams["Ids"] = Ids;
            inParams["Type"] = TypeId;
            System.Management.ManagementBaseObject outParams = InternalObject.InvokeMethod("SetObjectListElement", inParams, null);
            if (System.Convert.ToBoolean(outParams.Properties["ReturnValue"].Value) == true)
            {
                Log.Logger.Info("Succeeded to call SetObjectListElement");
                if (!this.elements.ContainsKey(TypeId))
                {
                    this.elements.Add(TypeId, this.GetElement(TypeId));
                }
                Log.Logger.Info("Leaving SetObjectListElement");
                return true;
            }
            Log.Logger.Info("Leaving SetObjectListElement with false.");
            return false;
        }

        public bool SetIntegerElement(UInt64 Integer, uint TypeId)
        {
            Log.Logger.Info("Entering SetIntegerElement");
            Log.Logger.Info(string.Format("Type : {0:X}", TypeId));
            Log.Logger.Info("Integer : " + Integer);
            System.Management.ManagementBaseObject inParams = InternalObject.GetMethodParameters("SetIntegerElement");
            inParams["Integer"] = Integer;
            inParams["Type"] = TypeId;
            System.Management.ManagementBaseObject outParams = InternalObject.InvokeMethod("SetIntegerElement", inParams, null);
            if (System.Convert.ToBoolean(outParams.Properties["ReturnValue"].Value) == true)
            {
                Log.Logger.Info("Succeeded to call SetIntegerElement");
                if (!this.elements.ContainsKey(TypeId))
                {
                    this.elements.Add(TypeId, this.GetElement(TypeId));
                }
                Log.Logger.Info("Leaving SetIntegerElement");
                return true;
            }
            Log.Logger.Info("Leaving SetIntegerElement with false.");
            return false;
        }

        public bool SetBooleanElement(Boolean Boolean, uint TypeId)
        {
            Log.Logger.Info("Entering SetBooleanElement");
            Log.Logger.Info(string.Format("Type : {0:X}", TypeId));
            Log.Logger.Info("Boolean : " + Boolean);
            System.Management.ManagementBaseObject inParams = InternalObject.GetMethodParameters("SetBooleanElement");
            inParams["Boolean"] = Boolean;
            inParams["Type"] = TypeId;
            System.Management.ManagementBaseObject outParams = InternalObject.InvokeMethod("SetBooleanElement", inParams, null);
            if (System.Convert.ToBoolean(outParams.Properties["ReturnValue"].Value) == true)
            {
                Log.Logger.Info("Succeeded to call SetBooleanElement");
                if (!this.elements.ContainsKey(TypeId))
                {
                    this.elements.Add(TypeId, this.GetElement(TypeId));
                }
                Log.Logger.Info("Leaving SetBooleanElement");
                return true;
            }
            Log.Logger.Info("Leaving SetBooleanElement with false.");
            return false;
        }

        public bool SetStringElement(string StringValue, uint TypeId)
        {
            Log.Logger.Info("Entering SetStringElement");
            Log.Logger.Info(string.Format("Type : {0:X}", TypeId));
            Log.Logger.Info("StringValue : " + StringValue);
            System.Management.ManagementBaseObject inParams = InternalObject.GetMethodParameters("SetStringElement");
            inParams["String"] = StringValue;
            inParams["Type"] = TypeId;
            System.Management.ManagementBaseObject outParams = InternalObject.InvokeMethod("SetStringElement", inParams, null);
            if (System.Convert.ToBoolean(outParams.Properties["ReturnValue"].Value) == true)
            {
                Log.Logger.Info("Succeeded to call SetStringElement");
                if (!this.elements.ContainsKey(TypeId))
                {
                    this.elements.Add(TypeId, this.GetElement(TypeId));
                }
                Log.Logger.Info("Leaving SetObjectListElement");
                return true;
            }
            Log.Logger.Info("Leaving SetObjectListElement with false.");
            return false;
        }

        public bool SetFileDeviceElement(
                    uint TypeId,
                    string Path,
                    uint DeviceType,
                    string AdditionalOptions,
                    uint ParentDeviceType,
                    string ParentAdditionalOptions,
                    string ParentPath
                    )
        {
            Log.Logger.Info("Entering SetFileDeviceElement");
            Log.Logger.Info(string.Format("Type : {0:X}", TypeId));
            System.Management.ManagementBaseObject inParams = InternalObject.GetMethodParameters("SetFileDeviceElement");
            inParams["Path"] = Path;
            inParams["Type"] = TypeId;
            inParams["DeviceType"] = DeviceType;
            inParams["AdditionalOptions"] = AdditionalOptions;
            inParams["ParentDeviceType"] = ParentDeviceType;
            inParams["ParentAdditionalOptions"] = ParentAdditionalOptions;
            inParams["ParentPath"] = ParentPath;
            System.Management.ManagementBaseObject outParams = InternalObject.InvokeMethod("SetFileDeviceElement", inParams, null);
            if (System.Convert.ToBoolean(outParams.Properties["ReturnValue"].Value) == true)
            {
                Log.Logger.Info("Succeeded to call SetFileDeviceElement");
                if (!this.elements.ContainsKey(TypeId))
                {
                    this.elements.Add(TypeId, this.GetElement(TypeId));
                }
                Log.Logger.Info("Leaving SetFileDeviceElement");
                return true;
            }
            Log.Logger.Info("Leaving SetFileDeviceElement with false.");
            return false;
        }

        public bool SetPartitionDeviceElement(string Path, uint TypeId, string AdditionalOptions, uint DeviceType)
        {
            Log.Logger.Info("Entering SetPartitionDeviceElement");
            Log.Logger.Info(string.Format("Type : {0:X}", TypeId));
            Log.Logger.Info("Path : " + Path);
            Log.Logger.Info("AdditionalOptions : " + Path);
            Log.Logger.Info("DeviceType : " + DeviceType);
            System.Management.ManagementBaseObject inParams = InternalObject.GetMethodParameters("SetPartitionDeviceElement");
            inParams["Path"] = Path;
            inParams["Type"] = TypeId;
            inParams["AdditionalOptions"] = AdditionalOptions;
            inParams["DeviceType"] = DeviceType;
            System.Management.ManagementBaseObject outParams = InternalObject.InvokeMethod("SetPartitionDeviceElement", inParams, null);
            if (System.Convert.ToBoolean(outParams.Properties["ReturnValue"].Value) == true)
            {
                Log.Logger.Info("Succeeded to call SetPartitionDeviceElement");
                if (!this.elements.ContainsKey(TypeId))
                {
                    this.elements.Add(TypeId, this.GetElement(TypeId));
                }
                Log.Logger.Info("Leaving SetPartitionDeviceElement");
                return true;
            }
            Log.Logger.Info("Leaving SetPartitionDeviceElement with false.");
            return false;
        }

        public bool DeleteElement(uint Type)
        {
            Log.Logger.Info("Entering DeleteElement");
            Log.Logger.Info(string.Format("Type : {0:X}", Type));
            if (elements.ContainsKey(Type))
            {
                System.Management.ManagementBaseObject inParams = InternalObject.GetMethodParameters("DeleteElement");
                inParams["Type"] = ((System.UInt32)(Type));
                System.Management.ManagementBaseObject outParams = InternalObject.InvokeMethod("DeleteElement", inParams, null);
                if (true == System.Convert.ToBoolean(outParams.Properties["ReturnValue"].Value))
                {
                    Log.Logger.Info("Succeeded to call DeleteElement");
                    this.elements.Remove(Type);
                }
                Log.Logger.Info("Leaving DeleteElement.");
                return true;
            }
            else
            {
                Log.Logger.Info(string.Format("No element with type : {0:X} found.", Type));
            }
            Log.Logger.Info("Leaving DeleteElement with false.");
            return false;
        }

        public BcdElement GetElement(uint Type)
        {
            Log.Logger.Info("Entering GetElement");
            Log.Logger.Info(string.Format("Type : {0:X}", Type));
            ManagementBaseObject Element = null;
            try
            {
                System.Management.ManagementBaseObject inParams = null;
                inParams = InternalObject.GetMethodParameters("GetElement");
                inParams["Type"] = ((System.UInt32)(Type));
                System.Management.ManagementBaseObject outParams = InternalObject.InvokeMethod("GetElement", inParams, null);
                if (true == System.Convert.ToBoolean(outParams.Properties["ReturnValue"].Value))
                {
                    Log.Logger.Info("Succeeded to call GetElement");
                    Element = ((System.Management.ManagementBaseObject)(outParams.Properties["Element"].Value));
                    Log.Logger.Info("Leaving GetElement");
                    return BcdElement.BcdElementFactory(Element);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(string.Format("Failed to Get Element with Type {0:x} with exception : {1}", Type, ex.Message));
            }
            Log.Logger.Info("Leaving GetElement with null");
            return null;
        }


        public class BcdElementDictionary
        {
            private readonly BcdObject InternalBcdObject;
            public BcdElementDictionary(BcdObject obj)
            {
                InternalBcdObject = obj;
            }

            public BcdElement this[uint elementType]
            {
                get
                {
                    BcdElement ele;
                    if (this.InternalBcdObject.elements.TryGetValue(elementType, out ele))
                    {
                        return ele;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}
