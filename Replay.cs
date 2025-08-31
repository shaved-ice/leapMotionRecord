using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using Newtonsoft.Json;

public class Replay : MonoBehaviour
{

    public string directoryPath; //the directory and file name where we read the recording file
    public string fileName;
    private GameObject handModelLeft; //the models of the hands we want to transform
    private GameObject handModelRight;
    private GameObject hands;
    public int frameIncrement = 10;
    private JsonSerializer serializer = new JsonSerializer();
    private FrameJson serialized = new FrameJson(); //our deserialized file
    private int maxSize;
    private int frameNum = 0;
    private bool playTime = true; //are we currently playing the recording? 
    private Vector3 forwardPos; //position when hands/palms are facing away from you
    private Vector3 backwardPos; //position when hands/palms are facing towards you
    private bool forwardFacing = false; //I want to start the program with the palms facing towards us as in the initial specification
    private int fileNum = 0; //this is for other scripts to be able to detect if the current file read has changed e.g new file loaded or read
    private bool validFile = false; //tells us if the current file is valid or not
    public int FrameNum
    {
        get { return frameNum; }
        set { frameNum = value; }
    }
    // I believe the three finger bones from palm to tip for each of the fingers of this model is the proximal, intermediate and distal 
    // transforming finger tips seems to do nothing so I have left them out.
    // Awake is called before the first frame update and before the Starts of other functions so this will allow the other scripts to be sure that the file and maxSize etc. variables will already be set
    void Awake()
    {
        handModelLeft = GameObject.Find("Left"); //finds the Game Object named "Left" - this way the suer doesn't have to manually place it into the script
        handModelRight = GameObject.Find("Right");
        hands = new GameObject("Hands");
        handModelLeft.transform.SetParent(hands.transform, false);
        handModelRight.transform.SetParent(hands.transform, false); //this scoops up the two hand objects found and places them as children under a gameObject named hands.
        forwardPos = hands.transform.position; //save this position as the forward hand position for if we need to revert back to this direction
        backwardPos = new Vector3(forwardPos.x, forwardPos.y, forwardPos.z + -0.1f); //this should move the hands object closer to the user by 0.1f - I chose this number manually.
        //since we spun the hands around for backwards hands it creates extra distance from the player. I estimate this position to make it so that it is not quite as far. 
        hands.transform.position = backwardPos; //since I've decided we always start in backwards mode - I initialise the hand position into the backwards one. 
        byte[] arr = new byte[32];
        UTF8Encoding ut = new UTF8Encoding(); //set up the things needed for deserialization
        DeserializeFile();
        DisplayFrame(); //deserialize and then display the first frame
    }

    //Update is called once per frame
    void FixedUpdate()
    {
        if (playTime && (maxSize != 0))
        {
            frameNum = (frameNum + 1) % maxSize; //increment the frameNum every time but don't go over the length of the framelist
            DisplayFrame();
        }
    }

    private void DeserializeFile()
    {
        string filePath = directoryPath + "\\" + fileName + ".txt";
        if (!(File.Exists(filePath)))
        {
            Debug.Log("File doesn't exist!");
            maxSize = 0;
        }
        else
        {
            using (Stream fs = File.OpenRead(filePath))
            {
                using (TextReader sr = new StreamReader(fs))
                {
                    serialized = (FrameJson)serializer.Deserialize(sr, typeof(FrameJson));
                }
            }
            ValidateFile();
            if (validFile)
            {
                maxSize = serialized.jsonList.Count;
            }
            else
            {
                maxSize = 0;
            }
        }
        fileNum++; //a new file has been read so increment the fileNum
    } //since the function name doesn't describe displaying the new file, I don't include it - this way it's more intuitive to use

    private void ValidateFile()
    {
        validFile = true;
        if (serialized == null) //if the object given isn't an actual FrameJson object it will be null so we know the file is invalid if it is null
        {
            validFile = false; 
        }
        else
        {
            foreach (FrameBreakdown frame in serialized.jsonList)
            {
                if (frame.handList.Count != 2) //if we don't have exactly two hands in the frame then the file is invalid. 
                {
                    validFile = false;
                }
            }
        }
    }

    private void DisplayFrame()
    {
        if (validFile) //if the file is invalid, don't display it. Putting it here ensures an error is never thrown even if display were to be called by anny of the UI button functions etc. 
        {
            TransformHand(serialized.jsonList[frameNum].handList[0]);
            TransformHand(serialized.jsonList[frameNum].handList[1]);
            if (!forwardFacing) //only do this if we are facing backwards
            {
                hands.transform.Rotate(0f, 180f, 0f, Space.World); //this flips the rotation of the hands 180 degrees around the y axis AFTER the hands are displayed, ensuring nothing else changes 
            }
        }
    }

    private void TransformHand(HandBreakdown b)
    {
        //these names aren't necessary but they make the code a lot more readable.
        GameObject handRig;
        GameObject wrist;
        GameObject hand;
        if (b.isLeft)
        {
            handRig = handModelLeft.transform.GetChild(0).gameObject; //find out if we are the left or right hand
        }
        else
        {
            handRig = handModelRight.transform.GetChild(0).gameObject;
        }
        wrist = handRig.transform.GetChild(0).gameObject;
        hand = wrist.transform.GetChild(0).gameObject;

        wrist.transform.position = b.arm.wristPos; //work through each part of the hand, transforming it to the right position and spinning it to the right rotation
        spin(wrist, b.arm.armRotation);

        hand.transform.position = b.palmPos;
        spin(hand, b.handRotation);

        // since it is children, it should go top to bottom so I will have thumb, index, etc. in that order
        // since I named each finger for easy access, I cannot look through the fingers. 
        FingerTransform(hand.transform.GetChild(0).gameObject, b.thumb, true);
        FingerTransform(hand.transform.GetChild(1).gameObject, b.index, false);
        FingerTransform(hand.transform.GetChild(2).gameObject, b.middle, false);
        FingerTransform(hand.transform.GetChild(3).gameObject, b.ring, false);
        FingerTransform(hand.transform.GetChild(4).gameObject, b.pinky, false);

    }

    private void FingerTransform(GameObject finger, FingerBreakdown f, bool thumbStatus) //every finger in this model has the distal, intermediate and proximal phalanges
                                                                                         //except for the thumb which only has the distal and intermediate phalanges.
                                                                                         //fingers start from the inside (proximal) and their children go further out to the distal bone.
    {
        GameObject intermediate;
        if (!thumbStatus) //since all fingers apart from thumbs have the proximal phalange and it is the one we access first - we do this one first
        {
            GameObject proximal = finger;
            intermediate = proximal.transform.GetChild(0).gameObject;
            proximal.transform.position = f.proximal.prevJoint; //using prevJoint instead of centre. This is because the model I'm using seems to have points on the fingers between the joints instead of actual bone parts.
            spin(proximal, f.proximal.rotation);
        }
        else
        {
            intermediate = finger; //we make sure we have an intermediate here so that we don't need an extra if statement to use the intermediate in the next section
        }
        intermediate.transform.position = f.intermediate.prevJoint;
        spin(intermediate, f.intermediate.rotation);
        GameObject distal = intermediate.transform.GetChild(0).gameObject;
        distal.transform.position = f.distal.prevJoint;
        spin(distal, f.distal.rotation);
    }

    public void spin(GameObject g, Quaternion q)
    {
        g.transform.rotation = Quaternion.identity; //reset the object's rotation
        g.transform.Rotate(q.eulerAngles, Space.World); //apply new rotation
    }

    //including the class structure from the recorder so I can access data after deserialization.
    //I removed the functions as I only need the format of the variables 
    public class FrameJson
    {
        public List<FrameBreakdown> jsonList;
        public FrameJson()
        {
            jsonList = new List<FrameBreakdown>();
        }

        public void addf(FrameBreakdown f)
        {
            jsonList.Add(f);
        }
    }

    public class FrameBreakdown
    {
        public List<HandBreakdown> handList = new List<HandBreakdown>();
    }

    public class HandBreakdown
    {
        public FingerBreakdown thumb;
        public FingerBreakdown index;
        public FingerBreakdown middle;
        public FingerBreakdown ring;
        public FingerBreakdown pinky;
        public List<FingerBreakdown> extraFingers;
        public Vector3 palmPos;
        public Vector3 palmVel;
        public Vector3 palmNormal;
        public Vector3 palmDirection;
        public Quaternion handRotation;
        public float pinchDistance;
        public float palmWidth;
        public Vector3 wristPos; //change to the wrist Transform object later (wristJoint) 
        public bool isLeft;
        public ArmBreakdown arm;
    }

    public class ArmBreakdown
    {
        public Vector3 elbowPos;
        public Vector3 wristPos;
        public Quaternion armRotation;

    }

    public class FingerBreakdown
    {
        public BoneBreakdown metacarpal;
        public BoneBreakdown proximal;
        public BoneBreakdown intermediate;
        public BoneBreakdown distal;
        public List<BoneBreakdown> boneList = new List<BoneBreakdown>();
        public Vector3 tipPos;
        public Vector3 direction;
        public float width;
        public float length;
    }

    public class BoneBreakdown
    {
        public Vector3 prevJoint;
        public Vector3 nextJoint;
        public Vector3 center;
        public Vector3 direction;
        public float length;
        public float width;
        public Quaternion rotation;
    }

    // ## FUNCTIONS FOR INTERACTION OUTSIDE OF THIS SCRIPT
    public void SetFrameIncrement(int newIncrement)
    {
        frameIncrement = newIncrement;
    }

    public int GetFrameIncrement()
    {
        return frameIncrement;
    }

    public int GetMaxSize()
    {
        return maxSize;
    }

    public bool FrameDiff(int fNum) //return true if the frameNum is different from ours
    {
        return (frameNum != fNum);
    }

    public int GetFileNum()
    {
        return fileNum;
    }

    public void AcceptNewPath(string newPath) //to be used for reading a new file 
    {
        directoryPath = newPath;
    }

    public void AcceptNewFileName(string newFileName)
    {
        fileName = newFileName;
    }

    public void ProjReadNewFile()
    {
        DeserializeFile();
        frameNum = 0; //reset to the beginning
        DisplayFrame();
    }

    public FrameJson GetFile() //send file to be saved etc.
    {
        return serialized;
    }

    public void SetFile(FrameJson newFile) //load in a given FrameJson file.
    {
        serialized = newFile;
        frameNum = 0;
        maxSize = serialized.jsonList.Count;
        fileNum++;
        DisplayFrame();

    }

    //USER INTERACTION FUNCTIONS
    //##########################

    public void PausePlay()
    {
        playTime = !playTime;
    }

    public void NextFrame() //we stop at the end because it's more intuitive
    {
        if (frameNum < maxSize - 1) //do nothing if you are on the last frame
        {
            frameNum = frameNum + 1;
        }
        DisplayFrame();
    }

    public void NextFrameLarge()
    {
        if (frameNum <= ((maxSize - 1) - frameIncrement)) //if we are more than frameIncrement away from the maxSize then just increment it
        {
            frameNum = frameNum + frameIncrement;
        }
        else //otherwise just make it the maxSize to account for any frame increment you would've been able to make
        {
            frameNum = maxSize - 1;
        }
        DisplayFrame();
    }

    public void PrevFrame() //we stop at the start because it's more intuitive
    {
        if (frameNum > 0) //do nothing if you are on the first frame
        {
            frameNum = frameNum - 1;
        }
        DisplayFrame();
    }

    public void PrevFrameLarge()
    {
        if (frameNum >= frameIncrement) //if we are more than frameIncrement away from the minimum size then just decrement it
        {
            frameNum = frameNum - frameIncrement;
        }
        else //otherwise just make it the minimum size to account for any frame decrement you would've been able to make
        {
            frameNum = 0;
        }
        DisplayFrame();
    }
    public void LastFrame()
    {
        frameNum = maxSize - 1;
        DisplayFrame();
    }

    public void FirstFrame()
    {
        frameNum = 0;
        DisplayFrame();
    }

    public void ReverseHands() //reverse the direction of the hands, move the Hands parent object to the correct position for the direction the hands are facing and then display the new hands.
    {
        if (forwardFacing)
        {
            forwardFacing = false;
            hands.transform.position = backwardPos;
        }
        else
        {
            forwardFacing = true;
            hands.transform.position = forwardPos;
        }
        DisplayFrame(); //update the current frame too
    }

}
