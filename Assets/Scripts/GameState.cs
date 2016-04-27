﻿using UnityEngine;

public class GameState : MonoBehaviour {
    public static int NumEnemiesRemaining = 0;
    public static bool Paused = false;
    public static bool InputLocked = false;
    public static bool TutorialMode = false;
    public static bool IsBossFight = false;

    // These are for score tracking (to display at the end of the game)
    public static int NumEnemiesKilled = 0;
    public static int LevelIndex = 0;

    // These are for locking inputs during tutorial mode. Maybe I should move these somewhere else.
    public static bool SpaceLocked = false;

    public static void ResetGameState()
    {
        NumEnemiesRemaining = 0;
        Paused = false;
        NumEnemiesKilled = 0;
        LevelIndex = 0;
    }

    public static void PauseGame()
    {
        Paused = true;
        Time.timeScale = 0;
    }

    public static void UnpauseGame()
    {
        Paused = false;
        Time.timeScale = 1;
    }

    public static void LockInputs()
    {
        InputLocked = true;
        Player.PlayerTransform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Player.PlayerTransform.GetComponent<Animator>().enabled = false;
    }

    public static void UnlockInputs()
    {
        Player.PlayerTransform.GetComponent<Animator>().enabled = true;
        InputLocked = false;
    }
}
