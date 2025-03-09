using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeDestroy : MonoBehaviour
{
    private float timer = 0;
    void Update()
    {
        timer += 1;
        if (timer > 20)
        {
            Destroy(gameObject);
        }
    }
}
