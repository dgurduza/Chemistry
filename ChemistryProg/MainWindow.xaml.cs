using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChemistryProg._3D;
using ChemistryProg.CameraControl;
using ChemistryProg.Data;
using ChemistryProg.Figures;
using ChemistryProg.InformationMenu;
using ChemistryProg.MakeSphere;
using ChemistryProg.Сalculations;
using ChemistryProg.Extension;

namespace ChemistryProg
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<CoordToAtom> RelativeList { get; set; } = new ObservableCollection<CoordToAtom>();
        public ObservableCollection<CoordToAtom> RealList { get; set; } = new ObservableCollection<CoordToAtom>();
        public ObservableCollection<Distance> DistanceList { get; set; } = new ObservableCollection<Distance>();
        public ObservableCollection<Distance> AllDistanceList { get; set; } = new ObservableCollection<Distance>();
        public ObservableCollection<Angle> AngleList { get; set; } = new ObservableCollection<Angle>();
        public ObservableCollection<FactChargeResult> FactChargeList { get; set; } = new ObservableCollection<FactChargeResult>();
        public ObservableCollection<Polyhedra> PolyhedraList { get; set; } = new ObservableCollection<Polyhedra>();



        public ObservableCollection<Groups> GroupsList { get; set; } = new ObservableCollection<Groups>();
        public ObservableCollection<Compound> CompoundsList { get; set; } = new ObservableCollection<Compound>();

        public List<string> CompoundFilenameList = new List<string>();

        public static readonly DependencyProperty IsCompoundSelected;
        public static readonly DependencyProperty IsGroupSelected;

        static MainWindow()
        {
            IsCompoundSelected = DependencyProperty.Register("IsCompoundSelected", typeof(bool), typeof(MainWindow));
            IsGroupSelected = DependencyProperty.Register("IsGroupSelected", typeof(bool), typeof(MainWindow));
        }


        Model3DGroup compoundGroup = new Model3DGroup();


        CompoundVisual3D currentCompoundVisual3D;

        public DistanceMesurer DistanceMesurer { get; set; }
        public AngleMesurer AngleMesurer { get; set; }

        public event Action<Atom3D> OnAtomClick;

        public event Action<Point3D, Point3D, Point3D> onAxesChanged;

        public SettingsViewModel SettingsViewModel = SettingsViewModel.GetInstanse();

        [Obsolete]
        Atom3D selectedAtom = null;

        Atom3D SelectedAtom
        {
            get
            {
                Atom3D temp = selectedAtom;
                selectedAtom = null;
                return temp;
            }
            set
            {
                selectedAtom = value;
            }
        }

        Groups currentGroup;
        Compound currentCompound;

        Groups CurrentGroup
        {
            get
            {
                return currentGroup;
            }
            set
            {
                SetValue(IsGroupSelected, value != null);
                currentGroup = value;
            }
        }

        Compound CurrentCompound
        {
            get
            {
                return currentCompound;
            }
            set
            {
                SetValue(IsCompoundSelected, value != null);
                currentCompound = value;
            }
        }


        public MainWindow()
        {
            DistanceMesurer = new DistanceMesurer((res) => DistanceList.Add(res));
            OnAtomClick += DistanceMesurer.AtomClicked;
            AngleMesurer = new AngleMesurer((res) => AngleList.Add(res));
            OnAtomClick += AngleMesurer.AtomClicked;
            InitializeComponent();
            DataContext = this;
        }

        // The camera.
        private OrthographicCamera TheCamera = null;

        // The camera controller.
        private CameraControler CameraController = null;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadGroups(SettingsViewModel.PathToGroups);
            ModelVisual3D visual3d = new ModelVisual3D
            {
                Content = compoundGroup
            };
            monitor.Children.Add(visual3d);
            // Define the camera, lights, and model.
            DefineCamera(monitor, compoundGroup);
            DefineLights(compoundGroup);
            //DefineModel(compoundGroup);
        }

        private void DistanceButton_Click(object sender, RoutedEventArgs e)
        {
            DistanceMesurer.Dist_Button_Click(sender, e);
        }
        private void AngleButton_Click(object sender, RoutedEventArgs e)
        {
            AngleMesurer.Angle_Button_Click(sender, e);
        }

        private void LoadGroups(string pathToGroups)
        {
            GroupsList.Clear();
            string[] files = Directory.GetFiles(pathToGroups, "*.gr");
            foreach (var file in files)
            {
                try
                {
                    GroupsList.Add(Groups.LoadGroup(file));
                }
                catch (Exception exception)
                {

                }
            }
        }

        private void LoadCompounds(Groups group, string pathToCompounds)
        {
            CompoundsList.Clear();
            CompoundFilenameList.Clear();
            string[] files = Directory.GetFiles(pathToCompounds, "*.cmp");
            foreach (var file in files)
            {
                try
                {
                    Compound compound = Compound.LoadCompound(file);
                    if (compound.GroupName == group.Name)
                    {
                        compound.Group = group;
                        CompoundsList.Add(compound);
                        CompoundFilenameList.Add(System.IO.Path.GetFileName(file));
                    }
                }
                catch (Exception exception)
                {

                }
            }

        }



        // Define the camera.
        private void DefineCamera(Viewport3D viewport, Model3DGroup group)
        {
            TheCamera = new OrthographicCamera();
            //TheCamera.FieldOfView = 60;
            Color light = Color.FromArgb(255, 148, 148, 148);
            DirectionalLight directionalLight = new DirectionalLight(light, new Vector3D());
            group.Children.Add(directionalLight);
            CameraController = new CameraControler
                (TheCamera, directionalLight, viewport, this, mainGrid, mainGrid);
            onAxesChanged += CameraController.AxesChangedHandler;
            CameraController.OnCameraMoved += LabelPositionChanged;
            CameraController.OnCameraMoved += RedrawSemitransparentObjects;

        }

        private void RedrawSemitransparentObjects(Point arg1, Point arg2, Point arg3)
        {
            foreach (var item in CompoundVisual3D.allAtoms)
            {
                if (item.HasSphere == true)
                {
                    item.RedrawSphere();
                }

            }
        }

        // Define the lights.
        private void DefineLights(Model3DGroup group)
        {
            Color dark = Color.FromArgb(255, 128, 128, 128);

            group.Children.Add(new AmbientLight(dark));


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show((CameraController.Theta / Math.PI * 180).ToString() + " " + (CameraController.Phi / Math.PI * 180).ToString());
        }

        private void Monitor_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;

            GetAtomVisualByPos(e.GetPosition(monitor));
            Atom3D atom = SelectedAtom;
            if (atom != null)
            {
                popup1.IsOpen = true;
                ((TextBlock)popup1.Child).Text = atom.ToString();
            }
            else
                popup1.IsOpen = false;

        }

        public HitTestResultBehavior MyHitTestResult(HitTestResult result)
        {
            // Add the hit test result to the list that will be processed after the enumeration.
            if (result.VisualHit.GetType() != typeof(Atom3D))
                return HitTestResultBehavior.Continue;
            //selectedAtom.po
            SelectedAtom = result.VisualHit as Atom3D;
            // Set the behavior to return visuals at all z-order levels.
            return HitTestResultBehavior.Stop;
        }

        private void GetAtomVisualByPos(Point mouse_pos)
        {

            // Perform the hit test.
            VisualTreeHelper.HitTest(monitor, null, new HitTestResultCallback(MyHitTestResult), new PointHitTestParameters(mouse_pos));

        }

        private void OpenWindow(Window createCompoundWindow, bool hideMainWindow = true, bool catchFocus = true)
        {
            createCompoundWindow.Show();
            if (hideMainWindow)
            {
                createCompoundWindow.Closed += ShowMainWindow;
                Hide();
            }
            else if (catchFocus)
            {
                EventHandler method = (
                    (obj, e) =>
                    {
                        createCompoundWindow.Activate();
                        MessageBox.Show("Close other window!");
                    });
                Activated += method;
                createCompoundWindow.Closing += (obj, e) =>
                {
                    Activated -= method;
                };
            }
        }

        private void ShowMainWindow(object sender, EventArgs e)
        {
            Show();
        }

        private void XY_ProjectionClick(object sender, RoutedEventArgs e)
        {
            CameraController.SetProjection(Math.PI * 2, Math.PI / 6);
        }

        private void XZ_ProjectionClick(object sender, RoutedEventArgs e)
        {
            CameraController.SetProjection(Math.PI / 2, -Math.PI / 6);
        }

        private void Monitor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GetAtomVisualByPos(e.GetPosition(monitor));
            Atom3D atom = SelectedAtom;
            if (atom == null)
                return;
            if (e.ChangedButton == MouseButton.Right)
            {
                ContextMenu cm = this.FindResource("AtomContext") as ContextMenu;
                cm.PlacementRectangle = new Rect(e.GetPosition(monitor).X, e.GetPosition(monitor).Y, cm.Width, cm.Height);
                cm.PlacementTarget = monitor;
                cm.IsOpen = true;
                return;
            }
            OnAtomClick?.Invoke(atom);
            RelativeList.Add(CoordToAtom.GetRelativeCoordinate(atom));
            RealList.Add(CoordToAtom.GetCoordinate(atom));
        }

        private void Group_Selected(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedItem is Groups group)
            {
                LoadCompounds(group, SettingsViewModel.PathToCompounds);
                CurrentGroup = group;
                CurrentCompound = null;
            }

        }

        private void Compound_Selected(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (comboBox.SelectedItem is Compound compound)
            {
                SetCompound(compound);
                CameraController.SetCenter(compound, SettingsViewModel.XCount, SettingsViewModel.YCount, SettingsViewModel.ZCount);
            }

        }

        private void SetCompound(Compound compound)
        {
            if (currentCompoundVisual3D != null)
            {
                monitor.Children.Remove(currentCompoundVisual3D);
            }
            CurrentCompound = compound;
            VisualisNewCompound(compound);
        }

        private void VisualisNewCompound(Compound compound)
        {
            currentCompoundVisual3D = new CompoundVisual3D(compound);
            InfoControlView.Instanse.Variables = compound;
            monitor.Children.Add(currentCompoundVisual3D);
            SetNewXYZLabelPosition(compound);
            ClearLists();
            SetFactCharges(currentCompoundVisual3D);
        }

        TextBlock xTb, yTb, zTb;

        private void SetNewXYZLabelPosition(Compound compound)
        {
            Params cellParams = compound.Params;
            int xCount = SettingsViewModel.GetInstanse().XCount;
            int yCount = SettingsViewModel.GetInstanse().YCount;
            int zCount = SettingsViewModel.GetInstanse().ZCount;
            xTb = CreateTB("X");
            yTb = CreateTB("Y");
            zTb = CreateTB("Z");
            monitorCanvas.Children.Clear();
            monitorCanvas.Children.Add(xTb);
            monitorCanvas.Children.Add(yTb);
            monitorCanvas.Children.Add(zTb);
            onAxesChanged(cellParams.ToCartesian(new Point3D(xCount + .1, 0, 0)),
                cellParams.ToCartesian(new Point3D(0, yCount + .1, 0)),
                cellParams.ToCartesian(new Point3D(0, 0, zCount + .1)));
        }

        private static TextBlock CreateTB(string text)
        {
            TextBlock tb = new TextBlock();
            tb.FontSize += 5;
            tb.FontWeight = FontWeights.Bold;
            tb.Foreground = new SolidColorBrush(Colors.Black);
            tb.Text = text;
            return tb;
        }

        private void LabelPositionChanged(Point xLabelPos, Point yLabelPos, Point zLabelPos)
        {
            if (xTb != null)
                SetTBPos(xTb, xLabelPos);
            if (yTb != null)
                SetTBPos(yTb, yLabelPos);
            if (zTb != null)
                SetTBPos(zTb, zLabelPos);
        }

        private void SetTBPos(TextBlock tb, Point labelPos)
        {
            Canvas.SetTop(tb, labelPos.Y - 5);
            Canvas.SetLeft(tb, labelPos.X + 5);
        }

        private void ClearLists()
        {
            RelativeList.Clear();
            RealList.Clear();
            DistanceList.Clear();
            AllDistanceList.Clear();
            AngleList.Clear();
            PolyhedraList.Clear();
        }

        private void SetFactCharges(CompoundVisual3D currentCompound)
        {
            FactChargeList.Clear();
            foreach (var repAtom in currentCompound.repAtoms)
            {
                if (repAtom.atoms.Count == 0 || !repAtom.atom.Name.StartsWith("Si"))
                    continue;
                Atom3D target = repAtom.atoms.First(atom => !atom.ShouldBeHidden);
                FactChargeList.Add(new FactChargeResult(target, CompoundVisual3D.allAtoms));
            }
        }

        private void Hide_Atom_Click(object sender, RoutedEventArgs e)
        {
            Atom3D atom = GetSelectedAtom();
            atom.IsHidden = true;

        }
        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private Atom3D GetSelectedAtom()
        {
            ContextMenu cm = this.FindResource("AtomContext") as ContextMenu;
            GetAtomVisualByPos(new Point(cm.PlacementRectangle.X, cm.PlacementRectangle.Y));
            return SelectedAtom;
        }

        public void AddPolyhedraResult(Atom3D atom, List<Triangles> edges)
        {
            PolyhedraList.Add(new Polyhedra(atom, edges));
        }

        private void Create_Polyhedra(object sender, RoutedEventArgs e)
        {
            Atom3D atom = GetSelectedAtom();
            if (atom.Atom.OxidationState < 3)
                return;
            atom.PolyhedraCreated += AddPolyhedraResult;
            atom.HasPolyhedra = true;
        }

        private void Delete_Polyhedra(object sender, RoutedEventArgs e)
        {
            Atom3D atom = GetSelectedAtom();
            if (atom.Atom.OxidationState < 3)
                return;
            atom.HasPolyhedra = false;
        }

        private void Show_All_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in CompoundVisual3D.allAtoms)
            {
                item.HasPolyhedra = false;
                item.HasSphere = false;
                item.InPolyhedra = false;
                item.IsHidden = item.ShouldBeHidden;
            }
        }

        private void Show_All_Polyhedras_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in CompoundVisual3D.allAtoms)
            {
                if (!item.IsHidden && item.Atom.OxidationState >= 3)
                {
                    item.PolyhedraCreated += AddPolyhedraResult;
                    item.HasPolyhedra = true;

                }
            }
        }

        private Point3D VisualiseSphere(Point3D absCenter, double radius, Color color)
        {
            Sphere3D visual1 = new Sphere3D(absCenter,
                radius, color);
            Point3D sphereCenter1 = InfoControlView.Instanse.Variables.Params.FromCartesian(absCenter);
            monitor.Children.Add(visual1);
            return sphereCenter1;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            Point3D sphereCenter1 = VisualiseSphere(new Point3D(20.1598780313957, 6.20445163574291, 16.8873937857437),
                4.6521736022588, Colors.Red);

            Point3D sphereCenter2 = VisualiseSphere(new Point3D(20.161078385214, 6.20256879903833, 16.8872454342841),
                3.06222388066096, Colors.Green);


            PathTB.Text = sphereCenter1.ToStringF3() + "\t" + sphereCenter2.ToStringF3();

        }

        private void Create_Sphere(object sender, RoutedEventArgs e)
        {
            Atom3D atom = GetSelectedAtom();
            atom.HasSphere = true;
        }

        private void Delete_Sphere(object sender, RoutedEventArgs e)
        {
            Atom3D atom = GetSelectedAtom();
            atom.HasSphere = false;
        }


        private void Hide_Atom_Without_Polyhedras(object sender, RoutedEventArgs e)
        {
            foreach (var item in CompoundVisual3D.allAtoms)
            {
                item.IsHidden = !item.InPolyhedra && !item.HasPolyhedra && !item.HasSphere;
            }
        }
    }
}
