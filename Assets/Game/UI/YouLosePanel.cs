using UnityEngine;
using System.Collections;

public class YouLosePanel : MonoBehaviour
{
	// Use this for initialization
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void OnRestartGameClicked()
    {
        Application.LoadLevel("gm14_environment_01_testBrains");
    }
}
