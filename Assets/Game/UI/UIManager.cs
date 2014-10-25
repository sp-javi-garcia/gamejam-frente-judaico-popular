using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

    public YouWinPanel YouWinPanel;
    public YouLosePanel YouLosePanel;

    public static UIManager Instance;

    void Start ()
    {
        Instance = this;
    }
}
