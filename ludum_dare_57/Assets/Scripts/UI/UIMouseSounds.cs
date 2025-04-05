using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class UIMouseSounds : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {
    AudioClip pointerEnterClip;
    AudioClip pointerDownClip;
    AudioClip pointerExitClip;

    const string PATH = "Sounds/UI/";
    const string ENTERCLIP = PATH + "PointerEnter";
    const string DOWNCLIP = PATH + "PointerDown";
    const string EXITCLIP = PATH + "PointerExit";

    Camera cam;

    private void Awake() {
        cam = Camera.main;
    }


    private void Start() {
        pointerEnterClip = Resources.Load<AudioClip>(ENTERCLIP);
        pointerDownClip = Resources.Load<AudioClip>(DOWNCLIP);
        pointerExitClip = Resources.Load<AudioClip>(EXITCLIP);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (pointerEnterClip == null) { Debug.LogError("missing PointerEnterSound"); return; }
        AudioSource.PlayClipAtPoint(pointerEnterClip, cam.transform.position);
    }


    public void OnPointerDown(PointerEventData eventData) {
        if (pointerDownClip == null) { Debug.LogError("missing PointerDownSound"); return; }
        AudioSource.PlayClipAtPoint(pointerDownClip, cam.transform.position);
    }


    public void OnPointerExit(PointerEventData eventData) {
        if (pointerExitClip == null) { Debug.LogError("missing PointerExitSound"); return; }
        AudioSource.PlayClipAtPoint(pointerExitClip, cam.transform.position);
    }
}
