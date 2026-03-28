using UnityEngine;

public class StartPad : MonoBehaviour
{
    private AccessPanelUI panel;
    private GameCore core;

    void Awake() 
    {
        core = GameCore.Instance;
        panel = core.ui_manager.access_panel;
        
        panel.OnSuccess += OnSucess;
    }

    void OnDestroy() {
        if (panel != null) {
            panel.OnSuccess -= OnSucess;
        }
    }

    void OnSucess(string value) {
        panel.Hide();
    }

    void OnMouseDown() {
        panel.target = "444";
        panel.Show();
    }
}
