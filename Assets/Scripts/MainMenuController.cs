using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    
    public void BTN_Play()
    {
        Observer.GameManager.OnStartGame.Notify();
    }

    public void BTN_Credits()
    {

    }

    public void BTN_Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
        Application.Quit();
    }

}
