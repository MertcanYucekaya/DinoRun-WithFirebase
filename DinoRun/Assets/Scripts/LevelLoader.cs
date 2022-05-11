using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class LevelLoader : MonoBehaviour
{
    private bool active = true;
    public Slider slider;
    public AudioSource music;

    public Sprite[] sprites;
    public GameObject dinoSprite;
    

    private void Start()
    {
        dinoChangeSprite();
        music.volume = PlayerPrefs.GetFloat("volume");
        slider.value= PlayerPrefs.GetFloat("volume");
        
        //if (PlayerPrefs.GetInt("lastScore") > PlayerPrefs.GetInt("highScore"))
       // {
            //PlayerPrefs.SetInt("highScore", PlayerPrefs.GetInt("lastScore"));
       // }
        //highScoreText.text = PlayerPrefs.GetInt("highScore").ToString();

    }
    public void Music()
    {
        active = !active;

        if (active) { music.Play(); }
        else { music.Stop(); }
    }
    public void LoadScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void dinoChangeSprite()
    {
        dinoSprite.GetComponent<Image>().sprite = sprites[PlayerPrefs.GetInt("DinoSkin")];
        
    }
    public void AdjustVolume(float vol)
    {
        music.volume = vol;
        PlayerPrefs.SetFloat("volume", vol);
    }
    public void exitButton()
    {
        Application.Quit();
    }
}
