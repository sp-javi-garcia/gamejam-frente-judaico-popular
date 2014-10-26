using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class YouLosePanel : MonoBehaviour
{
    public Text ReasonText;
	// Use this for initialization
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(string reason)
    {
        ReasonText.text = reason;
        gameObject.SetActive(true);
    }

    public void OnRestartGameClicked()
    {
        Application.LoadLevel("nodemap");
    }
}
