using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Audio : Singleton<Manager_Audio>
{

    [SerializeField] private AudioSource _audioMenu = null;

    [SerializeField] private AudioSource _audioGame = null;

    protected override void Init()
    {
        _audioMenu.Stop();

        _audioGame.Stop();

        _audioMenu.Play();

        DontDestroyOnLoad(gameObject);
    }

    private void OnStartGame()
    {
        _audioMenu.Stop();

        _audioGame.Play();
    }

    private void OnMainMenu()
    {
        _audioGame.Stop();

        _audioMenu.Play();
    }

    void OnEnable()
    {
        Observer.Audio.OnStartGame += OnStartGame;
        Observer.Audio.OnMainMenu += OnMainMenu;
    }

    void OnDisable()
    {
        Observer.Audio.OnStartGame -= OnStartGame;
        Observer.Audio.OnMainMenu -= OnMainMenu;
    }

}
