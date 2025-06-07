using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI instance;

    public bool isFinish = false;

    [SerializeField] private Text countdownText;
    [SerializeField] private float timerDuration = 30f;
    [SerializeField] private GameObject gameOverPanel;
    private float _timeRemaining;

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _timeRemaining = timerDuration;
        UpdateTimerUI();
    }

    private void Update()
    {
        if (isFinish) return;

        _timeRemaining -= Time.deltaTime;
        _timeRemaining = Mathf.Max(0f, _timeRemaining);

        UpdateTimerUI();

        if (!(_timeRemaining <= 0f)) return;
        gameOverPanel.SetActive(true);
        isFinish = true;
        Time.timeScale = 0;
    }

    private void UpdateTimerUI()
    {
        if (!countdownText) return;

        int seconds = Mathf.FloorToInt(_timeRemaining);
        int milliseconds = Mathf.FloorToInt((_timeRemaining - seconds) * 100);

        float colorFactor = Mathf.Clamp01(_timeRemaining / 10f);
        countdownText.color = Color.Lerp(Color.red, Color.white, colorFactor);

        countdownText.text = $"{seconds:00}.{milliseconds:00}";
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        isFinish = false;
        _timeRemaining = timerDuration;
        gameOverPanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}