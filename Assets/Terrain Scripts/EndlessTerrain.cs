using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class EndlessTerrain : MonoBehaviour
{
    private const float scale = 1f;
    public GameObject waterPlane;
    const float viewerMoveThreshold = 25f;
    const float sqrviewerMoveThreshold = viewerMoveThreshold * viewerMoveThreshold;
    public LODInfo[] detailLevels;
    public static float maxViewDst;
   public Transform viewer;
   public Material mapMaterial;
   public static Vector2 viewwePoition;
   private Vector2 viewerPositionOld;
   private static MapGenerator mapGenerator;
   private int chunkSize;
   private int chunksVisibleInViewDst;
   private Dictionary<Vector2, TerrainChunk> terrainChunkDic = new Dictionary<Vector2, TerrainChunk>();
   static List<TerrainChunk> TerrainChunksVisibleLastUpdate = new List<TerrainChunk>();
   private void Awake()
   {
       mapGenerator = FindObjectOfType<MapGenerator>();
       maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshhold;
       chunkSize = MapGenerator.mapChunkSize - 1;
       chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
       mapGenerator.seed = Random.Range(1, 10000);
       UpdateVisibleChunk();
   }

   private void Update()
   {
       viewwePoition = new Vector2(viewer.position.x, viewer.position.z) / scale;
           if ((viewerPositionOld - viewwePoition).sqrMagnitude > sqrviewerMoveThreshold)
           {
               viewerPositionOld = viewwePoition;
               UpdateVisibleChunk();
           }

   }

   void UpdateVisibleChunk()
   {
       for (int i = 0; i < TerrainChunksVisibleLastUpdate.Count; i++)
       {
           TerrainChunksVisibleLastUpdate[i].SetVisible(false);
       }
       TerrainChunksVisibleLastUpdate.Clear();
        
       int currentChunkCoordX = Mathf.RoundToInt(viewwePoition.x / chunkSize);
       int currentChunkCoordY = Mathf.RoundToInt(viewwePoition.y / chunkSize);
       for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
       {
           for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
           {
               Vector2 viewedChuckCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
               if (terrainChunkDic.ContainsKey(viewedChuckCoord))
               {
                   terrainChunkDic[viewedChuckCoord].UpdateTerrainChunk();
            
               }
               else
               {
                   terrainChunkDic.Add(viewedChuckCoord, new TerrainChunk(viewedChuckCoord, chunkSize,detailLevels ,transform, mapMaterial));
               }
           }

       }       
   }

   public class TerrainChunk
   {
       private GameObject meshObject;
       private Vector2 position;
       private Bounds bounds;
       MeshRenderer meshRenderer;
       MeshFilter meshFilter;
       MeshCollider meshCollider;
       LODInfo[] detailLevels;
       LODMesh[] lodMeshes;
       LODMesh collisionLODMesh;
       MapData mapData;
       bool mapDataRecieved;
       int previousLODIndex = -1;
       private GameObject waterPlane;
       private GameObject waterCopy;
       
       
       public TerrainChunk(Vector2 coord, int size,LODInfo[] detailLevels, Transform parent, Material material)
       {
           this.detailLevels = detailLevels;
           position = coord * size;
           bounds = new Bounds(position, Vector2.one * size);
           Vector3 positionV3 = new Vector3(position.x, 0, position.y);
           meshObject = new GameObject("Terrain Chunk");
           waterPlane = GameObject.FindGameObjectWithTag("WaterPlane");
           waterCopy = waterPlane;
           meshRenderer = meshObject.AddComponent<MeshRenderer>();
           meshFilter = meshObject.AddComponent<MeshFilter>();
           meshCollider = meshObject.AddComponent <MeshCollider>();
           meshRenderer.material = material;
           
           meshObject.transform.position = positionV3* scale;
           meshObject.transform.parent = parent;
           meshObject.transform.localScale = Vector3.one * scale;
           GameObject water =Instantiate(waterCopy, position, Quaternion.Euler(0, 0, 0)); 
           water.transform.parent = parent;
           positionV3.y = -3.7f;
           water.transform.position = positionV3* scale;
           SetVisible(false);
           lodMeshes = new LODMesh[detailLevels.Length];
           for (int i = 0; i < detailLevels.Length; i++)
           {
               lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
               if (detailLevels[i].useForCollider)
               {
                   collisionLODMesh = lodMeshes[i];
               }
           }
           mapGenerator.RequestMapData(position, OnMapDataRecieved);
          
          

       }

       void OnMapDataRecieved(MapData mapData)
       {
           this.mapData = mapData;
           mapDataRecieved = true;
           Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.colourMap, MapGenerator.mapChunkSize,
               MapGenerator.mapChunkSize);
           meshRenderer.material.mainTexture = texture;
           UpdateTerrainChunk();
       }
       

       public void UpdateTerrainChunk()
       {
           if (mapDataRecieved)
           {
               float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewwePoition));
               bool visable = viewerDstFromNearestEdge <= maxViewDst;

               if (visable)
               {
                   int lodIndex = 0;
                   for (int i = 0; i < detailLevels.Length - 1; i++)
                   {
                       if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshhold)
                       {
                           lodIndex = i + 1;
                       }
                       else
                       {
                           break;
                       }
                   }

                   if (lodIndex != previousLODIndex)
                   {
                       LODMesh lodMesh = lodMeshes[lodIndex];
                       if (lodMesh.hasMesh)
                       {
                           previousLODIndex = lodIndex;
                           meshFilter.mesh = lodMesh.mesh;
                       }
                       else if (!lodMesh.hasRequestedMesh)
                       {
                           lodMesh.RequestMesh(mapData);
                       }
                   }

                   if (lodIndex == 0)
                   {
                       if (collisionLODMesh.hasMesh)
                       {
                           meshCollider.sharedMesh = collisionLODMesh.mesh;
                       }else if (!collisionLODMesh.hasRequestedMesh)
                       {
                           collisionLODMesh.RequestMesh(mapData);
                       }
                   }
                   TerrainChunksVisibleLastUpdate.Add(this);
               }

               SetVisible(visable);
           }

       }

       public void SetVisible(bool visible)
       {
           if (meshObject)
           {
               meshObject.SetActive(visible);
           }

           if (waterCopy)
           {
               waterCopy.SetActive(visible);
           }

           // if (waterPlane)
           // {
           //     waterPlane.SetActive(visible);
           // }
       }

       public bool IsVisible()
       {
           return meshObject.activeSelf;
       }
       
   }

   class LODMesh
   {
       public Mesh mesh;
       public bool hasRequestedMesh;
       public bool hasMesh;
       private int lod;
       private System.Action updateCallBack;

       public LODMesh(int lod, System.Action updateCallBack)
       {
           this.lod = lod;
           this.updateCallBack = updateCallBack;

       }

       void OnMeshDataRecieved(MeshData meshData)
       {
           mesh = meshData.CreateMesh();
           hasMesh = true;

           updateCallBack();
       }

       public void RequestMesh(MapData mapData)
       {
           hasRequestedMesh = true;
           mapGenerator.RequestMeshData(mapData, lod, OnMeshDataRecieved);
       }
   }
   [System.Serializable]
   public struct LODInfo
   {
       public int lod;
       public float visibleDstThreshhold;
       public bool useForCollider;

   }
}
