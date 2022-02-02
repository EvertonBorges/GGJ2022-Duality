using System;
using System.Reflection;
using UnityEngine;

public static class Manager_Input
{

    private static InputActions inputActions;
    private static InputActions.GameplayActions gameplayActions;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void Setup()
    {
        HackFixForEditorPlayModeDelay();

        inputActions = new InputActions();
        gameplayActions = inputActions.Gameplay;

        gameplayActions.Player1Move.performed += ctx => Observer.Player.OnPlayer1Move.Notify(ctx.ReadValue<Vector2>());
        gameplayActions.LeftStick.performed += ctx => Observer.Player.OnLeftStick.Notify(ctx.ReadValue<Vector2>());
        gameplayActions.Interact1.performed += ctx => Observer.Player.OnInteract1.Notify();
        gameplayActions.LeftTrigger.performed += ctx => Observer.Player.OnLeftTrigger.Notify();
        
        gameplayActions.Player2Move.performed += ctx => Observer.Player.OnPlayer2Move.Notify(ctx.ReadValue<Vector2>());
        gameplayActions.RightStick.performed += ctx => Observer.Player.OnRightStick.Notify(ctx.ReadValue<Vector2>());
        gameplayActions.Interact2.performed += ctx => Observer.Player.OnInteract2.Notify();
        gameplayActions.RightTrigger.performed += ctx => Observer.Player.OnRightTrigger.Notify();

        inputActions.Enable();
    }

    private static void HackFixForEditorPlayModeDelay()
    {
#if UNITY_EDITOR
        // Using reflection, does this: InputSystem.s_SystemObject.exitEditModeTime = 0

        // Get InputSystem.s_SystemObject object
        FieldInfo systemObjectField = typeof(UnityEngine.InputSystem.InputSystem).GetField("s_SystemObject", BindingFlags.NonPublic | BindingFlags.Static);
        object systemObject = systemObjectField.GetValue(null);

        // Get InputSystemObject.exitEditModeTime field
        Assembly inputSystemAssembly = typeof(UnityEngine.InputSystem.InputSystem).Assembly;
        Type inputSystemObjectType = inputSystemAssembly.GetType("UnityEngine.InputSystem.InputSystemObject");
        FieldInfo exitEditModeTimeField = inputSystemObjectType.GetField("exitEditModeTime", BindingFlags.Public | BindingFlags.Instance);

        // Set exitEditModeTime to zero
        exitEditModeTimeField.SetValue(systemObject, 0d);
#endif
    }

}