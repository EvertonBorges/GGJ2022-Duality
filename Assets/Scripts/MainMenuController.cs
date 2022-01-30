using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{

    [SerializeField] private AudioSource _audio;

    void Awake()
    {
        _audio.Stop();
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

    private IEnumerator WaitToQuit()
    {
        yield return new WaitForSeconds(0.25f);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
        Application.Quit();
    }

}
