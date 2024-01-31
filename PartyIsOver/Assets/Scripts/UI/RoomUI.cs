using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class RoomUI : MonoBehaviour
{
    string _roomName = "[4]Room";
    string _arenaName = "[5]Arena";

    public Text PlayerNameText;

    public int PlayerReadyCount = 1;

    public bool SkillChange = false;
    public Image SkillChangeButton;
    public Sprite Skill1;
    public Sprite Skill2;
    public Text SkillName;

    public GameObject PlayButton;
    public Sprite GamePlayOff;
    public Sprite GamePlayOn;
    public bool CanPlay;

    public GameObject ReadyButton;
    public Sprite ReadyOff;
    public Sprite ReadyOn;
    public bool Ready;

    public GameObject SpawnPoint;
    public int PlayerCount;
    public int ActorNumber;


    public delegate void ChangeSkillEvent(bool isChange);
    public event ChangeSkillEvent OnChangeSkiilEvent;
    public delegate void LeaveRoom();
    public event LeaveRoom OnLeaveRoom;
    public delegate void ReadyEvent(bool isReady);
    public event ReadyEvent OnReadyEvent;


    void Start()
    {
        PlayerNameText.text = PhotonNetwork.NickName;
        Ready = false;

        Managers.Input.KeyboardAction -= OnKeyboardEvent;
        Managers.Input.KeyboardAction += OnKeyboardEvent;

        SpawnPoint = GameObject.Find("Spawn Point");
        PlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        ActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        for (int i = 0; i < PlayerCount; i++)
        {
            SpawnPoint.transform.GetChild(i).gameObject.SetActive(true);
        }
    }


    void OnKeyboardEvent(Define.KeyboardEvent evt)
    {
        switch (evt)
        {
            case Define.KeyboardEvent.Click:
                {
                    if (Input.GetKeyDown(KeyCode.F5))
                    {
                        OnClickReady();
                    }

                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        OnClickLeaveRoom();
                    }
                }
                break;
            case Define.KeyboardEvent.PointerDown:
                {
                    if (Input.GetKeyDown(KeyCode.Tab))
                    {
                        OnClickSkillChange();
                    }
                }
                break;
        }
    }

    public void ChangeMasterButton(bool canPlay)
    {
        if (canPlay)
            PlayButton.GetComponentInChildren<Image>().sprite = GamePlayOn;
        else
            PlayButton.GetComponentInChildren<Image>().sprite = GamePlayOff;
    }

    public void UpdatePlayerNumber(int totalPlayerNumber)
    {
        for (int i = 0; i < totalPlayerNumber; i++)
        {
            SpawnPoint.transform.GetChild(i).gameObject.SetActive(true);
        }

        PlayerCount = totalPlayerNumber;
    }

   
    public void OnClickReady()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if (CanPlay)
            {
                OnLeaveRoom();
                PhotonNetwork.LoadLevel(_arenaName);
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
        else
        {
            Ready = !Ready;
            OnReadyEvent(Ready);

            if (Ready == false)
            {
                AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-160");
                Managers.Sound.Play(uiSound, Define.Sound.UISound);

                ReadyButton.GetComponent<Image>().sprite = ReadyOff;
                ReadyButton.GetComponentInChildren<Text>().text = "�غ�! (F5)";

            }
            else
            {
                AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-160");
                Managers.Sound.Play(uiSound, Define.Sound.UISound);

                ReadyButton.GetComponent<Image>().sprite = ReadyOn;
                ReadyButton.GetComponentInChildren<Text>().text = "�غ�����! (F5)";
            }
        }
    }

    public void UnsubscribeKeyboardEvent()
    {
        Managers.Input.KeyboardAction -= OnKeyboardEvent;
    }


    public void OnClickLeaveRoom()
    {
        if (SceneManager.GetActiveScene().name != _arenaName)
        {
            PhotonManager.Instance.LeaveRoom();
            ChangeBgmSound("Sounds/Bgm/BongoBoogieMenuLOOPING");
        }
    }

    public void OnClickSkillChange()
    {
        if (SceneManager.GetActiveScene().name != _roomName)
        {
            Managers.Input.KeyboardAction -= OnKeyboardEvent;
            return;
        }

        SkillChange = !SkillChange;
        OnChangeSkiilEvent(SkillChange);
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-050");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        if (SkillChange)
        {
            SkillChangeButton.sprite = Skill1;
            SkillName.text = "��¡ ��ų\n\n\n\n�ɳ���ġ";
        }
        else
        {
            SkillChangeButton.sprite = Skill2;
            SkillName.text = "��¡ ��ų\n\n\n\n����ġ";
        }
    }

    void ChangeBgmSound(string path)
    {
        AudioSource endingBgm = Managers.Sound.GetBgmAudioSource();
        AudioClip audioClip = Managers.Resource.Load<AudioClip>(path);
        endingBgm.clip = audioClip;
        endingBgm.volume = Managers.Sound.SoundVolume[(int)Define.Sound.Bgm];
        Managers.Sound.Play(audioClip, Define.Sound.Bgm);
    }
}
