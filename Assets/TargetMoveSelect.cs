using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMoveSelect : MonoBehaviour
{
    [SerializeField] private TargetSpawner targetSpawner;
    [SerializeField] private int moveSelect;

    private void Start()
    {
        if (moveSelect == 0)
        {
            ChangeMoveMode();
        }
    }

    public void ChangeMoveMode()
    {
        if (targetSpawner.spawnTargetsOn)
        {
            return;
        }
        targetSpawner.movement = moveSelect;
        int currentIndex = transform.GetSiblingIndex();
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            Transform sibling = transform.parent.GetChild(i);
            if (sibling.GetSiblingIndex() != currentIndex)
            {
                if(sibling.GetComponent<TargetMoveSelect>())
                {
                    sibling.GetComponent<TargetMoveSelect>().ReturnButton();
                }
            }
        }
        SelectButton();
    }

    private void ReturnButton()
    {
        if (targetSpawner.movement != moveSelect)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.white;
        }
    }

    private void SelectButton()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.red;
    }
}
