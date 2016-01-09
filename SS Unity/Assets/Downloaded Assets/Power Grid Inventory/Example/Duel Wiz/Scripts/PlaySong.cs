using UnityEngine;
using System.Collections;

public class PlaySong : MonoBehaviour
{
    public AudioClip Music;
    AudioSource Bgm;


	// Use this for initialization
	void Awake()
    {
        Bgm = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
	}
	
	void OnEnable()
    {
        Bgm.Stop();
        Bgm.clip = Music;
        Bgm.Play();
    }
}
