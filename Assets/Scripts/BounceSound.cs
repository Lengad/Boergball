using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BounceSound : MonoBehaviour
{
    AudioSource audioData;

    private void Start()
    {
        audioData = GetComponent<AudioSource>();
    }
    private void OnCollisionEnter(Collision collision)
    {

        audioData.Play(0);
    }
}
