using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NAR.Forms.Tests.Controls
{
    public partial class AvailableItems : UserControl
    {
        #region Events / Delegates
        public delegate void InsertItem(object sender, InsertSelectedItemEventArgs e);
        public event InsertItem OnInsertItem;
        #endregion

        #region Constructors/Destructors
        public AvailableItems()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        private void Initialize()
        {
            this.treAvailable.Nodes.Clear();

            this.treAvailable.Nodes.Add(InitializeBorders());
            this.treAvailable.Nodes.Add(InitializeDistance());
            this.treAvailable.Nodes.Add(InitializeEffects());
            this.treAvailable.Nodes.Add(InitializeImages());
            this.treAvailable.Nodes.Add(InitializeMotion());
            this.treAvailable.Nodes.Add(InitializeOperations());
            this.treAvailable.Nodes.Add(InitializeOperators()); 
            this.treAvailable.Nodes.Add(InitializePixels());
            this.treAvailable.Nodes.Add(InitializeSize());
            this.treAvailable.Nodes.Add(InitializeUtils());
            this.treAvailable.Nodes.Add(InitializeSegmentation());
            this.treAvailable.Nodes.Add(InitializeCorners());
            this.treAvailable.Nodes.Add(InitializeContours());



            
        }
        private TreeNode InitializeBorders()
        {
            TreeNode nodeParent = new TreeNode("Borders");
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.BooleanEdgeCommand), true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.CannyCommand), true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.DifferenceCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.EdgeDetectQuickCommand), true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.EmbossLaplacianCommand), true)));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.EuclidianDistanceCommand), true)));            
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.HomogenityCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.HorizontalCommand))));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.HumanCommand), true)));                        
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.KirshCommand), true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.LaplaceCommand), true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.LaplaceOfGaussianCommand), true)));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.MaskCommand(new ImageProcessing.Borders.MaskData(2,2))));
            //nodeParent.Nodes.Add(CreateNode(new NAR.ImageProcessing.Borders.MaskData(2,2)));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.MarrHildrethCommand), true)));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.MultiFlashCommand), true)));            
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.PrewittCommand), true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.RichardsCommand), true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.RobinsonCommand), true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.SobelCommand), true)));


            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.BooleanEdgeCommand), true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.EuclidianDistanceCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.HumanCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.MarrHildrethCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Borders.MultiFlashCommand))));
            
            return nodeParent;
        }
        private TreeNode InitializeDistance()
        {
            TreeNode nodeParent = new TreeNode("Distance");

            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Distance.CalcDistanceCommand))));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Distance.DistanceCalculator))));
            //    0, 100, 0, 100, 50, true)));
            
            return nodeParent;
        }
        private TreeNode InitializeEffects()
        {
            TreeNode nodeParent = new TreeNode("Effects");
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Effects.DilatationCommand), (int)2)));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Effects.DistanceFunctionCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Effects.ErosionCommand), (int)2)));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Effects.FourierTransformCommand))));            
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Effects.GaussianBlurCommand), true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Effects.MeanRemovalCommand), true)));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Effects.MedianCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Effects.MoireCommand), (int)15)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Effects.PixelateCommand),(short)10,(bool)true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Effects.RandomJitterCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Effects.SharpenCommand), true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Effects.SmoothCommand), true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Effects.SphereCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Effects.SwirlCommand),(double)0.05)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Effects.TimeWarpCommand),(byte)15)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Effects.WaterCommand),(short)5)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Effects.ZhangSuenCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Effects.GaussianCommand), false)));
            
            return nodeParent;
        }
        private TreeNode InitializeImages()
        {
            TreeNode nodeParent = new TreeNode("Images");

            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.AverageCommand), false)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.BlackWhiteCommand), false)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.BlueCommand))));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.ColorCommand),ImageProcessing.Images.ColorCommand.Colors.Blue)));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.ColorCommand),ImageProcessing.Images.ColorCommand.Colors.Green)));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.ColorCommand),ImageProcessing.Images.ColorCommand.Colors.Red)));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.EqualizationCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.GrayscaleCommand), false)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.GreenCommand))));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.HysteresisThresholdCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.InvertCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.LuminosityCommand),(double)1.2,(double)5)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.NormalizeCommand),true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.RedCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.RotateCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.SepiaCommand))));



            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.Binarization.BayerDitheringCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.Binarization.BurkesDitheringCommand))));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.Binarization.ErrorDiffusionDitheringCommand))));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.Binarization.ErrorDiffusionToAdjacentNeighborsCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.Binarization.FloydSteinbergDitheringCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.Binarization.JarvisJudiceNinkeDitheringCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.Binarization.OrderedDitheringCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.Binarization.SierraDitheringCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.Binarization.StuckiDitheringCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.Binarization.ThresholdCommand), 128, true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.Binarization.ThresholdWithCarryCommand))));

            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.Binarization.Adaptative.BradleyLocalThresholdingCommand), 41, 0.15f)));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.Binarization.Adaptative.IterativeThresholdCommand))));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.Binarization.Adaptative.OtsuThresholdCommand))));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Images.Binarization.Adaptative.SISThresholdCommand))));
            

            return nodeParent;
        }
        private TreeNode InitializeMotion()
        {
            TreeNode nodeParent = new TreeNode("Motion");

            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Motion.DetectMotionCommand),(int)320,(int)240,(byte)10,(int)90,true)));
            
            return nodeParent;
        }
        private TreeNode InitializeOperations()
        {
            TreeNode nodeParent = new TreeNode("Operations");

            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Operations.BaseOperationCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Operations.DivisionCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Operations.MultiplicationCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Operations.SubtractionCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Operations.SumCommand))));


            return nodeParent;
        }
        private TreeNode InitializeOperators()
        {
            TreeNode nodeParent = new TreeNode("Operators");

            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Operators.AndCommand))));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Operators.BaseOperatorCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Operators.NandCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Operators.NorCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Operators.NotCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Operators.OrCommand))));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Operators.XorCommand))));

            

            return nodeParent;
        }
        private TreeNode InitializePixels()
        {
            TreeNode nodeParent = new TreeNode("Pixels");

            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Pixels.CountPixelCommand),(int)10,(int)200,(int)10,(int)200,true)));
            

            return nodeParent;
        }
        private TreeNode InitializeSize()
        {
            TreeNode nodeParent = new TreeNode("Size");

            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Size.SizeCalculatorCommand),
                (int)0,(int)256,(int)125,(int)125,(double)10,(bool)true)));
            
            return nodeParent;
        }
        private TreeNode InitializeUtils()
        {
            TreeNode nodeParent = new TreeNode("Utils");

            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Utils.Histogram))));
            

            return nodeParent;
        }
        private TreeNode InitializeSegmentation()
        {
            TreeNode nodeParent = new TreeNode("Segmentation");

            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Segmentation.HoughLineTransformationCommand), true)));

            return nodeParent;
        }
        private TreeNode InitializeCorners()
        {
            TreeNode nodeParent = new TreeNode("Corners");

            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Corner.HarrisCornersCommand), 0.05, 9000000, 0, 11, true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Corner.SusanCornersCommand), 25, 18, true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Corner.MoravecCornersCommand), 500, 3, true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Corner.BeaudetCornersCommand), true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Corner.CssCornersCommand), true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Corner.DericheCornersCommand), true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Corner.ForstnerCornersCommand), true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Corner.KitchenRosenfeldCornersCommand),true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Corner.PlesseyCornersCommand), true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Corner.ShiTomasiCornersCommand),true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Corner.TrajkovicHedley4CornersCommand),true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Corner.TrajkovicHedley8CornersCommand),true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Corner.WangBradCornersCommand),true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Corner.ZhengWangCornersCommand),true)));

            return nodeParent;
        }
        private TreeNode InitializeContours()
        {
            TreeNode nodeParent = new TreeNode("Contours");

            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Contours.GreedContoursCommand), true)));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Contours.KassContoursCommand), true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Contours.MooreNeighborContoursCommand),true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Contours.RadialSweepContoursCommand),true)));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Contours.SnakeContoursCommand),true)));
            nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Contours.SquareContoursCommand),true)));
            //nodeParent.Nodes.Add(CreateNode(new AvailableEntry(typeof(NAR.ImageProcessing.Contours.TheoPavlidisContoursCommand),true)));
            

            return nodeParent;
        }
        
        private TreeNode CreateNode(AvailableEntry entry)
        {
            TreeNode node = new TreeNode(entry.Command.Name);
            node.Tag = entry;
            return node;
        }
        private void InsertEntry()
        {
            TreeNode selectedNode = this.treAvailable.SelectedNode;

            if (selectedNode == null)
                return;

            if (selectedNode.Tag == null)
                return;

            if (OnInsertItem != null)
                OnInsertItem(this, new InsertSelectedItemEventArgs((AvailableEntry)selectedNode.Tag));
            
        }
        #endregion

        #region Events
        private void AvailableItems_Load(object sender, EventArgs e)
        {
            Initialize();
        }
        private void treAvailable_DoubleClick(object sender, EventArgs e)
        {
            InsertEntry();
        }
        private void treAvailable_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                InsertEntry();
            }
        }
        #endregion   
    }
}
