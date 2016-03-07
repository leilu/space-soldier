﻿using UnityEngine;
using UnityEngine.UI;

public class TutorialEngine : MonoBehaviour {
    [SerializeField]
    private TutorialState initialState;
    [SerializeField]
    private Text tutorialConsoleText;
    [SerializeField]
    private Text continueTextContainer;
    [SerializeField]
    private CanvasGroup fader;

    public static TutorialEngine Instance;
    public static bool SkillNodesDisabled = false;

    private TutorialState currentState;
    private Text continueText;

    void Awake ()
    {
        Instance = this;
        currentState = initialState;
        currentState.Initialize();
        SkillNodesDisabled = true;
    }

    void Update()
    {
        currentState.Update();

        if (Input.GetButtonDown("ConfirmInstruction"))
        {
            currentState.ConfirmInstruction();
        }
    }

	public void Trigger(TutorialTrigger trigger)
    {
        currentState.Trigger(trigger);
    }

    public void ChangeState(TutorialState nextState)
    {
        currentState = nextState;
        currentState.Initialize();
    }

    public void EndTutorial ()
    {
        Application.LoadLevel(0);
        TutorialEngine.SkillNodesDisabled = false;
        GameState.TutorialMode = false;
    }

    public void RenderText (string textToRender)
    {
        tutorialConsoleText.text = textToRender;
    }

    public void ShowContinueText()
    {
        if (!continueText)
        {
            continueText = continueTextContainer.GetComponent<Text>();
        }
        continueText.gameObject.SetActive(true);
    }

    public void HideContinueText()
    {
        continueText.gameObject.SetActive(false);
    }

    public void Fade ()
    {
        fader.alpha = 1;
    }

    public void Unfade ()
    {
        fader.alpha = 0;
    }
}
