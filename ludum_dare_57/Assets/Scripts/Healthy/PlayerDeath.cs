using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
            OnDead();
    }

    public void OnDead() {
        GameManager.instance.PlayerDeath();
        FindAnyObjectByType<Player>().enabled = false;
        FindAnyObjectByType<PlayerDeath>().enabled = false;
    }
}
