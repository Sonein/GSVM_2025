using System.Globalization;
using MathNet.Spatial.Euclidean;

namespace RenderStuff;

public static class StaticUtils
{
    public static UnitVector3D GetFaceNormal(Face face, Mesh mesh)
    {
        Vertex v1 = mesh.Vertices[face.GetVertices()[0]];
        Vertex v2 = mesh.Vertices[face.GetVertices()[1]];
        Vertex v3 = mesh.Vertices[face.GetVertices()[2]];
        
        Vector3D a = v1.Position - v2.Position;
        Vector3D b = v3.Position - v2.Position;
        Vector3D n = a.CrossProduct(b);

        return n.Normalize();
    }

    public static UnitVector3D GetVertexNormal(Vertex vertex, Mesh mesh)
    {
        List<Face> tempFaces = new List<Face>();
        foreach (Face face in mesh.Faces)
        {
            if (face.GetVertices().Contains(vertex.Index))
            {
                tempFaces.Add(face);
            }
        }
        Vector3D normal = new Vector3D(0, 0, 0);
        foreach (Face face in tempFaces)
        {
            normal += face.GetNormal();
        }
        return normal.Normalize();
    }

    public static Mesh Load(string path)
    {
        Mesh mesh = new Mesh();
        IEnumerable<string> lines = File.ReadLines(path);
        int i = 1;
        int j = 1;
        foreach (string line in lines)
        {
            string[] parts = line.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                switch (parts[0])
                {
                    case "v":
                        Point3D position = new Point3D(double.Parse(parts[1], CultureInfo.InvariantCulture), double.Parse(parts[2], CultureInfo.InvariantCulture), double.Parse(parts[3], CultureInfo.InvariantCulture));
                        Vertex v = new Vertex(position, i, new Vector3D(1, 0, 0));
                        mesh.AddVertex(v);
                        i++;
                        break;
                    case "f":
                        if (parts.Length > 4)
                        {
                            break;
                        }
                        Face face = new Face(j);
                        for (int i1 = 1; i1 <= 3; i1++)
                        {
                            string[] p2 = parts[i1].Split(new char[]{'/'}, StringSplitOptions.RemoveEmptyEntries);
                            face.AddVertex(int.Parse(p2[0]));
                        }
                        mesh.AddFace(face);
                        j++;
                        break;
                    default:
                        break;
                }
            }
        }

        foreach (Face face in mesh.Faces)
        {
            UnitVector3D normal = GetFaceNormal(face, mesh);
            face.SetNormal(normal);
        }

        foreach (Vertex vertex in mesh.Vertices.Values)
        {
            vertex.SetNormal(GetVertexNormal(vertex, mesh));
        }

        return mesh;
    }
}