using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class LevelLoader : MonoBehaviour {
    private GameUtils utils;

    public string current_level = "";

    void Awake()
    {
        utils = GameUtils.Instance;
        DontDestroyOnLoad(gameObject);
    }

    public async Task Load(string name, bool set_active = true)
    {
        if (utils == null)
        {
            utils = GameUtils.Instance;
        }

        if (!string.IsNullOrEmpty(current_level) && utils.isSceneLoaded(current_level))
        {
            await SceneManager.UnloadSceneAsync(current_level);
            Debug.Log("unloading old level: " + current_level);
        }

        await SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

        Debug.Log("loaded: " + name);

        // unload level start if needed
        if (utils.isSceneLoaded("Start") && set_active)
        {
            await SceneManager.UnloadSceneAsync("Start");
            Debug.Log("unloading level selector");
        }

        // TODO: handle errors
        if (set_active)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
            current_level = name;
        }
    }
};
