using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    Slider volumeSlider;
    public int volumeType;

    public bool isGameSetting = true;
    private AudioSource BGM;
    private AudioSource[] SFXs;

    void Start()
    {
        float[] savedVolumes =
        {
            PlayerPrefs.GetFloat("BGM_Volume", 1.0f),
            PlayerPrefs.GetFloat("SFX_Volume", 1.0f)
        };

        if (isGameSetting)
        {
            volumeSlider = GetComponent<Slider>();
            volumeSlider.value = savedVolumes[volumeType];
        }
        else
        {
            GameObject bgmTag = GameObject.FindGameObjectWithTag("BGM");
            BGM = bgmTag.GetComponent<AudioSource>();
            if (BGM) BGM.volume = savedVolumes[0];
        }
    }

    
    public void SetVolume()
    {
        if (isGameSetting)
        {
            if (volumeType == 0)
            {
                PlayerPrefs.SetFloat("BGM_Volume", volumeSlider.value);
                GameObject bgmObj = GameObject.FindGameObjectWithTag("BGM");
                bgmObj.GetComponent<AudioSource>().volume = volumeSlider.value;
            }
            else PlayerPrefs.SetFloat("SFX_Volume", volumeSlider.value);
        }
    }
}
