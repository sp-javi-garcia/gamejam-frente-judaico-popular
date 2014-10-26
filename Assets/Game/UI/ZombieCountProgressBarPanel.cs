using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ZombieCountProgressBarPanel : MonoBehaviour
{
    public Text ExtraZombiesText;
    public Image Bar;

	// Use this for initialization
    int _initialZombieAmount;
    int _currentZombieCount;
	void Start ()
    {
        _currentZombieCount = _initialZombieAmount = ZombieSquad.Instance.Zombies.Count;
        ExtraZombiesText.gameObject.SetActive(false);
	}

    void ChangeZombieCountAnimated()
    {
        float progress = Mathf.Min(1f, (float)ZombieSquad.Instance.Zombies.Count/(float)_initialZombieAmount);
        if (ZombieSquad.Instance.Zombies.Count > _initialZombieAmount)
        {
            ExtraZombiesText.gameObject.SetActive(true);
            ExtraZombiesText.text = "+" + (ZombieSquad.Instance.Zombies.Count - _initialZombieAmount).ToString();
        }
        else
        {
            ExtraZombiesText.gameObject.SetActive(false);
        }
        iTween.ScaleTo(Bar.gameObject, iTween.Hash("scale", new Vector3(progress, 1f, 1f),
                                                   "islocal", true,
                                                   "easetype", iTween.EaseType.easeOutQuad,
                                                   "time", 0.5f));
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (_currentZombieCount != ZombieSquad.Instance.Zombies.Count)
        {
            _currentZombieCount = ZombieSquad.Instance.Zombies.Count;
            ChangeZombieCountAnimated();
        }
	}

    public void OnClickExit()
    {
        Application.LoadLevel("nodemap");
    }
}
