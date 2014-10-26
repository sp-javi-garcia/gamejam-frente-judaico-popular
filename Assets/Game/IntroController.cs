using UnityEngine;
using System.Collections;

public class IntroController : MonoBehaviour
{
    public float MaxIntroDelay = 5f;
	void Update ()
    {
        MaxIntroDelay -= Time.deltaTime;
        if (((Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            && GetComponent<CanvasGroup>().alpha > 0.8f) || MaxIntroDelay < 0f)
        {
            Application.LoadLevel("nodemap");
        }
	}
}
