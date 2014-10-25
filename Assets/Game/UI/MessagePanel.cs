using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MessagePanel : MonoBehaviour
{
    public Text MessageText;

	// Use this for initialization
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(string message)
    {
        MessageText.text = message;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnRestartGameClicked()
    {
        Application.LoadLevel("gm14_environment_01_testBrains");
    }
}
