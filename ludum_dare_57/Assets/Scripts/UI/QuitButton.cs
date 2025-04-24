using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public bool hideInWeb = true;

#if UNITY_WEBGL
    void Awake() {
        if(hideInWeb)
            gameObject.SetActive(false);
    }
#endif

    public void QuitGame() {
        Debug.Log("QuitGame() has been called");
        Application.Quit();
    }
}
