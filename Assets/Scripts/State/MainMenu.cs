using System.Threading.Tasks;

public class MainMenuState: GameState 
{
    public MainMenuState(GameCore core) : base(core) {}

    public override Task OnEnter() {
        core.ui_manager.main_menu.Show();
        return Task.CompletedTask;
    }

    public override Task OnExit() {
        core.ui_manager.main_menu.Hide();
        return Task.CompletedTask;
    }

    public override Task OnPause() => throw new System.NotImplementedException();
    public override Task OnResume() => throw new System.NotImplementedException();
};