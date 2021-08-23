using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    public int currentLevel;
    public Player currentPlayer;
    public Material spring;
    public Material fixedJoint;
    public Sprite[] hudSprites;
    public AudioSource ambienceSource;

    [Header("Pause Menu")]
    public bool canPause;
    public bool paused;
    public RectTransform buttonPanel;
    public CanvasGroup alpha;
    public float transitionSpeed;

    private float timeVel;
    private float alphaVel;
    private Vector3 panelVel;

    private int oldLevel;

    public void Awake() {
        Instance = this;

        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject);
        LoadScene(1);
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
        alpha.blocksRaycasts = paused;

        var timeValue = paused ? 0f : 1f;
        var alphaValue = paused ? 1 : 0f;
        var panelPos = paused ? Vector3.zero : new Vector3(329.05f, 0);

        Time.timeScale = Mathf.SmoothDamp(Time.timeScale, timeValue, ref timeVel, transitionSpeed);
        alpha.alpha = Mathf.SmoothDamp(alpha.alpha, alphaValue, ref alphaVel, transitionSpeed);
        buttonPanel.anchoredPosition = Vector3.SmoothDamp(buttonPanel.anchoredPosition, panelPos, ref panelVel, transitionSpeed);
    }

    public void LoadScene(int handle) {
        SceneManager.LoadScene(handle);
        oldLevel = currentLevel;
        currentLevel = handle;
    }

    public void LoadNext() {
        LoadScene(currentLevel + 1);
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) {
        if (currentLevel != 0 && currentLevel != 1) {
            if (!ambienceSource.isPlaying) ambienceSource.Play();
            canPause = true;
        }

        if (currentLevel != oldLevel) {
            Globals.Save();
        } else {
            Globals.Load();
        }

        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players) {
            Debug.Log(player.name);
            if (player.GetComponent<Player>().active) {
                currentPlayer = player.GetComponent<Player>();
            }
        }
    }

    public void TogglePause() {
        if (canPause) paused = !paused;
    }

    public void Quit() {
        Application.Quit();
    }
}
