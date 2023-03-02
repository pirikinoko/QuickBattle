using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip[] SE;
    public static bool[] SETrigger;
    float coolTime;
    bool ready = true;
    // Start is called before the first frame update
    void Start()
    {
        coolTime = 0.1f;
        SETrigger = new bool[SE.Length];
        for (int i = 0; i < SE.Length; i++)
        {
            SETrigger[i] = false;
        }
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < SE.Length; i++)
        {
            if (SETrigger[i] && ready)
            {
                audioSource.PlayOneShot(SE[i]);
                SETrigger[i] = false;
                ready = false;
            }
            else  //�N�[���^�C�����Ȃ�SE�L�����Z��
            {
                SETrigger[i] = false;
            }
        }

        if (!(ready))
        {
            coolTime -= Time.deltaTime;
            if (coolTime < 0)
            {
                coolTime = 0.1f;
                ready = true;
            }
        }
    }
}