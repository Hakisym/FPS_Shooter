using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenu : MonoBehaviour
{
    public bool freezOnPause = true;
    public Transform UI;

    public UnityEvent OnPause;
    public UnityEvent OnResume;
    public Action<bool> OnPausedStateChanged;

    private CanvasGroup canvasGroup;
    
    public bool paused { get; set; }

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        OnPausedStateChanged += HandlePause;
    }

    private void OnDisable()
    {
        OnPausedStateChanged -= HandlePause;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
            OnPausedStateChanged?.Invoke(paused);
        }
        canvasGroup.alpha = paused ? Mathf.Lerp(canvasGroup.alpha, 1, Time.unscaledDeltaTime * 10) : 0;
    }

    private void HandlePause(bool paused)
    {
        UI.gameObject.SetActive(paused);
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;

        if (paused) OnPause?.Invoke();
        if (!paused) OnResume?.Invoke();

        if (freezOnPause) Time.timeScale = paused ? 0 : 1;
    }

    public void Resume()
    {
        paused = false;
        OnPausedStateChanged?.Invoke(paused);
    }

    public void Quit() => Application.Quit();
}
