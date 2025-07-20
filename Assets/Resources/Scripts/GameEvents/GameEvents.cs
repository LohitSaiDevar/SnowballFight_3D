using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static Action<PlayerController> OnDefeat;
    public static Action OnGameReset;
    public static Action OnGameOver;
    public static Action<PlayerController> OnVictory;
}
