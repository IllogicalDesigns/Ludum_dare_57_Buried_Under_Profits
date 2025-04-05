using UnityEngine;

public class TransitionButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TransitionToLevel(string levelName) {
        TransitionManager.Instance.TransitionToLevel(levelName);
    }
}
