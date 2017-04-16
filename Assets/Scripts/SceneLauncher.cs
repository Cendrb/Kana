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
        if (this.currentOperation == null || this.currentOperation.isDone)
        {
            this.currentOperation = SceneManager.LoadSceneAsync(sceneName);
            this.currentOperation.allowSceneActivation = true;
        }
        else
        {
            Debug.Log("Already opening a scene");
        }
    }

    private void Start()
    {
        if (this.loadingOverlayTexture == null)
        {
            throw new ArgumentException("loadingOverlayTexture cannot be null");
        }

        this.guiStyle = new GUIStyle {fontSize = 69, alignment = TextAnchor.LowerRight};
    }

    private void OnGUI()
    {
        if (this.currentOperation != null && !this.currentOperation.isDone)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.loadingOverlayTexture);
            GUI.Label(new Rect(Screen.width - 400, Screen.height - 100, 400, 100), "Loading...", this.guiStyle);
        }
    }
}
