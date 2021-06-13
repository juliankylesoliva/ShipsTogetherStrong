using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeSoundScript : MonoBehaviour
{
    private AudioSource soundPlayer;
    public AudioClip[] soundEffects;
    public int soundType = 0;
    public float explosionTime = 1.0f;

    void Start()
    {
        soundPlayer = this.gameObject.GetComponent<AudioSource>();
    }

    public void DoExplosion(int type = 0, float time = 1.0f)
    {
        StartCoroutine(ExplodeTimer(type, time));
    }

    private IEnumerator ExplodeTimer(int type, float time)
    {
        if (soundPlayer == null)
        {
            soundPlayer = this.gameObject.GetComponent<AudioSource>();
        }

        soundPlayer.clip = soundEffects[type];
        soundPlayer.Play();
        yield return new WaitForSeconds(time);
        GameObject.Destroy(this.gameObject);
    }
}
