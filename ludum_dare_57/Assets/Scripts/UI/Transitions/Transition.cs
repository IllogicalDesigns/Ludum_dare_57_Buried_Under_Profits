using UnityEngine;
using UnityEngine.Events;

public abstract class Transition : MonoBehaviour {
    public UnityEvent onBetweenEvent;
    public UnityEvent OnCompleteEvent;
    public virtual void PlayOutTransition() {
        Debug.Log("Playing animation...");
        OnComplete();
    }

    public virtual void PlayInTransition() {
        onBetweenEvent.Invoke();
        Debug.Log("Playing animation...");
        OnComplete();
    }

    public virtual void OnComplete() {
        OnCompleteEvent.Invoke();
        TransitionManager.Instance.TransitionCompleted();
    }
}