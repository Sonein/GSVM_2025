using MathNet.Spatial.Euclidean;

namespace RenderStuff;

public class Mesh
{
    public Dictionary<int, Vertex> Vertices {get;set;}
    public List<Face> Faces {get;set;}
    
    public Mesh()
    {
        this.Faces = new List<Face>();
        this.Vertices = new Dictionary<int, Vertex>();
    }

    public Mesh(List<Face> faces, List<Vertex> vertices)
    {
        this.Faces = faces;
        this.Vertices = new Dictionary<int, Vertex>();
        foreach (Vertex vertex in vertices)
        {
            this.Vertices.Add(vertex.Index, vertex);
        }
    }

    public void AddFace(Face face)
    {
        this.Faces.Add(face);
    }

    public void AddVertex(Vertex vertex)
    {
        this.Vertices.Add(vertex.Index, vertex);
    }
}