using MathNet.Spatial.Euclidean;

namespace RenderStuff;

public class Vertex
{
    public Point3D Position;
    public UnitVector3D Normal;
    public int Index;

    public Vertex(Point3D position, int i, Vector3D normal)
    {
        this.Position = position;
        this.Index = i;
        this.Normal = normal.Normalize();
    }
    
    public Vertex(Point3D position, int i, UnitVector3D normal)
    {
        this.Position = position;
        this.Index = i;
        this.Normal = normal;
    }

    public void SetNormal(UnitVector3D normal)
    {
        this.Normal = normal;
    }
}