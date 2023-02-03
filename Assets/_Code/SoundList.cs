using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct NamedAudioClip
{
    public string name;
    public AudioClip soundclip;
}

public class SoundList : MonoBehaviour
{


    private static SoundList instance;
    

    private void Awake()
    {
        if (null != instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        InitializeSoundList();
    }

    private void InitializeSoundList()
    {
        foreach (var VARIABLE in SoundInitializerList)
        {
            NamedAudioClip b = new NamedAudioClip();
            b.name = VARIABLE.name;
            b.soundclip = VARIABLE;
            Debug.Log("Initialized new sound with name + " + VARIABLE.name.ToString());
        }
    }

    [SerializeField] private List<NamedAudioClip> _genericAudioDatabase;
    [SerializeField] private List<AudioClip> SoundInitializerList;
    [SerializeField] private List<AudioClip> _footsteps;





    public static AudioClip GetSound(string soundName)
    {

        return instance._genericAudioDatabase.Where(i => i.name == soundName).FirstOrDefault().soundclip;
    }
    public static AudioClip GetRandomFootstep()
    {
        
        return instance._footsteps[UnityEngine.Random.Range(0, instance._footsteps.Count)];
    }
}



