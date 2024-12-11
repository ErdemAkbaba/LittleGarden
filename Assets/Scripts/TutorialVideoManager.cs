using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class TutorialVideoManager : MonoBehaviour
{
    public List<VideoClip> tutorialClips = new List<VideoClip>();
    public List<string> tutorialDescription = new List<string>();
    public TextMeshProUGUI tutorialDesText;
    public VideoPlayer videoPlayer;
    public int index = 0;
    void Start()
    {
        videoPlayer.clip = tutorialClips[index];
        tutorialDesText.text = LeanLocalization.GetTranslationText(tutorialDescription[index]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CallNextVideo()
    {
        if (index < tutorialClips.Count-1)
        {
            index++;
            videoPlayer.clip = tutorialClips[index];
            tutorialDesText.text = LeanLocalization.GetTranslationText(tutorialDescription[index]);
        }
    }


    public void CallPreviousVideo()
    {
        if (index > 0)
        {
            index--;
            videoPlayer.clip = tutorialClips[index];
            tutorialDesText.text = LeanLocalization.GetTranslationText(tutorialDescription[index]);
        }
    }
}
