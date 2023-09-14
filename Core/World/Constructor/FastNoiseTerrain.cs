using UnityEngine;

namespace TOAFL.Core.World {
	public class FastNoiseTerrain : MonoBehaviour { 	
		const int SIZE = 512;
		const float GENERATION_SCALE = 0.02f;
		const float HEIGHT_SCALE = 0.05f;
		
		void Start() {
			var terrain = GetComponent<Terrain>();
			TerrainData terrainData = Instantiate(terrain.terrainData);
			terrainData.heightmapResolution = SIZE + 1;
			terrainData.size = new Vector3(SIZE, SIZE, SIZE);

			// Generate cellular noise (example taken from BitmapGenerator class supplied with the library).
			FastNoise cellular = new FastNoise("CellularDistance");
			cellular.Set("ReturnType", "Index0Add1");
			cellular.Set("DistanceIndex0", 2);
			
			// Generate the heightmap using FastNoise.
			float[] noiseValues = new float[SIZE * SIZE];
            FastNoise.OutputMinMax minMax = cellular.GenUniformGrid2D(noiseValues, 0, 0, SIZE, SIZE, GENERATION_SCALE, Random.Range(int.MinValue, int.MaxValue));

			// Get all noise values to the range from 0 to 1. 
            for(int i = 0; i < noiseValues.Length; i++)
				noiseValues[i] = Mathf.InverseLerp(minMax.min, minMax.max, noiseValues[i]);
		
			// The generated height map is always given as 1D array while TerrainData only accepts 2D. 
			// So we need to convert out values. While doing it, we can also multiply them by necessary height.
			float[,] heights = new float[SIZE, SIZE];
			for (int x = 0; x < SIZE; x++)
				for (int y = 0; y < SIZE; y++)
					heights[x, y] = noiseValues[x + y * SIZE] * HEIGHT_SCALE;
		
			terrainData.SetHeights(0, 0, heights);

			// Assign the TerrainData to the Terrain and TerrainCollider components
			
			terrain.terrainData = terrainData;
			GetComponent<TerrainCollider>().terrainData = terrainData;
		}
	}
}
