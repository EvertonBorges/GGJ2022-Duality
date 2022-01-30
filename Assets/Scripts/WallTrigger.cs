using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTrigger : MonoBehaviour
{
    [SerializeField] private PlayerController.Player _playerType;

    [SerializeField] private Material _player1Mat;
    [SerializeField] private Material _player2Mat;
    [SerializeField] private Material _defaultMat;

    [Header("Switch Infos")]
    [SerializeField] private Transform _switch;

    private bool m_isOn = false;
    
    public bool IsOn => m_isOn;

    private Coroutine m_coroutine = null;

    void Awake()
    {
        ChangeSwitchMaterial();
    }

    private void ChangeSwitchMaterial()
    {
        var meshRenderer = _switch.GetComponent<MeshRenderer>();

        switch(_playerType)
        {
            case PlayerController.Player.Anyone: meshRenderer.material = _defaultMat; break;
            case PlayerController.Player.Player1: meshRenderer.material = _player1Mat; break;
            case PlayerController.Player.Player2: meshRenderer.material = _player2Mat; break;
        }
    }

    public void ForceToTurnOff()
    {
        m_isOn = false;
        CallSwitch(0f);
    }

    public void TurnOn(PlayerController.Player playerType)
    {
        m_isOn = true;
        CallSwitch();
        Observer.GameManager.TurnOnOff.Notify(playerType);
    }

    private void CallSwitch(float duration = 0.25f)
    {
        if (m_coroutine != null)
            StopCoroutine(m_coroutine);

        m_coroutine = StartCoroutine(Switch(duration));
    }

    private IEnumerator Switch(float duration)
    {
        float currentTime = 0f;

        float startPoint = m_isOn ? 1f : 0.25f;

        float endPoint = m_isOn ? 0.25f : 1f;

        while(currentTime < duration)
        {
            currentTime += Time.deltaTime;

            var yPosition = Mathf.Lerp(startPoint, endPoint, currentTime / duration);

            _switch.localPosition = new Vector3(_switch.localPosition.x, yPosition, _switch.localPosition.z);

            yield return null;
        }

        _switch.localPosition = new Vector3(_switch.localPosition.x, endPoint, _switch.localPosition.z);

        yield return null;

        m_coroutine = null;
    }
}
