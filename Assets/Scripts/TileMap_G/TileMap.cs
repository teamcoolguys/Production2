//Created by Jack Ng
//Production 1
//Updated by Dylan Fraser
//Production 1

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TileMap : MonoBehaviour {
	
	public int size_x = 100;
	public int size_z = 50;
	public float tileSize = 1.0f;
	public DTileMap MapInfo;

	public Texture2D terrainTiles;
	public int tileResolution;
	public Color[][] tiles;
	public Color[] tilesRow;
	Texture2D texture;
	public int i = 0;

	public void UpdateTexture(int x, int y)
	{
		int tile = (int)MapInfo.GetTileType (x, y);
		//Debug.Log (tile);
		tilesRow = tiles[tile];
		texture.SetPixels(x*tileResolution, y*tileResolution, tileResolution, tileResolution, tilesRow);
		texture.Apply();
	}
	// Use this for initialization
	void Start ()
	{
		MapInfo = new DTileMap(size_x, size_z);
		BuildMesh();
	}

	Color[][] ChopUpTiles()
	{
		int numTilesPerRow = terrainTiles.width / tileResolution;
		int numRows = terrainTiles.height / tileResolution;
		
		Color[][] tiles = new Color[numTilesPerRow*numRows][];
		
		for(int y=0; y<numRows; y++)
		{
			for(int x=0; x<numTilesPerRow; x++)
			{
				tiles[y*numTilesPerRow + x] = terrainTiles.GetPixels( x*tileResolution , y*tileResolution, tileResolution, tileResolution );
			}
		}

		return tiles;
	}

	void BuildTexture()
	{
		int texWidth = size_x * tileResolution;
		int texHeight = size_z * tileResolution;
		texture = new Texture2D(texWidth, texHeight);
		
		tiles = ChopUpTiles();
		
		for(int y = 0; y < size_z; y++) 
		{
			for(int x = 0; x < size_x; x++) 
			{
				tilesRow = tiles[ (int)MapInfo.GetTileType(x,y) ];
				texture.SetPixels(x*tileResolution, y*tileResolution, tileResolution, tileResolution, tilesRow);
			}
		}
		
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.Apply();
		
		MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
		Material newMaterial = new Material(mesh_renderer.sharedMaterial);
		newMaterial.mainTexture = texture;
		//Debug.Log (texture.GetInstanceID());
		mesh_renderer.material = newMaterial;
		
 		Debug.Log ("Done Texture!");
	}
	
	public void BuildMesh()
	{
		if(MapInfo != null)
		{
			int numTiles = size_x * size_z;
			int numTris = numTiles * 2;
			
			int vsize_x = size_x + 1;
			int vsize_z = size_z + 1;
			int numVerts = vsize_x * vsize_z;
			
			// Generate the mesh data
			Vector3[] vertices = new Vector3[ numVerts ];
			Vector3[] normals = new Vector3[numVerts];
			Vector2[] uv = new Vector2[numVerts];
			
			int[] triangles = new int[ numTris * 3 ];

			int x, z;
			for(z=0; z < vsize_z; z++) 
			{
				for(x=0; x < vsize_x; x++)
				{
					vertices[ z * vsize_x + x ] = new Vector3( x*tileSize, 0, (size_z-z)*tileSize );
					normals[ z * vsize_x + x ] = Vector3.up;
					uv[ z * vsize_x + x ] = new Vector2( (float)x / size_x, (float)(size_z-z) / size_z );
				}
			}
			//Debug.Log ("Done Verts!");
			
			for(z=0; z < size_z; z++) 
			{
				for(x=0; x < size_x; x++)
				{
					int squareIndex = z * size_x + x;
					int triOffset = squareIndex * 6;
					triangles[triOffset + 0] = z * vsize_x + x + 		   0;
					triangles[triOffset + 2] = z * vsize_x + x + vsize_x + 0;
					triangles[triOffset + 1] = z * vsize_x + x + vsize_x + 1;
					
					triangles[triOffset + 3] = z * vsize_x + x + 		   0;
					triangles[triOffset + 4] = z * vsize_x + x + 		   1;
					triangles[triOffset + 5] = z * vsize_x + x + vsize_x + 1;
				}
			}
			
			//Debug.Log ("Done Triangles!");
			
			// Create a new Mesh and populate with the data
			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.normals = normals;
			mesh.uv = uv;
			
			// Assign our mesh to our filter/renderer/collider
			MeshFilter mesh_filter = GetComponent<MeshFilter>();
			MeshCollider mesh_collider = GetComponent<MeshCollider>();
			
			mesh_filter.mesh = mesh;
			mesh_collider.sharedMesh = mesh;
			Debug.Log ("Done Mesh!");
			
			BuildTexture();
		}
		else
		{
			Debug.Log ("Map Empty");
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			//Debug.Log ("Writing");
			//We own this player: send the others our data
		}
		else
		{
			//Debug.Log ("Receiving");
			//Network player, receive data
		}
	}
	
}