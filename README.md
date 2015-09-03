# Automation : Windows Azure Storage Emulator

This library enables programmatic control of the Windows Azure Storage Emulator from .NET. This can be useful during integration testing, or anytime you need to work with the Windows Azure Storage Emulator from .NET code.

## Prerequisites

- .NET Framework 4.5
- Windows Azure Storage Emulator installed
  - It's been tested with Windows Azure Storage Emulator 4.1.0.0 locally by @kendaleiv and the build passes with Windows Azure Storage Emulator 3.3.0.0 on a build agent -- it may work with other versions, or, it may not.

## Installation

Install the [RimDev.Automation.StorageEmulator](https://www.nuget.org/packages/RimDev.Automation.StorageEmulator/) package from NuGet:

```
PM> Install-Package RimDev.Automation.StorageEmulator
```

## Quick Start (C#)

To start the Windows Azure Storage Emulator:

```csharp
new AzureStorageEmulatorAutomation().Start();
```

You can also do some other things, like:

```csharp
var automation = new AzureStorageEmulatorAutomation();

automation.Start();

AzureStorageEmulatorAutomation.IsEmulatorRunning(); // should be true

automation.ClearAll();

// Or, clear only certain things:
automation.ClearBlobs();
automation.ClearTables();
automation.ClearQueues();

automation.Stop();

AzureStorageEmulatorAutomation.IsEmulatorRunning(); // should be false
```
`AzureStorageEmulatorAutomation` implements `IDisposable`. The `Dispose` method will only stop the Windows Azure Storage Emulator if it was started by the `AzureStorageEmulatorAutomation` instance. We're nice and don't close it if it was opened by a different instance (or, a user opening it manually on their machine).

An example `IDispoable` implementation might look like:

```csharp
using (var automation = new AzureStorageEmulatorAutomation())
{
    automation.Start();

    // Work with the running Azure Storage Emulator here.
}

// Outside the scope of the using, if the Azure Storage Emulator was
// started by `automation.Start();` above, then it should be shut down.
// If it was already running, it should remain running.
```

## Thanks

Thanks to [Ritter IM](http://ritterim.com) for supporting OSS.
