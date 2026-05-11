using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    Animator animator;
    AudioSource audioData;

    public AudioClip sonOuverture;
    public AudioClip sonFermeture;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioData = GetComponent<AudioSource>();

        if (animator == null)
        {
            Debug.LogWarning("Attention : Pas d'Animator sur " + gameObject.name + ". Le script de porte est désactivé pour éviter les plantages.");
            enabled = false; 
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            if (animator != null) animator.SetBool("In", true);

            if (audioData != null && sonOuverture != null)
            {
                audioData.PlayOneShot(sonOuverture);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (animator != null) animator.SetBool("In", false);

            if (audioData != null && sonFermeture != null)
            {
                audioData.PlayOneShot(sonFermeture);
            }
        }
    }
}

