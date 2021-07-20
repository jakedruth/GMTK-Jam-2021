using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    public void OnPlayGame()
    {
        FindObjectOfType<CameraController>().FillScreenTo(0, 1f, Easing.Cubic.In);
        DelayFunctionCall(1.1f, () =>
        {
            int i = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(i + 1);
        });
    }

    public void ReplayGame()
    {
        FindObjectOfType<CameraController>().FillScreenTo(0, 1f, Easing.Cubic.In);
        DelayFunctionCall(1.1f, () =>
        {
            SceneManager.LoadScene(0);
        });
    }

    private void DelayFunctionCall(float delay, Action func)
    {
        StartCoroutine(delayEnumerator(delay, func));
    }

    private IEnumerator delayEnumerator(float delay, Action func)
    {
        yield return new WaitForSeconds(delay);
        func.Invoke();
    }
}
