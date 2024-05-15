using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private GameObject blackOutScreen;
    [SerializeField] private AudioSource mainMusic;
    [SerializeField] private GameObject body;

    private Rigidbody2D player;
    private RectTransform bar;
    private Image barImage;

    void Start()
    {
        player = body.GetComponent<Rigidbody2D>();
        bar = GetComponent<RectTransform>();
        barImage = GetComponent<Image>();
        if (Health.totalHealth < 0.3f)
        {
            barImage.color = Color.red;
        }
        SetSize(Health.totalHealth);
    }

    ///// ABSTRACTION - Health and damage functionality available
    /////               to other levels and objects
    public void Damage(float damage)
    {
        if((Health.totalHealth -= damage) >= 0f)
        {
            Health.totalHealth -= damage;
        }
        else if (Health.totalHealth < 0.3f)
        {
            barImage.color = Color.red;
        }

        if (Health.totalHealth <= 0f)
        {
            Health.totalHealth = 0f;
            // stop player movement
            
            StartCoroutine(PlayerDeath());
            Invoke("RestartLevel", 2.0f);          
        }

        SetSize(Health.totalHealth);
    }

    IEnumerator PlayerDeath(float fadeSpeed = 0.7f, int secondsToFadeOut = 3)
    {
        Color objectColor = blackOutScreen.GetComponent<Image>().color;
        float fadeAmount;
        // stop music from camera
        mainMusic = Camera.main.GetComponent<AudioSource>();
        while (mainMusic.volume > 0.01f)
        {
            mainMusic.volume -= Time.deltaTime / secondsToFadeOut;
            yield return null;
        }

        mainMusic.volume = 0;
        mainMusic.Stop();

        player.constraints = RigidbodyConstraints2D.FreezePosition;

        // play death sound
        while (blackOutScreen.GetComponent<Image>().color.a < 1)
        {
            fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            blackOutScreen.GetComponent<Image>().color = objectColor;
            yield return null;
        }
    }

    void RestartLevel ()
    {
        Debug.Log("Player Died");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Health.totalHealth = 1f; //reset health
    }

    public void SetSize(float size)
    {
        bar.localScale = new Vector3(size, 1f);
    }
}
