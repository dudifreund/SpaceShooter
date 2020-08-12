using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    [HideInInspector]
    public AudioSource source;

    public AudioClip clip;
    public bool loop = false;
}
