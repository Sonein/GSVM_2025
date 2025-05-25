using MathNet.Spatial.Euclidean;

namespace RenderStuff;

public class Face
{
    private List<int> _indices;
    private int _faceIndex;
    private UnitVector3D _normal;

    public Face(int faceIndex)
    {
        this._faceIndex = faceIndex;
        this._indices = new List<int>();
    }

    public Face(List<int> vertices, int faceIndex)
    {
        this._indices = vertices;
        this._faceIndex = faceIndex;
    }

    public int FaceIndex()
    {
        return this._faceIndex;
    }

    public List<int> GetVertices()
    {
        return this._indices;
    }

    public void AddVertex(int vertex)
    {
        this._indices.Add(vertex);
    }

    public void SetNormal(UnitVector3D normal)
    {
        this._normal = normal;
    }

    public UnitVector3D GetNormal()
    {
        return this._normal;
    }
}