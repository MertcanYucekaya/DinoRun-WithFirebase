using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour
{
    public Slider musicSlider;
    private void Awake()
    {
        musicSlider.value = PlayerPrefs.GetFloat("volume", 1f);
    }
    public void SetVolume(float val)
    {
        PlayerPrefs.SetFloat("volume", val);
    }
}
