#SCHOTT.CVLS
This library is to provide easy controls for the The CVLS Product Line, and leverages functions in the [SCHOTT.Core](https://github.com/SCHOTTNorthAmerica/SCHOTT.Core) library. The library uses ComPorts and Ethernet for communications and control, and provides both Synchronous and Asynchronous methods.

This library can be installed in a new project with a NuGet package (also installs the [SCHOTT.Core](https://github.com/SCHOTTNorthAmerica/SCHOTT.Core) library). In the Package Manager Console, type:
Install-Package SCHOTT.CVLS

The [CVLS Dashboard](https://github.com/SCHOTTNorthAmerica/CVLS-Dashboard) Application uses the Asynchronous methods for all of it's controls, and may be an easier way to see how they can be applied.</p>

## Simple ComPort
The sample code below shows how to connect to a simple ComPort and send commands.

```
private static void Main(string[] args)
{
	// connect to any CV-LS USB Ports
	var comPort = CVLSComPort.AutoConnectComPort(CVLSPortType.Usb);

	// connect to any CV-LS Rs232 Ports
	//var comPort = CVLSComPort.AutoConnectComPort(CVLSPortType.Rs232);

	// connect to any CV-LS USB or RS232 Ports
	//var comPort = CVLSComPort.AutoConnectComPort(CVLSPortType.Usb | CVLSPortType.Rs232);

	// connect to CV-LS Serial# 150, USB or RS232 Port
	//var comPort = CVLSComPort.AutoConnectComPort(CVLSPortType.Usb | CVLSPortType.Rs232, serialNumber: 150);

	// this will connect to a specific CVLSComPort name only - Format of 'COM#'
	//var comPort = CVLSComPort.AutoConnectComPort(portName: "COM5");

	Console.WriteLine(comPort.IsConnected
		? $"Connected to Serial Number: {int.Parse(comPort.SendCommandSingle("&z", true))}"
		: "Timed out, no unit found!");

	Console.WriteLine("Press any key to Shutdown CVLSComPort Thread");
	while (!Console.KeyAvailable) { }
	Console.ReadKey(true);
}
```

## Threaded ComPort
For applications that will have units connecting/disconnecting on a regular basis can use the ThreadedComPort. This style of ComPort uses threading, and the threads need to be closed properly before closing your application. The sample code below shows how to set up a ClosingWorker (that manages the threads), start a CVLSThreadedComPort, and tie a method call to connection events.

```
private static ClosingWorker _closingWorker;
private static CVLSThreadedComPort _comPort;

private static void ConnectionUpdate(ThreadedComPortBase.ConnectionUpdateArgs connectionArgs)
{
	// only send commands if we connected to a unit
	if (!connectionArgs.IsConnected)
		return;

	// let the user know what port we connected too
	Console.WriteLine($"Connected to unit on: {_comPort.PortName}");

	// turn the LED on
	_comPort.Protocol.Controls.Led.Enable = true;

	// set led to 100%
	_comPort.Protocol.Controls.Led.Power = 100.0;
	
	// wait half a second
	TimeFunctions.Wait(500);
	
	// turn the LED off
	_comPort.Protocol.Controls.Led.Enable = false;

}

private static void Main(string[] args)
{
	_closingWorker = new ClosingWorker();

	// the ThreadedComPortBase object will continuously try to connect to a unit given the connection rules supplied. 
	// It will also continue checking to make sure the connection is still good. 
	// It has a Register event to be notified when connections are made or lost.

	// this will connect to any CV-LS USB CVLSThreadedComPort
	_comPort = new CVLSThreadedComPort("CVLSThreadedComPort", _closingWorker, CVLSPortType.Usb);

	// this will connect to any CV-LS Rs232 CVLSThreadedComPort
	//_comPort = new CVLSThreadedComPort("CVLSThreadedComPort", _closingWorker, CVLSPortType.Rs232);

	// this will connect to any CV-LS USB or RS232 CVLSThreadedComPort
	//_comPort = new CVLSThreadedComPort("CVLSThreadedComPort", _closingWorker, CVLSPortType.Usb | CVLSPortType.Rs232);

	// this will connect to only CV-LS Serial# 150, either USB or RS232 CVLSThreadedComPort
	//_comPort = new CVLSThreadedComPort("CVLSThreadedComPort", _closingWorker, CVLSPortType.Usb | CVLSPortType.Rs232, 150);

	// this will connect to a specific CVLSComPort name only - Format of 'COM#'
	//_comPort = new CVLSThreadedComPort("CVLSThreadedComPort", _closingWorker, "COM5");

	// register a function to be called on connection events
	_comPort.RegisterConnectionUpdate(MessageBroker.MessageContext.DirectToData, ConnectionUpdate);

	// wait for key press
	Console.WriteLine("Press any key to shut down threads");
	while (!Console.KeyAvailable) { }
	Console.ReadKey(true);

	// close out the threads, writing messages to the console
	_closingWorker.WaitForThreadsToCloseConsoleOutput();

	// wait for key press to close
	Console.WriteLine("Press any key to close");
	while (!Console.KeyAvailable) { }
	Console.ReadKey(true);
}
```

## Legacy Socket
The CV-LS is backwards compatible with the LLS3 Ethernet socket connections, but adds additional commands for the enhanced functionality of the CVLS. This is an asyncronus socket that has the same protocol and command structure as both the CVLSComPort and CVLSThreadedComPort. The code below demonstrates creating a simple Legacy Socket, connecting, and sending commands.

```
private static void Main(string[] args)
{
	// create a new legacy socket
	legacySocket = new LegacySocket();
	
	// connect to a known IP address
	legacySocket.Connect("192.168.0.2");
	
	Console.WriteLine(legacySocket.IsConnected
		? $"Connected to Serial Number: {legacySocket.Protocol.Status.Identification.Serial}"
		: "Timed out, no unit found!");

	Console.WriteLine("Press any key to Shutdown CVLSComPort Thread");
	while (!Console.KeyAvailable) { }
	Console.ReadKey(true);
}
```

If the IP address is unknown, the discovery tool can be used to find all units on the same network as the computer. This requires that the computer and units all be on the same subnet, and the the computer's firewall and network router allow broadcast UDP packets. Also be aware that while the discovery tool is connected, no other program on the same computer will be able to run the tool. This is because it requires the application bind the listening UDP port, and only one application can bind a port at any given time.

```
private static readonly Program P = new Program();
private static readonly List<DiscoveryObject> FoundUnits = new List<DiscoveryObject>();
private static readonly List<LegacySocket> LegacySockets = new List<LegacySocket>();

private static void AddUnit(DiscoveryObject args)
{
	var unitCopy = new DiscoveryObject();
	unitCopy.CopyFrom(args);
	FoundUnits.Add(unitCopy);
	Console.WriteLine($"Serial # {unitCopy.Serial} Found!");
}

private static void Main(string[] args)
{
	// only one application per computer can have a discovery object listening at a time. 
	// This is a limitation of a networking card. Only one object can listen to a port at once.
	var discovery = new Discovery();

	// subscribe to the discover tool, every unit found on the network will report to the "AddUnit" function
	discovery.RegisterDiscoveredUnit(MessageBroker.MessageContext.NewThreadToGui, AddUnit);

	// this queries the network for devices, generally all devices on the network should report in less than 0.5 seconds
	discovery.DiscoverDevices();

	// everything below this comment is to just allow the program to wait while the discovery tool does its job
	Console.WriteLine("Waiting 1 second for units on the network to respond.");
	TimeFunctions.Wait(1000);

	if (FoundUnits.Count == 0)
	{
		Console.WriteLine();
		Console.WriteLine("No units found. Make sure the unit has power and is connected to the same network as the computer.");
	}
	else
	{
		Console.WriteLine();
		Console.WriteLine("Connecting to Legacy Sockets:");
		FoundUnits.ForEach(u =>
		{
			var socket = new LegacySocket();
			var connectionStatus = socket.Connect(u.IpAddress);

			if (connectionStatus == AsyncSocket.ConnectionStatus.Connected)
			{
				Console.WriteLine($"Connected to {u.Serial} at {u.IpAddress}");
				LegacySockets.Add(socket);
			}
			else
			{
				Console.WriteLine($"Could not connect to {u.Serial} at {u.IpAddress}");
			}
		});

		Console.WriteLine();

		for (var i = 1; i <= 6; i++)
		{
			Console.WriteLine($"Turning all connected unit LEDs {(i % 2 == 0 ? "OFF" : "ON")}, then wait 500ms.");
			LegacySockets.ForEach(s =>
			{
				s.SendCommandSingle($"&l{i % 2}");
			});

			TimeFunctions.Wait(500);
		}

		Console.WriteLine();
		Console.WriteLine("Disconnecting from all sockets.");
		LegacySockets.ForEach(s =>
		{
			s.Disconnect();
		});
	}

	Console.WriteLine();
	Console.WriteLine("Application is finished.");
	Console.WriteLine("Press any key to pause shutdown");
	var timeout = DateTime.Now.AddMilliseconds(2500);
	while (DateTime.Now < timeout)
	{
		if (Console.KeyAvailable)
		{
			Console.ReadKey(true);
			Console.WriteLine("Press esc to quit.");
			while (Console.ReadKey(true).Key != ConsoleKey.Escape)
			{
				// wait for esc key to be pressed
				TimeFunctions.Wait(10);
			}
		}

		// wait for any key
		TimeFunctions.Wait(10);
	}
}
```

## Binary Socket
The CV-LS has a new Binary Socket for asyncronus communications that have some enhanced functionality. Because of the speed increase this type of interface allows, the binary socket has functions to stream data out of the unit on a continuous basis. The [CVLS Dashboard](https://github.com/SCHOTTNorthAmerica/CVLS-Dashboard) can be used to see sample code on how this can be achieved. 

