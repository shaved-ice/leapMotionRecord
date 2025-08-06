using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MixedReality.Toolkit.UX;

public class ScrubSliderBarScript : MonoBehaviour
{
    // Start is called before the first frame update
    private Deserialize deserializer;
    public Slider scrubSlider;
    private float slidescript;
    private UnityEvent<SliderEventData> slideEvent;
    private float fNum = 0f;
    private bool selectBool = false; //let's us know if we're currently being selected or not.
    private int fileNum;

    void Start()
    {
        deserializer = FindFirstObjectByType<Deserialize>();
        scrubSlider = FindFirstObjectByType<Slider>();
        initialiseSlider();
        fileNum = deserializer.getFileNum();
    }

    // Update is called once per frame
    void Update()
    {
        int newFileNum = deserializer.getFileNum();
        if (fileNum != newFileNum)
        {
            fileNum = newFileNum; //new file read so we restart everything
            initialiseSlider();
        }
        if ((deserializer.frameDiff((int)fNum)) && !selectBool) //if our frameNum is different from the one actually being replayed AND the user isn't currently selecting us then update the bar
        {
            fNum = (float)(deserializer.FrameNum);
            scrubSlider.Value = fNum;
        }

    }

    public void initialiseSlider()
    {
        scrubSlider.MaxValue = (deserializer.getMaxSize() - 1); //this way we don't have to waste time calculating the percentage space we need to put the slider in
        scrubSlider.Value = 0f; //always restart the bar to 0
    }

    public void functionSelectUpdate()
    {
        selectBool = true; //lets us know we are currently being selected so avoid updating the bar as not to hinder the user
    }
    public void functionUnselectUpdate()
    {
        deserializer.FrameNum = (int)scrubSlider.Value; //update deserializer with the new frame num
        selectBool = false;
    }
}
