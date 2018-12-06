using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour {

	public float hungerRate = 0.0001f; // The rate in which the hunger bar depletes [Default - 0.0001f]

    public Image hungerBar;

	private Image gameOverScreen;
	private Text restartText;
	private Text quitText;
	private Button restartButton;
	private Button quitButton;
    private Color screenColor;
    private Color textColor;
    private Color restartColor;
    private Color quitColor;

	public GameObject pauseMenu;
	public Image pauseButton;

	public GameObject winScreen;

	public bool isDebugging = false;
	public bool isPaused = false;

    // Use this for initialization
    void Start () {
		hungerBar = GameObject.Find("Hunger Bar").GetComponent<Image>();
		gameOverScreen = GameObject.Find("Game Over Screen").GetComponent<Image>();
		restartText = GameObject.Find("Restart Text").GetComponent<Text>();
		restartButton = GameObject.Find("Restart Text").GetComponent<Button>();
		quitText = GameObject.Find("Quit Text").GetComponent<Text>();
		quitButton = GameObject.Find("Quit Text").GetComponent<Button>();
	
		pauseButton = GameObject.Find("Pause Button").GetComponent<Image>();
		pauseMenu = GameObject.Find("Pause Menu").gameObject;
		pauseMenu.SetActive(false);

		winScreen = GameObject.Find("Win Screen").gameObject;
		winScreen.SetActive(false);

		restartButton.interactable = false;
		quitButton.interactable = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.F5)) {
			isDebugging = !isDebugging;
		}

		if (Input.GetKeyDown(KeyCode.P)) {
			pauseGame();
		}

		if (!pauseMenu.activeSelf) hungerBar.fillAmount -= hungerRate;
        if (hungerBar.fillAmount <= 0) {
			pauseButton.enabled = false;

        	screenColor = gameOverScreen.color;
			restartColor = restartText.color;
			quitColor = quitText.color;
			
        	screenColor.a += 0.01f;
        	textColor.a += 0.01f;
			restartColor.a += 0.01f;
			quitColor.a += 0.01f;
			
           	gameOverScreen.color = screenColor;
			restartText.color = restartColor;
			quitText.color = quitColor;

			if (restartText.color.a > 0.5f)
				restartButton.interactable = true;
			if (quitText.color.a > 0.5f)
				quitButton.interactable = true;
        }
    }

	public void pauseGame() {
        if (hungerBar.fillAmount <= 0)
            return;
        isPaused = !isPaused;
		Debug.Log("Pausing game...");
		if (pauseMenu.activeSelf) {
			pauseMenu.SetActive(false);
			Time.timeScale = 1.0f;
            GameObject.Find("MusicTheme").GetComponent<AudioScript>().PlayTheme();
		} else {
			pauseMenu.SetActive(true);
			Time.timeScale = 0.0f;
		}
	}

	public void winGame() {
		Debug.Log("You won!");
		winScreen.SetActive(true);
	}

    public void restartGame() {
		Debug.Log("Restarting scene...");
		Time.timeScale = 1.0f;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void exitToMenu() {
		Debug.Log("Exiting to menu...");
		Time.timeScale = 1.0f;
		SceneManager.LoadScene("Main Menu");
	}

	public void exitGame() {
		Debug.Log("Exiting game...");
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif
	}
}
