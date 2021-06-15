using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelHandler : MonoBehaviour
{
    public Transform commandHolder;
    private GameObject _commandImageOrig;
    public Sprite[] sprites;

    public Player player;
    public Player otherPlayer;

    private CameraController _cam;
    
    public string otherInput;
    private int _inputIndex;
    private bool _levelFailed;

    void Awake()
    {
        player.onEndMoving.AddListener(OnPlayerEndMove);
        otherPlayer.onEndMoving.AddListener(OnOtherEndMove);
        _cam = FindObjectOfType<CameraController>();
        _commandImageOrig = commandHolder.GetChild(0).gameObject;

        CreateCommandHUDImages();
        SetUpHUDCommands();
    }

    void Start()
    {
        _cam.SetFillScreen(0);
        _cam.FillScreenTo(1.1f, 1f, Easing.Quadratic.Out);
        DelayFunctionCall(1.1f, () =>
        {
            player.AllowInput(true);
            otherPlayer.AllowInput(true);
        });
    }

    private void CreateCommandHUDImages()
    {
        int loopCount = 0;
        while (loopCount * otherInput.Length < 7)
            loopCount++;

        bool skipFirst = true;

        const int charValue0 = 48;  // the int value for char '0' is 48
        int command = otherInput[0] - charValue0;

        _commandImageOrig.GetComponent<Image>().sprite = sprites[command];

        for (int i = 0; i < loopCount; i++)
        {
            for (int j = 0; j < otherInput.Length; j++)
            {
                if (skipFirst)
                {
                    skipFirst = false;
                    continue;
                }
                GameObject temp = Instantiate(_commandImageOrig, commandHolder);

                command = otherInput[j % otherInput.Length] - charValue0;

                temp.GetComponent<Image>().sprite = sprites[command];
            }
        }
    }

    private void SetUpHUDCommands()
    {
        const int charValue0 = 48;  // the int value for char '0' is 48
        for (int i = 0; i < commandHolder.childCount; i++)
        {
            int command = otherInput[i % otherInput.Length] - charValue0;
            commandHolder.GetChild(i).GetComponent<Image>().sprite = sprites[command];
        }
    }

    void Update()
    {
        if (_levelFailed)
        {
            if (Input.anyKeyDown)
            {
                Reset();
            }
        }
        else
        {
            // Check if chain hits a spike
            if (player.isMoving || otherPlayer.isMoving)
            {
                RaycastHit2D[] hits = Physics2D.LinecastAll(player.transform.position, otherPlayer.transform.position);
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.transform.CompareTag("Spike"))
                    {
                        FailLevel(hit.point);
                        return;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Reset();
            }
        }
    }

    public void FailLevel(Vector3 point)
    {
        _levelFailed = true;

        _cam.MoveTo(point, 1f, Easing.Quadratic.InOut);
        _cam.ZoomTo(3, 1f, Easing.Quadratic.InOut);
        _cam.FillScreenTo(0.3f, 1f, Easing.Quadratic.InOut);

        player.StopAllCoroutines();
        otherPlayer.StopAllCoroutines();
        player.canMove = player.isMoving = false;
        otherPlayer.canMove = otherPlayer.isMoving = false;
    }

    private void Reset()
    {
        BeginReset();
    }

    private void BeginReset()
    {
        _inputIndex = 0;

        player.StopAllCoroutines();
        otherPlayer.StopAllCoroutines();
        player.canMove = player.isMoving = false;
        otherPlayer.canMove = otherPlayer.isMoving = false;

        _cam.FillScreenTo(0, 0.25f, Easing.Quadratic.In);
        DelayFunctionCall(0.25f, MidReset);
    }

    private void MidReset()
    {
        player.Reset();
        otherPlayer.Reset();
        _cam.Reset();

        _levelFailed = false;

        SetUpHUDCommands();

        DelayFunctionCall(0.5f, EndReset);
    }

    private void EndReset()
    {
        _cam.FillScreenTo(1.1f, 1f, Easing.Quadratic.Out);
        DelayFunctionCall(1f, () =>
        {
            player.AllowInput(true);
            otherPlayer.AllowInput(true);
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

    public void OnPlayerEndMove()
    {
        CheckForLevelCompleted();
        SendInputToOtherPlayer();
        UpdateCommandUI();
    }

    public void OnOtherEndMove()
    {
        CheckForLevelCompleted();
        SetPlayerToCanMove();
    }

    private void UpdateCommandUI()
    {
        //commandHolder.GetChild(0).SetAsLastSibling();
        StartCoroutine(UpdateCommandUIOverTime());
    }

    private IEnumerator UpdateCommandUIOverTime()
    {
        float timer = 0;
        const float time = 0.3f;

        RectTransform command = commandHolder.GetChild(0).GetComponent<RectTransform>();
        Vector3 angle = Vector3.zero;

        Vector2 startSize = new Vector2(200, 200);
        Vector2 sizeDelta = startSize;
        const float targetDelta = -30f;

        while (timer <= time)
        {
            timer += Time.deltaTime;
            float k = timer / time;
            float t = Easing.Cubic.InOut(k);

            angle.x = t * 180;
            command.eulerAngles = angle;

            yield return null;
        }

        timer = 0;

        while (timer <= time)
        {
            timer += Time.deltaTime;
            float k = timer / time;
            float t = Easing.Cubic.InOut(k);

            sizeDelta.x = Mathf.Lerp(startSize.x, targetDelta, t);
            command.sizeDelta = sizeDelta;

            yield return null;
        }

        commandHolder.GetChild(0).SetAsLastSibling();
        command.eulerAngles = Vector3.zero;
        command.sizeDelta = startSize;
    }

    public void SetPlayerToCanMove()
    {
        player.canMove = true;
    }

    public void SendInputToOtherPlayer()
    {
        const int charValue0 = 48;  // the int value for char '0' is 48
        int command = otherInput[_inputIndex % otherInput.Length] - charValue0;

        player.canMove = false;
        otherPlayer.Move(command);

        _inputIndex++;
    }

    public void CheckForLevelCompleted()
    {
        foreach (GoalMarker goalMarker in FindObjectsOfType<GoalMarker>())
        {
            if (!goalMarker.isOverlapping)
            {
                return;
            }
        }

        player.StopAllCoroutines();
        otherPlayer.StopAllCoroutines();
        player.canMove = player.isMoving = false;
        otherPlayer.canMove = otherPlayer.isMoving = false;

        const float time = 0.3f;
        _cam.FillScreenTo(0, time, Easing.Quadratic.InOut);
        
        DelayFunctionCall(time, () =>
        {
            int i = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(i + 1);
        });
    }
}

