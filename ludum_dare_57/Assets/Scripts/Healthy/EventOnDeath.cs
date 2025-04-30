using UnityEngine;
using UnityEngine.Events;

public class EventOnDeath : MonoBehaviour
{
    public UnityEvent OnDeath;

    public void OnDead() {
        OnDeath?.Invoke();
    }
}
