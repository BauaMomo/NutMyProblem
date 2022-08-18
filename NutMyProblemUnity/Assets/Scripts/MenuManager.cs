using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class MenuManager : MonoBehaviour
{
    GameObject MainMenuPanel;
    GameObject SettingsMenuPanel;
    GameObject CreditsMenuPanel;
    GameObject PauseMenuPanel;
    GameObject LastMenuPanel;
    GameObject BetreuerScreen;
    GameObject EntwicklerScreen;
    [SerializeField] GameObject Background_Tree;
    [SerializeField] GameObject Background_Prince;

    Scene MainMenu;
    Scene SampleScene;

    bool gamePaused = false;

    List<GameObject> MenuList;

    TMPro.TMP_Dropdown resolutionDropdown;
    List<Vector2Int> resolutions = new List<Vector2Int> { new Vector2Int(1280, 720), new Vector2Int(1920, 1080), new Vector2Int(2560, 1440), new Vector2Int(3840, 2160) };

    Toggle fullscreenToggle;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        MainMenuPanel = transform.Find("MainMenu").gameObject;
        SettingsMenuPanel = transform.Find("SettingsMenu").gameObject;
        CreditsMenuPanel = transform.Find("Credits").gameObject;
        PauseMenuPanel = transform.Find("PauseMenu").gameObject;
        BetreuerScreen = transform.Find("Betreuer").gameObject;
        EntwicklerScreen = transform.Find("Entwickler").gameObject;
        LastMenuPanel = MainMenuPanel;

        MenuList = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            MenuList.Add(transform.GetChild(i).gameObject);
        }

        MainMenu = SceneManager.GetSceneByName("MainMenu");
        SampleScene = SceneManager.GetSceneByName("SampleScene");
        SceneManager.activeSceneChanged += OnSceneChange;

        foreach (GameObject menu in MenuList) menu.SetActive(false);
        if (SceneManager.GetActiveScene() == MainMenu) MainMenuPanel.SetActive(true);

        resolutionDropdown = transform.Find("SettingsMenu/ResolutionDropdown").GetComponent<TMPro.TMP_Dropdown>();
        resolutionDropdown.value = resolutions.FindIndex(x => x.x == Screen.width);

        fullscreenToggle = transform.Find("SettingsMenu/ToggleFullscreen").GetComponent<Toggle>();
        fullscreenToggle.isOn = Screen.fullScreen;
    }

    void Update()
    {
        if (gamePaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1.0f;
        }

        if (SettingsMenuPanel.activeSelf)
        {
            TMPro.TextMeshProUGUI toggleFullscreenText = transform.Find("SettingsMenu/ToggleFullscreen/Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>();
            if (Screen.fullScreen) toggleFullscreenText.SetText("Fullscreen");
            else toggleFullscreenText.SetText("Windowed");

        }

    }

    //---- Main Menu ----

    public void OnStartGameButton()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OnSettingsButton()
    {
        ChangeMenuPanel(SettingsMenuPanel);
    }

    public void OnCreditsButton()
    {
        ChangeMenuPanel(CreditsMenuPanel);
    }
    public void OnExitButton()
    {
        Application.Quit();
    }

    public void OnLevelBlockoutButton()
    {
        SceneManager.LoadScene("LevelScene");
    }

    //---- Pause Menu ----
    public void OnContinueButton()
    {
        PauseMenuPanel.SetActive(false);
        gamePaused = false;
    }

    public void OnToMainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnEsc(InputAction.CallbackContext context)
    {
        if (!PauseMenuPanel.activeSelf && gamePaused) return;
        if (context.started) gamePaused = !gamePaused;
        PauseMenuPanel.SetActive(gamePaused);
    }

    //---- Settings Menu ----
    public void OnOKButton()
    {
        ChangeMenuPanel(LastMenuPanel);
    }

    public void OnToggleFullscreen(bool _value)
    {
        Screen.SetResolution(Screen.width, Screen.height, fullscreenToggle.isOn);
    }

    public void OnChangeResolution()
    {
        Vector2Int newResolution = resolutions[resolutionDropdown.value];
        //Debug.Log(newResolution.ToString());

        Screen.SetResolution(newResolution.x, newResolution.y, Screen.fullScreen);
    }

    public void ChangeMenuPanel(GameObject _newPanel)
    {
        GameObject currentPanel = MenuList.Find(x => x.activeSelf);
        Debug.Log(currentPanel.name);

        LastMenuPanel = currentPanel;

        currentPanel.SetActive(false);
        _newPanel.SetActive(true);
    }

    public void OnSceneChange(Scene _a, Scene _b)
    {
        Debug.Log("Scene changed to " + _b.name);

        if (_b == SceneManager.GetSceneByName("MainMenu"))
        {
            SceneManager.activeSceneChanged -= OnSceneChange;
            DestroyImmediate(this.gameObject);
            return;
        }

        gamePaused = false;
        foreach (GameObject menu in MenuList) menu.SetActive(false);
    }

    //---- Credits ----
    public void OnButton_DozentenButton()
    {
        ChangeMenuPanel(BetreuerScreen);

    }
    public void OnButton_EntwicklerButton()
    {
        ChangeMenuPanel(EntwicklerScreen);
    }
}
