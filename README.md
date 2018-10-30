# NexStarSharp
A Simple C# Library for working with NexStar Telescopes.

# "How do I use this?"
- Its simple. You create a "NexStarDevice" object and then call the "Connect" method.

# Example Code
- Connecting (This is required!)
```c#
var nexStarDevice = new NexStarDevice();
nexStarDevice.Connect("COM6"); //Connect to the telescope. Replace COM6 with the COM port the telescope is connected on.
```

- Pointing/moving the telescope
```c#
//If the telescope is aligned, putting 0 as the azimuth will point the telescope to true north.
nexStarDevice.GoToAzElev(270.0000, 45.0000); //This will have the telescope go to 270 degrees azimuth, 45 degrees elevation.
```

- Getting the telescope model
```c#
TelescopeModel model = nexStarDevice.GetTelescopeModel();
```

- Checking if the telescope is aligned
```c#
bool isAligned = nexStarDevice.IsAligned()
```

- Immediately stop the telescope from moving
```c#
nexStarDevice.CancleGoto();
```
