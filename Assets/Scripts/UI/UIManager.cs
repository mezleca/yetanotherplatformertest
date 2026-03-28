using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour 
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private UIDocument player_ui_doc;
    [SerializeField] private UIDocument access_panel_doc;
    [SerializeField] private UIDocument pause_doc;
    [SerializeField] private UIDocument death_doc;
    [SerializeField] private UIDocument main_menu_doc;
    [SerializeField] private UIDocument level_selector_doc;

    public PlayerUI player_ui { get; private set; }
    public AccessPanelUI access_panel { get; private set; }
    public PauseMenuUI pause_panel { get; private set; }
    public DeathUI death_ui { get; private set; }
    public MainMenuUI main_menu { get; private set; }
    public LevelSelectorUI level_selector { get; private set; }

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        player_ui = new PlayerUI(player_ui_doc.rootVisualElement);
        access_panel = new AccessPanelUI(access_panel_doc.rootVisualElement);
        pause_panel = new PauseMenuUI(pause_doc.rootVisualElement);
        death_ui = new DeathUI(death_doc.rootVisualElement);
        main_menu = new MainMenuUI(main_menu_doc.rootVisualElement);
        level_selector = new LevelSelectorUI(level_selector_doc.rootVisualElement);

        Debug.Log("initialized (UI MANAGER)");

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
