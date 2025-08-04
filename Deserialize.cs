using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using Newtonsoft.Json;

public class Deserialize : MonoBehaviour
{

    public string directoryPath;
    public string fileName;
    private string projDirectoryPath;
    private string projFileName;
    private GameObject handModelLeft;
    private GameObject handModelRight;
    private GameObject hands;
    public int frameIncrement;
    private JsonSerializer serializer;
    private FrameJson serialized;
    private int maxSize;
    private int frameNum = 0;
    private bool playTime = true;
    private Vector3 forwardPos; //position when hands/palms are facing away from you
    private Vector3 backwardPos; //position when hands/palms are facing towards you
    private bool forwardFacing = false; //I want to start the program with the palms facing towards us as in the initial specification
    private int fileNum = 0;
    public int FrameNum
    {
        get { return frameNum; }
        set { frameNum = value; }
    }
    // I believe the three finger bones from palm to tip for each of the fingers of this model is the proximal, intermediate and distal 
    // transforming finger tips seems to do nothing so I have left them out.
    // Start is called before the first frame update
    void Awake()
    {
        handModelLeft = GameObject.Find("Left");
        handModelRight = GameObject.Find("Right");
        hands = new GameObject("Hands"); 
        handModelLeft.transform.SetParent(hands.transform, false);
        handModelRight.transform.SetParent(hands.transform, false); //this scoops up the two hand objects given and places them as children under a gameObject named hands.
        forwardPos = hands.transform.position;
        backwardPos = new Vector3(forwardPos.x, forwardPos.y, forwardPos.z + -0.1f); //this should move the hands object closer to the user by 0.1f - I chose this number manually.
        hands.transform.position = backwardPos; //since I've decided we always start in backwards mode - I initialise the hand position into the backwards one. 
        //since we spun the hands around for backwards hands it creates extra distance from the player. I estimate this position to be make it so it is not quite as far. 
        serializer = new JsonSerializer();
        serialized = new FrameJson();
        byte[] arr = new byte[32];
        UTF8Encoding ut = new UTF8Encoding();
        deserializeFile(directoryPath, fileName);
        DisplayFrame();
        //old testing of frame1
        //TransformHand(serialized.jsonList[0].handList[0]); //stage 1 of development: displaying frame 1 correctly. 
        //TransformHand(serialized.jsonList[0].handList[1]);

        ////simple transform code to test finger movement
        //HandBreakdown Hand = serialized.jsonList[0].handList[0];
        //GameObject finger;
        //GameObject palm;
        //if (Hand.isLeft)
        //{
        //    palm = handModelLeft.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        //    finger = palm.transform.GetChild(1).gameObject;
        //}
        //else
        //{
        //    palm = handModelLeft.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        //    finger = palm.transform.GetChild(1).gameObject;
        //}

        //Debug.Log(handModelLeft.transform.GetChild(0).GetChild(0).gameObject); //prints wrists
        //finger.transform.TransformPoint(Vector3.forward + Vector3.forward);
        //finger.transform.GetChild(0).TransformPoint(Vector3.forward);



        //buffered version attempt:
        //if (!(File.Exists(filePath)))
        //{
        //    Debug.Log("File doesn't exist!");
        //}
        //else
        //{
        //    using (Stream fs = File.OpenRead(filePath))
        //    {
        //        using (BufferedStream b = new BufferedStream(fs, 10))
        //        {
        //            while (r != 0 && x < 10) // read function returns 0 when it reaches the end of the stream 
        //            {
        //                r = b.Read(arr, 0, 1);
        //                x++;
        //            }
        //        }
        //    }
        //}
        //foreach (byte by in arr)
        //{
        //    string st = by.ToString();
        //}
    }

    //Update is called once per frame
    void FixedUpdate()
    {
        //if (frameNum < maxSize)
        //{
        //    TransformHand(serialized.jsonList[frameNum].handList[0]);
        //    TransformHand(serialized.jsonList[frameNum].handList[1]);
        //    frameNum++;
        //}
        if (playTime)
        {
            frameNum = (frameNum + 1) % maxSize; //increment the frameNum every time but don't go over the length of the framelist
            DisplayFrame();
        }
    }

    private void deserializeFile(string dPath, string fName)
    {
        string filePath = dPath + "\\" + fName + ".txt";
        if (!(File.Exists(filePath)))
        {
            Debug.Log("File doesn't exist!");
        }
        else
        {
            using (Stream fs = File.OpenRead(filePath))
            {
                using (TextReader sr = new StreamReader(fs))
                {
                    //serial = sr.ReadLine();
                    serialized = (FrameJson)serializer.Deserialize(sr, typeof(FrameJson));
                }
            }
            //Debug.Log(serialized.jsonList[0].handList[0].palmPos);
            //Debug.Log(serialized.JsonList[1]);
        }
        maxSize = serialized.jsonList.Count;
        fileNum++; //a new file has been read so increment the fileNum
    } //since the function name doesn't describe displaying the new file, I don't include it

    //USER INTERACTION FUNCTIONS
    //##########################

    public void PausePlay()
    {
        playTime = !playTime;
    }
    
    public void NextFrame()
    {
        if (frameNum < maxSize - 1) //do nothing if you are on the last frame
        {
            frameNum = frameNum + 1;
        }
        DisplayFrame();
    }

    public void NextFrameLarge()
    {
        if (frameNum <= ((maxSize - 1) - frameIncrement))
        {
            frameNum = frameNum + frameIncrement;
        }
        else
        {
            frameNum = maxSize - 1;
        }
        DisplayFrame();
    }

    public void PrevFrame()
    {
        if (frameNum > 1) //do nothing if you are on the first frame
        {
            frameNum = frameNum - 1;
        }
        DisplayFrame();
    }

    public void PrevFrameLarge()
    {
        if (frameNum >= frameIncrement)
        {
            frameNum = frameNum - frameIncrement;
        }
        else
        {
            frameNum = 0; // stopping at the start and not looping around is more intuitive. 
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

    public void reverseHands()
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
    //##########################

    private void DisplayFrame()
    {
        TransformHand(serialized.jsonList[frameNum].handList[0]);
        TransformHand(serialized.jsonList[frameNum].handList[1]);
        if (!forwardFacing) //only do this if we are facing backwards
        {
            hands.transform.Rotate(0f, 180f, 0f, Space.World); //this flips the rotation of the hands 180 degrees around the y axis AFTER the hands are displayed, ensuring nothing else changes 
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
            handRig = handModelLeft.transform.GetChild(0).gameObject;
        }
        else
        {
            handRig = handModelRight.transform.GetChild(0).gameObject;
        }
        wrist = handRig.transform.GetChild(0).gameObject;
        hand = wrist.transform.GetChild(0).gameObject;

        //example moving palm to random location + rotation
        //hand = handRig.transform.GetChild(0).GetChild(0).gameObject;
        //moveToCoords(hand, Vector3.right);
        //Quaternion quat = new Quaternion(1, 3, 50, 1);
        //rotateQ(hand, quat);

        //handRig.transform.position = b.arm.elbowPos;

        //let's start from the wrist and work our way down to the fingers.
        //handRig.transform.position = b.palmPos;
        //spin(handRig, b.handRotation);

        wrist.transform.position = b.arm.wristPos;
        spin(wrist, b.arm.armRotation);

        hand.transform.position = b.palmPos;
        spin(hand, b.handRotation);

        // since it is children, it should go top to bottom so I will have thumb, index, etc. in that order
        // since I named each finger for easy access, I cannot look through the fingers. 
        fingerTransform(hand.transform.GetChild(0).gameObject, b.thumb, true);
        fingerTransform(hand.transform.GetChild(1).gameObject, b.index, false);
        fingerTransform(hand.transform.GetChild(2).gameObject, b.middle, false);
        fingerTransform(hand.transform.GetChild(3).gameObject, b.ring, false);
        fingerTransform(hand.transform.GetChild(4).gameObject, b.pinky, false);

    }

    private void fingerTransform(GameObject finger, FingerBreakdown f, bool thumbStatus) //every finger in this model has the distal, intermediate and proximal phalanges
                                                                                         //except for the thumb which only has the distal and intermediate phalanges.
                                                                                         //fingers start from the inside (proximal) and their children go further out to the distal bone.
    {
        GameObject intermediate;
        if (!thumbStatus)
        {
            GameObject proximal = finger; 
            intermediate = proximal.transform.GetChild(0).gameObject;
            proximal.transform.position = f.proximal.prevJoint; //using prevJoint instead of centre. This is because the model I'm using seems to have points on the fingers between the joints instead of actual bone parts.
            spin(proximal, f.proximal.rotation);
        }
        else
        {
            intermediate = finger; 
        }
        intermediate.transform.position = f.intermediate.prevJoint;
        spin(intermediate, f.intermediate.rotation);
        GameObject distal = intermediate.transform.GetChild(0).gameObject;
        distal.transform.position = f.distal.prevJoint;
        spin(distal, f.distal.rotation);

        //GameObject proximal = finger.transform.GetChild(0).gameObject;
        //GameObject intermediate = intermediate.transform.GetChild(0).gameObject;
        //proximal.transform.position = f.proximal.center;
        //spin(distal, f.distal.rotation);
        //intermediate.transform.position = f.intermediate.center;
        //spin(intermediate, f.intermediate.rotation);

        //GameObject fingerTip;

        //if (!thumbStatus) //if this finger is NOT a thumb, include the proximal phalange.
        //{
        //    GameObject proximal = intermediate.transform.GetChild(0).gameObject;
        //    proximal.transform.position = f.proximal.center;
        //    spin(proximal, f.proximal.rotation);
        //    fingerTip = proximal.transform.GetChild(0).gameObject;
        //}
        //else
        //{
        //    fingerTip = intermediate.transform.GetChild(0).gameObject;
        //}
        ////for the pointing of the fingertips, since the leap motion finger tips are not seperate from the distal bone, 
        //fingerTip.transform.position = f.tipPos;


    }

    public void spin(GameObject g, Quaternion q)
    {
        //Quaternion inverse = Quaternion.Inverse(g.transform.rotation); //by applying the inverse rotation to the current rotation I hope to fully reset the object's rotation.
        g.transform.rotation = Quaternion.identity;
        //g.transform.Rotate(inverse.eulerAngles, Space.World);
        g.transform.Rotate(q.eulerAngles, Space.World);
    }

    //including the class structure so I can access data after deserialization.
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

    public int getMaxSize()
    {
        return maxSize;
    }

    public bool frameDiff(int fNum)
    {
        return (frameNum != fNum);
    }

    public int getFileNum()
    {
        return fileNum;
    }

    public void acceptNewPath(string newPath) //for some reason the exact same code here from fileSubmitter fails to work.
    {
        projDirectoryPath = newPath;
        Debug.Log(projDirectoryPath);
    }

    public void acceptNewFileName(string newFileName)
    {
        projFileName = newFileName;
        Debug.Log(projFileName);
    }

    public void projReadNewFile()
    {
        deserializeFile(projDirectoryPath, projFileName);
        frameNum = 0; //reset to the beginning
        DisplayFrame();
    }
}
