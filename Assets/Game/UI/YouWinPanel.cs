using UnityEngine;
using System.Collections;

public class YouWinPanel : MonoBehaviour
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
        Application.LoadLevel("nodemap");
    }
}
