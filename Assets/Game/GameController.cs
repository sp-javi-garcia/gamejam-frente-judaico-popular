using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public float LevelTime;
    UI3dController _ui3dController;
    ZombieSquad _zombieSquad;
    HumanBase _humanBase;
    float _elapsedTime;

    public static GameController Instance;

    // Use this for initialization
    void Start ()
    {
        Application.LoadLevelAdditive("main_ui");
        Instance = this;
        _ui3dController = FindObjectOfType<UI3dController>();
        _zombieSquad = FindObjectOfType<ZombieSquad>();
        _humanBase = FindObjectOfType<HumanBase>();
        _elapsedTime = 0f;
    }

    bool _gameOver;
    void TimeUp()
    {
        if (!_gameOver)
        {
            _gameOver = true;
            ShowYouLose();
        }
    }

    void OutOfZombies()
    {
        if (!_gameOver)
        {
            _gameOver = true;
        }
    }

    public void ZombiesArrived()
    {
        if (!_gameOver)
        {
            _gameOver = true;
            if (_zombieSquad.Zombies.Count >= _humanBase.RequiredZombies)
            {
                ShowYouWin();
            }
            else
            {
                ShowYouLose();
            }
        }
    }

    void ShowYouLose()
    {
        UIManager.Instance.YouLosePanel.Show();
        _ui3dController.Hide();
        Debug.Log("You lose");
    }

    void ShowYouWin()
    {
        UIManager.Instance.YouWinPanel.Show();
        _ui3dController.Hide();
        Debug.Log("You win");
    }

    void Update ()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= LevelTime)
        {
            TimeUp();
        }
    }
}
