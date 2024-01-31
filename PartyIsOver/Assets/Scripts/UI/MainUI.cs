using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;



public class MainUI : MonoBehaviour
{
    public GameObject StartObject;
    public GameObject CancelPanel;
    public GameObject LoadingPanel;
    public GameObject StoryBoardPanel;
    public GameObject StoryEndingPanel;
    public GameObject SettingsPanel;
    public GameObject CreditPanel;

    public Text NickName;
    public Animator Animator;
    public Image LoadingBar;

    public Slider BGMAudioSlider;
    public Slider EffectAudioSlider;


    private bool _gameStartFlag;
    private bool _loadingFlag;
    private bool _loadingDelayFlag;
    private bool _keyBoardOn;
    private bool _creditOn;
    private bool _storyBoardOn;

    private float _angle = 0f;
    private float _delayTime = 0.0f;
    private float _loadingTime = 2f;
    
    private int _clickedNumber = 0;
    private string[] _savedStory = new string[8];

    private Transform _storyBoard;
    private Text _storyText;

    private GameObject _mainObject;
    private GameObject _keyBoardObject;

    private Button _prevButton;
    private Button _nextButton;
    public Sprite NextButtonImage;
    public Sprite HomeButtonImage;


    private void Start()
    {
        CancelPanel.SetActive(false);
        LoadingPanel.SetActive(false);
        StoryBoardPanel.SetActive(false);
        StoryEndingPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        CreditPanel.SetActive(false);

        NickName.text = PhotonNetwork.NickName;

        _mainObject = GameObject.Find("Main Object");
        _keyBoardObject = SettingsPanel.transform.GetChild(5).GetChild(1).gameObject;
        _keyBoardObject.SetActive(false);


        _storyBoard = StoryBoardPanel.transform.GetChild(0);
        _storyText = _storyBoard.GetChild(10).GetComponentInChildren<Text>();

        _savedStory[0] = "�� ���� ���� �������� ���� <����Ʈ>���� ��Ƽ�� ���Ƚ��ϴ�!\n���� ��ΰ� �԰� ���ð� ��ſ� ���̳׿�.";
        _savedStory[1] = "�׷��� �� �ϳ��� �ذ� ������.\n��Ƽ�� ��� ���͵��� �ῡ ������ϴ�.";
        _savedStory[2] = "������ ��ħ, ���͵��� ���� ������ ���ư����� �մϴ�.";
        _savedStory[3] = "������, �Ұ����ϴٴ� ���� �� �˰� �Ǿ���.";
        _savedStory[4] = "������ ���� <����Ʈ>�� ���� ���� ���͵��� ���� ���� ������ ��ٸ��� �� ��ٷȽ��ϴ�.";
        _savedStory[5] = "������ �������� ���� ���� �ʾҰ�.\n< ����Ʈ > �� ���ĵ� ���Ǿ� ���� �����߽��ϴ�.";
        _savedStory[6] = "���ָ��� ��ģ ���͵��� ���� ���� \n���� ��Ƽ ������ ������ ������ ���ϱ�� �Ͽ����ϴ�.";
        _savedStory[7] = "������ �¸� ��Ģ�� �� �ϳ�, �������� ��Ƴ��� ���Ͱ� �� ��!\n��Ƽ ������ ������ ���ϱ� ���� ��ġ �ο��� ���� ���۵˴ϴ�!";


        _nextButton = _storyBoard.GetChild(11).GetComponent<Button>();
        _prevButton = _storyBoard.GetChild(12).GetComponent<Button>();


        Managers.Input.KeyboardAction -= OnKeyboardEvent;
        Managers.Input.KeyboardAction += OnKeyboardEvent;
    }

    void OnKeyboardEvent(Define.KeyboardEvent evt)
    {
        switch (evt)
        {
            case Define.KeyboardEvent.Click:
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        if (_creditOn)
                        {
                            AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
                            Managers.Sound.Play(uiSound, Define.Sound.UISound);

                            OnClickCreditExit();
                            _creditOn = false;

                            ChangeBgmSound("Sounds/Bgm/BongoBoogieMenuLOOPING");
                        }

                        if (_keyBoardOn)
                        {
                            AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
                            Managers.Sound.Play(uiSound, Define.Sound.UISound);

                            _keyBoardObject.SetActive(false);
                            _keyBoardOn = false;
                        }

                        if(_storyBoardOn)
                        {
                            AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
                            Managers.Sound.Play(uiSound, Define.Sound.UISound);

                            _clickedNumber = 0;
                            StoryBoardPanel.SetActive(false);
                            _storyBoardOn = false;

                            ChangeBgmSound("Sounds/Bgm/BongoBoogieMenuLOOPING");
                        }
                    }
                }
                break;
        }
    }


    void Update()
    {
        if(_gameStartFlag)
        {
            _angle -= 90f * Time.deltaTime;
            StartObject.transform.rotation = Quaternion.Euler(_angle, 180, 0);

            if (_angle <= -90)
            {
                _gameStartFlag = false;
                _loadingFlag = true;
            }
        }

        if (_loadingFlag)
            DelayTime();
    }

    void DelayTime()
    {
        _delayTime += Time.deltaTime;

        if (_delayTime >= 1 && _loadingDelayFlag == false)
        {
            LoadingPanel.SetActive(true);
            _loadingDelayFlag = true;
        }

        LoadingBar.fillAmount = (_delayTime - 1) / _loadingTime;

        if ((_delayTime - 1) >= _loadingTime)
        {
            _delayTime = 0;
            _gameStartFlag = false;
            _loadingFlag = false;

            PhotonManager.Instance.Connect();
            PhotonManager.Instance.LoadNextScene("[3]Lobby");
            SceneManager.LoadSceneAsync("[3]Lobby");

            Managers.Input.KeyboardAction -= OnKeyboardEvent;
        }
    }

    public void OnClickStart()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-160");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        _gameStartFlag = true;
        Animator.SetBool("Pose", true);
    }

    #region Game Quit
    public void OnClickPopup()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        CancelPanel.SetActive(true);
    }

    public void OnClickPopUpCancel()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        CancelPanel.SetActive(false);
    }

    public void OnClickPopUpGameQuit()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    #endregion

    #region Settings
    public void OnClickSettings()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        SettingsPanel.SetActive(true);
    }
    
    public void OnClickSettingsOK()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        Managers.Sound.SoundVolume[(int)Define.Sound.Bgm] = 0.1f * BGMAudioSlider.value;
        Managers.Sound.SoundVolume[(int)Define.Sound.UISound] = 1f * EffectAudioSlider.value;
        Managers.Sound.ChangeVolume(Define.Sound.Bgm, Define.Sound.UISound);

        SettingsPanel.SetActive(false);
    }

    public void OnClickSettingsCancel()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);
        
        Managers.Sound.SoundVolume[(int)Define.Sound.Bgm] = 0.1f * 1;
        Managers.Sound.SoundVolume[(int)Define.Sound.UISound] = 1f * 1;
        Managers.Sound.ChangeVolume(Define.Sound.Bgm, Define.Sound.UISound);

        BGMAudioSlider.value = 1;
        EffectAudioSlider.value = 1;

        SettingsPanel.SetActive(false);
    }

    public void OnClickSettingsKeyboard()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        _keyBoardObject.SetActive(true);
        _keyBoardOn = true;
    }
    #endregion

    #region Credit
    public void OnClickCredit()
    {
        ChangeBgmSound("Sounds/Credit/Duck Funk Looping");

        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        _mainObject.SetActive(false);
        _creditOn = true;
        CreditPanel.SetActive(true);
    }

    public void OnClickCreditExit()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        _mainObject.SetActive(true);
        CreditPanel.SetActive(false);
    }
    #endregion

    // StoryBoard
    public void OnClickStoryBoard()
    {
        ChangeBgmSound("Sounds/StoryBoard Sound/Freaky Factory LOOPING");

        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        StoryBoardPanel.SetActive(true);
        _storyBoard.GetChild(0).gameObject.SetActive(true);
        _storyText.text = _savedStory[0];

        _clickedNumber = 0;
        _storyBoardOn = true;

        _nextButton.GetComponent<Image>().sprite = NextButtonImage;
        _prevButton.gameObject.SetActive(false);
    }

    public void OnClickStoryBoardNext()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);


        _clickedNumber++;
        _prevButton.gameObject.SetActive(true);


        if (_clickedNumber >= 8)
        {
            StoryBoardPanel.SetActive(false);

            ChangeBgmSound("Sounds/Bgm/BongoBoogieMenuLOOPING");
        }

        for (int i = 0; i < 8; i++)
        {
            if(i == _clickedNumber)
            {
                _storyBoard.GetChild(i).gameObject.SetActive(true);
                _storyText.text = _savedStory[i];
            }
            else
                _storyBoard.GetChild(i).gameObject.SetActive(false);
        }

        if (_clickedNumber == 7)
            _nextButton.GetComponent<Image>().sprite = HomeButtonImage;

    }

    public void OnClickStoryBoardPrev()
    {
        AudioClip uiSound = Managers.Sound.GetOrAddAudioClip("UI/Funny-UI-030");
        Managers.Sound.Play(uiSound, Define.Sound.UISound);

        _clickedNumber--;

        if (_clickedNumber <= 0)
        {
            _prevButton.gameObject.SetActive(false);
        }

        for (int i = 0; i < 8; i++)
        {
            if (i == _clickedNumber)
            {
                _storyBoard.GetChild(i).gameObject.SetActive(true);
                _storyText.text = _savedStory[i];
            }
            else
                _storyBoard.GetChild(i).gameObject.SetActive(false);
        }

        if(_clickedNumber < 7)
            _nextButton.GetComponent<Image>().sprite = NextButtonImage;

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
