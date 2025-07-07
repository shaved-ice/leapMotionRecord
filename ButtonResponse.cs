using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using System.IO;
using Newtonsoft.Json;

public class ButtonResponse : MonoBehaviour
{
    Leap.Controller player;
    public GameObject indicator; //place your indicator object here in unity. It simply changes colour to indicate to the player what is happening
    public Color readyColour = Color.green; //this allows users to change these colours to account for colorblindess etc. 
    public Color waitColour = new Color(0.6f, 0.6f, 0.6f);
    public Color recColour = Color.red;
    public int secondsBeforeRecording = 5; //time before program starts recording
    public int secondsToRecord = 5;
    public string filePath;
    public string fileName = "leapMotionRecord";
    private bool recTime = false;
    private bool first = false;
    private bool saveFlag = false;
    private float saveTime;
    private Material m;
    private List<Frame> frameList = new List<Frame>();
    private Frame lastf;
    private JsonSerializerSettings serializeSettings = new JsonSerializerSettings();
    // Start is called before the first frame update
    void Start()
    {
        m = indicator.GetComponent<MeshRenderer>().material;
        player = new Leap.Controller();
        m.color = readyColour;
        serializeSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;  
    }

    public void ButtonPress()
    {
        if (!recTime && !saveFlag)
        {
            m.color = waitColour;
            Invoke("delayCall", secondsBeforeRecording);
        }
        //for (int i = 0; i < f.Hands.Count; i++) //loop through every hand in the frame
        //{
        //    for (int j = 0; j < f.Hands[i].fingers.Length; j++) //in the hand, go through every finger
        //    {
        //        if (f.Hands[i].fingers[j] != null)
        //        {
        //            for (int k = 0; k < f.Hands[i].fingers[j].bones.Length; k++) //in our finger, go through every bone
        //            {
        //                Debug.Log(f.Hands[i].fingers[j].bones[k].ToString() + " on hand: " + i + " on finger: " + j);
        //            }
        //        }

        //    }
        //}
    }

    private void delayCall()
    {
        recTime = true;
    }

    void Update()
    {
        if (recTime && !first)
        {
            frameList.Add(player.Frame());
            lastf = player.Frame();
            saveTime = Time.time;
            //Debug.Log(Time.time);
            m.color = recColour;
            first = true;
        }
        else if (recTime)
        {
            if (Time.time > saveTime + secondsToRecord)
            {
                //Debug.Log(Time.time);
                //for (int i = 0; i < frameList.Count; i++) //prints every frame in our frameList
                //{
                //    Debug.Log(frameList[i]);
                //}
                first = false;
                recTime = false;
                saveFlag = true;
                m.color = waitColour;
                Invoke("saveRec", 1);
            }
            else
            {
                if (!(player.Frame().Equals(lastf)))
                {
                    frameList.Add(player.Frame());
                }
                lastf = player.Frame();
            }
        }
    }

    private void saveRec()
    {
        string path = filePath + "\\" + fileName + ".txt";
        string jsonString = "";
        string s;
        for (int i = 0; i < frameList.Count; i++) //loop through every Frame we recorded
        {
            s = createJson(frameList[i]);
            jsonString = jsonString + s;
        }
        frameList.Clear(); //we've got the information we need, so we can delete this
        File.WriteAllText(path, jsonString);
        m.color = readyColour;
        saveFlag = false;
    }

    private string createJson(Frame f) //extract the informtion we want, put it into our saveClass object and convert the object into JSON.
    {
        //saveClass x = new saveClass(9);
        frameBreakdown x;
        x = new frameBreakdown(f);

        return JsonConvert.SerializeObject(x, serializeSettings); //the newtonsoft serialization method wrapper 
        //the commands for many other serialization methods
        //return JsonUtility.ToJson(x);
        //return JsonSerialization.ToJson(x);
        //return JsonSerializer.Serialize(x);
    }
}

//this is my estimated object of what information is needed for the recording from each Frame. 
// We can just serialize the frame itself, but it's big and causes a huge lagspike when doing recordings. 
//I hope by taking only the information we need, the program can be more efficient and a lag spike will be small - if not unnoticable. 
//this will need to be adjusted in future models depending on what the replaying software requires. 

public class frameBreakdown
{
    public List<handBreakdown> handList = new List<handBreakdown>();

    public frameBreakdown(Frame f)
    {
        foreach (Hand h in f.Hands)
        {
            handBreakdown hb = new handBreakdown(h);
            handList.Add(hb);
        }

    }
}

public class handBreakdown
{
    public fingerBreakdown thumb;
    public fingerBreakdown index;
    public fingerBreakdown middle;
    public fingerBreakdown ring;
    public fingerBreakdown pinky;
    public List<fingerBreakdown> extraFingers;
    public Vector3 palmPos;
    public Vector3 palmVel;
    public Vector3 palmNormal;
    public Vector3 palmDirection;
    public Quaternion handRotation;
    public float pinchDistance;
    public float palmWidth;
    public Vector3 wristPos;
    public bool isLeft;
    public armBreakdown arm;

    public handBreakdown(Hand h)
    {
        foreach (Finger fi in h.fingers)
        {
            fingerBreakdown fb = new fingerBreakdown(fi);
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

        armBreakdown a = new armBreakdown(h.Arm);
        arm = a;

    }
}

public class armBreakdown
{
    public Vector3 elbowPos;
    public Vector3 wristPos;
    public armBreakdown(Arm a)
    {
        elbowPos = a.ElbowPosition;
        wristPos = a.WristPosition;
    }

}

public class fingerBreakdown
{
    public boneBreakdown metacarpal;
    public boneBreakdown proximal;
    public boneBreakdown intermediate;
    public boneBreakdown distal;
    public List<boneBreakdown> boneList = new List<boneBreakdown>();
    public Vector3 tipPos;
    public Vector3 direction;
    public float width;
    public float length;
    public fingerBreakdown(Finger f)
    {
        foreach (Bone b in f.bones)
        {
            boneBreakdown bb = new boneBreakdown(b);
            switch (b.Type)
            {
                case Bone.BoneType.METACARPAL: //Note: thumbs have a 0 length metacarpal as they do not typically have one in real life.
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

public class boneBreakdown
{
    public Vector3 prevJoint;
    public Vector3 nextJoint;
    public Vector3 center;
    public Vector3 direction;
    public float length;
    public float width;
    public Quaternion rotation;
    public boneBreakdown(Bone b)
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
//example classes to make objects out of so I can test the Json Serialization
//public class saveClass
//{
//    public int x;
//public int z = 79;
//public ha h = new ha();

//public saveClass(int y)
//{
//    x = y;
//}
//}

//public class ha
//{
//    private int x = 3;
//    public int y = 5;
//}
