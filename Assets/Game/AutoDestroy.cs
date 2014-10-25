using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour
{
    public float DestroyTime = 5f;

    void Update ()
    {
        DestroyTime -= Time.deltaTime;
        if (DestroyTime <= 0f)
            Destroy(gameObject);
    }
}
