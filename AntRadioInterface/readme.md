﻿## Small Earth Technology ANT+ Radio Interface Library
Derive concrete class implementations from the AntRadioInterface class to support the hardware being used. This largely
follows Dynastream's .NET and native implementations of their ANT USB stick. Multiple interfaces are defined along with
classes to derive from (AntResponse and DeviceCapabilities). These classes and interfaces are used by the ANT+ class library.

See AntUsbStick in the [repository](https://github.com/StephenHidem/AntPlus/tree/master/Examples/AntUsbStick) for an example
that supports the Dynastream/Garmin ANT USB stick.
One important thing to note is this example uses the .NET and native DLLs provided in Dynastream's PC SDK.