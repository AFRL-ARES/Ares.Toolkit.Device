using Ares.Device;
using ReactiveUI;
using System;

namespace Ares.Toolkit.Device.UI;

public abstract class DeviceUnitControlViewModel<TDevice> : ReactiveObject where TDevice : IAresDevice
{
  protected DeviceUnitControlViewModel(TDevice device)
  {
    DeviceName = device.Name;
    DeviceId = device.UniqueId;
    Device = device;
  }

  public TDevice Device { get; }
  public string DeviceName { get; }
  public string DeviceId { get; }
  public int DefaultWidth { get; set; } = 20;
  public Type? ViewType { get; set; }
}
