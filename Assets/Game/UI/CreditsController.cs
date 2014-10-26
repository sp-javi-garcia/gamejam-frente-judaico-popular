using UnityEngine;
using System.Collections;

public class CreditsController : MonoBehaviour
{
    public void OnPressOk()
    {
        Application.LoadLevel("nodemap");
    }
}
