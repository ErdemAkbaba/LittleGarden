using Lean.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsController : MonoBehaviour
{
    List<Resolution> resolutions = new List<Resolution>();
    List<Resolution> filterdRes = new List<Resolution>();

    private int resIndex;
    public TextMeshProUGUI resText;

    private bool fullScreen = true;
    public TextMeshProUGUI fullScreenText;

    private int masterVolume = 100;
    private int masterVolumeValue = 0;
    public TextMeshProUGUI masterVolumeText;

    private int qualityIndex;
    private List<string> qualityNames = new List<string>();
    public TextMeshProUGUI qualityText;

    public int currentLangIndex;
    public TextMeshProUGUI langText;
    public List<string> langs = new List<string>();

    public AudioMixer soundFX;
    public AudioMixer music;
    public AudioMixer master;

    private void Start()
    {
        resolutions = Screen.resolutions.ToList();

        for (int i = 0; i < resolutions.Count; i++)
        {
            if (resolutions[i].refreshRateRatio.value == Screen.currentResolution.refreshRateRatio.value)
            {
                filterdRes.Add(resolutions[i]);
            }
        }

        int index = filterdRes.IndexOf(filterdRes.Find(p => p.height == Screen.currentResolution.height && p.width == Screen.currentResolution.width));
        ChangeResulation(index);

        qualityIndex = QualitySettings.GetQualityLevel();
        qualityNames.AddRange(QualitySettings.names);
        qualityText.text = qualityNames[qualityIndex];

        fullScreenText.text = fullScreen.ToString();
        masterVolumeText.text = masterVolume.ToString();
        langText.text = langs[0];
    }

    public void ChangeResulation(int i)
    {
            resIndex += i;

        if (resIndex > filterdRes.Count - 1)
            resIndex = filterdRes.Count - 1;

        if (resIndex < 0)
            resIndex = 0;

        resText.text = filterdRes[resIndex].width + "x" + filterdRes[resIndex].height;

        Screen.SetResolution(filterdRes[resIndex].width, filterdRes[resIndex].height, fullScreen);
    }

    public void ChangeFullScreenMode()
    {
        fullScreen = !fullScreen;
        fullScreenText.text = fullScreen.ToString();
        Screen.fullScreen = fullScreen;
    }

    public void IncreaseVolume()
    {
        if (masterVolume < 100)
        {
            masterVolume += 10;
            masterVolumeValue += 2;
            master.SetFloat("Master", masterVolumeValue);
        }



        masterVolumeText.text = masterVolume.ToString();
    }

    public void DecreaseVolume()
    {
        if (masterVolume > 0)
        {
            masterVolume -= 10;

            masterVolumeValue -= 2;
            master.SetFloat("Master", masterVolumeValue);
        }
        if (masterVolume == 0)
        {
            master.SetFloat("Master", -80);
        }
        masterVolumeText.text = masterVolume.ToString();
    }
    public void ChangeQuality(int i)
    {
        qualityIndex += i;

        if (qualityIndex > qualityNames.Count - 1)
            qualityIndex = qualityNames.Count - 1;

        if (qualityIndex < 0)
            qualityIndex = 0;

        qualityText.text = qualityNames[qualityIndex];
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void NextLang()
    {
        currentLangIndex += 1;

        if (currentLangIndex > langs.Count - 1)
            currentLangIndex = langs.Count - 1;

        langText.text = langs[currentLangIndex];
        LeanLocalization.SetCurrentLanguageAll(langs[currentLangIndex]);

        UIManager.uiManager.ChangeLocal();
        Recipepanel.recipepanel.FillRecipe();
    }
    public void BackLang()
    {
        currentLangIndex -= 1;

        if (currentLangIndex < 0)
            currentLangIndex = 0;

        langText.text = langs[currentLangIndex];
        LeanLocalization.SetCurrentLanguageAll(langs[currentLangIndex]);

        UIManager.uiManager.ChangeLocal();
        Recipepanel.recipepanel.FillRecipe();
    }
}