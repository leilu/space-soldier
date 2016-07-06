﻿using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
    [SerializeField]
    private Texture2D crosshair1X;
    [SerializeField]
    private Texture2D crosshair2X;
    [SerializeField]
    private Texture2D crosshair3X;
    [SerializeField]
    private Texture2D crosshair4X;
    [SerializeField]
    private float initialMaxScrollFraction;
    [SerializeField]
    private float cameraEdgeThreshold;

    public delegate void CameraFunction();
    private Rigidbody2D rb;

    private static float DEFAULT_Z = -10;
    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;
    private float lastScreenHeight;
    private CameraEvent cameraEvent;
    private float maxScrollFraction;

    void Start()
    {
        rb = GameObject.Find("Soldier").GetComponent<Rigidbody2D>();
        transform.position = rb.position;
        maxScrollFraction = initialMaxScrollFraction;
    }

    void Update()
    {
        if (Screen.height != lastScreenHeight)
        {
            lastScreenHeight = Screen.height;
            int scalingFactor;
            Texture2D cursor;
            if (Screen.height < 540)
            {
                cursor = crosshair1X;
                scalingFactor = 1;
            } else if (Screen.height < 720)
            {
                cursor = crosshair2X;
                scalingFactor = 2;
            } else if (Screen.height < 1080)
            {
                cursor = crosshair3X;
                scalingFactor = 3;
            } else
            {
                cursor = crosshair4X;
                scalingFactor = 4;
            }
            Camera.main.orthographicSize = Screen.height / (24f * 2f * scalingFactor);
            Cursor.SetCursor(cursor, new Vector2(cursor.width / 2, cursor.width / 2), CursorMode.ForceSoftware);
        }
    }

    void FixedUpdate () {
        Vector3 targetPosition = cameraEvent == null || cameraEvent.Completed ? rb.position : cameraEvent.Target;
        float activeDampTime = cameraEvent == null ? dampTime : cameraEvent.DampTime;
        targetPosition.z = DEFAULT_Z;

        if (cameraEvent == null)
        {
            HandleMouseLookaround(ref targetPosition);
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, activeDampTime);

        if (cameraEvent != null)
        {
            if (cameraEvent.Completed && ((Vector2)transform.position - rb.position).sqrMagnitude < .5f)
            {
                // Must grab this reference or else the closure will refer to null cameraEvent, causing an exception.
                CameraFunction func = cameraEvent.EndFunction;
                StartCoroutine(Do(cameraEvent.EndWaitTime, func));
                GameState.UnlockInputs();
                cameraEvent = null;
            }
            else if (cameraEvent.Started == false && (cameraEvent.Target - (Vector2)transform.position).sqrMagnitude < 1f)
            {
                StartCoroutine(Do(cameraEvent.FocusHeadWindow, () => cameraEvent.CameraFunction()));
                cameraEvent.Started = true;
            }
        }
    }

    void HandleMouseLookaround(ref Vector3 targetPosition)
    {
        float screenHeightUnits = Screen.height / (Camera.main.orthographicSize * 2);

        if (Input.mousePosition.y >= Screen.height * cameraEdgeThreshold)
        {
            float multiplier = (Input.mousePosition.y - Screen.height * cameraEdgeThreshold) / (Screen.height - Screen.height * cameraEdgeThreshold);
            targetPosition.y += multiplier * maxScrollFraction * screenHeightUnits;
        }

        if (Input.mousePosition.y <= Screen.height * (1 - cameraEdgeThreshold))
        {
            float multiplier = (Screen.height * (1 - cameraEdgeThreshold) - Input.mousePosition.y) / (Screen.height * (1 - cameraEdgeThreshold));
            targetPosition.y -= multiplier * maxScrollFraction * screenHeightUnits;
        }

        if (Input.mousePosition.x >= Screen.width * cameraEdgeThreshold)
        {
            float multiplier = (Input.mousePosition.x - Screen.width * cameraEdgeThreshold) / (Screen.width - Screen.width * cameraEdgeThreshold);
            targetPosition.x += multiplier * maxScrollFraction * screenHeightUnits;
        }

        if (Input.mousePosition.x <= Screen.width * (1 - cameraEdgeThreshold))
        {
            float multiplier = (Screen.width * (1 - cameraEdgeThreshold) - Input.mousePosition.x) / (Screen.width * (1 - cameraEdgeThreshold));
            targetPosition.x -= multiplier * maxScrollFraction * screenHeightUnits;
        }
    }

    public void LoadCameraEvent(CameraFunction cameraFunction, CameraFunction endFunction, float startWaitTime,
            float endWaitTime, Vector2 target, float dampTime, float focusHeadWindow, float focusTailWindow) {
        StartCoroutine(Do(startWaitTime, () => {
                cameraEvent = new CameraEvent(cameraFunction, endFunction,
                    endWaitTime, target, dampTime, focusHeadWindow, focusTailWindow);
                GameState.LockInputs();
            }
        ));
    }

    public void UnloadCameraEvent()
    {
        StartCoroutine(Do(cameraEvent.FocusTailWindow, () => cameraEvent.Completed = true));
    }

    public void SetMaxScrollFraction(float newMaxScrollFraction)
    {
        maxScrollFraction = newMaxScrollFraction;
    }

    public void ResetMaxScrollFraction()
    {
        maxScrollFraction = initialMaxScrollFraction;
    }

    IEnumerator Do(float waitTime, CameraFunction func)
    {
        yield return new WaitForSeconds(waitTime);
        func();
    }

    // Remember always to unload the camera event when done.
    public class CameraEvent {
        public CameraFunction CameraFunction;
        public CameraFunction EndFunction;
        public Vector2 Target;
        public float EndWaitTime;
        public float FocusHeadWindow;
        public float FocusTailWindow;
        public float DampTime;
        public bool Started;
        public bool Completed;

        public CameraEvent(CameraFunction cameraFunction, CameraFunction endFunction, float endWaitTime, Vector2 target, float dampTime,
            float focusHeadWindow, float focusTailWindow)
        {
            CameraFunction = cameraFunction;
            EndFunction = endFunction;
            Target = target;
            EndWaitTime = endWaitTime;
            Started = false;
            Completed = false;
            DampTime = dampTime;
            FocusHeadWindow = focusHeadWindow;
            FocusTailWindow = focusTailWindow;
        }
    }
}