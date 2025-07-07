# Leap Motion Recorder
my project for the Athena Swan Bursary.\
The goal is to record and serialize information from some Leap Motion hands.

This has been designed for the original Leap Motion Controller.

### Setup and usage
Please first install the relevant packages as advised for unity on the leap motion website in your unity project as well as the unity Newtonsoft package. Guidance
for using Unity with leap motion can be found [here](https://docs.ultraleap.com/xr-and-tabletop/xr/unity/getting-started/index.html).

After doing so, add the ButtonReponse.cs script onto a Leap Motion button's button pressed event function in Unity and fill in the relevant variables. 

More information of what to fill in the variables with can be found at the bottom of the README. \

After filling in the variables, simply press play to start the scene and use your leap motion hands to press the button with the script on it.\
Your indicator object will turn to the Wait Colour to indicate it is going to start (this is to give you time to get into a starting position.) and will then turn to the Rec Colour to
indicate the recording has started. \
The indicator object will turn to the Wait Colour once again to show the recording has ended and the frame information is being serialized and saved into your chosen file. 
Once the file writing is done, the indicator object will turn back to the Ready Colour to show that the script is ready to record again.

### Variable meanings and inputs
Indicator: Indicator Object should be put here. This is an object you put in the background that can change colour. It's job is to indicate the status of the script. e.g a sphere\
Ready Colour: The colour the indicator object will turn when the script is ready to start a new recording.\
Wait Colour: The colour the indicator object will turn when it is waiting to start the recording or is writing the recording to a file.\
Rec Colour: The colour the indicator object will turn when it is currently recording.\
Seconds Before Recording: The number of seconds after the button is pressed that the script will wait before it starts recording. Increase this if you feel like you need more time.\
Seconds To Record: The number of seconds the script will record for.\
File Path: The path to the folder you want to save your file in (Do not include file name.) e.g C:\Users\Harry\OneDrive\Desktop\Work\
File Name: The name of the file you want to create or overwrite (do not include .txt) e.g recording1
