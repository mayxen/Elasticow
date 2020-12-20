using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class SystemController : MonoBehaviour
{

    [Header("Menu Prefabs")]
    [SerializeField] GameObject menu = null;
    [SerializeField] GameObject lvlSelect = null;
    [SerializeField] GameObject settings = null;

    [Header("Lvl Prefabs")]
    [SerializeField] GameObject LvlBtn = null;
    [SerializeField] Transform BtnPanel = null;
    [SerializeField] int totalLevels = 24;


    [Header("Lvl Prefabs")]
    [SerializeField] Animator animator = null;
    [SerializeField] GameObject nextLvl = null;
    [SerializeField] int nextScene = 0;

    [Header("Settings")]
    [SerializeField] Slider sliderVolumen = null;
    [SerializeField] SoundList SoundList = null;
    [SerializeField] AudioSource audioSource = null;

    [Header("Animations")]
    [SerializeField] GameObject animationLvl0 = null;
    [SerializeField] GameObject animationEnd = null;

    public static SystemController instance;
    public enum ControllerType { menu, lvl, end };
    public ControllerType controllerType;
    List<Switch> switches = new List<Switch>();
    int lvlsCompleted = 0;


    private void Awake()
    {
        SetUpSingleton();
        SetUpController();
    }
    private void SetUpSettings()
    {
        SoundList.VolumeMusic = PlayerPrefs.GetFloat("volumeSet", 0.5f);

        SetUpAudioSource();
        SetUpAudioInterface();

    }

    private void SetUpAudioInterface()
    {
        sliderVolumen.value = SoundList.VolumeMusic * 100f;
    }

    private void SetUpAudioSource()
    {
        audioSource.Play();
    }
    public void PlayDoorSound(AudioSource audioSource)
    {
        audioSource.clip = SoundList.DoorSound;
        audioSource.volume = SoundList.VolumeMusic;
        audioSource.loop = false;
        audioSource.Play();
    }

    public void PlayMooSound(AudioSource audioSource)
    {
        audioSource.clip = SoundList.MooSound2;
        audioSource.volume = SoundList.VolumeMusic;
        audioSource.loop = false;
        audioSource.Play();
    }

    public void ResetSwitches(AudioSource audioSource)
    {
        foreach (Switch switchInstace in switches) {
            switchInstace.SetSwitchOff();
        }
        FindObjectOfType<Door>().ResetDoor();
        audioSource.clip = SoundList.MooSound3;
        audioSource.volume = SoundList.VolumeMusic;
        audioSource.loop = false;
        audioSource.Play();
    }
    public void PlaySwitchSound(AudioSource audioSource)
    {
        audioSource.clip = SoundList.SwitchSound;
        audioSource.volume = SoundList.VolumeMusic;
        audioSource.loop = false;
        audioSource.Play();
    }

    void PlayLobbyMusic()
    {
        audioSource.clip = SoundList.AmbientMusicLobby;
        audioSource.volume = SoundList.VolumeMusic;
        audioSource.loop = true;
        audioSource.Play();
    }

    void PlayGameMusic()
    {
        audioSource.volume = SoundList.VolumeMusic;
        audioSource.loop = true;
        audioSource.clip = SoundList.AmbientMusicGame;
        audioSource.Play();
    }

    public void PlayIntroMusic()
    {
        audioSource.volume = SoundList.VolumeMusic;
        audioSource.clip = SoundList.AmbientIntro;
        audioSource.loop = true;
        audioSource.Play();
    }
    public void PlayEndMusic()
    {
        audioSource.volume = SoundList.VolumeMusic;
        audioSource.loop = true;
        audioSource.clip = SoundList.AmbientEnd;
        audioSource.Play();
    }
    public bool CheckSwitches()
    {
        foreach (Switch switchInstace in switches) {
            if (!switchInstace.Active)
                return true;
        }
        return false;
    }
    public void ChangeScene(int level = -1)
    {
        if (level == -1) {
            TogglePause();
            if (nextScene == -2) {
                SceneManager.LoadScene("End");
                return;
            }
            level = nextScene;
            if (lvlsCompleted < nextScene)
                PlayerPrefs.SetInt("lvlsCompleted", nextScene);
        }
        if (level == 0) {
            animationLvl0.SetActive(true);
            return;
        }

        SceneManager.LoadScene("Lvl_" + level);
    }
    public void ChangeToMenuScene()
    {
        TogglePause();
        SceneManager.LoadScene("Menu");
    }
    public void TogglePause()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }
    public void ToggleLvlSelect()
    {
        lvlSelect.SetActive(!lvlSelect.activeSelf);
        menu.SetActive(!menu.activeSelf);
    }
    public void ToggleSettings()
    {
        TogglePause();
        settings.SetActive(!settings.activeSelf);
    }
    public void ToggleNextLvl(AudioSource audioSource)
    {
        TogglePause();
        nextLvl.SetActive(!nextLvl.activeSelf);
        audioSource.clip = SoundList.MooSound1;
        audioSource.volume = SoundList.VolumeMusic;
        audioSource.loop = false;
        audioSource.Play();
    }
    public void SetVolume()
    {
        SoundList.VolumeMusic = sliderVolumen.value / 100f;
        audioSource.volume = SoundList.VolumeMusic;
        PlayerPrefs.SetFloat("volumeSet", SoundList.VolumeMusic);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    private void SetUpSingleton()
    {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }
    }
    private void GenerateLvlBtns()
    {
        lvlsCompleted = PlayerPrefs.GetInt("lvlsCompleted", 0);
        for (int i = 0; i < totalLevels; i++) {
            GameObject lvlInstance = Instantiate(LvlBtn);
            lvlInstance.transform.SetParent(BtnPanel);
            if (i <= lvlsCompleted) {
                lvlInstance.GetComponent<LvlSelectButton>().SetUpUnlockedLvl(i);
            } else {
                lvlInstance.GetComponent<LvlSelectButton>().SetUpLockedLvl(i);
            }
        }
    }
    private void SetUpController()
    {
        switch (controllerType) {
            case ControllerType.lvl:
            GetSwiches();
            SetUpAudioInterface();
            PlayGameMusic();
            break;
            case ControllerType.menu:
            SetUpSettings();
            PlayLobbyMusic();
            GenerateLvlBtns();
            break;
            case ControllerType.end:
            SetUpAudioSource();
            break;
        }
    }
    private void GetSwiches()
    {
        Switch[] switches = FindObjectsOfType<Switch>();

        foreach (Switch switchInstace in switches) {
            switchInstace.SetSwitchOff();
            this.switches.Add(switchInstace);
        }
    }
}
