using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
    public Player player;

    // player ui
    public GameObject ui_stamina_bar;
    private Image ui_stamina_image;
    public GameObject ui_health_bar;
    private Image ui_health_image;

    void Awake()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }

        if (ui_stamina_bar && !ui_stamina_image)
        {  
            ui_stamina_image = ui_stamina_bar.GetComponent<Image>();  
        }

        if (ui_health_bar && !ui_health_image)
        {  
            ui_health_image = ui_stamina_bar.GetComponent<Image>();  
        }
    }

    void Update()
    {
        gameObject.SetActive(player && gameObject.activeSelf);

        if (player != null)
        {
            EntityAttributes attributes = player.ent.movement.attributes;
            ui_stamina_image.fillAmount = attributes.stamina / attributes.max_stamina * 100 / 100;
        }
    }
};