using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class StartGame : MonoBehaviour
{
    public string startSceneName;

    void Awake()
    {
#if UNITY_EDITOR
        SceneManager.LoadScene(System.IO.Path.GetFileNameWithoutExtension(EditorPrefs.GetString("SceneAutoLoader.PreviousScene")));
#else
        SceneManager.LoadScene(startSceneName);
#endif
    }
}
