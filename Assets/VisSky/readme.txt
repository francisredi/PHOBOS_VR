VISky by VIS-Games


--------------------------------------------------
Version 3.1

Changelog:
-Approved for Unity 4.x

--------------------------------------------------
Version 3.0

Changelog:
-Added Rain Fx
-Added Thunderstorm
-Added Weather Sound FX
-Optimized cloud motion
-Improved color/intensity look-up-tables
-Added WindZone Modifier so the sky can handle the wind of unity trees

--------------------------------------------------
Version 2.0

Changelog:
-Improved Color/Intensity tables



--------------------------------------------------
Version 2.1

Changelog:
-Some smaller fixes to approve package for Unity 3.5




--------------------------------------------------


Features UnityPro:
-------------------
-Static or dynamic time-system

-Day/Night-change (configurable time-speed)

-Simple cloud-density change (weather change) 

-Stars and moon at nightsky

-Contrails from airplanes

-controlable windspeed for clouds

-Shadows for Sun and Moon

-changing light/shadow intensities by cloud-sun-occlusion
 Light will get more diffuse and shadows will loose intensity when the sun or moon are occluded by clouds
 Light will get more bright and shadows get harder when no clouds are in front of the sun/moon

-Changing Sky/Cloud-Colors by daytime and cloud-density



Features Free Unity Version:
----------------------------
-Static or dynamic time-system

-Day/Night-change (configurable time-speed)

-Simple cloud-density change (weather change) 

-Stars and moon at nightsky

-Contrails from airplanes

-controlable windspeed for clouds

-changing light/shadow intensities by cloud-sun-occlusion (this will be faked by the current cloud count)
 Light will get more diffuse when more clouds are at the sky
 Light will get more bright when less clouds are at the sky

-Changing Sky/Cloud-Colors by daytime and cloud-density



---------------------------------------------------------------
---------------------------------------------------------------
---------------------------------------------------------------
Please notice:

To use all features like shadows and cloud-sun/moon occlusion 
UnityPro is needed.

When running under the free version of unity the cloud-sun/moon
occlusion will be faked using the current cloud-count
and shadows will be disabled.

This Sky-System is not created by physically correct parameters!
It is designed to give a nice look.
No Shaders are used. This will cause some more Drawcalls, but 
it will run on each grafic-card and will run fast.

Only a few drawcalls are used caused by the batching of unity.

---------------------------------------------------------------
---------------------------------------------------------------
---------------------------------------------------------------
How to use:

1) Import the package into your project

2) load one of the example scenes (this will init some needed internal values, so 
   you don´t have to set up them all by hand)

3) open your own scene

4) Drag the "VisSky" Prefab (in the Prefab-Folder) into your scene (to Position 0,0,0)

5) Set your Far-Clipping-Plane from your Scene-Camera to a value > 5000

6) Rotate the VisSky Prefab around the Y-Axis as you like to get the sunrise and sunset whereever you want it.

7) Set the "Viewport Camera or Object" in the Inspector in the VisSky Editor

8) Create a new Layer name "Clouds"

9) Open the Prefab VisSky in your Hierarchy and selected the GameObject "Clouds" and set it to the Layer "Clouds"

10) Select the GameObject "CloudCam" and set the Culling Mask only to the Layer "Clouds"

11) Run your Scene


In the Example_Scenes-Folder you will also Find some example-scenes with different presets for the system
The Example Assets in this Project can be used in all your productions as well (car, trees and textures).

Please notice, that the trees in the example scene are not created with the tree-designer
and they will not move in wind! For this case you need Unity-Tree-Designer trees!

---------------------------------------------------------------
---------------------------------------------------------------
---------------------------------------------------------------
Paramter Documenation:
----------------------
Running under UnityPro:		Set this to enabled when you are using Unity Pro to enable all 
				features of the sky-system.

Viewport Camera or Object: 	The System needs a GameObject or a Camera that represents the Viewport Center 
(only needed when running	(Main Camera for example, or another GameObject that is always at the Player/Viewport-
 under UnityPro)		Center Position).
				If this is not defined, the System will show an Error-Message in the Console and
				terminate the script.

The following paramters can be changed during the application is running:
-------------------------------------------------------------------------

Time hours:			Defines the Daytime hour    (0-23)
Time minutes:			Defines the Daytime minute  (0-59)
Time speed			Defines the speed the daytime changes

Wind X-Speed:			Defines the direction and Speed of the clouds in the X-direction
Wind Z-Speed:			Defines the direction and Speed of the clouds in the Z-direction


Weather SFX Volume		Defines the total max volume of the wind and rain sound effects
-------------------------------------------------------------------------------
The following paramters can only be changed when the application is stopped:
-------------------------------------------------------------------------------
Create Wind Zone for Unity Trees: Enables the Wind-Zone-Modifier for Unity Trees

Enable Rain:			Rain will be created when the cloud-count value is greater than 400 clouds
				Here are some internal random values for the start delay, intensity and duration.

Max Rain Particles:		Defines the number of rain particles that will be emitted at once 
				(max emission value of the unity particle system, not the number of max particles)
				If you want to reduce the max amount of particles for this particle system,
  				select the Object "VisSky -> Weather -> Rain" and change the "Max Particles" value
  				of the particle System in the Inspector.

Number of clouds:		Defines the number of Clouds in the Sky


Enable Thunderstorm:		Thunderstorm will be created if rain is enabled. The highter the number of clouds, the higher the intensity of 
				the thunderstorm will be. Here are some internal random values for the start delay, intensity and duration. 

Moon cast shadows:		Enables the moon to cast shadows at night.


Stars at Nightsky:		Enables/Disables stars at the nightsky
Max Stars at Nightsky:		Defines the number of stars drawn at the nightsky

Show Airplane Contrails:	Enables/Disables the effect that airplanes crossing the sky and leaving contrails
Max Airplanes:			Defines how much simultious airplanes can cross the sky (1-10)


---------------------------------------------------------------
---------------------------------------------------------------
---------------------------------------------------------------
If you want to trigger some of the parameters from another script,
than these are the target values in the main-VISky script:

Script/Classname: VisSky

Viewport Camera or Object:	(GameObject) viewport_center_obj

Time hours:			(int) time_hour
Time minutes:			(int) time_minutes
Time speed:			(float) time_speed

Wind X-Speed:			(float) wind_xspeed		
Wind Z-Speed:			(float) wind_zspeed		

Moon cast shadows:		(bool) moon_shadows

---------------------------------------------------------------
---------------------------------------------------------------
---------------------------------------------------------------

for any futher questions mail to:

ab@vis-games.de (mail subject: Unity3d VISky)







