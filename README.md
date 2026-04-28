# Ares.Toolkit.Device

The foundational library for building device drivers and hardware integrations within the ARES ecosystem. It provides base classes, interfaces, and utilities to standardize hardware communication, state management, and user interface integration.

## Features

- **Standardized Hardware Interface**: Inherit from `AresDevice` to provide a consistent API for ARES across different hardware types.
- **Reactive State & Status**: Built-in support for `System.Reactive` to stream device state and operational status updates.
- **Command Infrastructure**: Built-in pattern for describing and executing asynchronous commands.
- **Fluent State Builder**: `AresStateBuilder` simplifies the construction of complex `AresStruct` data objects used in the ARES datamodel.
- **UI Integration**: Includes base ViewModels for Blazor-based device controls using `ReactiveUI` and `Radzen`.
- **Fully Asynchronous**: Designed for modern .NET 10 with `async/await` and `CancellationToken` support throughout.

## Core Components

### AresDevice
The primary base class for all ARES devices. It handles status synchronization and provides the scaffolding for commands and settings.

### IAresDevice
The core contract that defines what a "Device" is in ARES, including metadata (Name, Version, HardwareIdentity) and lifecycle methods (`Activate`, `EnterSafeMode`).

### AresStateBuilder
A utility to construct immutable `AresStruct` objects fluently:
```csharp
var state = AresStateBuilder.Create()
    .Add("Temperature", 22.5)
    .Add("Pressure", 101.3)
    .Add("IsActive", true)
    .Build();
```

## Usage

### Implementing a Custom Device

```csharp
public class MySensorDevice : AresDevice
{
    private readonly BehaviorSubject<AresStruct> _stateSubject = new(new AresStruct());

    public MySensorDevice(DeviceConnectionInfo info) : base(info) 
    {
        UpdateStatus(OperationalState.Inactive, "Waiting for activation...");
    }

    public override async Task<bool> Activate(CancellationToken ct)
    {
        // Hardware initialization logic here
        UpdateStatus(OperationalState.Active, "Sensor online.");
        return true;
    }

    public override Task EnterSafeMode(CancellationToken ct) => Task.CompletedTask;

    public override async Task<CommandResult> ExecuteCommand(string command, List<DeviceCommandArgument> arguments, CancellationToken token)
    {
        // Handle incoming commands from the ARES OS
        return new CommandResult { Success = true };
    }

    protected override Task<List<DeviceCommandDescriptor>> BuildCommandDescriptorsAsync()
    {
        return Task.FromResult(new List<DeviceCommandDescriptor>
        {
            new() { Name = "Reset", Description = "Resets the sensor hardware." }
        });
    }

    public override IObservable<AresStruct> StateStream => _stateSubject.AsObservable();
}
```

## Integration

`Ares.Toolkit.Device` is designed to work seamlessly with:
- **Ares.Datamodel**: For standardized data serialization (Protobuf).
- **Ares.Toolkit.Serial / Modbus**: For low-level communication.
- **ReactiveUI**: For building reactive user interfaces in Blazor.

## CI/CD
This project uses GitHub Actions for:
- **CI**: Automated builds and NUnit testing on every push to the `Develop` branch.
- **Release**: Automated NuGet packaging and deployment to the internal ARES registry upon release creation.
