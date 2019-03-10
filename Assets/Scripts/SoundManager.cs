using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public AudioSource lightningSound;
    public AudioSource rainSound;
    public AudioSource birdSound;
    public AudioSource explosionSound;
    public AudioSource launchSound;
    public AudioSource fizzleSound;
    public AudioSource fizzlefinishSound;

    public AudioSource[] missSounds;
    public AudioSource[] critSounds;
    public AudioSource[] killSounds;

    // Use this for initialization
    void Start() {

    }

    public IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }
        audioSource.Stop();
    }

    public void PlayOneFromList(AudioSource[] aList)
    {
        int rand = Random.Range(0, aList.Length);
        aList[rand].Play();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
