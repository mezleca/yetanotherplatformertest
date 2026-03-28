using Unity.VisualScripting;
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
        if (other.gameObject.CompareTag("Player")) {
            core.ui_manager.level_selector.Show();
        }
    }
}
