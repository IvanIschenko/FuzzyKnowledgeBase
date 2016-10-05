using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WebApplication1;
using WebApplication1.FuzzyLogicBase;

namespace WebApplication1
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        private const double ScaleRate = 1.5;
        private const int padding = 100;
        public EdgeMetagraph<int, int> metagraph;
        public Visualizator<int, int> vis;

        public VisualizAlgorithm<int, int> algorithm;
        private Canvas _canvas;

        //
        private Point _mousePos;
        private int _currentMetagraphType;
        private int currentAlgorithmType;
        private int _verticesCount;
        private int _metaVertexCount;
        private float _sparseness;
        private float _includeChance;
        public int _stepCount = 1000;
        private float _kGrav = 5;
        //
        private float _fInnerToInner;
        private float _kInnerVertToMv;
        private float _kMv;
        private double _magneticStrength;

        private float maxTemperature;
        private float _visualisationTime, _oldVisualisationTime;

        public MainWindowVM()
        {
            vis = new Visualizator<int, int>();
        }

        public float MaxTemperature
        {
            get { return maxTemperature; }
            set
            {
                maxTemperature = value;
                if (algorithm != null && algorithm is ModyfiedFruhtermanAlgoritm<int, int>)
                {
                    (algorithm as ModyfiedFruhtermanAlgoritm<int, int>).MaxTemperature = maxTemperature;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("Delta"));
            }
        }

        public float KMv
        {
            get { return _kMv; }
            set
            {
                _kMv = value;
                PropertyChanged(this, new PropertyChangedEventArgs("KMv"));
            }
        }

        public float KInnerVertToMv
        {
            get { return _kInnerVertToMv; }
            set { _kInnerVertToMv = value; PropertyChanged(this, new PropertyChangedEventArgs("KInnerVertToMv")); }
        }

        public float KinnerToInner
        {
            get { return _fInnerToInner; }
            set { _fInnerToInner = value; PropertyChanged(this, new PropertyChangedEventArgs("KinnerToInner")); }
        }
        public double MagneticStrength
        {
            get { return _magneticStrength; }
            set
            {
                _magneticStrength = value;
                if (algorithm != null && algorithm is ModyfiedFruhtermanAlgoritm<int, int>)
                {
                    (algorithm as ModyfiedFruhtermanAlgoritm<int, int>).MagneticStrength = _magneticStrength;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("MagneticStrength"));
            }
        }
        public Point MousePosition
        {
            get
            {
                //  System.Diagnostics.Debug.WriteLine("GET " + _mousePos);
                return _mousePos;
            }
            set
            {
                // System.Diagnostics.Debug.WriteLine("SET " + value);
                _mousePos = value;
                PropertyChanged(this, new PropertyChangedEventArgs("MousePosition"));
            }
        }
        public int CurrentMetagraphType
        {
            get { return _currentMetagraphType; }
            set { _currentMetagraphType = value; }
        }

        public int CurrentAlgorithmType
        {
            get { return currentAlgorithmType; }
            set { currentAlgorithmType = value; }
        }
        public int VerticesCount
        {
            get { return _verticesCount; }
            set { _verticesCount = value; }
        }
        public int MetaVertexCount
        {
            get { return _metaVertexCount; }
            set { _metaVertexCount = value; }
        }
        public float Sparseness
        {
            get { return _sparseness; }
            set { _sparseness = value; }
        }
        public float IncludeChance
        {
            get { return _includeChance; }
            set { _includeChance = value; }
        }
        public int StepCount
        {
            get { return _stepCount; }
            set
            {
                _stepCount = value;
                if (algorithm != null && algorithm is ModyfiedFruhtermanAlgoritm<int, int>)
                {
                    (algorithm as ModyfiedFruhtermanAlgoritm<int, int>).MaxIterationCount = _stepCount;
                    RunAlgorithmWithTask();
                }

                //PropertyChanged(this, new PropertyChangedEventArgs("StepCount"));
            }
        }
        public float KGrav
        {
            get { return _kGrav; }
            set
            {
                _kGrav = value;
                if (algorithm != null && algorithm is ModyfiedFruhtermanAlgoritm<int, int>)
                {
                    (algorithm as ModyfiedFruhtermanAlgoritm<int, int>).GravitationCoefficient = _kGrav;
                }
            }
        }
        private Brush _runButtonColo;
        public Brush RunButtonColor
        {
            get { return _runButtonColo; }
            set { _runButtonColo = value; PropertyChanged(this, new PropertyChangedEventArgs("RunButtonColor")); }
        }
        public Brush TabsColor { get; set; }
        private Task _runAlgorithmTask;
        public Visualizator<int, int> Visualizator
        {
            get { return vis; }
            set { vis = value; PropertyChanged(this, new PropertyChangedEventArgs("Visualizator")); }
        }
        private string _statusBarText;
        public string StatusBarText
        {
            get { return _statusBarText; }
            set
            {
                _statusBarText = value;
                PropertyChanged(this, new PropertyChangedEventArgs("StatusBarText"));
            }
        }
        public ObservableCollection<string> GeneratedMetagraphTypeNames { get; set; }
        public ObservableCollection<string> AlgorithmTypeNames { get; set; }
        //
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        //
        public Command CurrentMetagraphTypeChangedCommand { get; set; }
        public Command SaveImageCommand { get; set; }
        public Command SaveGraphCommand { get; set; }
        public Command LoadGraphCommand { get; set; }
        public Command RefreshCommand { get; set; }
        //public Command ShowKoeffMatrixCommand { get; set; }
        //public Command ShowKoeffMatrixWithoutFormulaCommand { get; set; }
        public Command ShowMetagraphInTextCommand { get; set; }
        public Command GenerateMetagraphFromTextCommand { get; set; }
        public Command VisualiseStepCommand { get; set; }
        public Command RandomizeMetagraphCoordinates { get; set; }
        public Command ChangeAlgorithmCommand { get; set; }
        public MainWindowVM(Canvas canvas, FuzzyKnowledgeBase FKB)
        {
            this._canvas = canvas;
            this.GeneratedMetagraphTypeNames = new ObservableCollection<string>();
            this.AlgorithmTypeNames = new ObservableCollection<string>();
            cbGenerationTypeLoad();
            AlgorithmTypeNamesLoad();
            GenerateMetagraph((int)_canvas.ActualWidth, (int)_canvas.ActualHeight, FKB);
            RunButtonColor = Brushes.Black;
            TabsColor = Brushes.PapayaWhip;

            this.VerticesCount = 15;
            this.MetaVertexCount = 4;
            this.Sparseness = 0.03f;
            this.IncludeChance = 0.03f;
            canvas.LayoutUpdated += canvas_LayoutUpdated;
            this.KGrav = 5f;
            this.KinnerToInner = 1;
            this.KInnerVertToMv = 1;
            this.KMv = 10;
            this.MagneticStrength = 0;
            this.MaxTemperature = 0.5f;
            this.StepCount = 10000;
            this.StatusBarText = "";

            this.SaveImageCommand = new Command(SaveImage);
            this.SaveGraphCommand = new Command(SaveGraph);
            this.LoadGraphCommand = new Command(o => LoadGraph());
            this.CurrentMetagraphTypeChangedCommand = new Command(CurrentMetagraphTypeChanged);
            this.RefreshCommand = new Command(o => { GenerateMetagraph((int)_canvas.ActualWidth, (int)_canvas.ActualHeight, FKB); RefreshMetagraph(); });
            //this.ShowKoeffMatrixCommand = new Command(ShowKoeffMatrix);
            //this.ShowKoeffMatrixWithoutFormulaCommand = new Command(ShowKoeffMatrixWithoutFormula);
            this.ShowMetagraphInTextCommand = new Command(ShowMetagraphInText);
            this.VisualiseStepCommand = new Command(VisualiseStep);
            this.RandomizeMetagraphCoordinates = new Command(RandomizeCoordinates);
            this.ChangeAlgorithmCommand = new Command(ChangeAlgorithm);
            this.Visualizator = new Visualizator<int, int>(metagraph);
            this.algorithm = new ModyfiedFruhtermanAlgoritm<int, int>(metagraph, new Vector(padding, padding), _canvas.ActualWidth - padding * 2, _canvas.ActualHeight - padding * 2,
                (float)((_canvas.ActualWidth) * (_canvas.ActualHeight)),
                MaxTemperature, 10f, KMv, KInnerVertToMv, KinnerToInner, KGrav, StepCount, MagneticStrength);
            RunAlgorithmWithTask();
            GenerateMetagraphFromTextCommand = new Command(GenerateMetagraphFromText);
            //  System.Diagnostics.Debug.WriteLine("height " + _canvas.ActualHeight);
        }

        public void ChangeAlgorithm(object obj)
        {
            switch (currentAlgorithmType)
            {
                case 1:
                    this.algorithm = new ModyfiedFruhtermanAlgoritm<int, int>(metagraph, new Vector(padding, padding), 1500 - padding * 2, 1500 - padding * 2,
                           (float)((1500) * (1500)),
                           0.5, 10f, 50000, 50000, 50000, 250, 1000, 0);
                    break;
                case 0:
                    this.algorithm = new TreeVisualizationAlgorithm<int, int>(metagraph, new Vector(padding, padding), 1200 - padding * 2, 800 - padding * 2,
                               0.5f, 10f, 1, 1, 10, 0, 100, 0);
                    break;
            }
        }

        private void AlgorithmTypeNamesLoad()
        {
            this.AlgorithmTypeNames.Add("ModyfiedFruhtermanAlgoritm");
            this.AlgorithmTypeNames.Add("TreeVisualizationAlgoritm");
        }

        void canvas_LayoutUpdated(object sender, EventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(_canvas.ActualHeight + " " + _canvas.ActualWidth);
        }

        public void GenerateMetagraph(int maxCoordBoxWidth, int maxCoordBoxHeight, FuzzyKnowledgeBase FKB)
        {
            switch (_currentMetagraphType)
            {
                //Random
                case 0:
                    metagraph = MetagraphGenerator.GenerateRandomMetagraph(FKB, VerticesCount, MetaVertexCount, maxCoordBoxWidth, maxCoordBoxHeight,
                        Sparseness, IncludeChance);
                    break;
            }
        }

        private void cbGenerationTypeLoad()
        {
            this.GeneratedMetagraphTypeNames.Add("Random");

            this.GeneratedMetagraphTypeNames.Add("Special1");
            this.GeneratedMetagraphTypeNames.Add("Special");
            this.GeneratedMetagraphTypeNames.Add("Special2");
            this.GeneratedMetagraphTypeNames.Add("SimpleTree");
            this.GeneratedMetagraphTypeNames.Add("RandomTreeMetagraph");
            this.GeneratedMetagraphTypeNames.Add("RandomTreeMetagraph1");
            this.GeneratedMetagraphTypeNames.Add("GenerateTreeSpecialMetagraph");
        }

        public void RefreshMetagraph()
        {
            StatusBarText = "";
            _visualisationTime = 0;
            _oldVisualisationTime = 0;
            // System.Diagnostics.Debug.WriteLine("Metagraph refresh");
            metagraph.Optimisation();
            ChangeAlgorithm(algorithm);
            //algorithm = new ModyfiedFruhtermanAlgoritm<int, int>(metagraph, new Vector(padding, padding), 1500 - padding * 2, 1500 - padding * 2,
            //    (float)((1500) * (1500)),
            //    MaxTemperature, 10f, KMv, KInnerVertToMv, KinnerToInner, KGrav, StepCount, MagneticStrength);
            //TaskScheduler.FromCurrentSynchronizationContext();
            RunAlgorithmWithTask();
        }

        public void Visinit()
        {
            Visualizator = new Visualizator<int, int>(metagraph);
            VisualiseStep(this);
        }

        public void VisualiseStep(object o)
        {
            if (_runAlgorithmTask == null)
            {
                return;
            }
            if (_runAlgorithmTask.IsFaulted)
                // StatusBarText = "ERROR!";
                if (_runAlgorithmTask.IsCanceled)
                    // StatusBarText = "CANCELED!";
                    if (!_runAlgorithmTask.IsCompleted)
                    {
                        // StatusBarText = "Busy at the moment...";
                        return;
                    }

            vis.VisualizeGraph();

            //StatusBarText = String.Format("Visualization finished after {0:N0}ms", _visualisationTime);
            //RunAlgorithmWithTask();
        }

        public void RunAlgorithmWithTask()
        {
            Stopwatch sw = new Stopwatch();
            if (StepCount != 0)
            {
                // StatusBarText = "Busy...";
                //  RunButtonColor = Brushes.Red;

                _runAlgorithmTask = new Task(() =>
                {
                    sw.Start();
                    algorithm.RunAlgorithm();
                    sw.Stop();
                });

                _runAlgorithmTask.ContinueWith((t) =>
                {
                    //RunButtonColor = Brushes.Black;
                    //StatusBarText = String.Empty;
                    if (_visualisationTime != 0)
                    {
                        // StatusBarText = String.Format("Visualization finished after {0:N0}ms", _visualisationTime);
                        _oldVisualisationTime += _visualisationTime;
                    }
                    _visualisationTime = sw.ElapsedMilliseconds;
                }, TaskScheduler.FromCurrentSynchronizationContext());

                _runAlgorithmTask.Start();
            }
        }
        //private void ShowKoeffMatrix(object o)
        //{
        //    ModyfiedFruhtermanAlgoritm<int, int> alg = algorithm as ModyfiedFruhtermanAlgoritm<int, int>;
        //    new WindowKoeffMatrixShow(alg.KRepullMatrix, alg.KAttrMatrix).ShowDialog();
        //}
        //private void ShowKoeffMatrixWithoutFormula(object o)
        //{
        //    ModyfiedFruhtermanAlgoritm<int, int> alg = algorithm as ModyfiedFruhtermanAlgoritm<int, int>;
        //    new WindowKoeffMatrixShow(alg.KRepMatrixWithoutFormula, alg.KAttrMatrixWithoutFormula1).ShowDialog();
        //}
        private void ShowMetagraphInText(object o)
        {
            //new WindowMetagraphInText(metagraph).ShowDialog(); //NEED to WORK!!!!
        }
        private void GenerateMetagraphFromText(object o)
        {
            //new WindowGenerateMetagraph(metagraph, (int)_canvas.ActualWidth, (int)_canvas.ActualHeight).ShowDialog();
            //metagraph.Vertices.ForEach(v => System.Diagnostics.Debug.WriteLine(v.ToString() + " level=" + v.Level));
            //(metagraph as EdgeMetagraph<int, int>).Edges.ForEach(e => System.Diagnostics.Debug.WriteLine(e.ToString())); 
            //RefreshMetagraph();
            LoadGraphFromTextFile();
        }
        private void CurrentMetagraphTypeChanged(object o)
        {
            RefreshMetagraph();
        }
        private void SaveGraph(object o)
        {
            System.Xml.Serialization.XmlSerializer ser =
                new System.Xml.Serialization.XmlSerializer(typeof(EdgeMetagraph<int, int>));
            string path = Directory.GetCurrentDirectory() + "\\Graphs";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.OverwritePrompt = true;
            sfd.CheckPathExists = true;
            sfd.InitialDirectory = path;
            sfd.FileName = String.Format("graph({0};{1};{2};{3}-{4}ms)", _verticesCount,
                _metaVertexCount, Sparseness, IncludeChance, _oldVisualisationTime);
            sfd.DefaultExt = ".txt";
            sfd.Filter = "Graph Files (*.txt)|*.txt";

            bool? result = sfd.ShowDialog();
            string filename = "";
            if (result.HasValue && result.Value)
                filename = sfd.FileName;
            else
                return;

            using (Stream s = new FileStream(filename, FileMode.Create))
            {
                ser.Serialize(s, metagraph);
            }
        }
        private void LoadGraph()
        {
            System.Xml.Serialization.XmlSerializer ser =
                new System.Xml.Serialization.XmlSerializer(typeof(EdgeMetagraph<int, int>));
            string path = Directory.GetCurrentDirectory() + "\\Graphs";
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Title = "Выберите метаграф";
            ofd.InitialDirectory = Directory.GetCurrentDirectory() + "\\Graphs";

            ofd.CheckPathExists = true;
            ofd.InitialDirectory = path;
            ofd.DefaultExt = ".txt";
            ofd.Filter = "Graph Files (*.txt)|*.txt";

            bool? result = ofd.ShowDialog();
            string filename = "";
            if (result.HasValue && result.Value)
                filename = ofd.FileName;
            else
                return;

            using (Stream s = new FileStream(ofd.FileName, FileMode.Open))
            {
                metagraph = (EdgeMetagraph<int, int>)ser.Deserialize(s);
            }
            //переопределение вершин в метавершинах
            foreach (var item in metagraph.Vertices)
            {
                if (item.IsMetavertex)
                    for (int i = 0; i < item.IncludedVertices.Count; i++)
                    {
                        item.IncludedVertices[i] = metagraph.Vertices.Find(v => v.ID == item.IncludedVertices[i].ID);
                    }
            }

            //переопределение вершин в еджах
            foreach (var edge in metagraph.Edges)
            {
                edge.StartVertex = metagraph.Vertices.Find(v => v.ID == edge.StartVertex.ID);
                edge.EndVertex = metagraph.Vertices.Find(v => v.ID == edge.EndVertex.ID);
            }

            RefreshMetagraph();

            //System.Diagnostics.Debug.WriteLine("LoadOK");
        }
        private void LoadGraphFromTextFile()
        {
            string path = Directory.GetCurrentDirectory() + "\\Graphs";
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Title = "Выберите метаграф";
            ofd.InitialDirectory = Directory.GetCurrentDirectory() + "\\Graphs";

            ofd.CheckPathExists = true;
            ofd.InitialDirectory = path;
            ofd.DefaultExt = ".txt";
            ofd.Filter = "Graph Files (*.txt)|*.txt";

            bool? result = ofd.ShowDialog();
            string filename = "";
            if (result.HasValue && result.Value)
                filename = ofd.FileName;
            else
                return;
            String metagrText;
            using (StreamReader sr = new StreamReader(ofd.FileName))
            {
                metagrText = sr.ReadToEnd();
            }

            metagraph = MetagraphGenerator.GenerateMetagraphFromText(metagrText, (int)_canvas.ActualWidth, (int)_canvas.ActualHeight);
            foreach (MetaVertex<int> item in metagraph.Vertices)
            {
                Console.Write(item.Name + " l: " + item.Level + ", ");
            }
            // metagraph.RandomizeCoordinates(0, (int)_canvas.ActualWidth, 0, (int)_canvas.ActualHeight);
            RefreshMetagraph();

            //System.Diagnostics.Debug.WriteLine("LoadOK");
        }
        private void SaveImage(object o)
        {
            _canvas.Width = _canvas.ActualWidth;
            _canvas.Height = _canvas.ActualHeight;
            SaveCanvasToFile(_canvas);
        }
        private void SaveCanvasToFile(Canvas surface)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();

            sfd.OverwritePrompt = true;
            sfd.CheckPathExists = true;
            sfd.FileName = String.Format("graph({0};{1};{2};{3}-{4}ms)", _verticesCount,
                _metaVertexCount, Sparseness, IncludeChance, _oldVisualisationTime);
            sfd.DefaultExt = ".png";
            sfd.Filter = "Graph Files (*.png)|*.png";

            bool? result = sfd.ShowDialog();
            string filename = "";
            if (result.HasValue && result.Value)
                filename = sfd.FileName;
            else
                return;
            // Save current canvas transform
            //Transform transform = surface.LayoutTransform;
            // reset current transform (in case it is scaled or rotated)
            surface.LayoutTransform = null;
            // Get the size of canvas
            Size size = new Size(surface.Width, surface.Height);
            // Measure and arrange the surface
            // VERY IMPORTANT
            surface.Measure(size);
            surface.Arrange(new Rect(size));
            surface.UpdateLayout();
            // Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBitmap =
              new RenderTargetBitmap(
                (int)size.Width,
                (int)size.Height,
                96d,
                96d,
                PixelFormats.Pbgra32);
            renderBitmap.Render(surface);
            // Create a file stream for saving image
            using (FileStream outStream = new FileStream(filename, FileMode.Create))
            {
                // Use png encoder for our data
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                // push the rendered bitmap to it
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                // save the data to the stream
                encoder.Save(outStream);
            }
        }
        private void RandomizeCoordinates(object o)
        {
            metagraph.RandomizeCoordinates(0, (int)_canvas.ActualWidth, 0, (int)_canvas.ActualHeight);
            vis.VisualizeGraph();
            StatusBarText = "";
            _visualisationTime = 0;
            _oldVisualisationTime = 0;
        }
        public void MouseMoveOnCanvas(MouseEventArgs e)
        {
            MousePosition = Mouse.GetPosition(_canvas);
        }
    }
}
