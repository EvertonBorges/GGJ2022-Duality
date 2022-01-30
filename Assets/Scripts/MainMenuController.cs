using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{

    [SerializeField] private AudioSource _audio;

    [SerializeField] private Button _btnMenu;

    void Awake()
    {
        _audio.Stop();

        if (_btnMenu != null)
        {
            _btnMenu.gameObject.SetActive(false);

            StartCoroutine(WaitToShowBtnMenu());
        }
    }

    private IEnumerator WaitToShowBtnMenu()
    {
        yield return new WaitForSeconds(2f);

        _btnMenu.gameObject.SetActive(true);
    }

    public void BTN_Play()
    {
        Observer.GameManager.OnStartGame.Notify();

        Observer.Audio.OnStartGame.Notify();

        _audio.Play();
    }

    public void BTN_Credits()
    {
        _audio.Play();

    }

    public void BTN_Quit()
    {
        _audio.Play();

        StartCoroutine(WaitToQuit());
    }

    public void BTN_Menu()
    {
        _audio.Play();

        Observer.GameManager.OnMainMenu.Notify();
    }

    private IEnumerator WaitToQuit()
    {
        yield return new WaitForSeconds(0.25f);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
        Application.Quit();
    }

}
