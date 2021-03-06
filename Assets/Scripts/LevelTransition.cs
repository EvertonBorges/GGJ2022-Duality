using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelTransition : Singleton<LevelTransition>
{

    [SerializeField] private CanvasGroup _canvasGroup;

    protected override void Init()
    {
        if (_canvasGroup != null) _canvasGroup.alpha = 0f;

        DontDestroyOnLoad(gameObject);
    }

    protected override void StartInit()
    {
        Observer.Player.OnCanPlay.Notify();
    }

    private void StartGame()
    {
        var nextLevelScene = "Level_01";

        StartCoroutine(GoToScene(nextLevelScene));
    }

    private void NextLevel()
    {
        var nextLevelScene = NextLevelName();

        StartCoroutine(GoToScene(nextLevelScene));
    }

    private void RestartLevel()
    {
        var nextLevelScene = SceneManager.GetActiveScene().name;

        StartCoroutine(GoToScene(nextLevelScene));
    }

    private void MainMenu()
    {
        var nextLevelScene = "Main_Menu";

        StartCoroutine(GoToScene(nextLevelScene));
    }

    private IEnumerator PlayerCanPlay()
    {
        yield return new WaitForSeconds(0.15f);

        Observer.Player.OnCanPlay.Notify();
    }

    private IEnumerator GoToScene(string sceneName, float transitionTime = 0.5f)
    {
        var asyncScene = SceneManager.LoadSceneAsync(sceneName);

        asyncScene.allowSceneActivation = false;

        _canvasGroup.alpha = 0f;

        float currentTime = 0f;

        while (currentTime < transitionTime)
        {
            currentTime += Time.deltaTime;

            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, currentTime / transitionTime);

            yield return null;
        }

        _canvasGroup.alpha = 1f;

        yield return null;

        while (asyncScene.progress < 0.9f)
            yield return null;

        Observer.GameManager.OnLevelUnload.Notify();

        asyncScene.allowSceneActivation = true;

        currentTime = 0f;

        while (currentTime < transitionTime)
        {
            currentTime += Time.deltaTime;

            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, currentTime / transitionTime);

            yield return null;
        }

        _canvasGroup.alpha = 0f;

        Observer.Player.OnCanPlay.Notify();
    }

    private string NextLevelName()
    {
        var sceneName = SceneManager.GetActiveScene().name;

        var name = sceneName.Split('_')[0];

        var sceneNumber = int.Parse(sceneName.Split('_')[1]);

        if (sceneNumber >= 8)
            return "Credits";

        sceneNumber++;

        var nextLevelNumber = sceneNumber < 10 ? ("0" + sceneNumber) : ("" + sceneNumber);

        return name + "_" + nextLevelNumber;
    }

    void OnEnable()
    {
        Observer.GameManager.OnStartGame += StartGame;
        Observer.GameManager.OnNextLevel += NextLevel;
        Observer.GameManager.OnRestartlevel += RestartLevel;
        Observer.GameManager.OnMainMenu += MainMenu;
    }

    void OnDisable()
    {
        Observer.GameManager.OnStartGame -= StartGame;
        Observer.GameManager.OnNextLevel -= NextLevel;
        Observer.GameManager.OnRestartlevel -= RestartLevel;
        Observer.GameManager.OnMainMenu -= MainMenu;
    }

}
