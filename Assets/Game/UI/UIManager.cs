using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

    public YouWinPanel YouWinPanel;
    public YouLosePanel YouLosePanel;
    public MessagePanel MessagePanel;
    public ZombieCountProgressBarPanel ZombieCountProgressBarPanel;

    public static UIManager Instance;

    void Start ()
    {
        Instance = this;
    }
}
