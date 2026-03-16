using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {
    private GameUtils utils;

    public string current_level = "";

    void Awake() 
    {
        utils = GameUtils.Instance;
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator Load(string name, bool set_active = true)
    {
        if (utils == null)
        {
            utils = GameUtils.Instance;
        }

        Debug.Log(utils);

        if (!string.IsNullOrEmpty(current_level) && utils.isSceneLoaded(current_level))
        {
            AsyncOperation unload = SceneManager.UnloadSceneAsync(current_level);
            yield return unload;
        }

        yield return SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

        // TODO: handle errors 
        if (set_active) 
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
            current_level = name; 
        }
    }
};
