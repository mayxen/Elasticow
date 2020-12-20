using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundList")]
public class SoundList : ScriptableObject
{
    public AudioClip AmbientMusicGame;
    public AudioClip AmbientMusicLobby;
    public AudioClip AmbientIntro;
    public AudioClip AmbientEnd;
    public AudioClip ClickSound;
    public AudioClip MooSound1;
    public AudioClip MooSound2;
    public AudioClip MooSound3;
    public AudioClip DoorSound;
    public AudioClip SwitchSound;

    public float VolumeMusic;
    public bool Music;
    public bool Effects;
}
