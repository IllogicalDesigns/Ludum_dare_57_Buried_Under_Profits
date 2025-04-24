using UnityEngine;

public class TransitionButton : MonoBehaviour
{
    public void TransitionToLevel(string levelName) {
        TransitionManager.Instance.TransitionToLevel(levelName);
    }
}
