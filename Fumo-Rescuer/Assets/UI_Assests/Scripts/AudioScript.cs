using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{
    public bool preventDestroyOnLoad = false;
    public bool preventDuplicatedAudio = true;

    private AudioSource m_AudioSource;

    private void Start()
    {

    }

    private void Awake()
    {
        if (preventDuplicatedAudio)
        {
            Object[] gameObjects = GameObject.FindObjectsOfType(typeof(AudioScript));
            foreach (Object obj in gameObjects)
            {
                if (obj != this) return;
            }
        }

        if (preventDestroyOnLoad) DontDestroyOnLoad(this.gameObject);
        m_AudioSource = GetComponent<AudioSource>();
        PlayMusic();
    }

    public void PlayMusic()
    {
        if (!m_AudioSource || m_AudioSource.isPlaying) return;

        m_AudioSource.Play();
    }

    public void StopMusic()
    { 
        m_AudioSource.Stop();
        Destroy(gameObject);
    }
}
