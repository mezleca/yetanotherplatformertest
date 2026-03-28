using System.Threading.Tasks;

public abstract class GameState
{
    protected GameCore core;

    public GameState(GameCore core) {
        this.core = core;
    }

    public abstract Task OnEnter();
    public abstract Task OnExit();
    public abstract Task OnPause();
    public abstract Task OnResume();
}