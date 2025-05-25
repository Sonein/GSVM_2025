using MathNet.Spatial.Euclidean;
using SkiaSharp;

namespace RenderStuff;

public static class StaticMath
{

    public static double TriangleArea(Point3D a, Point3D b, Point3D c)
    {
        return (b - a).CrossProduct(c - a).Length / 2;
    }

    public static Mt MollerTrumborov(Point3D p0, Point3D v0, Point3D v1, Point3D v2, UnitVector3D d)
    {
        Vector3D e1 = v1 - v0;
        Vector3D e2 = v2 - v0;
        Vector3D h = d.CrossProduct(e2);
        double a = e1.DotProduct(h);
        if(a == 0){ return new Mt(false, Point3D.NaN, 0);}

        double f = 1 / a;
        Vector3D s = p0 - v0;
        double u = f * s.DotProduct(h);
        if(u < 0 || u > 1){return new Mt(false, Point3D.NaN, 0);}
        
        Vector3D q = s.CrossProduct(e1);
        double v = f * d.DotProduct(q);
        if(v < 0 || u + v > 1){return new Mt(false, Point3D.NaN, 0);}
        
        double t = f * e2.DotProduct(q);
        if(t < 0){return new Mt(false, Point3D.NaN, 0);}

        Point3D output = p0 + t*d;
        return new Mt(true, output, t);
    }

    public static SKBitmap RayTrace(Point3D lightSrc, Camera camera, Point3D middle, int imgWidth, int imgHeight, Mesh objMesh, Point3D kd, Point3D ks, Point3D ka, double ns, double fov = Math.PI/6)
    {
        SKColor id = new SKColor(1, 1, 1);
        SKColor iS = new SKColor(1, 1, 1);
        SKColor ia = new SKColor(0, 0, 0);
        double zNear = middle.Z;
        double zFar = 10000;
        double hit;

        UnitVector3D u;
        UnitVector3D v;
        UnitVector3D w;
        w = (camera.Position - middle).Normalize();
        Console.WriteLine(camera.VectorUp.CrossProduct(w));
        u = (camera.VectorUp.CrossProduct(w)).Normalize();
        v = w.CrossProduct(u);

        double fovY = fov;
        double aspectRatio = imgWidth / (double)imgHeight;
        double height = 2 * Math.Tan(fovY / 2);
        double width = aspectRatio * height;
        
        SKBitmap bitmap = new SKBitmap(imgWidth, imgHeight);

        for (int c = 0; c < imgWidth; c++)
        {
            for (int r = 0; r < imgHeight; r++)
            {
                SKColor color = new SKColor(242, 98, 252);
                hit = zFar;
                double cC = width * c / imgWidth - width/2;
                double rC = height * r / imgHeight - height/2;
                
                UnitVector3D rayDir = (cC*u + rC*v - w).Normalize();

                foreach (Face face in objMesh.Faces)
                {
                    Dictionary<int, Vertex> vertices = objMesh.Vertices;
                    List<int> facePoints = face.GetVertices();
                    Mt mt = MollerTrumborov(camera.Position, vertices[facePoints[0]].Position, vertices[facePoints[1]].Position, vertices[facePoints[2]].Position, rayDir);
                    if (!mt.Result) continue;
                    if (mt.T < hit)
                    {
                        hit = mt.T;
                        UnitVector3D light = ((camera.Position + mt.T * rayDir) - lightSrc).Normalize();

                        Point3D hitPoint = mt.Crossing;
                        double alpha = TriangleArea(hitPoint, vertices[facePoints[1]].Position, vertices[facePoints[2]].Position)/
                                       TriangleArea(vertices[facePoints[0]].Position, vertices[facePoints[1]].Position, vertices[facePoints[2]].Position);
                        double beta = TriangleArea(vertices[facePoints[0]].Position, hitPoint, vertices[facePoints[2]].Position)/
                                      TriangleArea(vertices[facePoints[0]].Position, vertices[facePoints[1]].Position, vertices[facePoints[2]].Position);
                        double gama = TriangleArea(vertices[facePoints[0]].Position, vertices[facePoints[1]].Position, hitPoint)/
                                      TriangleArea(vertices[facePoints[0]].Position, vertices[facePoints[1]].Position, vertices[facePoints[2]].Position);
                        double hnx = (vertices[facePoints[0]].Normal.X * alpha) +
                                     (vertices[facePoints[1]].Normal.X * beta) +
                                     (vertices[facePoints[2]].Normal.X * gama);
                        double hny = (vertices[facePoints[0]].Normal.Y * alpha) +
                                     (vertices[facePoints[1]].Normal.Y * beta) +
                                     (vertices[facePoints[2]].Normal.Y * gama);
                        double hnz = (vertices[facePoints[0]].Normal.Z * alpha) +
                                     (vertices[facePoints[1]].Normal.Z * beta) +
                                     (vertices[facePoints[2]].Normal.Z * gama);
                        UnitVector3D hitNormal = new Vector3D(hnx, hny, hnz).Normalize();
                        
                        UnitVector3D reflect = (2 * light.DotProduct(hitNormal) * hitNormal - light).Normalize();
                        double dnl = hitNormal.DotProduct(light);
                        double drv = reflect.DotProduct(rayDir);
                        if (drv < 0)
                        {
                            drv = 0;
                        }

                        double clr = ka.X * ia.Red + kd.X * id.Red * dnl + ks.X * iS.Red * double.Pow(drv, ns);
                        if (clr < 0)
                        {
                            clr = 0;
                        }

                        double clg = ka.Y * ia.Green + kd.Y * id.Green * dnl + ks.Y * iS.Green * double.Pow(drv, ns);
                        if (clg < 0)
                        {
                            clg = 0;
                        }
                        
                        double clb = ka.Z * ia.Blue + kd.Z * id.Blue * dnl + ks.Z * iS.Blue * double.Pow(drv, ns);
                        if (clb < 0)
                        {
                            clb = 0;
                        }
                        
                        //Console.WriteLine(dnl);
                        byte colr = (byte)double.Floor(clr*255);
                        byte colg = (byte)double.Floor(clg*255);
                        byte colb = (byte)double.Floor(clb*255);
                        
                        color = new SKColor(colr, colg, colb);
                    }
                }
                bitmap.SetPixel(c, imgHeight - r - 1, color);
            }
        }
        return bitmap;
    }

    public static Mesh Translate(Mesh mesh, Vector3D translation)
    {
        foreach (int key in mesh.Vertices.Keys)
        {
            mesh.Vertices[key].Position += translation;
        }
        return mesh;
    }

    public static Mesh Scale(Mesh mesh, Vector3D scale)
    {
        int c = mesh.Vertices.Values.Count;
        double x = 0;
        double y = 0;
        double z = 0;
        foreach (Vertex vertex in mesh.Vertices.Values)
        {
            x += vertex.Position.X*(1.0/c);
            y += vertex.Position.Y*(1.0/c);
            z += vertex.Position.Z*(1.0/c);
            
        }
        Point3D t = new Point3D(x,y,z);
        Vector3D translation = (new Point3D(0, 0, 0)) - t;
        Mesh mesh1 = Translate(mesh, translation);
        foreach (int key in mesh1.Vertices.Keys)
        {
            mesh1.Vertices[key] = new Vertex(new Point3D(scale.X * mesh1.Vertices[key].Position.X, scale.Y * mesh1.Vertices[key].Position.Y, scale.Z * mesh1.Vertices[key].Position.Z), 
                mesh1.Vertices[key].Index, 
                new Vector3D(mesh1.Vertices[key].Normal.X * scale.X, mesh1.Vertices[key].Normal.Y * scale.Y, mesh1.Vertices[key].Normal.Z * scale.Z));
        }
        Mesh mesh2 = Translate(mesh1, -translation);
        return mesh2;
    }

    private static UnitVector3D RotVector3D(UnitVector3D v, char axis, double cos, double sin)
    {
        UnitVector3D ret = v;
        double x = ret.X;
        double y = ret.Y;
        double z = ret.Z;
        switch (axis)
        {
            case 'x':
                x = ret.X;
                y = ret.Y*cos - ret.Z*sin;
                z = ret.Y*sin + ret.Z*cos;
                break;
            case 'y':
                x = ret.X*cos + ret.Z*sin;
                y = ret.Y;
                z = ret.Z*cos - ret.X*sin;
                break;
            case 'z':
                x = ret.X*cos - ret.Y*sin;
                y = ret.X*sin + ret.Y*cos;
                z = ret.Z;
                break;
        }
        ret = new Vector3D(x, y, z).Normalize();
        return ret;
    }

    private static Point3D RotPoint3D(Point3D point, char axis, double cos, double sin)
    {
        Point3D ret = point;
        double x = ret.X;
        double y = ret.Y;
        double z = ret.Z;
        switch (axis)
        {
            case 'x':
                x = ret.X;
                y = ret.Y*cos - ret.Z*sin;
                z = ret.Y*sin + ret.Z*cos;
                break;
            case 'y':
                x = ret.X*cos + ret.Z*sin;
                y = ret.Y;
                z = ret.Z*cos - ret.X*sin;
                break;
            case 'z':
                x = ret.X*cos - ret.Y*sin;
                y = ret.X*sin + ret.Y*cos;
                z = ret.Z;
                break;
        }
        ret = new Point3D(x, y, z);
        return ret;
    }


    public static Mesh Rotate(Mesh mesh, double angle, char axis)
    {
        double angleRad = angle * Math.PI / 180;
        double cos = Math.Cos(angleRad);
        double sin = Math.Sin(angleRad);

        int c = mesh.Vertices.Values.Count;
        double x = 0;
        double y = 0;
        double z = 0;
        foreach (Vertex vertex in mesh.Vertices.Values)
        {
            x += vertex.Position.X*(1.0/c);
            y += vertex.Position.Y*(1.0/c);
            z += vertex.Position.Z*(1.0/c);
        }
        Point3D t = new Point3D(x,y,z);
        Vector3D translation = (new Point3D(0, 0, 0)) - t;
        Mesh mesh1 = Translate(mesh, translation);
        foreach (int key in mesh1.Vertices.Keys)
        {
            mesh1.Vertices[key] = new Vertex(RotPoint3D(mesh1.Vertices[key].Position, axis, cos, sin), mesh1.Vertices[key].Index, RotVector3D(mesh1.Vertices[key].Normal, axis, cos, sin));
        }
        Mesh mesh2 = Translate(mesh1, -translation);
        return mesh2;
    }

    public static Camera RotateCamera(Camera camera, double angle, char axis)
    {
        double angleRad = angle * Math.PI / 180;
        double cos = Math.Cos(angleRad);
        double sin = Math.Sin(angleRad);
        
        Camera c1 = new Camera();
        c1.Position = RotPoint3D(camera.Position, axis, cos, sin);
        c1.VectorUp = camera.VectorUp;
        return c1;
    }
}