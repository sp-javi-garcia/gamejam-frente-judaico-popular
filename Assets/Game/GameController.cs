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

    public void OutOfZombies()
    {
        if (!_gameOver)
        {
            _gameOver = true;
            ShowYouLose();
        }
    }

    public void ZombiesArrived()
    {
        if (!_gameOver)
        {
            _gameOver = true;

            GameObject cameraGO = Camera.main.gameObject;
            ZombieCameraController cameraController = _zombieSquad.GetComponent<ZombieCameraController>();
            Vector3 offset = cameraController.DistanceToCamera;
            Destroy(cameraController);

            EndCameraController endCameraController = cameraGO.AddComponent<EndCameraController>();
            Vector3 finalOffset = new Vector3(10f, 10f, 10f);
            endCameraController.Init(offset, _humanBase.transform.position, finalOffset, () => {
                if (_zombieSquad.Zombies.Count >= _humanBase.RequiredZombies)
                {
                    ShowYouWin();
                }
                else
                {
                    ShowYouLose();
                }
            });
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
    }

    void Update ()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= LevelTime)
        {
            TimeUp();
        }

        float remainingTime = Mathf.Max(0f, LevelTime - _elapsedTime);
        int minutes = (int)remainingTime / 60;
        int seconds = (int)remainingTime % 60;
        UI3dController.Instance.RemainingTimeText.text = minutes.ToString() + ":" + seconds.ToString("00");
    }
}
