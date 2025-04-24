using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
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
