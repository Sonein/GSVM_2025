using System;
using MathNet.Spatial.Euclidean;
using SkiaSharp;
using Gtk;
using System.Threading.Tasks;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.Gtk;

namespace RenderStuff;

public class RenderApplication : Window
{
    private SKDrawingArea _skiaArea;
    private SKBitmap _image;
    
    private Button _triggerButton;
    private Entry _cameraX;
    private Entry _cameraY;
    private Entry _cameraZ;
    private Entry _kdEntryX;
    private Entry _kdEntryY;
    private Entry _kdEntryZ;
    private Entry _ksEntryX;
    private Entry _ksEntryY;
    private Entry _ksEntryZ;
    private Entry _kaEntryX;
    private Entry _kaEntryY;
    private Entry _kaEntryZ;
    private Entry _nsEntry;
    private Entry _fovEntry;
    private Entry _lightX;
    private Entry _lightY;
    private Entry _lightZ;
    private Entry _scaleX;
    private Entry _scaleY;
    private Entry _scaleZ;
    private Entry _rORad;
    private Entry _rCRad;
    private Entry _aO;
    private Entry _aC;
    
    private Camera _camera;
    private Point3D _kd;
    private Point3D _ks;
    private Point3D _ka;
    private double _ns;
    private double _fov;
    private Point3D _lightSource;
    private Vector3D _scaling;
    private double _rotateObj;
    private double _rotateCam;
    private char _axisObj;
    private char _axisCam;
    private Mesh _mesh;

    public RenderApplication() : base("Rendering App :3")
    {
        SetDefaultSize(1280, 720);
        SetPosition(WindowPosition.Center);
        
        DeleteEvent += (o, args) => Gtk.Application.Quit();
        
        VBox vBox = new VBox();
        HBox hBox = new HBox();
        
        _mesh = StaticUtils.Load("cirno2.obj");
        _camera = new Camera();
        _camera.Position = new Point3D(1, 2.5, -5);
        _camera.VectorUp = new Vector3D(0, 0, 1);
        _kd = new Point3D(0.2775, 0.2775, 0.2775);
        _ks = new Point3D(0.773911, 0.773911, 0.773911);
        _ka = new Point3D(0.23125, 0.23125, 0.23125);
        _ns = 89.6;
        _fov = Math.PI / 6;
        _lightSource = new Point3D(3, 3, 0);
        _scaling = new Vector3D(1, 1, 1);
        _rotateObj = 0;
        _rotateCam = 0;
        _axisObj = 'x';
        _axisCam = 'x';
        _image = StaticMath.RayTrace(_lightSource, _camera, new Point3D(0, 0, 0), 848, 480, _mesh, _kd, _ks, _ka, _ns);
        
        _skiaArea = new SKDrawingArea();
        _skiaArea.SetSizeRequest(_image.Width, _image.Height);
        _skiaArea.PaintSurface += OnPaintSurface;
        
        Label cameraLabel = new Label("Camera X, Y, Z");
        _cameraX = new Entry();
        _cameraY = new Entry();
        _cameraZ = new Entry();
        VBox cameraBox = new VBox();
        cameraBox.PackStart(cameraLabel, false, false, 5);
        cameraBox.PackStart(_cameraX, false, false, 5);
        cameraBox.PackStart(_cameraY, false, false, 5);
        cameraBox.PackStart(_cameraZ, false, false, 5);
        
        Label kdLabel = new Label("KD, KS, KS, NS");
        _kdEntryX = new Entry();
        _kdEntryY = new Entry();
        _kdEntryZ = new Entry();
        _ksEntryX = new Entry();
        _ksEntryY = new Entry();
        _ksEntryZ = new Entry();
        _kaEntryX = new Entry();
        _kaEntryY = new Entry();
        _kaEntryZ = new Entry();
        _nsEntry = new Entry();
        VBox kdBox = new VBox();
        kdBox.PackStart(kdLabel, false, false, 5);
        kdBox.PackStart(new Label("kd"), false, false, 5);
        kdBox.PackStart(_kdEntryX, false, false, 5);
        kdBox.PackStart(_kdEntryY, false, false, 5);
        kdBox.PackStart(_kdEntryZ, false, false, 5);
        kdBox.PackStart(new Label("ks"), false, false, 5);
        kdBox.PackStart(_ksEntryX, false, false, 5);
        kdBox.PackStart(_ksEntryY, false, false, 5);
        kdBox.PackStart(_ksEntryZ, false, false, 5);
        kdBox.PackStart(new Label("ka"), false, false, 5);
        kdBox.PackStart(_kaEntryX, false, false, 5);
        kdBox.PackStart(_kaEntryY, false, false, 5);
        kdBox.PackStart(_kaEntryZ, false, false, 5);
        kdBox.PackStart(new Label("ns"), false, false, 5);
        kdBox.PackStart(_nsEntry, false, false, 5);
        
        VBox fovBox = new VBox();
        fovBox.PackStart(new Label("FovY"), false, false, 5);
        _fovEntry = new Entry();
        fovBox.PackStart(_fovEntry, false, false, 5);
        
        VBox pack1 = new VBox();
        pack1.PackStart(cameraBox, false, false, 5);
        pack1.PackStart(kdBox, false, false, 5);
        pack1.PackStart(fovBox, false, false, 5);
        
        Label lightLabel = new Label("Light pos X, Y, Z");
        _lightX = new Entry();
        _lightY = new Entry();
        _lightZ = new Entry();
        VBox lightBox = new VBox();
        lightBox.PackStart(lightLabel, false, false, 5);
        lightBox.PackStart(_lightX, false, false, 5);
        lightBox.PackStart(_lightY, false, false, 5);
        lightBox.PackStart(_lightZ, false, false, 5);
        
        Label scaleLabel = new Label("Scaling X, Y, Z");
        _scaleX = new Entry();
        _scaleY = new Entry();
        _scaleZ = new Entry();
        VBox scaleBox = new VBox();
        scaleBox.PackStart(scaleLabel, false, false, 5);
        scaleBox.PackStart(_scaleX, false, false, 5);
        scaleBox.PackStart(_scaleY, false, false, 5);
        scaleBox.PackStart(_scaleZ, false, false, 5);
        
        Label rotationLabelO = new Label("Rotation of object, Degrees, axis");
        _rORad = new Entry();
        _aO = new Entry();
        VBox rotationBoxO = new VBox();
        rotationBoxO.PackStart(rotationLabelO, false, false, 5);
        rotationBoxO.PackStart(_rORad, false, false, 5);
        rotationBoxO.PackStart(_aO, false, false, 5);
        
        Label rotationLabelC = new Label("Rotation of camera, Degrees, axis");
        _rCRad = new Entry();
        _aC = new Entry();
        VBox rotationBoxC = new VBox();
        rotationBoxC.PackStart(rotationLabelC, false, false, 5);
        rotationBoxC.PackStart(_rCRad, false, false, 5);
        rotationBoxC.PackStart(_aC, false, false, 5);
        
        VBox pack2 = new VBox();
        pack2.PackStart(lightBox, false, false, 5);
        pack2.PackStart(scaleBox, false, false, 5);
        pack2.PackStart(rotationBoxO, false, false, 5);
        pack2.PackStart(rotationBoxC, false, false, 5);
        
        _triggerButton = new Button("Transform");
        _triggerButton.Clicked += OnButtonClicked;
        
        vBox.PackStart(_triggerButton, false, false, 5);
        VBox stinky = new VBox();
        stinky.PackStart(_skiaArea, false, false, 5);
        hBox.PackStart(stinky, false, false, 5);
        hBox.PackStart(vBox, false, false, 5);
        hBox.PackStart(pack1, false, false, 5);
        hBox.PackStart(pack2, false, false, 5);
        
        Add(hBox);
        ShowAll();
    }
    
    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var surface = e.Surface;
        var canvas = surface.Canvas;

        // Clear canvas
        canvas.Clear(new SKColor(242, 98, 252));
        
        var destRect = new SKRect(0, 0, e.Info.Width, e.Info.Height);
        canvas.DrawBitmap(_image, destRect);
    }
    
    private async void OnButtonClicked(object sender, EventArgs e)
    {
        if (double.TryParse(_cameraX.Text, out double x) && double.TryParse(_cameraY.Text, out double y) && double.TryParse(_cameraZ.Text, out double z))
        {
            _camera.Position = new Point3D(x, y, z);
        }

        if (double.TryParse(_kdEntryX.Text, out double kd) && double.TryParse(_kdEntryY.Text, out double kd2) && double.TryParse(_kdEntryZ.Text, out double kd3))
        {
            _kd = new Point3D(kd, kd2, kd3);
        }
        
        if (double.TryParse(_kaEntryX.Text, out double ka) && double.TryParse(_kaEntryY.Text, out double ka2) && double.TryParse(_kaEntryZ.Text, out double ka3))
        {
            _ka = new Point3D(ka, ka2, ka3);
        }
        
        if (double.TryParse(_ksEntryX.Text, out double ks) && double.TryParse(_ksEntryY.Text, out double ks2) && double.TryParse(_ksEntryZ.Text, out double ks3))
        {
            _ks = new Point3D(ks, ks2, ks3);
        }

        if (double.TryParse(_nsEntry.Text, out double ns))
        {
            _ns = ns;
        }
        
        if (double.TryParse(_fovEntry.Text, out double fov))
        {
            _fov = fov;
        }

        if (double.TryParse(_lightX.Text, out double lightX) && double.TryParse(_lightY.Text, out double lightY) && double.TryParse(_lightZ.Text, out double lightZ))
        {
            _lightSource = new Point3D(lightX, lightY, lightZ);
        }

        if (double.TryParse(_scaleX.Text, out double scaleX) && double.TryParse(_scaleY.Text, out double scaleY) && double.TryParse(_scaleZ.Text, out double scaleZ))
        {
            _scaling = new Vector3D(scaleX, scaleY, scaleZ);
        }

        if (double.TryParse(_rCRad.Text, out double rC) && char.TryParse(_aC.Text, out char aC))
        {
            _rotateCam = rC;
            _axisCam = aC;
        }
        
        if (double.TryParse(_rORad.Text, out double rO) && char.TryParse(_aO.Text, out char aO))
        {
            _rotateObj = rO;
            _axisObj = aO;
        }

        if (!_scaling.Equals(new Vector3D(1, 1, 1)))
        {
            _mesh = StaticMath.Scale(_mesh, _scaling);
        }
        if (!_rotateObj.Equals(0))
        {
            _mesh = StaticMath.Rotate(_mesh, _rotateObj, _axisObj);
        }
        if (!_rotateCam.Equals(0))
        {
            _camera = StaticMath.RotateCamera(_camera, _rotateCam, _axisCam);
        }
        
        if (_camera.Position.X == 0 && _camera.Position.Y == 0)
        {
            _camera.Position = new Point3D(0, 0.001, _camera.Position.Z);
        }

        _image = StaticMath.RayTrace(_lightSource, _camera, new Point3D(0, 0, 0), 848, 480, _mesh, _kd, _ks, _ka, _ns, _fov);
        _skiaArea.QueueDraw();
        
        _scaling = new Vector3D(1, 1, 1);
        _rotateObj = 0;
        _rotateCam = 0;
        _axisObj = 'x';
        _axisCam = 'x';
    }
    
    public static void Main()
    {
        Application.Init();
        new RenderApplication();
        Application.Run();
    }
}