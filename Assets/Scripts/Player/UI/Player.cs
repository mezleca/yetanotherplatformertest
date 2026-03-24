using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class PlayerUI : MonoBehaviour {
    public VisualElement ui;
    public Player player;

    // player ui
    private VisualElement stamina_fill;
    private VisualElement health_fill;

    void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;

        // setup ui elements
        health_fill = ui.Q<VisualElement>("health-fill");
        stamina_fill = ui.Q<VisualElement>("stamina-fill");

        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }
    }

    void Update()
    {
        gameObject.SetActive(player && gameObject.activeSelf);

        if (player != null)
        {
            EntityAttributes attributes = player.ent.movement.attributes;
            stamina_fill.style.width = new StyleLength(Length.Percent(attributes.stamina / attributes.max_stamina * 100.0f));
        }
    }
};