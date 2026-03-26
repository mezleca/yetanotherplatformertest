using UnityEngine;
using UnityEngine.EventSystems;

public class StartPad : MonoBehaviour, IPointerClickHandler
{
    private GameCore core;

    void Awake()
    {
        if (core == null) 
        {
            core = GameCore.Instance;
        }
    }

    public void OnPointerClick(PointerEventData event_data)
    {
        Debug.Log("clicked");
        if (event_data.button == PointerEventData.InputButton.Left)
        {
            _ = core.LoadAccessPanel();
        }
    }

    void Update()
    {
        
    }
}
