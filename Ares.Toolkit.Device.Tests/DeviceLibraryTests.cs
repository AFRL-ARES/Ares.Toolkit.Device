using Ares.Datamodel;
using Ares.Datamodel.Device;
using Ares.Datamodel.Extensions;
using Ares.Datamodel.Templates;
using Ares.Device.Tests.Device;
using System.Reactive.Linq;

namespace Ares.Device.Tests;

internal class DeviceLibraryTests
{
  [Test]
  public async Task Device_Should_Execute_Given_Command()
  {
    var device = new TestDevice();
    var commandDescriptor = device.CommandDescriptors.First(metadata => metadata.Name == TestDeviceCommand.Record.ToString());

    var parameter = new DeviceCommandArgument
    {
      ArgName = "ReplyParameter",
      ArgValue = AresValueHelper.CreateNumber(12345)
    };

    var result = await device.ExecuteCommand(commandDescriptor.Name, [parameter], CancellationToken.None);

    Assert.That(result.Success);
    var num = result.Result;
    Assert.That(num.NumberValue, Is.EqualTo(12345));
  }

  [Test]
  public async Task Device_Should_Fail_Command_When_Parameter_Is_Not_Number()
  {
    var device = new TestDevice();
    var commandName = TestDeviceCommand.Record.ToString();

    var parameter = new DeviceCommandArgument
    {
      ArgName = "ReplyParameter",
      ArgValue = AresValueHelper.CreateString("NotaNumber")
    };

    var result = await device.ExecuteCommand(commandName, [parameter], CancellationToken.None);

    Assert.That(result.Success, Is.False);
    Assert.That(result.Error, Does.Contain("expected a number"));
  }

  [Test]
  public void Device_Should_Throw_When_Parameter_Is_Missing()
  {
    var device = new TestDevice();
    var commandName = TestDeviceCommand.Record.ToString();

    Assert.ThrowsAsync<InvalidOperationException>(async () =>
      await device.ExecuteCommand(commandName, [], CancellationToken.None));
  }

  [Test]
  public void Device_Should_Throw_When_Command_Is_Unknown()
  {
    var device = new TestDevice();

    Assert.ThrowsAsync<ArgumentException>(async () =>
      await device.ExecuteCommand("UnknownCommand", [], CancellationToken.None));
  }

  [Test]
  public async Task Device_Should_Activate_Successfully()
  {
    var device = new TestDevice();
    var result = await device.Activate(CancellationToken.None);
    Assert.That(result, Is.True);
  }

  [Test]
  public void Device_Should_Have_Correct_Metadata()
  {
    var device = new TestDevice();
    Assert.That(device.Name, Is.EqualTo("Test Device"));
    Assert.That(device.UniqueId, Is.EqualTo("TestDevice"));

    var recordCommand = device.CommandDescriptors.FirstOrDefault(c => c.Name == TestDeviceCommand.Record.ToString());
    Assert.That(recordCommand, Is.Not.Null);
    Assert.That(recordCommand!.Description, Is.EqualTo("A test command"));
  }

  [Test]
  public async Task Device_Should_Return_Empty_State()
  {
    var device = new TestDevice();
    var state = await device.GetState();
    Assert.That(state, Is.Not.Null);
    Assert.That(state.Fields, Is.Empty);
  }

  [Test]
  public async Task Device_Should_Emit_Initial_State_On_Stream()
  {
    var device = new TestDevice();
    var state = await device.StateStream.FirstAsync();
    Assert.That(state, Is.Not.Null);
    Assert.That(state.Fields, Is.Empty);
  }

  [Test]
  public void Device_Should_Handle_SafeMode()
  {
    var device = new TestDevice();
    Assert.DoesNotThrowAsync(async () => await device.EnterSafeMode(CancellationToken.None));
  }

  [Test]
  public void Device_Should_Accept_Settings_Update()
  {
    var device = new TestDevice();
    var settings = new AresStruct();
    Assert.DoesNotThrowAsync(async () => await device.UpdateSettings(settings));
  }

  [Test]
  public async Task Device_Should_Report_Initial_Status()
  {
    var device = new TestDevice();
    Assert.That(device.Status.OperationalState, Is.EqualTo(OperationalState.Inactive));

    var status = await device.StatusObservable.FirstAsync();
    Assert.That(status.OperationalState, Is.EqualTo(OperationalState.Inactive));
  }

  [Test]
  public void Device_Should_Complete_Status_On_Dispose()
  {
    var device = new TestDevice();
    var completed = false;

    using var sub = device.StatusObservable.Subscribe(_ => { }, () => completed = true);
    device.Dispose();

    Assert.That(completed, Is.True);
  }
}
