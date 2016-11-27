using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLauncher : MonoBehaviour
{
    public Texture loadingOverlayTexture;

    private AsyncOperation currentOperation;
    private GUIStyle guiStyle;

    public void Launch(string sceneName)
    {
        if (currentOperation == null || currentOperation.isDone)
        {
            currentOperation = SceneManager.LoadSceneAsync(sceneName);
            currentOperation.allowSceneActivation = true;
        }
        else
        {
            Debug.Log("Already opening a scene");
        }
    }

    private void Start()
    {
        if (loadingOverlayTexture == null)
            throw new ArgumentException("loadingOverlayTexture cannot be null");

        guiStyle = new GUIStyle {fontSize = 69, alignment = TextAnchor.LowerRight};
    }

    private void OnGUI()
    {
        if (currentOperation != null && !currentOperation.isDone)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), loadingOverlayTexture);
            GUI.Label(new Rect(Screen.width - 400, Screen.height - 100, 400, 100), "Loading...", guiStyle);
        }
    }
}
