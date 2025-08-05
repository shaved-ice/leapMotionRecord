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
    //private RectTransform rect;
    private float slidescript;
    private UnityEvent<SliderEventData> slideEvent;
    private float fNum = 0f;
    private bool selectBool = false; //let's us know if we're currently being selected or not.
    //private UnityAction action;

    void Start()
    {
        deserializer = FindFirstObjectByType<Deserialize>();
        scrubSlider = FindFirstObjectByType<Slider>();
        scrubSlider.MaxValue = (deserializer.getMaxSize() - 1); //this way we don't have to waste time calculating the percentage space we need to put the slider in
        scrubSlider.Value = 0f; //always restart the bar to 0
        //an attempt at adding the function to the change value UnityEvent without using the editor.
        //slideEvent = scrubSlider.OnValueUpdated;
        //action = action + functionForSlideUpdate;
        //scrubSlider.OnValueUpdated.AddListener(action);

        ////old code to change the width of the slider.
        //rect = scrubSlider.GetComponent<RectTransform>();
        //Vector2 newDelta = rect.sizeDelta;
        //newDelta.x = 300; //I chose this number as it looked long enough to be able to accomodate a large range of video sizes but wasn't too big as to be hard to use.
        //rect.sizeDelta = newDelta; 

    }

    // Update is called once per frame
    void Update()
    {
        if ( (!(deserializer.frameDiff((int)fNum))) && !selectBool)
        {
            fNum = (float)(deserializer.FrameNum);
            scrubSlider.Value = fNum;
        }

    }

    public void functionSelectUpdate()
    {
        selectBool = true;
    }
    public void functionUnselectUpdate()
    {
        deserializer.FrameNum = (int)scrubSlider.Value;
        selectBool = false;
    }
}
