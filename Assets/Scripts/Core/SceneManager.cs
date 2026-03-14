using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    public string current_scene = "";

    void Awake() 
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadLevel(string name)
    {
        StartCoroutine(LoadScene(name));
    }

    public IEnumerator LoadScene(string name, bool set_active = true)
    {
        // unload previous scene if needed
        if (!string.IsNullOrEmpty(current_scene) && set_active)
        {
            AsyncOperation unload = SceneManager.UnloadSceneAsync(current_scene);
            yield return unload;
        }

        AsyncOperation load = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        yield return load;

        // TODO: handle errors 
        if (set_active) 
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
            current_scene = name; 
        }
    }
};
