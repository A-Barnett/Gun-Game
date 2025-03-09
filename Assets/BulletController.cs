using System;
using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private float timer;
    public ParticleSystem explosionPrefab;


    private void OnCollisionEnter(Collision collision)
    {
        ParticleSystem explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosion.Play();
        Destroy(gameObject);
    }

    private void Update()
    {
        timer += 1;
        if (timer > 100)
        {
            Destroy(gameObject);
        }
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     MeshFilter meshFilter = collision.gameObject.GetComponent<MeshFilter>();
    //     if (meshFilter != null)
    //     {
    //         Mesh mesh = meshFilter.mesh;
    //         Vector3 collisionPoint = collision.contacts[0].point;
    //         Vector3 collisionNormal = collision.contacts[0].normal;
    //
    //         float dot = Vector3.Dot(collisionNormal, Vector3.up);
    //         Vector3 raycastDirection = Vector3.Cross(collisionNormal, Vector3.up).normalized;
    //             RaycastHit hit;
    //             float rayLength = mesh.bounds.extents.magnitude * 10f;
    //             if (Physics.Raycast(collisionPoint + collisionNormal, raycastDirection, out hit, rayLength))
    //             {
    //                 Renderer renderer = hit.collider.GetComponent<Renderer>();
    //                 if (renderer != null)
    //                 {
    //                     Texture2D texture = (Texture2D)renderer.material.mainTexture;
    //                     Vector2 pixelUV = hit.textureCoord;
    //                     pixelUV.x *= texture.width;
    //                     pixelUV.y *= texture.height;
    //                     Color vertexColor = texture.GetPixel((int)pixelUV.x, (int)pixelUV.y);
    //
    //                     ParticleSystem explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    //                     var mainModule = explosion.main;
    //                     mainModule.startColor = vertexColor;
    //                     explosion.Play();
    //                     Destroy(gameObject);
    //                 
    //             }
    //         }
    //     }
    // }
}

