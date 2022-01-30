using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    [SerializeField] private Transform _door;
    
    private bool m_isLocked = true;

    private Coroutine m_coroutine = null;

    public void Open()
    {
        m_isLocked = false;

        CallDoorSwitch();
    }

    public void Close()
    {
        m_isLocked = true;

        CallDoorSwitch();
    }

    private void CallDoorSwitch()
    {
        if (m_coroutine != null)
            StopCoroutine(m_coroutine);

        m_coroutine = StartCoroutine(DoorSwitch(0.5f));
    }

    private IEnumerator DoorSwitch(float duration)
    {
        float currentTime = 0f;

        float startPosition = m_isLocked ? -5f : 0f;

        float endPosition = m_isLocked ? 0f : -5f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;

            var yPosition = Mathf.Lerp(startPosition, endPosition, currentTime / duration);

            _door.localPosition = new Vector3(_door.position.x, yPosition, _door.position.z);

            yield return null;
        }

        _door.localPosition = new Vector3(_door.position.x, endPosition, _door.position.z);

        yield return null;
    }

}
