using System;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIPanel 
{
    protected VisualElement root;

    public UIPanel(VisualElement root) {
        this.root = root;
        root.style.display = DisplayStyle.None;
        Initialize();    
    }

    protected abstract void Initialize();

    public void Show() {
        root.style.display = DisplayStyle.Flex;
        root.BringToFront();
    }

    public void Hide() => root.style.display = DisplayStyle.None;
    public bool Visible() => root.style.display == DisplayStyle.Flex;
}

public class PlayerUI : UIPanel 
{
    public VisualElement health_fill;
    public VisualElement stamina_fill;

    public PlayerUI(VisualElement root) : base(root) { }

    protected override void Initialize() {
        health_fill = root.Q<VisualElement>("health-fill");
        stamina_fill = root.Q<VisualElement>("stamina-fill");
    }

    public void UpdateHealth(float value, float max) {
        health_fill.style.width = new StyleLength(Length.Percent(value / max * 100.0f));
    }

    public void UpdateStamina(float value, float max) {
        stamina_fill.style.width = new StyleLength(Length.Percent(value / max * 100.0f));
    }
}

public class DeathUI : UIPanel 
{
    public Button btn_retry;

    public DeathUI(VisualElement root) : base(root) { }

    protected override void Initialize() {
        btn_retry = root.Q<Button>("retry-btn");
        btn_retry.clicked += OnRetry;
    }

    public new void Show() {
        root.style.display = DisplayStyle.Flex;
        root.BringToFront();
        Time.timeScale = 0.0f;
    }

    public async void OnRetry() {
        var core = GameCore.Instance;
        Time.timeScale = 1.0f;
        await core.TransitionTo(new LevelState(core, core.last_loaded_level));
        Hide();
    }
}

public class MainMenuUI : UIPanel 
{
    public Button btn_play;
    public Button btn_settings;
    public Button btn_exit;

    public MainMenuUI(VisualElement root) : base(root) { }

    protected override void Initialize() {
        btn_play = root.Q<Button>("play");
        btn_settings = root.Q<Button>("settings");
        btn_exit = root.Q<Button>("exit");

        btn_play.clicked += OnPlay;
        btn_settings.clicked += OnSettings;
        btn_exit.clicked += OnExit;
    }

    public async void OnPlay() {
        var core = GameCore.Instance;
        await core.TransitionTo(new LevelState(core, "Start"));
    }

    public void OnSettings() => throw new NotImplementedException();
    public void OnExit() => throw new NotImplementedException();
}

public class PauseMenuUI : UIPanel 
{
    public Button btn_resume;
    public Button btn_return;
    
    public bool paused = false; 

    public PauseMenuUI(VisualElement root) : base(root) { }

    protected override void Initialize() {
        btn_resume = root.Q<Button>("resume");
        btn_return = root.Q<Button>("return");

        btn_resume.clicked += Resume;
        btn_return.clicked += OnReturn;
    }

    public void Toggle() {
        if (paused) Resume();
        else Pause();
    }

    public void Pause() {
        Show();
        Time.timeScale = 0.0f;
        paused = true;
    }

    public void Resume() {
        Hide();
        Time.timeScale = 1.0f;
        paused = false;
    }

    private async void OnReturn() {
        var core = GameCore.Instance;
        Resume();
        await core.TransitionTo(new MainMenuState(core));
    }
}

public class LevelSelectorUI : UIPanel 
{
    public VisualElement level_list;
    public VisualElement back_btn;

    private readonly string[] Levels = new string[] {
        "Level 1",
        "Level 2"
    };

    public LevelSelectorUI(VisualElement root) : base(root) { }

    protected override void Initialize() {
        level_list = root.Q<VisualElement>("level-list");
        back_btn = root.Q<VisualElement>("back-btn");

        level_list.Clear();

        foreach (var level in Levels) {
            var container = new VisualElement
            {
                name = "level"
            };

            var level_label = new Label
            {
                name = "level-text",
                text = level
            };

            var level_btn = new Button
            {
                name = "level-load",
                text = "load"
            };

            level_btn.clicked += () => OnLevelClick(level);

            container.Add(level_label);
            container.Add(level_btn);

            level_list.Add(container);
        }

        // setup events
        back_btn.RegisterCallback<ClickEvent>(OnBack);
    }

    public void OnLevelClick(string level) {
        var core = GameCore.Instance;
        _ = core.TransitionTo(new LevelState(core, level));
    }

    public void OnBack(ClickEvent ev) {
        var core = GameCore.Instance;
        _ = core.PopState();
    }

    public async void OnReturn() {
        var core = GameCore.Instance;
        await core.TransitionTo(new MainMenuState(core));
    } 
}

public class AccessPanelUI : UIPanel 
{
    public Action<string> OnSuccess;
    public Action<string> OnFailure;

    public string target = "";
    private bool locked = false;

    private VisualElement access_buttons;
    private Label access_text;

    public AccessPanelUI(VisualElement root) : base(root) { }

    protected override void Initialize() {
        access_buttons = root.Q<VisualElement>("access-buttons");

        access_text = root.Q<Label>("access-text");
        access_text.text = "";

        access_buttons.Query<Button>().ForEach((btn) => 
        {
            var text = access_text.text;

            if (btn.ClassListContains("confirm")) {
                btn.clicked += OnConfirm;
            } else if (btn.ClassListContains("delete")) {
                btn.clicked += () => UpdateText("");
            } else {
                btn.clicked += () => UpdateText(access_text.text + btn.text);
            }
        });
    }

    private void OnConfirm() {
        var core = GameCore.Instance;
        string value = access_text.text;

        Debug.Log($"value: {value} | target: {target}");

        if (value != target) {
            locked = true;
            access_text.text = "invalid!!!";

            core.utils.delay_then(1.0f, () => {
                access_text.text = value;
                locked = false;
            });

            OnFailure?.Invoke(value);
            return;
        }

        Clear();
        OnSuccess?.Invoke(value);
    }

    private void UpdateText(string value) {
        if (locked) return;
        access_text.text = value;
    }

    public void Clear() {
        target = "";
        access_text.text = "";
    }
}