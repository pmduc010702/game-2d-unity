using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    [SerializeField] private Image fadeImage = null;
    public void PlayGame()
    {
        StartCoroutine(StartTransition());
    }

    IEnumerator StartTransition(bool fadeToBlack = true, float fadeSpeed = 0.4f)
    {
        Color objectColor = fadeImage.color;
        float fadeAmount;

        while (fadeImage.color.a < 1)
        {
            fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            fadeImage.color = objectColor;
            yield return null;
        }
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
