using System;
using UnityEngine;
using UnityEngine.UIElements;

class AccessPanel : MonoBehaviour 
{
    public GameCore core;

    public Action<string> onSuccess;
    public string target = "111";

    private bool locked = false;

    private VisualElement ui;
    private VisualElement access_buttons;

    private Label access_text;

    void Awake()
    {
        if (core == null) 
        {
            core = GameCore.Instance;
        }

        ui = GetComponent<UIDocument>().rootVisualElement;

        access_buttons = ui.Q<VisualElement>("access-buttons");
        access_text = ui.Q<Label>("access-text");

        access_buttons.Query<Button>().ForEach((btn) => {
            var text = access_text.text;

            if (btn.ClassListContains("confirm"))
            {
                btn.clicked += onConfirm;
            }
            else if (btn.ClassListContains("delete"))
            {
                btn.clicked += () => updateText("");
            }
            else 
            {
                btn.clicked += () => updateText(access_text.text + btn.text);
            }
        });
    }

    void updateText(string value) 
    {   
        if (locked) return;
        access_text.text = value;
    }

    void onConfirm() 
    {
        if (access_text.text != target) 
        {
            var old_text = access_text.text;

            locked = true;
            access_text.text = "invalid!!!";

            core.utils.delay_then(1.0f, () => {
                access_text.text = old_text;
                locked = false;
            });

            return;
        }

        onSuccess?.Invoke(target);
    }
};