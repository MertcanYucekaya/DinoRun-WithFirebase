using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinoSelect : MonoBehaviour
{
    LevelLoader levelLoader;
    private void Start()
    {
        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        
    }
    public void SelectDino(int id)
    {
        PlayerPrefs.SetInt("DinoSkin", id);
        //Debug.Log(PlayerPrefs.GetInt("DinoSkin") + "    " + id);
        levelLoader.dinoChangeSprite();
    }
}
