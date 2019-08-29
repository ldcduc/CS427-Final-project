using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundInCollision : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioSource audio;
    void Start()
    {
        audio = GetComponent<AudioSource>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        audio.Play();
    }
}
