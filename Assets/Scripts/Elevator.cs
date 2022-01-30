using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{

    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _unlockColor;

    [SerializeField] private MeshRenderer _meshRenderer = null;

    private bool m_isLock = true;

    private bool m_player1 = false;
    private bool m_player2 = false;

    private Coroutine m_coroutine = null;

    void Awake()
    {
        _meshRenderer.material.color = _defaultColor;
    }

    public void Unlock()
    {
        m_isLock = false;

        CallChangeLock();
    }

    private void CallChangeLock(float duration = 0.25f)
    {
        if (m_coroutine != null)
            StopCoroutine(m_coroutine);

        m_coroutine = StartCoroutine(ChangeLock(duration));
    }

    private IEnumerator ChangeLock(float duration)
    {
        float currentTime = 0f;

        Color startColor = m_isLock ? _unlockColor : _defaultColor;

        Color endColor = m_isLock ? _defaultColor : _unlockColor;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;

            var color = Color.Lerp(startColor, endColor, currentTime / duration);

            _meshRenderer.material.color = color;

            yield return null;
        }

        _meshRenderer.material.color = endColor;

        yield return null;

        m_coroutine = null;
    }

    private void LevelCompleted()
    {
        Observer.Player.OnCantPlay.Notify();

        CallChangeLevel();
    }

    private void CallChangeLevel(float duration = 5f)
    {
        if (m_coroutine != null)
            StopCoroutine(m_coroutine);

        m_coroutine = StartCoroutine(ChangeLevel(duration));
    }

    private IEnumerator ChangeLevel(float duration)
    {
        float currentTime = 0f;

        float startPosition = 0f;

        float endPosition = 10f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;

            var yPosition = Mathf.Lerp(startPosition, endPosition, currentTime / duration);

            transform.localPosition = new Vector3(transform.localPosition.x, yPosition, transform.localPosition.z);

            yield return null;
        }

        transform.localPosition = new Vector3(transform.localPosition.x, endPosition, transform.localPosition.z);

        yield return null;

        m_coroutine = null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (m_isLock || !other.TryGetComponent<PlayerController>(out PlayerController player))
            return;
        
        switch(player.PlayerType)
        {
            case PlayerController.Player.Player1: m_player1 = true; break;
            case PlayerController.Player.Player2: m_player2 = true; break;
        }

        if (m_player1 && m_player2)
            LevelCompleted();
    }

    void OnTriggerExit(Collider other)
    {
        if (m_isLock || !other.TryGetComponent<PlayerController>(out PlayerController player))
            return;
        
        switch(player.PlayerType)
        {
            case PlayerController.Player.Player1: m_player1 = false; break;
            case PlayerController.Player.Player2: m_player2 = false; break;
        }
    }
}
