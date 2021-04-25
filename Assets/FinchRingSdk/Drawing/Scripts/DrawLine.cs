using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class FMesh
{
    public Mesh mesh;

    public const int INDICES_PER_FACE = 6; // two triangles - quad faces
    public int Faces;
    public int Vertices;
    public float Length;

    public const int DIVIDE = 6; // segments on X axis
    public const int MAX_POSITIONS = DrawLine.SPLINE_POSITIONS_PER_SEGMENT * 512 * 5;
    public const int MAX_VERTS = MAX_POSITIONS * DIVIDE; // not critical, critycal is (2^16 - 'UInt16')
    public const int MAX_INDICES = (MAX_POSITIONS - 1) * INDICES_PER_FACE * (DIVIDE - 1);

    // Minimize garbage
    List<Vector3> verts;
    List<Vector2> uvs;
    List<Vector2> uvs2;
    List<Vector3> centers;
    List<Vector4> normals;
    List<int> indices;

    public Material material;

    Bounds bounds;
    public FMesh(Mesh mesh)
    {
        this.mesh = mesh;

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt16;

        verts = new List<Vector3>(MAX_VERTS);
        uvs = new List<Vector2>(MAX_VERTS);
        uvs2 = new List<Vector2>(MAX_VERTS);
        centers = new List<Vector3>(MAX_VERTS);
        normals = new List<Vector4>(MAX_VERTS);
        indices = new List<int>(MAX_INDICES);

        // use fake bounds for speedup
        bounds = new Bounds(Vector3.zero, Vector3.one * 100);
        mesh.bounds = bounds;
    }

    private static Ray ray = new Ray();
    // Return true, if been cliped - vertex is back of plane
    public static bool ClipVertex(Plane prevPlane, ref Vector3 sideVertex, Vector3 center, Vector3 direction)
    {
        float distance = prevPlane.GetDistanceToPoint(sideVertex);
        if (distance < 0)
        {
            ray.origin = center;
            ray.direction = direction;
            float enter;
            if (prevPlane.Raycast(ray, out enter))
            {
                sideVertex = ray.GetPoint(enter);
            }
            return true;
        }
        return false;
    }

    // [xyz - position, w - length]
    public bool Update(List<FSplinePosition> SplinePositions, int splineStartPosition, out bool bReadyForNextSubline, bool bWorldSpace, float linewidth)
    {
        // Output bool
        bool bBeenUpdateSize = false;

        int targetPositions = SplinePositions.Count - splineStartPosition;
        int targetVerts = targetPositions * DIVIDE;
        int targetIndices = (targetPositions - 1) * INDICES_PER_FACE * (DIVIDE - 1);

        if (targetPositions < 2)
        {
            bReadyForNextSubline = false;
            return bBeenUpdateSize;
        }

        bReadyForNextSubline = targetPositions > MAX_POSITIONS;

        if (verts.Count != targetVerts)
        {
            for (int i = verts.Count; i < targetVerts; ++i)
            {
                verts.Add(Vector3.zero);
                uvs.Add(Vector2.zero);
                uvs2.Add(Vector2.zero);
                centers.Add(Vector3.zero);
                normals.Add(Vector4.zero);
            }

            for (int i = indices.Count / INDICES_PER_FACE; i < targetIndices / INDICES_PER_FACE; ++i)
            {
                int j = i / (DIVIDE - 1);
                int d = i % (DIVIDE - 1);
                //for (int d = 0; d < DIVIDE - 1; ++d)
                {
                    int p0u = ((j + 0) * DIVIDE) + 0 + d;
                    int p0d = ((j + 0) * DIVIDE) + 1 + d;
                    int p1u = ((j + 1) * DIVIDE) + 0 + d;
                    int p1d = ((j + 1) * DIVIDE) + 1 + d;

                    indices.Add(p0u);
                    indices.Add(p1u);
                    indices.Add(p0d);

                    indices.Add(p0d);
                    indices.Add(p1u);
                    indices.Add(p1d);
                }
            }
            bBeenUpdateSize = true;
        }

        int startPos = targetPositions - 3 * DrawLine.SPLINE_POSITIONS_PER_SEGMENT - 1;
        startPos = Mathf.Max(startPos, 0);

        // use plane for clip overlaping edges

        // int prevIndex = 0;
        // int countPrevPlanes = previousPlane.Length;

        float lineRadius = linewidth * 0.5f;
        for (int posIndex = startPos; posIndex < targetPositions; ++posIndex)
        {
            int ofsetedPosIndex = splineStartPosition + posIndex;

            var sp0 = SplinePositions[ofsetedPosIndex];

            Vector3 pL = sp0.position;
            Vector3 p0 = sp0.position;
            Vector3 p1 = Vector4.zero;
            float length = 0;

            Vector3 d0 = sp0.direction;

            if (posIndex == 0)
            {
                Vector3 v1 = p0;
                Vector3 v0 = SplinePositions[ofsetedPosIndex + 1].position;
                pL = v1 + (v1 - v0).normalized * lineRadius;
            }
            else
            {
                // more range, for better quality
                int lastStep = Mathf.Max(0, ofsetedPosIndex - DrawLine.SPLINE_POSITIONS_PER_SEGMENT);
                pL = SplinePositions[lastStep].position;
            }

            if (targetPositions == posIndex + 1)
            {
                Vector3 v1 = p0;
                Vector3 v0 = SplinePositions[ofsetedPosIndex - 1].position;
                p1 = v1 + (v1 - v0).normalized * lineRadius / DrawLine.SPLINE_POSITIONS_PER_SEGMENT;
                length = sp0.length + lineRadius / DrawLine.SPLINE_POSITIONS_PER_SEGMENT;
            }
            else
            {
                p1 = SplinePositions[ofsetedPosIndex + 1].position;
            }

            Vector3 normal = p1 - p0;
            Vector3 normalLast = p0 - pL;

            Vector3 ortho;
            Vector3 direction;
            if (bWorldSpace)
            {
                // use precomputed direction for tangent definition - best look 
                normalLast.Normalize();
                normal.Normalize();

                direction = Vector3.Cross(normal, d0).normalized;
            }
            else
            {
                normalLast.z = 0;
                normalLast.Normalize();

                normal.z = 0;
                normal.Normalize();

                direction = new Vector3(-normal.y, normal.x);
            }
            ortho = direction * lineRadius;

            float angle = Vector3.Angle(normal, normalLast);

            // final vertex
            Vector3 f0 = p0 + ortho;
            Vector3 f1 = p0 - ortho;


            float uv0 = 0;
            float uv1 = 1;


            int j = posIndex * DIVIDE;
            for (int vi = 0; vi < DIVIDE; ++vi)
            {
                float t = vi / (float)(DIVIDE - 1);
                verts[j + vi] = Vector3.Lerp(f0, f1, t);
                uvs[j + vi] = new Vector2(Mathf.Lerp(uv0, uv1, t), sp0.length);
                uvs2[j + vi] = new Vector2(angle, t);
                centers[j + vi] = p0;

                normals[j + vi] = normal;
            }
        }

        mesh.Clear();
        mesh.vertices = verts.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.uv2 = uvs2.ToArray();
        mesh.normals = centers.ToArray();
        mesh.tangents = normals.ToArray();

        return bBeenUpdateSize;
    }

    public void Destroy()
    {
        Material.DestroyImmediate(material);
        material = null;


        mesh.Clear();
        Mesh.DestroyImmediate(mesh);
        mesh = null;


        verts.Clear();
        uvs.Clear();
        uvs2.Clear();
        centers.Clear();
        normals.Clear();
        indices.Clear();

        verts = null;
        uvs = null;
        uvs2 = null;
        centers = null;
        normals = null;
        indices = null;
    }
}

public static class FShaderIDs
{
    public static readonly int g_fLineLength = Shader.PropertyToID("_LineLength");
    public static readonly int g_fLineWidth = Shader.PropertyToID("_LineWidth");

    public static readonly int g_fStylusActive = Shader.PropertyToID("_StylusActive"); // Stylus.shader
    public static readonly int g_fActive = Shader.PropertyToID("_Active");       // Hollowing.shader

    public static readonly int g_UIParticles = Shader.PropertyToID("g_UIParticles");
}


// Could be more params
public class FSublineParams
{
    public int SplineStartPosition = 0;
}


public class FSplineKey
{
    public static FSplineKey zero { get { return new FSplineKey(Vector3.zero, Vector3.forward); } }
    public Vector3 position;
    public Vector3 direction;

    public FSplineKey(Vector3 inPosition, Vector3 inDirection)
    {
        position = inPosition;
        direction = inDirection;
    }
}

public class FSplinePosition
{
    public static FSplinePosition zero { get { return new FSplinePosition(Vector3.zero, Vector3.forward, 0); } }
    public float length;
    public Vector3 position;
    public Vector3 direction;

    public FSplinePosition(Vector3 inPosition, Vector3 inDirection, float inLength)
    {
        position = inPosition;
        direction = inDirection;
        length = inLength;
    }
}

public class DrawLine : MonoBehaviour
{
    public List<FSublineParams> SublineParams = new List<FSublineParams>();

    public List<MeshFilter> MeshFilters = new List<MeshFilter>();
    public List<FMesh> Meshes = new List<FMesh>();
    private static List<DrawLine> lines = new List<DrawLine>();

    class FSplineConsts
    {
        public float RADIUS = 0.01f * 0.2f;
        public float WIDTH = 0.045f * 1.3f;
        public float OFFSET = 0.001f;
    }

    private static FSplineConsts SPLINE_CONSTS_DOF6 = new FSplineConsts();
    private static FSplineConsts SPLINE_CONSTS_DOF3 = new FSplineConsts() { RADIUS = 0.0125f * 0.2f, OFFSET = 0.001f, WIDTH = 0.06f * 1.3f };
    private static FSplineConsts SPLINE_CONSTS(DrawLine line) { return line.bWorldSpace ? SPLINE_CONSTS_DOF6 : SPLINE_CONSTS_DOF3; }
    private bool bWorldSpace;
    private Material lineMaterial;

    private DrawLine()
    {
        lines.Add(this);
    }

#if UNITY_EDITOR
    private static bool bDebugSplineKeys = false;
    private static bool bDebugSplinePositions = true;
#endif
    private void Update()
    {

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha0)) bDebugSplineKeys = !bDebugSplineKeys;
            if (Input.GetKeyDown(KeyCode.Alpha9)) bDebugSplinePositions = !bDebugSplinePositions;
        }

        if (bDebugSplineKeys)
            Debug_SplineKeys();

        if (bDebugSplinePositions)
            Debug_SplinePositions();
#endif

    }

    #region Delete
    public static void DeleteAll()
    {
        foreach (var i in lines)
        {
            if (i != null)
            {

                foreach (var m in i.Meshes)
                    if (m != null)
                        m.Destroy();


                i.Meshes.Clear();
                i.SublineParams.Clear();

                foreach (var j in i.MeshFilters)
                {
                    if (j != null)
                    {
                        DestroyImmediate(j.gameObject);
                    }
                }
                i.MeshFilters.Clear();

                DestroyImmediate(i.gameObject);
            }
        }

        lines.Clear();
    }

    #endregion

    public static DrawLine CreateLine(Vector3 position, Quaternion rotation, Transform gamePrefab, bool planeLine, Material material = null)
    {
        DrawLine line = new GameObject().AddComponent<DrawLine>();
        line.transform.name = "Line";
        line.transform.position = Vector3.zero;
        line.transform.rotation = Quaternion.identity;
        line.transform.parent = gamePrefab;

        line.lineMaterial = material;
        line.CreateSubline();
        line.bWorldSpace = !planeLine;

        return line;
    }

    private void CreateSubline()
    {
        var mesh = new FMesh(new Mesh());

        MeshFilter filter = new GameObject().AddComponent<MeshFilter>();
        filter.transform.position = transform.position;
        filter.transform.rotation = transform.rotation;
        filter.transform.parent = transform;
        filter.transform.name = "SubLine";

        filter.sharedMesh = mesh.mesh;

        MeshRenderer renderer = filter.gameObject.AddComponent<MeshRenderer>();
        mesh.material = new Material(lineMaterial);
        renderer.material = mesh.material;


        MeshFilters.Add(filter);
        SublineParams.Add(new FSublineParams());
        Meshes.Add(mesh);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Update Line Drawing
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private List<FSplineKey> SplineKeys;
    public const int SPLINE_POSITIONS_PER_SEGMENT = 2;
    private List<FSplinePosition> SplinePositions;

    public void UpdateLineDrawing(Vector3 position, Vector3 direction, float radius, Vector2 palelte01Coords, bool ignoreDistance)
    {
        // The rule:
        // 1. Handle List of spline key points

        Handle_PointToSpline_Adding(position, direction, ignoreDistance);

        // 2. Update Spline
        // Recompute only changable spline positions
        // Only Last [3] spline keys intended in 'variable position'
        Handle_Spline();

        // 3. Refresh mesh
        // Update only last subline, all previus are done.
        int idx = SublineParams.Count - 1;
        {
            var sublineParams = SublineParams[idx];
            var mesh = Meshes[idx];
            var meshFilter = MeshFilters[idx];

            bool bReadyForNextSubline;
            if (mesh.Update(SplinePositions, sublineParams.SplineStartPosition, out bReadyForNextSubline, bWorldSpace, SPLINE_CONSTS(this).WIDTH))
            {
                // Is not necessary more
                //meshFilter.sharedMesh = mesh.mesh;
            }

            if (bReadyForNextSubline)
            {
                CreateSubline();

                idx++;
                sublineParams = SublineParams[idx];
                mesh = Meshes[idx];

                sublineParams.SplineStartPosition = SplinePositions.Count;
            }
        }

        // 4. Refresh materials
        {
            var mat = Meshes[idx].material;

            if (SplinePositions.Count > 0)
            {
                // length of last spline position == spline length
                float lineLength = SplinePositions[SplinePositions.Count - 1].length;
                mat.SetFloat(FShaderIDs.g_fLineLength, lineLength);
                mat.SetFloat(FShaderIDs.g_fLineWidth, SPLINE_CONSTS(this).WIDTH);
            }
        }
    }

    #region [Handle] Spline, Spline Updating
    public void Handle_Spline()
    {
        // 2. Update Spline
        // Recompute only changable spline positions
        // Only Last [3] spline keys intended in 'variable position'

        if (SplinePositions == null)
            SplinePositions = new List<FSplinePosition>();

        // If we want to draw a curve that goes through k points, 
        // we need k + 2 control points because the curve is not drawn through the first and the last ones.
        // * For most left & right point add fake point

        int targetPositions = (SplineKeys.Count - 1) * SPLINE_POSITIONS_PER_SEGMENT;
        targetPositions = Mathf.Max(targetPositions, 0);


        int startRecompute = SplinePositions.Count - SPLINE_POSITIONS_PER_SEGMENT * 3;
        startRecompute = Mathf.Max(startRecompute, 0);

        // A) Align SplinePositions
        if (SplinePositions.Count != targetPositions)
        {
            for (int i = SplinePositions.Count; i < targetPositions; ++i)
            {
                // just tmporary 0, in stage (B) will recompute anyway
                SplinePositions.Add(FSplinePosition.zero);
            }
        }

        // B) Recompute
        Vector3 A = Vector3.zero,
                B = Vector3.zero,
                C = Vector3.zero,
                D = Vector3.zero;

        /// direction
        Vector3 Ad = Vector3.zero,
                Bd = Vector3.zero,
                Cd = Vector3.zero,
                Dd = Vector3.zero;

        float L = 0;
        float L0 = startRecompute - 1 > 0 ? SplinePositions[startRecompute - 1].length : 0;
        L = L0;

        // Best look on tension ~ 0, alpha ~ 0.5
        const float tension = 0;
        const float alpha = 0.5f;

        for (int posIndex = startRecompute; posIndex < SplinePositions.Count; ++posIndex)
        {
            int interKeyIndex = posIndex % SPLINE_POSITIONS_PER_SEGMENT;

            // Recompute segment spline parameters 
            if (interKeyIndex == 0)
            {
                int keyIndex0 = posIndex / SPLINE_POSITIONS_PER_SEGMENT;

                Vector3 p0 = Vector3.zero,
                          p1 = Vector3.zero,
                          p2 = Vector3.zero,
                          p3 = Vector3.zero;

                Vector3 d0 = Vector3.zero,
                          d1 = Vector3.zero,
                          d2 = Vector3.zero,
                          d3 = Vector3.zero;


                if (keyIndex0 - 1 < 0)
                {
                    p0 = SplineKeys[keyIndex0 + 0].position + (SplineKeys[keyIndex0 + 0].position - SplineKeys[keyIndex0 + 1].position).normalized * SPLINE_CONSTS(this).RADIUS * 0.15f;
                    d0 = SplineKeys[keyIndex0 + 0].direction + (SplineKeys[keyIndex0 + 0].direction - SplineKeys[keyIndex0 + 1].direction);
                    d0.Normalize();
                }
                else
                {
                    p0 = SplineKeys[keyIndex0 - 1].position;
                    d0 = SplineKeys[keyIndex0 - 1].direction;
                }


                p1 = SplineKeys[keyIndex0 + 0].position;
                p2 = SplineKeys[keyIndex0 + 1].position;

                d1 = SplineKeys[keyIndex0 + 0].direction;
                d2 = SplineKeys[keyIndex0 + 1].direction;


                if (keyIndex0 + 2 >= SplineKeys.Count)
                {
                    p3 = SplineKeys[keyIndex0 + 1].position + (SplineKeys[keyIndex0 + 1].position - SplineKeys[keyIndex0 + 0].position).normalized * SPLINE_CONSTS(this).RADIUS * 0.15f;

                    d3 = SplineKeys[keyIndex0 + 1].direction + (SplineKeys[keyIndex0 + 1].direction - SplineKeys[keyIndex0 + 0].direction);
                    d3.Normalize();
                }
                else
                {
                    p3 = SplineKeys[keyIndex0 + 2].position;
                    d3 = SplineKeys[keyIndex0 + 2].direction;
                }

                L0 = L;
                L = L0 + (p2 - p1).magnitude;

                // position
                {
                    float t0 = 0.0f;
                    float t1 = t0 + Mathf.Pow((p0 - p1).magnitude, alpha);
                    float t2 = t1 + Mathf.Pow((p1 - p2).magnitude, alpha);
                    float t3 = t2 + Mathf.Pow((p2 - p3).magnitude, alpha);

                    Vector3 m1 = (1.0f - tension) * (t2 - t1) *
                        ((p1 - p0) / (t1 - t0) - (p2 - p0) / (t2 - t0) + (p2 - p1) / (t2 - t1));
                    Vector3 m2 = (1.0f - tension) * (t2 - t1) *
                        ((p2 - p1) / (t2 - t1) - (p3 - p1) / (t3 - t1) + (p3 - p2) / (t3 - t2));

                    A = 2.0f * (p1 - p2) + m1 + m2;
                    B = -3.0f * (p1 - p2) - m1 - m1 - m2;
                    C = m1;
                    D = p1;
                }

                bool bFallBackToLinear = false;


                {
                    float t0 = 0.0f;
                    float t1 = t0 + Mathf.Pow((d0 - d1).magnitude, alpha);
                    float t2 = t1 + Mathf.Pow((d1 - d2).magnitude, alpha);
                    float t3 = t2 + Mathf.Pow((d2 - d3).magnitude, alpha);

                    Vector3 m1 = (1.0f - tension) * (t2 - t1) *
                        ((d1 - d0) / (t1 - t0) - (d2 - d0) / (t2 - t0) + (d2 - d1) / (t2 - t1));
                    Vector3 m2 = (1.0f - tension) * (t2 - t1) *
                        ((d2 - d1) / (t2 - t1) - (d3 - d1) / (t3 - t1) + (d3 - d2) / (t3 - t2));

                    Ad = 2.0f * (d1 - d2) + m1 + m2;
                    Bd = -3.0f * (d1 - d2) - m1 - m1 - m2;
                    Cd = m1;
                    Dd = d1;

                    if (Single.IsNaN(Ad.x) ||
                        Single.IsNaN(Bd.x) ||
                        Single.IsNaN(Cd.x))
                    {
                        bFallBackToLinear = true;
                    }


                    // Linear Interpolation for direction
                    if (bFallBackToLinear)
                    {
                        Cd = (d2 - d1);
                        Dd = d1;
                    }
                }
            }


            float t = interKeyIndex / (float)SPLINE_POSITIONS_PER_SEGMENT;
            // t = [0; 1-step], step = (SPLINE_POSITIONS_PER_SEGMENT) ^ (-1)

            // just F(x) ~ polynomial x^3
            Vector3 point = A * t * t * t +
                             B * t * t +
                             C * t +
                             D;


            Vector3 direction = //Ad * t * t * t +
                                //Bd * t * t +
                                 Cd * t +
                                 Dd;

            // Use length for uv.[y]
            float length = Mathf.Lerp(L0, L, t);

            SplinePositions[posIndex] = new FSplinePosition(point, direction, length);
        }

    }

    private void Debug_SplinePositions()
    {
        float L = SplinePositions.Count - 1;
        for (int i = 0; i < SplinePositions.Count - 1; ++i)
        {
            float S = (i) / L;

            FSplinePosition p0 = SplinePositions[i];
            FSplinePosition p1 = SplinePositions[i + 1];

            Color C = Color.Lerp(Color.red, Color.green, S);
            Debug.DrawLine(p0.position, p1.position, C);
        }
    }
    #endregion

    #region [Handle] PointToSpline - Adding

    private float lsatTIme = 0;
    private void Handle_PointToSpline_Adding(Vector3 position, Vector3 direction, bool ignoreDistance)
    {
        if (SplineKeys == null)
        {
            SplineKeys = new List<FSplineKey>();
        }
        // transform.InverseTransformPoint
        // Let`s keep every line at Matrix.Identity, and work in world space
        Vector3 nextPoint = position;


        if (ignoreDistance)
        {
            return;
        }
        else
        {
            if (SplineKeys.Count < 2)
            {
                SplineKeys.Add(new FSplineKey(nextPoint, direction));
            }
            else
            {
                Vector3 pointPrev = SplineKeys[SplineKeys.Count - 2].position;
                Vector3 pointTarget = position;

                float distToTarget = (pointPrev - pointTarget).magnitude;
                float MaxLen = SPLINE_CONSTS(this).RADIUS * 2;

                float offset = SPLINE_CONSTS(this).OFFSET;

                if (distToTarget > MaxLen)
                {
                    pointTarget = pointPrev + (pointTarget - pointPrev).normalized * (distToTarget - offset);
                }

                float dt = Time.realtimeSinceStartup - lsatTIme;
                lsatTIme = Time.realtimeSinceStartup;

                SplineKeys[SplineKeys.Count - 1].position = pointTarget;
                SplineKeys[SplineKeys.Count - 1].direction = direction;

                if (distToTarget >= MaxLen + offset)
                {
                    SplineKeys.Add(new FSplineKey(nextPoint, direction));
                }
            }
        }

    }

    private void Debug_SplineKeys()
    {
        float L = SplineKeys.Count;
        int i = 0;
        foreach (var key in SplineKeys)
        {
            float S = (i++) / L;
            Color C = Color.Lerp(Color.red, Color.green, S);
            //DebugExtension.DebugWireSphere(key.position, C, SPLINE_CONSTS(this).RADIUS);
        }
    }

    #endregion
}