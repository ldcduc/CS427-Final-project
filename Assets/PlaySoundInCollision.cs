using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundInCollision : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioSource audio;
    private Animation anim;
    void Start()
    {
        audio = GetComponent<AudioSource>(); 
        anim = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        audio.Play();
        anim.Play();
    }
}
