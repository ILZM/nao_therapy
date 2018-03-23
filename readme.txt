
GENERAL INFO:
This is a readme file of the Nao Therapy project source code of Human-Computer Interaction course
dedicated to make rehabilitation therapies more motivational for a patient.

A patient follows the Nao Robot instructions and do exercise, after which the Robot informs the
patient about the progress and result.


NOTIFICATION:
The project uses only arm elbow rehabilitation, but other therapies can be added. There is a debug
info from console and application form. Also, application writes a file containing raw angle data
of exercise and a file containing smoothed data. There is some bad code on data transfer. The
reason is that Nao Robot creates a new connection with the server everytime it wants to send
something. So, some parts of code is hardcoded.


ADDITIONAL INFO:
There is a additional project PseudoNao with single source file to simulate nao connection.
Usage: run the server (NaoTherapy) firstly. After it run PseudoNao. Instructions can be found in
the source code.

If you want to use NaoRobot then use the choreographe project file included in the archive.

LICENSE:
No license

The project uses some source codes from other sources. Unfortunately, the references were lost.
All the right belongs to their authors.


SPECIFICATIONS:
Used software: 	Operating system - Windows 7
				Integrated Development Environment (IDE) - Microsoft Visual Studio Express 2013 for
				 Windows Desktop
				NaoRobot applications - Choreographe 

Used libraries:	TCD.Kinect.1.2.2 (included in the project)
				HelixToolkit.2013.1.10.1 (included in the project)
				KinectSDK 1.8 (need to install)
				KinectSDK 1.8 Development Kit (need to install)

The application is a part of Nao Robot and Kinect system. To make it work, you need 1 Nao robot, 1
Windows Kinect and 1 Wi-Fi router. Also, you need to install KinectSDK 1.8 and KinectSDK 1.8 for
developers.

You need to reinclude all the references (libraries) used in the project source code.


WORK PROCESS:
The project includes Kinect and server for calculations and communication.

Turn on Nao Robot and Wi-Fi router. Nao Robot must connect to the router's connection. Plug in
Kinect to the computer and run the project. The application waits a connection from the Nao
Robot. Run the choreograph project file in Choreographe program. Nao connects to the server
(application) when the therapy was chosen. In our case you can choose the arm orientation
(left or right) by pressing the head sensor on the Nao Robot. When connection established, Nao
Robot starts to speak and give instructions to the patient. Patient must sit or stand in front
of Kinect to make better arm recognition. After the exercise end the server sends to the Nao
Robot a result of therapy and the Robot informs the patient.


EDITABLE VARIABLES:
The source code includes some constants of therapy exercise. You can adjust them as you wish.


SOURCE CODE FILES:
MainWindow.xaml.cs 		- contains main method

Main/Kinect.cs 			- class to manage Kinect
Main/Logic.cs 			- main class responsible for application logic
Main/Server.cs 			- adjusts server and client connections and communication via TCP socket
Main/TherapyExercise.cs - contains arm elbow rehabilitation. Exercise constants can be found here

Misc/Constant.cs 		- contains some constants used through the project
Misc/CustomMath.cs 		- helper class used for calculations
Misc/Point.cs 			- helper class to create a graph of exercise work
Misc/Scanner.cs 		- class to parse strings
Misc/Vector3.cs 		- Custom 3D Vector implementation