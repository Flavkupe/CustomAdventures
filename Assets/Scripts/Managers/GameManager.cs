using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonObject<GameManager>
{
    public bool IsPaused = false;

    public float MousedownSpeedMultiplier = 3.0f;

    private GameState state;

    public GameState State
    {
        get
        {
            return state;
        }

        set
        {
            state = value;
        }
    }

    // Use this for initialization
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update () {
	}

    public float GetMouseDownSpeedMultiplier()
    {
        return Input.GetMouseButton(0) ? this.MousedownSpeedMultiplier : 1.0f;
    }    
}

public enum GameState
{
    Neutral,
    DrawingCard,
    PlayerTurn,
    CharacterMoving,
}