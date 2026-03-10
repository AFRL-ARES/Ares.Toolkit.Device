using Ares.Datamodel;
using Ares.Datamodel.Device;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Ares.Device.Tests.Device;

public class TestDevice : AresDevice
{
  private readonly BehaviorSubject<AresStruct> _stateSubject = new(new AresStruct());

  public TestDevice() : base(new DeviceConnectionInfo() { DeviceName = "Test Device", DeviceId = "TestDevice" })
  {
    CommandDescriptors = [new DeviceCommandDescriptor() { Name = "Record", Description = "A test command" }];
    Status = new DeviceOperationalStatus() { OperationalState = OperationalState.Inactive, Message = "I'm not ready to do experiments!" };
  }

  public override Task EnterSafeMode(CancellationToken ct)
    => Task.CompletedTask;

  public override Task<bool> Activate(CancellationToken ct)
  {
    UpdateStatus(OperationalState.Active, "I'm ready to do experiments!");
    return Task.FromResult(true);
  }

  public override Task<AresStruct> GetState()
    => Task.FromResult(new AresStruct());

  public override Task<CommandResult> ExecuteCommand(string command, List<DeviceCommandArgument> arguments, CancellationToken token)
  {
    var cmd = Enum.Parse(typeof(TestDeviceCommand), command);

    switch(cmd)
    {
      case TestDeviceCommand.Record:
      case TestDeviceCommand.Record2:
      case TestDeviceCommand.Record3:
        var result = new CommandResult();
        var param = arguments.First(parameter => parameter.ArgName == TestDeviceCommandParameter.ReplyParameter.ToString());

        if(!param.ArgValue.HasNumberValue)
        {
          result.Success = false;
          result.Error = "Test Device expected a number as it's parameter, but none was received!";
          return Task.FromResult(result);
        }

        result.Result = param.ArgValue;
        result.Success = true;
        result.UniqueId = Guid.NewGuid().ToString();
        return Task.FromResult(result);
      default:
        throw new ArgumentOutOfRangeException(nameof(cmd), cmd, null);
    }
  }

  public override Task UpdateSettings(AresStruct settings)
  {
    return Task.CompletedTask;
  }

  public override IObservable<AresStruct> StateStream => _stateSubject.AsObservable();
}
