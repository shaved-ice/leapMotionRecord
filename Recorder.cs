using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Leap;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using TMPro;

public class Recorder : MonoBehaviour
{
    Leap.Controller player;
    public TMP_Text textBox; //text box to inform the user of what is happening and of what to do
    public int secondsBeforeRecording = 5; //time before program starts recording
    public int secondsToRecord = 5;
    public string filePath;
    public string fileName = "leapMotionRecording"; //default file name
    private bool recTime = false;
    private bool first = false;
    private bool saveFlag = false;
    private float saveTime;
    private RotationFinder rotFinder;
    private List<Frame> frameList = new List<Frame>();
    private Frame lastf;
    private JsonSerializerSettings serializeSettings = new JsonSerializerSettings();
    // Start is called before the first frame update
    void Start()
    {
        textBox.text = "Press the button on your right to begin the recording program";
        rotFinder = new RotationFinder(); //need this to find rotation angles when we are recording
        player = new Leap.Controller();
        serializeSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;  //we need to ignore the reference loop handling to avoid errors 
    }

    public void ButtonPress()
    {
        if (!recTime && !saveFlag) //don't start a new recording if we are currently saving or recording
        {
            textBox.text = Convert.ToString(secondsBeforeRecording) + " seconds before recording starts";
            Invoke("DelayCall", secondsBeforeRecording); //delay the start of the recording by secondsBeforeRecording amount of seconds
        }
    }

    private void DelayCall()
    {
        recTime = true; //activates the recording to happen in the FixedUpdate function
    }

    void FixedUpdate() //fixed update decreases the amount of random slowing and speeding up in the replay later
    {
        if (recTime && !first) //if it's the first FixedUpdate call where we are recording then we set everything up
        {
            textBox.text = "Recording in progress";
            frameList.Add(player.Frame());
            lastf = player.Frame(); //ensure no duplicates
            saveTime = Time.time; //to check how much time has passed since we started
            first = true;
        }
        else if (recTime)
        {
            if (Time.time > saveTime + secondsToRecord) //since we've passed the amount of time to record, we end the recording and begin the saving
            {
                first = false;
                recTime = false;
                saveFlag = true;
                textBox.text = "Recording finished. Saving in progress.";
                Invoke("SaveRec", 1); //we delay by one second to give the textbox time to update. Without this the test we just put in is skipped entirely.
            }
            else
            {
                if (!(player.Frame().Equals(lastf))) //if we have a frame that is different from the last frame then add it to the recording
                {
                    frameList.Add(player.Frame());
                }
                lastf = player.Frame();
            }
        }
    }

    private void SaveRec() //serializes and saves the recording into a file
    {
        string path = filePath + "\\" + fileName + ".txt"; // the first \ is an escape charater for the second \
        string s = "";
        byte[] e;
        UTF8Encoding utf = new UTF8Encoding();
        if (File.Exists(path))
        {
            File.Delete(path); //delete a file in this location with this name if there is one
        }
        using (Stream w = File.Create(path))
        {
            using (BufferedStream b = new BufferedStream(w, 10)) //my attempt at buffered writing to try to minimise the saving process's huge delay
            {
                s = CreateJson(frameList); //create the serialized object from the frames
                e = utf.GetBytes(s);
                b.Write(e, 0, e.Length); //write our serialized object into the file
            }
        }
        frameList.Clear(); //we've got the information we need, so we can delete this
        textBox.text = "Recording saved successfully! Press the button again to begin another recording.";
        saveFlag = false; //allows us to start a new recording
    }

    private string CreateJson(List<Frame> f) //extract the informtion we want, put it into our saveClass object and convert the object into JSON.
    {
        FrameJson fj = new FrameJson();
        FrameBreakdown x;
        for (int i = 0; i < f.Count; i++) //loop through every Frame we recorded
        {
            x = new FrameBreakdown(f[i], rotFinder);
            fj.addf(x);
        }

        return JsonConvert.SerializeObject(fj, serializeSettings); //the newtonsoft serialization method wrapper. We return the serialized form of the object we made.

    }
}

//this is my estimated object of what information is needed for the recording from each Frame. 
// We can just serialize the frame itself, but it's big and causes a huge lagspike when doing recordings. 
//I hope by taking only the information we need, the program can be more efficient and a lag spike will be small - if not unnoticable. 
//this version is intended to include a lot of information so that future users can easily just trim off all excess information for their versions.

public class FrameJson
{
    public List<FrameBreakdown> jsonList;
    public FrameJson()
    {
        jsonList = new List<FrameBreakdown>();
    }

    public void addf (FrameBreakdown f)
    {
        jsonList.Add(f);
    }
}

public class FrameBreakdown
{
    public List<HandBreakdown> handList = new List<HandBreakdown>();

    public FrameBreakdown(Frame f, RotationFinder rotFinder)
    {
        foreach (Hand h in f.Hands)
        {
            HandBreakdown hb = new HandBreakdown(h, rotFinder);
            handList.Add(hb);
        }

    }
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
    public Vector3 wristPos;
    public bool isLeft;
    public ArmBreakdown arm;

    public HandBreakdown(Hand h, RotationFinder rotFinder)
    {
        foreach (Finger fi in h.fingers)
        {
            FingerBreakdown fb = new FingerBreakdown(fi);
            switch (fi.Type)
            {
                case Finger.FingerType.THUMB:
                    thumb = fb;
                    break;
                case Finger.FingerType.INDEX:
                    index = fb;
                    break;
                case Finger.FingerType.MIDDLE:
                    middle = fb;
                    break;
                case Finger.FingerType.RING:
                    ring = fb;
                    break;
                case Finger.FingerType.PINKY:
                    pinky = fb;
                    break;
                default:
                    extraFingers.Add(fb); //for now I only account for fingers not of the 5 types. 
                    //if required, maybe a bool could be added to account for extras of any of the 5 fingers on each hand. 
                    //as this is a prototype, I have opted not to do this.
                    break;
            }

        }
        palmPos = h.PalmPosition;
        palmVel = h.PalmVelocity;
        palmNormal = h.PalmNormal;
        palmDirection = h.Direction;
        handRotation = h.Rotation;
        pinchDistance = h.PinchDistance;
        palmWidth = h.PalmWidth;
        wristPos = h.WristPosition;
        isLeft = h.IsLeft;

        ArmBreakdown a = new ArmBreakdown(h.Arm, rotFinder);
        arm = a;

    }
}

public class ArmBreakdown
{
    public Vector3 elbowPos;
    public Vector3 wristPos;
    public Quaternion armRotation;
    public ArmBreakdown(Arm a, RotationFinder rotFinder)
    {
        elbowPos = a.ElbowPosition;
        wristPos = a.WristPosition;
        armRotation = rotFinder.findRot(elbowPos, wristPos);
    }
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
    public FingerBreakdown(Finger f)
    {
        foreach (Bone b in f.bones)
        {
            BoneBreakdown bb = new BoneBreakdown(b);
            switch (b.Type)
            {
                case Bone.BoneType.METACARPAL: //Note: thumbs have a 0 length metacarpal as they do not typically have one in real life. (this is mentioned in the leap motion documentation) 
                    metacarpal = bb;
                    break;
                case Bone.BoneType.PROXIMAL:
                    proximal = bb;
                    break;
                case Bone.BoneType.INTERMEDIATE:
                    intermediate = bb;
                    break;
                case Bone.BoneType.DISTAL:
                    distal = bb;
                    break;
                default:
                    boneList.Add(bb); //similarly to fingers, I only account for bones not of the 4 types
                    break;
            }

        }
        tipPos = f.TipPosition;
        direction = f.Direction;
        width = f.Width;
        length = f.Length;
    }
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
    public BoneBreakdown(Bone b)
    {
        prevJoint = b.PrevJoint;
        nextJoint = b.NextJoint;
        center = b.Center;
        direction = b.Direction;
        length = b.Length;
        width = b.Width;
        rotation = b.Rotation;
    }
}

public class RotationFinder //since I can't find a way to convert from vector3 to quarternion, I am using an invisible object to find rotation values that the leap motion API does not provide.
//I'm putting this in a class for readability and ease of code.
{
    private GameObject rotationFinder;
    private GameObject rotationTarget;
    public RotationFinder()
    {
        rotationFinder = new GameObject("rotate"); //this starts a GameObject with no name and only a transform. We don't need it for anything else so this is sufficient
        rotationTarget = new GameObject("target");
    }
    //to use this, simply use the findRot function and input where your object is and what position you want your vector to point to. It will then return the Quaternion of the world rotation calculated.
    public Quaternion findRot(Vector3 startPos, Vector3 targetPos)
    {
        rotationFinder.transform.position = startPos;
        rotationTarget.transform.position = targetPos; //move our objects to the coords given
        rotationFinder.transform.LookAt(rotationTarget.transform); //now our rotationFinder object is turned to look at our target! We have our rotation Quaternion.
        return rotationFinder.transform.rotation;
    }
}
