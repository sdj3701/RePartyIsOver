using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.SceneManagement;
 
public class SoundManager 
{
    string _launcher = "[1]Launcher";

    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.Maxcount];
    //ĳ�� ����
    Dictionary<string,AudioClip> _audioClip = new Dictionary<string, AudioClip>();


    public float[] SoundVolume = new float[(int)Define.Sound.Maxcount];


    //���� ������Ʈ ����
    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");

        SceneManagerEx sceneManagerEx = new SceneManagerEx();
        string currentSceneName = sceneManagerEx.GetCurrentSceneName();
        AudioClip audioClip = null;

        if (root == null) 
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
            //�ƽ� ī��Ʈ��� �ְ� ������ ����
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }
            _audioSources[(int)Define.Sound.Bgm].loop = true;
            _audioSources[(int)Define.Sound.InGameStackSound].loop = true;


            SoundVolume[(int)Define.Sound.Bgm] = 0.1f;
            SoundVolume[(int)Define.Sound.UISound] = 1f;
        }

        if (currentSceneName == _launcher)
        {
            audioClip = Managers.Resource.Load<AudioClip>("Sounds/Bgm/BongoBoogieMenuLOOPING");
            _audioSources[(int)Define.Sound.Bgm].clip = audioClip;
            _audioSources[(int)Define.Sound.Bgm].volume = 0.1f;
            Managers.Sound.Play(audioClip, Define.Sound.Bgm);
        }
    }
    public AudioSource GetBgmAudioSource()
    {
        return _audioSources[(int)Define.Sound.Bgm];
    }

    public void Clear()
    {
        foreach(AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClip.Clear();
    }

    public void Play( string path, Define.Sound type = Define.Sound.PlayerEffect, float pitch =1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, null, pitch);
    }
    public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.PlayerEffect, AudioSource playaudioSource = null, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        if (type == Define.Sound.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];

            if (audioSource.isPlaying)
                audioSource.Stop();
            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else if(type == Define.Sound.UISound)
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.UISound];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
        else if (type == Define.Sound.UIInGameSound)
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.UIInGameSound];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
        else if((type == Define.Sound.PlayerEffect))
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.PlayerEffect];
            audioSource.pitch = pitch;
            audioSource.volume = 0.2f;
            playaudioSource.PlayOneShot(audioClip);
        }
        else if(type == Define.Sound.InGameStackSound)
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.InGameStackSound];

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.volume = 0.4f;
            audioSource.Play();
        }
        else if(type == Define.Sound.MagneticWarning)
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.MagneticWarning];
            audioSource.pitch = pitch;
            audioSource.volume = 0.3f;
            audioSource.PlayOneShot(audioClip);
        }
        else if (type == Define.Sound.SnowStormWarning)
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.SnowStormWarning];
            audioSource.pitch = pitch;
            audioSource.volume = 0.3f;
            audioSource.PlayOneShot(audioClip);
        }
    }

    //��� ���� ����� ������ �ߺ��Ǹ� Resource�� ã�� �ʰ� ĳ���� �Ͽ� ����Ͽ� �� ������ ����Ѵ�.
    public AudioClip GetOrAddAudioClip(string path , Define.Sound type = Define.Sound.PlayerEffect)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";

        AudioClip audioClip = null;

        if (type == Define.Sound.Bgm)
        {
            //.Bgm�� ������ ���� ����
            audioClip = Managers.Resource.Load<AudioClip>(path);
        }
        else
        {
            if (_audioClip.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Managers.Resource.Load<AudioClip>(path);
                _audioClip.Add(path, audioClip);
            }
        }
        

        if (audioClip == null)
            Debug.Log($"AudioClip Missing : {path}");
        
        return audioClip;
    }


    public void ChangeVolume(Define.Sound sound1, Define.Sound sound2)
    {
        _audioSources[(int)sound1].volume = SoundVolume[(int)sound1];
        _audioSources[(int)sound2].volume = SoundVolume[(int)sound2];
    }
}
