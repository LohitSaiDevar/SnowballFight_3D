using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayerController player1;
    [SerializeField] PlayerController player2;
    public bool IsGameOver {  get; set; }
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    private void OnEnable()
    {
        GameEvents.OnDefeat += PlayDefeat;
        GameEvents.OnDefeat += PlayerVictory;
        GameEvents.OnVictory += PlayVictory;
    }

    private void OnDisable()
    {
        GameEvents.OnDefeat -= PlayDefeat;
        GameEvents.OnDefeat -= PlayerVictory;
        GameEvents.OnVictory -= PlayVictory;
    }

    void GameOver()
    {
        Debug.Log("Game Over");
        GameEvents.OnGameOver?.Invoke();
    }

    void PlayerVictory(PlayerController defeatedPlayer)
    {
        IsGameOver = true;
        UIManager.Instance.DisplayGameOverMenuUI();
        if (defeatedPlayer == player1)
        {
            Debug.Log("Player 2 wins");
            GameEvents.OnVictory?.Invoke(player2);
            if (UIManager.Instance.playerWinText != null)
            {
                UIManager.Instance.playerWinText.text = "Player Two";
            }
        }
        else if (defeatedPlayer == player2)
        {
            Debug.Log("Player 1 wins");
            GameEvents.OnVictory?.Invoke(player1);
            if (UIManager.Instance.playerWinText != null)
            {
                UIManager.Instance.playerWinText.text = "Player One";
            }
        }
    }
    void PlayDefeat(PlayerController player)
    {
        Debug.Log("Play defeat");
        player.ChangeAnimationState(PlayerController.Animation_Defeat);
    }
    void PlayVictory(PlayerController player)
    {
        Debug.Log("Play Victory");
        player.SetAsWinner();
    }
}
