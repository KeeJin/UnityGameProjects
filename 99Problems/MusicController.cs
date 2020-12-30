using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
 // Controller class for Pre-game Music
 public class MusicController : MonoBehaviour
 {
     public AudioSource _audioSource;
     private void Awake()
     {
         DontDestroyOnLoad(transform.gameObject); // Ensures that background music is able to continue playing even when the scene changes
         _audioSource = GetComponent<AudioSource>();
     }
 
     public void PlayMusic()
     {
         if (_audioSource.isPlaying) return;
         _audioSource.Play();
     }
 
     public void StopMusic()
     {
         _audioSource.Stop();
     }
 }
