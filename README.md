# C172HC - Cessna 172 Home Cockpit

Home Cockpits for Flight Simulators need realistic instruments.
Such individual instruments are expensive and require either multiple USB connections or a wiring harness to connect stepper motors, servos, lights, etc. to a main controller.

This project reads telemetry data from MS Flight Simulator and an Arduino and ESP32 based controller distributes it to each avionics instrument (gauge) via a two-wire interface connected to each gauge which has its own microcontroller.
This greatly reduces the wiring.

## c172hc-fsconnect

### Project SerialPortTest

### Project Managed Data Request

See: http://www.prepar3d.com/SDKv3/LearningCenter/utilities/simconnect/sample_code/managed_data_requests.html

##  Gauges and Gauge Codes

We want to support the following gauges / gauge codes:

1. Airspeed Indicator (ASI) 		0x21	
2. Attitude Indicator 			0x22	
(Artificiak Horizon or Vertical Gyro)   
3. Altimeter				0x23
4. Verical Speed Indicator (VSI)	0x24
5. Heading Indicator (HI)		0x25		 
aka Directional Gyro (DG)
6. Turn Coordinator			0x26

7. Engine RPM				0x27

8. VOR Nav1				0x28
9  VOR Nav2				0x29
10.CDI					0x30

11. FUEL L/R				0x31
12. FUEL temp/Flow			0x32
13. OIL Temp(Pressure			0x33
14. VAC/AMP				0x34