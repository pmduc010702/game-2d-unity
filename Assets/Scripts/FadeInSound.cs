using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInSound : MonoBehaviour
{
    public int secondsToFadeIn = 15;
    [SerializeField] private float MaxVol;
    void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        AudioSource audioMusic = GetComponent<AudioSource>();
        audioMusic.Play();

        while (audioMusic.volume < MaxVol)
        { 
            audioMusic.volume += Time.deltaTime / secondsToFadeIn;
            yield return null;
        }

    }

}
