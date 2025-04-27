using UnityEngine;
using UnityEngine.Events;

public class EventOnTrigger : TriggerBase
{
    public UnityEvent OnEnter;
    public UnityEvent OnExit;


    protected override void OnTriggerEnter(Collider other) {
        if (!IsCorrectTag(other)) {
            return;
        }

        base.OnTriggerEnter(other);

        OnEnter.Invoke();
    }

    //     protected override void OnTriggerExit(Collider other) {
    //     if (!IsCorrectTag(other)) {
    //         return;
    //     }

    //     base.OnTriggerEnter(other);

    //     OnExit.Invoke();
    // }
}
