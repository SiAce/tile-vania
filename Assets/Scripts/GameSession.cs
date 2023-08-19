using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameSession : MonoBehaviour
{
    [SerializeField] int initialPlayerLives = 5;
    [SerializeField] int playerLives;
    [SerializeField] int score = 0;
    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI scoreText;


    public static GameSession Instance;
 
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerLives = initialPlayerLives;
    }

    private void Start()
    {
        livesText.text = $"<color=#ff0000ff>Lives</color>: {playerLives}";
        scoreText.text = $"<color=#ffa500ff>Score</color>: {score}";

    }

    public IEnumerator ProcessPlayerDeath()
    {
        yield return new WaitForSeconds(2);

        if (playerLives > 1)
        {
            TakeLife();
        }
        else
        {
            ResetGameSession();
        }
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = $"<color=#ffa500ff>Score</color>: {score}";
    }

    private void TakeLife()
    {
        playerLives--;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        livesText.text = $"<color=#ff0000ff>Lives</color>: {playerLives}";
    }

    private void ResetGameSession()
    {
        playerLives = initialPlayerLives;
        ScenePersist.Instance.Reset();
        SceneManager.LoadScene(0);
    }
}
