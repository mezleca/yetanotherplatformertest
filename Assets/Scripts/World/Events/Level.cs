using UnityEngine;

public class Level : MonoBehaviour
{
    private GameCore core;

    void Awake() 
    {
        core = GameCore.Instance;
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.layer == 6) {
            _ = core.LoadLevelSelector();
        }
    }
}
