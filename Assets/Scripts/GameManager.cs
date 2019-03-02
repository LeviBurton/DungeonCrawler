using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[System.Serializable]
public enum Turn
{
    Player,
    Enemy
}

public class GameManager : MonoBehaviour
{
    Board m_board;
    PlayerManager m_player;
    Turn m_currentTurn = Turn.Player;

    bool m_hasLevelStarted = false;
    bool m_isGamePlaying = false;
    bool m_isGameOver = false;
    bool m_hasLevelFinished = false;

    public bool HasLevelStarted { get => m_hasLevelStarted; set => m_hasLevelStarted = value; }
    public bool IsGamePlaying { get => m_isGamePlaying; set => m_isGamePlaying = value; }
    public bool IsGameOver { get => m_isGameOver; set => m_isGameOver = value; }
    public bool HasLevelFinished { get => m_hasLevelFinished; set => m_hasLevelFinished = value; }
    public Turn CurrentTurn { get => m_currentTurn; }
    public float delay = 1f;

    public UnityEvent setupEvent;
    public UnityEvent startLevelEvent;
    public UnityEvent playLevelEvent;
    public UnityEvent endLevelEvent;
    public UnityEvent loseLevelEvent;

    void Awake()
    {
        m_board = FindObjectOfType<Board>().GetComponent<Board>();
        m_player = FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();
    }

    void Start()
    {
        if (m_player != null && m_board != null)
        {
            StartCoroutine("RunGameLoop");
        }
        else
        {
            Debug.LogWarning("GameManager Error: No player or board found!");
        }
    }

    IEnumerator RunGameLoop()
    {
        Debug.Log("RunGameLoop");
        yield return StartCoroutine("StartLevelRoutine");
        yield return StartCoroutine("PlayLevelRoutine");
        yield return StartCoroutine("EndLevelRoutine");
    }

    IEnumerator StartLevelRoutine()
    {
        Debug.Log("  StartLevelRoutine");
        if (setupEvent != null)
        {
            setupEvent.Invoke();
        }

       // m_player.playerInput.InputEnabled = false;

        while (!HasLevelStarted)
        {
            // show start screen
            // user presses button to start
            // hasLevelStarted = true
            yield return null;
        }

        if (startLevelEvent != null)
        {
            startLevelEvent.Invoke();
        }
    }

    IEnumerator PlayLevelRoutine()
    {
        Debug.Log("  PlayLevelRoutine");
        IsGamePlaying = true;

        yield return new WaitForSeconds(delay);
       // m_player.playerInput.InputEnabled = true;

        if (playLevelEvent != null)
        {
            playLevelEvent.Invoke();
        }

        while (!IsGameOver)
        {
            yield return null;

           // m_isGameOver = IsWinner();
        }
    }
    public void LoseLevel()
    {
        StartCoroutine(LoseLevelRoutine());
    }

    IEnumerator LoseLevelRoutine()
    {
        m_isGameOver = true;

        yield return new WaitForSeconds(1.5f);

        if (loseLevelEvent != null)
        {
            loseLevelEvent.Invoke();
        }

        yield return new WaitForSeconds(2f);

        Debug.Log("GameManager: Lose Level");

        RestartLevel();
    }

    IEnumerator EndLevelRoutine()
    {
      //  m_player.playerInput.InputEnabled = false;

        // show end screen, win/lose, points earned, etc.
        // level objectives, et.c
        if (endLevelEvent != null)
        {
            endLevelEvent.Invoke();
        }

        while (!HasLevelFinished)
        {
            // user presses button to continue

            // HasLevelFinished = true;
            yield return null;
        }

        RestartLevel();
    }

    void RestartLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    // public method to attach to StartButton
    public void PlayLevel()
    {
        HasLevelStarted = true;
    }

    bool IsWinner()
    {
        //if (m_board.PlayerNode != null)
        //{
        //    return (m_board.PlayerNode == m_board.GoalNode);
        //}

        return false;
    }
    void PlayPlayerTurn()
    {
        m_currentTurn = Turn.Player;
        m_player.IsTurnComplete = false;
    }

    public void UpdateTurn()
    {
        if (m_currentTurn == Turn.Player && m_player != null)
        {
            if (m_player.IsTurnComplete && !AreEnemiesAllDead())
            {
                PlayEnemyTurn();
            }
        }
        else if (m_currentTurn == Turn.Enemy)
        {
            if (IsEnemyTurnComplete())
            {
                PlayPlayerTurn();
            }
        }
    }

    void PlayEnemyTurn()
    {
        m_currentTurn = Turn.Enemy;

        //foreach (var enemy in m_enemies)
        //{
        //    if (enemy != null && !enemy.IsDead)
        //    {
        //        enemy.IsTurnComplete = false;
        //        enemy.PlayTurn();
        //    }
        //}
    }

    bool IsEnemyTurnComplete()
    {
        //foreach (var enemy in m_enemies)
        //{
        //    if (enemy.IsDead)
        //    {
        //        continue;
        //    }

        //    if (!enemy.IsTurnComplete)
        //    {
        //        return false;
        //    }
        //}

        return true;
    }

    bool AreEnemiesAllDead()
    {
        //foreach (var enemy in m_enemies)
        //{
        //    if (!enemy.IsDead)
        //    {
        //        return false;
        //    }
        //}

        return false;
    }
}

