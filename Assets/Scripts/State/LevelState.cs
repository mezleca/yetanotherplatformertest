using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelState : GameState
{
    private readonly string name = "";

    public LevelState(GameCore core, string name) : base(core) {
        this.name = name;
    }

    public override async Task OnEnter() {
        core.ui_manager.level_selector.Hide();

        await SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        await SceneManager.LoadSceneAsync("Player", LoadSceneMode.Additive);

        core.last_loaded_level = name;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
    }

    public override async Task OnExit() {
        var utils = GameUtils.Instance;

        if (utils.isSceneLoaded("Player")) {
            await SceneManager.UnloadSceneAsync("Player");
        }

        if (utils.isSceneLoaded(name)) {
            await SceneManager.UnloadSceneAsync(name);
        }

        core.last_loaded_level = "";
    }

    public override Task OnPause() => throw new System.NotImplementedException();
    public override Task OnResume() => throw new System.NotImplementedException();
};