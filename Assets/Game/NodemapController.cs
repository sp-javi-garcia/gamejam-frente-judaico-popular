﻿using UnityEngine;
using System.Collections;

public class NodemapController : MonoBehaviour
{
    public void OnLevel1Clicked()
    {
        Application.LoadLevel("test_level");
    }

    public void OnLevel2Clicked()
    {
        Application.LoadLevel("area_02");
    }

    public void OnLevel3Clicked()
    {
        Application.LoadLevel("test_level");
    }

    public void OnLevel4Clicked()
    {
        Application.LoadLevel("test_level");
    }

    public void OnInfoButtonClicked()
    {
        Application.LoadLevel("credits");
    }
}
