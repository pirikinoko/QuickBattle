using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    //BGM切り替え用スクリプト

    AudioSource audioSource;
    public AudioClip[] music;
    public static bool[] musicTrigger;

    // Start is called before the first frame update
    void Start()
    {
        musicTrigger = new bool[music.Length];
        for (int i = 0; i < music.Length; i++)
        {
            musicTrigger[i] = false;
        }
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < music.Length; i++)　//BGM切り替え
        {
            if (musicTrigger[i])
            {
                audioSource.Stop();
                audioSource.PlayOneShot(music[i]);
                musicTrigger[i] = false;
            }
        }
    }
}
