using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class LevelSelector : MonoBehaviour {
    private GameCore core;
    private GameUtils utils;

    private readonly string[] Levels = new string[]
    {
        "Level 1",
        "Level 2"
    };

    // ui
    private VisualElement ui;
    private VisualElement level_list;
    private VisualElement back_btn;

    void Awake()
    {
        core = GameCore.Instance;
        utils = GameUtils.Instance;

        ui = GetComponent<UIDocument>().rootVisualElement;

        // setup elements
        level_list = ui.Q<VisualElement>("level-list");
        back_btn = ui.Q<VisualElement>("back-btn");

        // setup events
        back_btn.RegisterCallback<ClickEvent>(onBack);
    }

    void Start()
    {
        BuildLevels();
    }

    void OnDestroy()
    {
        back_btn.UnregisterCallback<ClickEvent>(onBack);
        level_list.Clear();
    }

    void BuildLevels()
    {
        level_list.Clear();

        foreach (var level in Levels)
        {
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

            level_btn.clicked += () => onLevelClick(level);

            container.Add(level_label);
            container.Add(level_btn);

            level_list.Add(container);
        }
    }

    // events
    void onBack(ClickEvent ev)
    {
        core.UnloadLevelSelector();
    }

    void onLevelClick(string level)
    {
        core.LoadNewLevel(level);
    }
};
