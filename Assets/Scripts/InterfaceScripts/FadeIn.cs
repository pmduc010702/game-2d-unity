using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    [Header("FadeIn Image")]
    public GameObject blackOutScreen;
    public void Awake()
    {
        StartCoroutine(StartTransition());
    }

    IEnumerator StartTransition(bool fadeToBlack = true, float fadeSpeed = 0.5f)
    {

        Color objectColor = blackOutScreen.GetComponent<Image>().color;
        float fadeAmount;

        while (blackOutScreen.GetComponent<Image>().color.a > 0)
        {

            fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            blackOutScreen.GetComponent<Image>().color = objectColor;
            yield return null;
        }

    }
}
