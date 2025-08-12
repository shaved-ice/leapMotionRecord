# Leap Motion Recorder
This is my repository for my Athena Swan Bursary project.\
The goal is to record and serialize information about the hand motion, position etc. from the Leap Motion Controller then 
read and deserialize the same information and replay the recording using a different hand model.

This has been designed for the original Leap Motion Controller.

## Setup and usage of the Recording script
1. Please first install the relevant packages as advised for unity on the leap motion website in your unity project as well as the unity Newtonsoft package. Guidance
for using Unity with leap motion can be found [here](https://docs.ultraleap.com/xr-and-tabletop/xr/unity/getting-started/index.html).
2. Create an instance of the Recorder.cs script in your scene by placing it on an in-scene object (It will be cleaner if you create a new object in the scene for this purpose) 
3. Create a Text (TMP) object instance by right clicking the Hierarchy window, going to the UI section and clicking Text - TextMeshPro. Position this somewhere the user will be able to see when you start the scene. Feel free to change the size, colour etc. 
4. Add the Recorder.cs script onto a Leap Motion button's button pressed event function in Unity and fill in the relevant variables. More information can be found in the section below. 

### Variable meanings and inputs
Text Box: a Text (TMP) object in the scene that will be used to inform the user of what to do/what is happening. 

Seconds Before Recording: The number of seconds after the button is pressed that the script will wait before it starts recording. Increase this if you feel like you need more time.

Seconds To Record: The number of seconds the script will record for.

File Path: The path to the folder you want to save your file in (Do not include file name.) e.g C:\Users\Harry\OneDrive\Desktop\Work\

File Name: The name of the file you want to create or overwrite (do not include .txt) e.g recording1

## Setup and usage of the Replay script
Ideally this script would be recreated/modified for the specific hand model you are using.
The script provided is designed for the [Stylized - Simple Hands](https://assetstore.unity.com/packages/3d/characters/stylized-simple-hands-221297) asset from the Unity store and the setup described here will reflect that.
1. Please import the Simple Hands package and add a hand prefab into the Unity Scene. Name this GameObject "Left". 
2. Please multiply the scale of it's x, y and z axis by 0.14455 (this is because the Simple Hands' model is significantly larger than the leap motion hand's model.)
3. Create a duplicate of that GameObject and name it "Right". 
4. Please then make the z axis of both hands negative and make the x axis of "Left" negative as well. 
5. Now create an instance of the Deserializer.cs script on an object and fill in the variables on the Deserializer.cs instance. More information can be found in the section below.

### Variable meanings and inputs
Directory Path: The path to the folder you want to save your file in (Do not include file name.) e.g C:\Users\Harry\OneDrive\Desktop\LeapMotionRecordings\

File Name: The name of the file you want to create or overwrite (do not include .txt) e.g recording1

Frame Increment: the number of frames you would want a skip button to skip forward or backwards (only relevant if you plan to implement such a UI) 

#### Extra things to do
There are a number of functions that can be used to interact with the Replay script.\
If you wish to, you can create buttons in the scene and make them call these interaction buttons when pressed.\

Buttons that can be used with no extra processing: 
- PausePlay  &nbsp; &nbsp; &nbsp; (pauses or plays the replay)
- NextFrame  &nbsp; &nbsp; &nbsp; (goes to the next frame)
- NextFrameLarge  &nbsp; &nbsp; &nbsp; (skips an amount of frames forward - the increment variable controls have many. Stops at the end of the video)
- PrevFrame &nbsp; &nbsp; &nbsp;  (goes to the previous frame)
- PrevFrameLarge  &nbsp; &nbsp; &nbsp; (skips an amount of frames backward - the increment variable controls have many. Stops at the start of the video)
- LastFrame  &nbsp; &nbsp; &nbsp; (goes to the end of the video)
- FirstFrame  &nbsp; &nbsp; &nbsp; (goes to the start of the video)
- ReverseHands  &nbsp; &nbsp; &nbsp; (flips the direction of the hands away or towards the user.) 

## Setup and usage of the extra UI scripts
Once you have set up the replay script you can use these extra UI scripts to interact with the replay script whilst in play mode.\
We will quickly go through the Editor script as it's setup is quite different from the others
Please keep in mind all these files automatically look for a Replay.cs file instance so if you use these, please do not rename the file.
When the instructions tell you to make something call a function when an event happens - It is recommend to use the UnityEvents often found in the editor for objects such as buttons, text input boxes etc.

### Setup of ReplayEditor.cs
1. Please find the Editor folder in your Assets folder in the Project window. If you cannot locate one, please create a folder named "Editor".
2. Place the ReplayEditor.cs script in this folder.

For each of the remaining scripts, simply put it onto a gameobject and have the variables filled in with the correct objects (these will need to be created as needed).\
To simplify the explanation I will only provide a small description of the script and the variables for each script rather than give step by step instructions on how to set it up.\


### Setup of SaveScript.cs
*A script that handles the saving of files being played. It can hold up to three save files at a time.*

1. Create an instance of this script and fill in the variables as described in the section below.
2. Create a text input object (I use the MRTK one) and make it call the UpdateSaveFileName function (it is assumed you will provide the string found in your text input)
3. Create Save, Load, Delete, Previous and Next buttons and make them call the corresponding function when pressed e.g SaveAFile() is called when the Save button is pressed.

#### Variable meanings and names
File Holder: three Text (TMP) objects that will hold the name and provide a visual queue for how the save files are being stored. The Text (TMP) objects from top to bottom in the inspector should be displayed left to right in the scene.

Status: a Text (TMP) object that will tell the user of events that have occured e.g file has been saved, file has been deleted etc. 

### Setup of ScrubBarScript.cs
*A script that displays the current progress in the video e.g if the replay is halfway through the recording, the bar will be halfway through it's length. This version cannot be interacted with.*

1. Create a Pointer and a Bar for it to sit on. The scene provided uses two modified cube objects.
2. Place the Pointer at the very left side of the Bar in the scene.
3. Create an empty Game Object and Make the Bar and Pointer children of this Game Object. Ensure the Bar is the first child from the top of the list and the Pointer is the second. 
4. Create an instance of this script in the empty Game Object you just made and fill in the variables as described in the section below.

### Setup Of ScrubSliderBarScript.cs
*A script that makes a MRTK slider bar correspond to the current progress of the video and allows the user to use the slider bar to interact with the video progress.*

This script finds the first instance of a Slider in the scene.\
If you intend to have more than one Slider object in the scene, please change this script to manually take in a slider object instead.

1. Create an instance of the MRTK CanvasSlider object.
2. Create an instance of this script.

### Setup of IncrementScript.cs
*A script that allows you to change the increment variable in the Replay.cs script*

1. Create an instance of a text input object.
2. Create an instance of the script. Fill in the relevant variables as described in the section below.

#### Variable meanings and names
Increment Text: a Text (TMP) object that tells the user what the current increment value is

increment Error: a Text (TMP) object that tells the user if any error has occired (mainly invalid input) 



