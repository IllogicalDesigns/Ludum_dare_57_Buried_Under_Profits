using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class TriggerBase : MonoBehaviour {
    [SerializeField] protected string requiredTag = "Player";
    [SerializeField] protected bool singleUse;
    [SerializeField] protected bool hideRender = true;

    protected Renderer _renderer;
    protected Collider _collider;
    protected Rigidbody _rigidbody;

    private const int IGNORE_RAYCAST_LAYER = 1;
    private LayerMask ignoreRaycastLayer = (1 << IGNORE_RAYCAST_LAYER);

    protected virtual void Awake() {
        _renderer = GetComponent<Renderer>();
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void Reset() {
        ForceTriggerSetup();
    }

    protected virtual void OnTriggerEnter(Collider other) {
        if (singleUse) {
            DisableTrigger();
        }
    }

    protected virtual bool IsCorrectTag(Collider other) {
        if (requiredTag == "") {
            return false;
        }

        if (!other.CompareTag(requiredTag)) {
            return false;
        }

        return true;
    }

    protected virtual void DisableTrigger() {
        this.enabled = false;
        _collider.enabled = false;
    }

    public virtual void ForceTriggerSetup() {
        if (_renderer == null) _renderer = GetComponent<Renderer>();
        if (_collider == null) _collider = GetComponent<Collider>();
        if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();

        if (hideRender) {
            _renderer.enabled = false;
        }

        _collider.isTrigger = true;
        _rigidbody.isKinematic = true;

        gameObject.layer = ignoreRaycastLayer;
    }
}