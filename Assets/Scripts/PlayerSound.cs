using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] public AudioClip jumpSound;
    [SerializeField] public AudioClip hurtSound;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayJumpSound()
    {
        audioSource.PlayOneShot(jumpSound, .5f);
    }
    public void PlayHurtSound()
    {
        audioSource.PlayOneShot(hurtSound, .5f);
    }
}
