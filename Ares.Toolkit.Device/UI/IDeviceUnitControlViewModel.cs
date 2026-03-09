using Ares.Device;
using System;

namespace Ares.Toolkit.Device.UI;

public interface IDeviceUnitControlViewModel
{
  string DeviceName { get; }
  string DeviceId { get; }
  int DefaultWidth { get; set; }
  Type? ViewType { get; set; }
}
