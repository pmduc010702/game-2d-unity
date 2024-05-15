using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutSound : MonoBehaviour
{
    public int secondsToFadeOut = 7;
    void Start()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        AudioSource audioMusic = GetComponent<AudioSource>();

        while(audioMusic.volume > 0.01f)
        {
            audioMusic.volume -= Time.deltaTime / secondsToFadeOut;
            yield return null;
        }

        audioMusic.volume = 0;
        audioMusic.Stop();
    }
}
