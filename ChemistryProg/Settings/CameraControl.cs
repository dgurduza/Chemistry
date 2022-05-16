using ChemistryProg._3D;
using ChemistryProg.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ChemistryProg.CameraControl
{
    public class CameraControler
    {
        // The camera.
        public OrthographicCamera TheCamera = null;
        DirectionalLight directional_light;

        private Viewport3D viewport;

        Point3D labelXPos3D, labelYPos3D, labelZPos3D;

        public event Action<Point, Point, Point> OnCameraMoved;

        // The controls that provide events.
        private UIElement KeyboardControl = null;
        private UIElement WheelControl = null;
        private UIElement MouseControl = null;

        // Adjustment values.
        public double CameraDR = 2;
        public double CameraDTheta = Math.PI / 30;
        public double CameraDPhi = Math.PI / 15;

        // The current position.
        private double CameraR = 100.0;
        private double CameraTheta = Math.PI / 3.0;
        private double CameraPhi = Math.PI / 3.0;

        public double Phi => CameraPhi;
        public double Theta => CameraTheta;

        public double CameraWidth = 30;

        // Get or set the spherical coordinates.
        // The point's coordinates are (r, theta, phi).
        public Point3D SphericalCoordinates
        {
            get
            {
                return new Point3D(CameraR, CameraTheta, CameraPhi);
            }
            set
            {
                CameraR = value.X;
                CameraTheta = value.Y;
                CameraPhi = value.Z;
            }
        }

        // Get or set the Cartesian coordinates.
        public Point3D CartesianCoordinates
        {
            get
            {
                double x, y, z;
                SphericalToCartesian(CameraR, CameraTheta, CameraPhi, out x, out y, out z);
                return new Point3D(x, y, z);
            }
            set
            {
                double r, theta, phi;
                CartesianToSpherical(value.X, value.Y, value.Z, out r, out theta, out phi);
                CameraR = r;
                CameraTheta = theta;
                CameraPhi = phi;
            }
        }

        // Constructor.
        public CameraControler(OrthographicCamera camera, DirectionalLight light, Viewport3D viewport,
            UIElement keyboardControl, UIElement wheelControl, UIElement mouseControl)
        {
            TheCamera = camera;
            viewport.Camera = TheCamera;
            this.directional_light = light;
            KeyboardControl = keyboardControl;
            KeyboardControl.PreviewKeyDown += KeyboardControl_KeyDown;

            WheelControl = wheelControl;
            WheelControl.PreviewMouseWheel += WheelControl_PreviewMouseWheel;

            MouseControl = mouseControl;
            MouseControl.MouseDown += MouseControl_MouseDown;
            this.viewport = viewport;
            PositionCamera();
        }

        // Update the camera's position.
        public void IncreaseR(double amount)
        {
            CameraWidth += amount;
            if (CameraWidth < CameraDR)
                CameraWidth = CameraDR;
        }
        public void IncreaseR()
        {
            IncreaseR(CameraDR);
        }
        public void DecreaseR(double amount)
        {
            IncreaseR(-amount);
        }
        public void DecreaseR()
        {
            IncreaseR(-CameraDR);
        }

        public void IncreaseTheta(double amount)
        {
            CameraTheta += amount;
        }
        public void IncreaseTheta()
        {
            IncreaseTheta(CameraDTheta);
        }
        public void DecreaseTheta(double amount)
        {
            IncreaseTheta(-amount);
        }
        public void DecreaseTheta()
        {
            IncreaseTheta(-CameraDTheta);
        }

        public void IncreasePhi(double amount)
        {
            CameraPhi += amount;
        }
        public void IncreasePhi()
        {
            IncreasePhi(CameraDPhi);
        }
        public void DecreasePhi(double amount)
        {
            IncreasePhi(-amount);
        }
        public void DecreasePhi()
        {
            IncreasePhi(-CameraDPhi);
        }

        #region Camera Control

        // Adjust the camera's position.
        private void KeyboardControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
            switch (e.Key)
            {
                case Key.Up:
                    IncreasePhi();
                    break;
                case Key.Down:
                    DecreasePhi();
                    break;
                case Key.Left:
                    IncreaseTheta();
                    break;
                case Key.Right:
                    DecreaseTheta();
                    break;
                case Key.Add:
                case Key.OemPlus:
                    IncreaseR();
                    break;
                case Key.Subtract:
                case Key.OemMinus:
                    DecreaseR();
                    break;
            }

            // Update the camera's position.
            PositionCamera();
        }

        // Zoom in or out.
        private void WheelControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            DecreaseR(Math.Sign(e.Delta) * CameraDR);
            PositionCamera();
        }

        // Use the mouse to change the camera's position.
        private Point LastPoint;
        private void MouseControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;
            MouseControl.CaptureMouse();
            MouseControl.MouseMove += new MouseEventHandler(MouseControl_MouseMove);
            MouseControl.MouseUp += new MouseButtonEventHandler(MouseControl_MouseUp);
            LastPoint = e.GetPosition(MouseControl);
        }

        internal void AxesChangedHandler(Point3D arg1, Point3D arg2, Point3D arg3)
        {
            labelXPos3D = arg1;
            labelYPos3D = arg2;
            labelZPos3D = arg3;
            PositionCamera();
        }

        private void MouseControl_MouseMove(object sender, MouseEventArgs e)
        {
            const double xscale = 0.1;
            const double yscale = 0.1;

            Point newPoint = e.GetPosition(MouseControl);
            double dx = newPoint.X - LastPoint.X;
            double dy = newPoint.Y - LastPoint.Y;

            CameraTheta -= dx * CameraDTheta * xscale;
            CameraPhi -= dy * CameraDPhi * yscale;

            LastPoint = newPoint;
            PositionCamera();
        }

        internal void SetProjection(double xAngle, double zAngle)
        {
            CameraPhi = xAngle;
            CameraTheta = zAngle;
            PositionCamera();
        }

        private void MouseControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;
            MouseControl.ReleaseMouseCapture();
            MouseControl.MouseMove -= new MouseEventHandler(MouseControl_MouseMove);
            MouseControl.MouseUp -= new MouseButtonEventHandler(MouseControl_MouseUp);
        }

        public void SetCenter(Compound compound, double xCount = 1, double yCount = 1, double zCount = 1)
        {
            var cellParams = compound.Params;
            SetCenter(cellParams.ToCartesian(new Point3D(xCount / 2, yCount / 2, zCount / 2)));
        }

        public void SetCenter(Point3D point)
        {
            TheCamera.Transform = new TranslateTransform3D((Vector3D)point);
        }

        // Use the current values of CameraR, CameraTheta,
        // and CameraPhi to position the camera.
        private void PositionCamera()
        {
            // Calculate the camera's position in Cartesian coordinates.
            double x, y, z;
            SphericalToCartesian(CameraR, CameraTheta, CameraPhi,
                out x, out y, out z);
            TheCamera.Position = new Point3D(x, y, z);


            // Look toward the origin.
            TheCamera.LookDirection = new Vector3D(-x, -y, -z);

            directional_light.Direction = new Vector3D(-x, -y, -z); ;
            TheCamera.FarPlaneDistance = 1000000;//@
            TheCamera.NearPlaneDistance = 0.001;//@

            // Set the Up direction.
            TheCamera.UpDirection = new Vector3D(0, 1, 0);

            TheCamera.Width = CameraWidth;
            ChangeLabelPos();

        }

        public static Point Get2DPoint(Point3D p3d, Viewport3D vp)
        {
            bool TransformationResultOK;
            Viewport3DVisual vp3Dv = VisualTreeHelper.GetParent(
              vp.Children[0]) as Viewport3DVisual;
            Matrix3D m = Util.TryWorldToViewportTransform(vp3Dv, out TransformationResultOK);
            if (!TransformationResultOK) return new Point(0, 0);
            Point3D pb = m.Transform(p3d);
            Point p2d = new Point(pb.X, pb.Y);
            return p2d;
        }

        private void ChangeLabelPos()
        {

            Point labelXCoord = Get2DPoint(labelXPos3D, viewport);
            Point labelYCoord = Get2DPoint(labelYPos3D, viewport);
            Point labelZCoord = Get2DPoint(labelZPos3D, viewport);
            OnCameraMoved?.Invoke(labelXCoord, labelYCoord, labelZCoord);
        }

        // Convert from Cartesian to spherical coordinates.
        private void CartesianToSpherical(double x, double y, double z,
            out double r, out double theta, out double phi)
        {
            r = Math.Sqrt(x * x + y * y + z * z);
            double h = Math.Sqrt(x * x + z * z);
            theta = Math.Atan2(x, z);
            phi = Math.Atan2(h, y);
        }

        // Convert from spherical to Cartesian coordinates.
        private void SphericalToCartesian(double r, double theta, double phi,
            out double x, out double y, out double z)
        {
            y = r * Math.Cos(phi);
            double h = r * Math.Sin(phi);
            x = h * Math.Sin(theta);
            z = h * Math.Cos(theta);
        }



        #endregion Camera Control
    }
}
