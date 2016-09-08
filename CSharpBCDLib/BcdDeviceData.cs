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
    public enum BCDDeviceType : uint
    {
        PartitionDevice = 2,
        RamdiskDevice = 4
    }

    public class BcdDeviceData
    {
        protected ManagementBaseObject InternalDeviceDataObject;
        public BcdDeviceData(ManagementBaseObject deviceData)
        {
            InternalDeviceDataObject = deviceData;
            DeviceType = (UInt32)Convert.ToUInt32(InternalDeviceDataObject.Properties["DeviceType"].Value.ToString());
            AdditionalOptions = (string)InternalDeviceDataObject.Properties["AdditionalOptions"].Value.ToString();
        }

        public static BcdDeviceData BcdDeviceDataFactory(ManagementBaseObject obj)
        {
            uint deviceType = Convert.ToUInt32(obj.Properties["DeviceType"].Value.ToString());
            if (deviceType == (uint)BCDDeviceType.PartitionDevice)
            {
                return new BcdDevicePartitionData(obj);
            }
            else if (deviceType == (uint)BCDDeviceType.RamdiskDevice)
            {
                return new BcdDeviceFileData(obj);
            }
            return new BcdDeviceData(obj);
        }

        public override string ToString()
        {
            return "DeviceType : " + DeviceType + " AdditionalOptions : " + AdditionalOptions;
        }

        public uint DeviceType { get; private set; }
        public string AdditionalOptions { get; private set; }
    }

    public class BcdDeviceFileData : BcdDeviceData
    {
        public BcdDeviceFileData(ManagementBaseObject deviceData) : base(deviceData)
        {
            Path = (string)InternalDeviceDataObject.Properties["Path"].Value.ToString();
            Parent = BcdDeviceDataFactory((ManagementBaseObject)InternalDeviceDataObject.Properties["Parent"].Value);
        }

        public override string ToString()
        {
            return base.ToString() + " Path : " + Path + " Parent : " + Parent.ToString();
        }

        public string Path { get; private set; }
        public BcdDeviceData Parent { get; private set; }
    }

    public class BcdDevicePartitionData : BcdDeviceData
    {
        public BcdDevicePartitionData(ManagementBaseObject deviceData) : base(deviceData)
        {
            Path = (string)InternalDeviceDataObject.Properties["Path"].Value.ToString();
        }

        public override string ToString()
        {
            return base.ToString() + " Path : " + Path;
        }
        public string Path { get; private set; }
    }
}
