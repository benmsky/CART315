using System.Collections.Generic;
using UnityEngine;

// Covers the ground in 3D grass when the game starts.
// The grass is built while the game runs, so you only see it in Play mode.
public class GrassField : MonoBehaviour
{
    public Material grassMaterial;
    public Texture2D groundMask; // bright parts are dirt, so no grass grows there

    public float fieldSize = 116f;  // metres across
    public float chunkSize = 12f;   // the field is split into squares so Unity can skip the ones you can't see
    public float tuftsPerSquareMetre = 10f;
    public float shortestTuft = 0.25f;
    public float tallestTuft = 0.55f;
    public float tuftWidth = 0.6f;

    public Color dryGrass = new Color(0.40f, 0.38f, 0.22f);
    public Color greenGrass = new Color(0.22f, 0.29f, 0.15f);

    // no grass inside these boxes
    public Bounds cabinArea = new Bounds(new Vector3(0f, 0f, 1.6f), new Vector3(13.4f, 4f, 14.6f));
    public Bounds carArea = new Bounds(new Vector3(6.5f, 0f, 16f), new Vector3(4.5f, 4f, 6.5f));

    private const float MapSize = 140f; // the size the ground mask covers

    void Start()
    {
        BuildGrass();
    }

    public void BuildGrass()
    {
        Random.InitState(1981); // same seed = same grass every time

        int chunks = Mathf.CeilToInt(fieldSize / chunkSize);
        for (int x = 0; x < chunks; x++)
        {
            for (int z = 0; z < chunks; z++)
            {
                Vector2 corner = new Vector2(x, z) * chunkSize - Vector2.one * fieldSize / 2f;
                BuildChunk(corner);
            }
        }
    }

    void BuildChunk(Vector2 corner)
    {
        List<Vector3> points = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Color> colours = new List<Color>();
        List<int> triangles = new List<int>();

        int tufts = Mathf.RoundToInt(chunkSize * chunkSize * tuftsPerSquareMetre);
        for (int i = 0; i < tufts; i++)
        {
            float x = corner.x + Random.Range(0f, chunkSize);
            float z = corner.y + Random.Range(0f, chunkSize);
            if (GrassCanGrow(x, z))
            {
                AddTuft(new Vector3(x, 0f, z), points, uvs, colours, triangles);
            }
        }

        if (points.Count == 0) return;

        Mesh mesh = new Mesh();
        mesh.SetVertices(points);
        mesh.SetUVs(0, uvs);
        mesh.SetColors(colours);
        mesh.SetTriangles(triangles, 0);
        mesh.normals = UpNormals(points.Count); // lit like the ground, so the grass blends into it
        mesh.RecalculateBounds();

        GameObject chunk = new GameObject("Grass Chunk");
        chunk.transform.SetParent(transform, false);
        chunk.AddComponent<MeshFilter>().sharedMesh = mesh;
        MeshRenderer meshRenderer = chunk.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = grassMaterial;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    bool GrassCanGrow(float x, float z)
    {
        Vector3 spot = new Vector3(x, 0f, z);
        if (cabinArea.Contains(spot) || carArea.Contains(spot)) return false;

        float dirt = groundMask.GetPixelBilinear(x / MapSize + 0.5f, z / MapSize + 0.5f).r;
        return dirt < 0.3f;
    }

    // one tuft is two flat cards crossed like an X, so it looks full from every side
    void AddTuft(Vector3 spot, List<Vector3> points, List<Vector2> uvs, List<Color> colours, List<int> triangles)
    {
        float angle = Random.Range(0f, 180f);
        float height = Random.Range(shortestTuft, tallestTuft);
        Color colour = Color.Lerp(dryGrass, greenGrass, Random.value) * Random.Range(0.8f, 1.15f);
        int picture = Random.Range(0, 4); // the blade texture has 4 different tufts side by side

        AddCard(spot, angle, height, picture, colour, points, uvs, colours, triangles);
        AddCard(spot, angle + 90f, height, picture, colour, points, uvs, colours, triangles);
    }

    void AddCard(Vector3 spot, float angle, float height, int picture, Color colour,
        List<Vector3> points, List<Vector2> uvs, List<Color> colours, List<int> triangles)
    {
        int first = points.Count;
        Vector3 side = Quaternion.Euler(0f, angle, 0f) * Vector3.right * tuftWidth / 2f;

        points.Add(spot - side);
        points.Add(spot + side);
        points.Add(spot + side + Vector3.up * height);
        points.Add(spot - side + Vector3.up * height);

        float left = picture / 4f, right = (picture + 1) / 4f;
        uvs.Add(new Vector2(left, 0f));
        uvs.Add(new Vector2(right, 0f));
        uvs.Add(new Vector2(right, 1f));
        uvs.Add(new Vector2(left, 1f));

        for (int i = 0; i < 4; i++) colours.Add(colour);

        triangles.Add(first); triangles.Add(first + 2); triangles.Add(first + 1);
        triangles.Add(first); triangles.Add(first + 3); triangles.Add(first + 2);
    }

    Vector3[] UpNormals(int count)
    {
        Vector3[] normals = new Vector3[count];
        for (int i = 0; i < count; i++) normals[i] = Vector3.up;
        return normals;
    }
}
