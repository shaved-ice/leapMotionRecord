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
    public GameObject handModelLeft;
    public GameObject handModelRight;
    private FrameJson serialized;
    private int maxSize;
    private int frameNum = 0;
    private bool playTime = true;

    // I believe the three finger bones from palm to tip for each of the fingers of this model is the proximal, intermediate and distal 
    // transforming finger tips seems to do nothing so I have left them out.
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(handModelLeft.transform.GetChild(0).localRotation);
        Type t = typeof(FrameJson);
        JsonSerializer j = new JsonSerializer();
        serialized = new FrameJson();
        byte[] arr = new byte[32];
        UTF8Encoding ut = new UTF8Encoding();
        string filePath = directoryPath + "\\" + fileName + ".txt";
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
                    serialized = (FrameJson)j.Deserialize(sr, t);
                }
            }
            //Debug.Log(serialized.jsonList[0].handList[0].palmPos);
            //Debug.Log(serialized.JsonList[1]);
        }
        maxSize = serialized.jsonList.Count;
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
            DisplayFrame();
            frameNum = (frameNum + 1) % maxSize; //increment the frameNum every time but don't go over the length of the framelist
        }
    }

    public void Pause()
    {
        playTime = false;
    }

    public void Play()
    {
        playTime = true;
    }
    
    public void NextFrame()
    {
        frameNum = (frameNum + 1) % maxSize;
        DisplayFrame();
    }

    public void LastFrame()
    {
        if (frameNum == 0)
        {
            frameNum = maxSize - 1;
        }
        else
        {
            frameNum = frameNum - 1;
        }
        DisplayFrame();
    }

    private void DisplayFrame()
    {
        TransformHand(serialized.jsonList[frameNum].handList[0]);
        TransformHand(serialized.jsonList[frameNum].handList[1]);
    }

    private void TransformHand(HandBreakdown b)
    {
        //these names aren't necessary but I think they make the code a lot more readable.
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
            proximal.transform.position = f.proximal.prevJoint; //using prevJoint instead of centre to account for rotation.
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
}
