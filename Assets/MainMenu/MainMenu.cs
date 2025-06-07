using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject levelPanel;

    private Transform _contentTransform;
    private bool _isLevelButtonsCreated = false;

    private void Awake()
    {
        _contentTransform = levelPanel.transform.Find("Scroll View/Viewport/Content");
        if (!_contentTransform)
        {
            Debug.LogError("Content transform not found! Check the hierarchy inside levelPanel.");
        }
    }

    public void GameStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void SelectLevelPanel()
    {
        mainMenu.SetActive(!mainMenu.activeSelf);
        levelPanel.SetActive(!levelPanel.activeSelf);

        if (!levelPanel.activeSelf || _isLevelButtonsCreated) return;
        CreateLevelButtons();
        _isLevelButtonsCreated = true;
    }

    private void CreateLevelButtons()
    {
        if (!_contentTransform) return;

        int sceneCount = SceneManager.sceneCountInBuildSettings;
        int lastLoadedLevel = PlayerPrefs.GetInt("LastLoadedLevel", 1);

        for (int i = 0; i < sceneCount; i++)
        {
            if (i == 0) continue;

            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);

            GameObject buttonObj = Instantiate(buttonPrefab, _contentTransform);
            buttonObj.name = "Button_" + sceneName;

            Text buttonText = buttonObj.GetComponentInChildren<Text>();
            if (buttonText)
                buttonText.text = sceneName;

            Button btn = buttonObj.GetComponent<Button>();

            if (i <= lastLoadedLevel)
            {
                btn.interactable = true;
                int index = i;
                btn.onClick.AddListener(() => { SceneManager.LoadScene(index); });
            }
            else
            {
                btn.interactable = false;

                CanvasGroup cg = buttonObj.GetComponent<CanvasGroup>();
                if (!cg)
                    cg = buttonObj.AddComponent<CanvasGroup>();

                cg.alpha = 0.5f;
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_contentTransform);
    }
}