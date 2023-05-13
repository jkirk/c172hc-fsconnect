# C172HC - Cessna 172 Home Cockpit

Home Cockpits for Flight Simulators need realistic instruments.
Such individual instruments are expensive and require either multiple USB connections or a wiring harness to connect stepper motors, servos, lights, etc. to a main controller.

This project reads telemetry data from MS Flight Simulator and an Arduino and ESP32 based controller distributes it to each avionics instrument (gauge) via a two-wire interface connected to each gauge which has its own microcontroller.

We have designed a system in which extracts the information from the Flight Simulator and an Arduino and ESP32 based controller distributes to each Avionics instrument (Gauge) through a two-wire interface connecting to all the gauges having their own microcontroller. This greatly reduces the wiring.

## c172hc-fsconnect

`c172hc-fsconnect` is part of the C172HC project and a tool written in C# using Visual Studio 2022 that reads telemetry data from Flight Simulator and sends it via the serial port to `c172hc-fsmaster`, which in turn controls cockpit instruments.

## Installation

To install `fsconnect`, you will need to build the project in Visual Studio 2022. 

1. Clone the `c172hc-fsconnect` repository to your local machine.
2. Open the `c172hc-fsconnect.sln` solution file in Visual Studio 2022.
3. Build the solution by clicking on Build > Build Solution in the top menu bar.

TODO: Once the solution has been built, the `fsconnect.exe` file will be located in the `bin/Debug` or `bin/Release` directory depending on which configuration was built.

## Usage

To use `fsconnect`, you have to connect `c172hc-fsmaster` via a USB / a serial port interface. First, start `fsconnect` by running the following command:

```sh
fsconnect.exe
```

This will start `fsconnect` and connect it to the default serial port (`COM1` on Windows). If you need to use a different serial port, you can specify it using the `-p` or `--port` option, like this:

```sh
fsconnect.exe --port COM2
```

Once `fsconnect` is running, start `fsmaster` by running the following command:

```sh
fsmaster.exe
```

This will start `fsmaster` and connect it to `fsconnect`. `fsmaster` will then begin receiving telemetry data from Flight Simulator and controlling cockpit instruments.

## Contributing

Contributions to `fsconnect` are welcome! If you find a bug or want to suggest an improvement, please open an issue or submit a pull request.

Before submitting a pull request, please make sure to follow the coding standards used in the project.

## License

`fsconnect` is released under the MIT License. See the [LICENSE](LICENSE) file for details.
