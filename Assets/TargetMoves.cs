using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class TargetMoves : MonoBehaviour
{
   private bool moveRight;
   private int moveMode;

   private void Start()
   {
      moveRight = Random.Range(0, 2) == 0;
   }

   public void SetMoveMode(int move)
   {
      moveMode = move;
   }

   private void FixedUpdate()
   {
      if (moveMode ==0)
      {
         return;
      }
      if (transform.position.z <= -10)
      {
         moveRight = true;
      }else if (transform.position.z >= 10)
      {
         moveRight = false;
      }
      Vector3 startPos = transform.position;
      if (moveRight)
      {
         if (moveMode == 1)
         {
            startPos.z += 0.01f;
         }else if (moveMode == 2)
         {
            startPos.z += 0.05f;
         }else if (moveMode == 3)
         {
            startPos.z += 0.1f;
         }
      }
      else
      {
         if (moveMode == 1)
         {
            startPos.z -= 0.01f;
         }else if (moveMode == 2)
         {
            startPos.z -= 0.05f;
         }else if (moveMode == 3)
         {
            startPos.z -= 0.1f;
         }
      }
      transform.position = startPos;
   }
}
