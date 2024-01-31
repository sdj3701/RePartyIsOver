using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;


public class ScoreBoardUI : MonoBehaviour
{
    public GameObject Info;
    public Sprite PointStar;
    public GameObject Tab;

    private GameObject _scoreBoardPanel;
    private GameObject[] _score = new GameObject[6];
    private Text[] _nickName = new Text[6];
    private int _playerNumber;
    
    public void InitScoreBoard()
    {
        _scoreBoardPanel = GameObject.Find("ScoreBoard Panel");
        _scoreBoardPanel.SetActive(false);

        _playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    public void SetScoreBoard()
    {
        Managers.Input.KeyboardAction -= OnKeyboardEvents;
        Managers.Input.KeyboardAction += OnKeyboardEvents;

        for (int i = 0; i < _playerNumber; i++)
        {
            Info.transform.GetChild(i).gameObject.SetActive(true);
            _score[i] = Info.transform.GetChild(i).GetChild(1).gameObject;
            _nickName[i] = Info.transform.GetChild(i).GetChild(2).GetComponent<Text>();
        }
    }


    public void DisplayFixedScoreBoard()
    {
        Managers.Input.KeyboardAction -= OnKeyboardEvents;
        _scoreBoardPanel.SetActive(true);
    }

    public void ChangeScoreBoard(int[] score, string[] name, int[] rank)
    {
        _playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;

        for (int i = 0; i < _playerNumber; i++)
        {
            Info.transform.GetChild(i).gameObject.SetActive(true);
            _score[i] = Info.transform.GetChild(i).GetChild(1).gameObject;
            _nickName[i] = Info.transform.GetChild(i).GetChild(2).GetComponent<Text>();
        }


        for (int i = 0; i < _playerNumber; i++)
        {
            for (int j = 0; j < score[i]; j++)
            {
                _score[i].transform.GetChild(j).GetComponent<Image>().sprite = PointStar;
            }
        }

        for (int i = 0; i < _playerNumber; i++)
        {
            _nickName[i].text = name[i];
        }
    }


    void OnKeyboardEvents(Define.KeyboardEvent evt)
    {
        switch (evt)
        {
            case Define.KeyboardEvent.Press:
                {
                    if (Input.GetKeyDown(KeyCode.Tab))
                    {
                        SetScoreBoardActive();
                        Tab.SetActive(false);
                    }
                }
                break;

            case Define.KeyboardEvent.Click:
                {
                    if (Input.GetKeyUp(KeyCode.Tab))
                    {
                        SetScoreBoardInactive();
                        Tab.SetActive(true);
                    }
                }
                break;
        }
    }

    void SetScoreBoardActive()
    {
        _scoreBoardPanel.SetActive(true);
    }
    void SetScoreBoardInactive()
    {
        _scoreBoardPanel.SetActive(false);
    }
}