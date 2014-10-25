using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ZombieHealthBar : MonoBehaviour
{
    public Image Bar;

    void Start()
    {
        gameObject.SetActive(false);
    }

    IEnumerator HideHealth()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }

    public void SetHealth(int value)
    {
        gameObject.SetActive(true);
        Vector3 scale = new Vector3((float)value/ 3f, 1f, 1f);
        iTween.ScaleTo(Bar.gameObject, iTween.Hash("scale", scale,
                                               "islocal", true,
                                               "easetype", iTween.EaseType.linear,
                                               "time", 1.4f));
        StartCoroutine(HideHealth());
    }

	// Update is called once per frame
	void Update ()
    {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
	}
}
