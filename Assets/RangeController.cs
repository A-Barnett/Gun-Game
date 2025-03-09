using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeController : MonoBehaviour
{
    [SerializeField] private TargetSpawner targetSpawner;
    [SerializeField] private int rangeMode;

    private void Start()
    {
        if (rangeMode == 4)
        {
            ChangeRange();
        }
    }

    public void ChangeRange()
    {
        if (targetSpawner.spawnTargetsOn)
        {
            return;
        }
        targetSpawner.range = rangeMode;
        int currentIndex = transform.GetSiblingIndex();
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            Transform sibling = transform.parent.GetChild(i);
            if (sibling.GetSiblingIndex() != currentIndex)
            {
                if(sibling.GetComponent<RangeController>())
                {
                    sibling.GetComponent<RangeController>().ReturnButton();
                }
            }
        }
        SelectButton();
    }

    private void ReturnButton()
    {
        if (targetSpawner.range != rangeMode)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.white;
        }
    }

    private void SelectButton()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.red;
    }
}
