using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NAR.Console.Tests
{
    public class Execution
    {
        #region Constructors/Destructors
        public Execution()
        {
        }
        #endregion

        #region Methods
        public void Run()
        {
            ARKit.ARToolKit kit = new ARKit.ARToolKit();
            Bitmap bmp = new Bitmap(".\\Resources\\Contours.png");
            Model.IImage image = new Model.ImageBitmap(bmp);

            int [] label_ref = null;
            int [] area = null;
            double[] pos= null;
            int[] clip = null;
            int label_num = 0;

            int marker_num = 0;

            ARKit.ARToolKit.ARMarkerInfo [] marker_info =null;


            kit.arDetectMarker(image, 100, ref marker_info, ref marker_num); 



            //int [] limage = kit.labeling2(image, 100, ref label_num, ref area, ref pos, ref clip,ref label_ref );
            
            //Model.IImage step1 =  kit.CreateLabelImage(bmp.Width, bmp.Height);

            //image.Image.Save("C:\\temp\\step0.bmp");
            //step1.Image.Save("c:\\temp\\step1.bmp");

            //ARKit.ARToolKit.ARMarkerInfo2 [] resStep3 = kit.arDetectMarker2(image, label_num, label_ref, area, pos, clip, 100000, 70, 1.0, ref marker_num); 


            RunImageProcessingContoursContourCounterCommandTests();


            RunImageProcessingImagesTests();
            //RunImageProcessingCameraCalibrationTests();

            RunNARTests();
        }

        #region NAR Tests

        private void RunNARTests()
        {
            RunArManagementTests();            
            RunImageProcessingTests();
            //RunModelTests();
            RunCaptureTests();
        }

        #region Capture Tests

        private void RunCaptureTests()
        {
            RunCaptureDriverTests();
        }

        #region Capture Driver Tests

        private void RunCaptureDriverTests()
        {
            RunCaptureDriverWebCamWinDriverTests();
        }
        private void RunCaptureDriverWebCamWinDriverTests()
        {
            NAR.Tests.Capture.Drivers.WebCamWinAPI.WebCamWinDriver test = new NAR.Tests.Capture.Drivers.WebCamWinAPI.WebCamWinDriver();
            test.Init();
            test.TestConnectDisconnect();
            test.TestGrabFrame();
            test.TestRecordStartAndStopByEvent();
            //test.TestRecordStartAndStopByTimeLimit();
            test.TestGrabbingTime();
            test.Cleanup();
        }

        #endregion
        
        #endregion

        #region Image Processing Tests

        private void RunImageProcessingTests()
        {
            RunImageProcessingContoursTests();
            
            RunImageProcessingBorderTests();
            RunImageProcessingImagesTests();            
            RunImageProcessingTexturesTests();
            RunImageProcessingCornerTests();
            RunImageProcessingSegmentationTests();            
            RunImageProcessingCameraCalibrationTests();
            RunImageProcessingUtilTests();
            RunImageProcessingOperationTests();
            RunImageProcessingOperatorTests();
            RunImageProcessingEffectsTests();
            RunImageProcessingMotionTests();
            RunImageProcessingDistanceTests();
            RunImageProcessingSizeTests();
            RunImageProcessingPixelsTests();
            RunImageProcessingImageInvokerTests();
        }

        #region Image Processing Operators Tests

        private void RunImageProcessingOperatorTests()
        {
            RunImageProcessingOperatorAndCommandTests();
            RunImageProcessingOperatorNandCommandTests();
            RunImageProcessingOperatorNorCommandTests();
            RunImageProcessingOperatorNotCommandTests();
            RunImageProcessingOperatorOrCommandTests();
            RunImageProcessingOperatorXorCommandTests();
        }

        private void RunImageProcessingOperatorAndCommandTests()
        {
            NAR.Tests.ImageProcessing.Operators.AndCommandTests test = new NAR.Tests.ImageProcessing.Operators.AndCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingOperatorNandCommandTests()
        {
            NAR.Tests.ImageProcessing.Operators.NandCommandTests test = new NAR.Tests.ImageProcessing.Operators.NandCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingOperatorNorCommandTests()
        {
            NAR.Tests.ImageProcessing.Operators.NorCommandTests test = new NAR.Tests.ImageProcessing.Operators.NorCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingOperatorNotCommandTests()
        {
            NAR.Tests.ImageProcessing.Operators.NotCommandTests test = new NAR.Tests.ImageProcessing.Operators.NotCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingOperatorOrCommandTests()
        {
            NAR.Tests.ImageProcessing.Operators.OrCommandTests test = new NAR.Tests.ImageProcessing.Operators.OrCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingOperatorXorCommandTests()
        {
            NAR.Tests.ImageProcessing.Operators.XorCommandTests test = new NAR.Tests.ImageProcessing.Operators.XorCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }


        #endregion

        #region Image Processing Operations Tests 

        private void RunImageProcessingOperationTests()
        {
            RunImageProcessingOperationSumCommandTests();
            RunImageProcessingOperationSubtractionCommandTests();
            RunImageProcessingOperationMultiplicationCommandTests();
            RunImageProcessingOperationDivisionCommandTests();
        }

        private void RunImageProcessingOperationSumCommandTests()
        {
            NAR.Tests.ImageProcessing.Operations.SumCommandTests test = new NAR.Tests.ImageProcessing.Operations.SumCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingOperationSubtractionCommandTests()
        {
            NAR.Tests.ImageProcessing.Operations.SubtractionCommandTests test = new NAR.Tests.ImageProcessing.Operations.SubtractionCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingOperationMultiplicationCommandTests()
        {
            NAR.Tests.ImageProcessing.Operations.MultiplicationCommandTests test = new NAR.Tests.ImageProcessing.Operations.MultiplicationCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingOperationDivisionCommandTests()
        {
            NAR.Tests.ImageProcessing.Operations.DivisionCommandTests test = new NAR.Tests.ImageProcessing.Operations.DivisionCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }


        #endregion

        #region Image Processing Effects Tests
        private void RunImageProcessingEffectsTests()
        {

            RunImageProcessingEffectsGaussianCommandTests();
            RunImageProcessingEffectsGaussianBlurCommandTests();
            RunImageProcessingEffectsMeanRemovalCommandTests();
            RunImageProcessingEffectsSharpenCommandTests();
            RunImageProcessingEffectsEmbossLaplacianCommandTests();
            RunImageProcessingEffectsSphereCommandTests();
            RunImageProcessingEffectsSwirlCommandTests();
            RunImageProcessingEffectsTimeWarpCommandTests();
            RunImageProcessingEffectsMoireCommandTests();
            RunImageProcessingEffectsWaterCommandTests();
            RunImageProcessingEffectsPixelateCommandTests();
            RunImageProcessingEffectsDilatationCommandTests();
            RunImageProcessingEffectsErosionCommandTests();
            RunImageProcessingEffectsZhangSuenCommandTests();
        }

        private void RunImageProcessingEffectsGaussianBlurCommandTests()
        {
            NAR.Tests.ImageProcessing.Effects.GaussianBlurCommandTests test = new NAR.Tests.ImageProcessing.Effects.GaussianBlurCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingEffectsMeanRemovalCommandTests()
        {
            NAR.Tests.ImageProcessing.Effects.MeanRemovalCommandTests test = new NAR.Tests.ImageProcessing.Effects.MeanRemovalCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingEffectsSharpenCommandTests()
        {
            NAR.Tests.ImageProcessing.Effects.SharpenCommandTests test = new NAR.Tests.ImageProcessing.Effects.SharpenCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingEffectsEmbossLaplacianCommandTests()
        {
            NAR.Tests.ImageProcessing.Effects.EmbossLaplacianCommandTests test = new NAR.Tests.ImageProcessing.Effects.EmbossLaplacianCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingEffectsSphereCommandTests()
        {
            NAR.Tests.ImageProcessing.Effects.SphereCommandTests test = new NAR.Tests.ImageProcessing.Effects.SphereCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingEffectsSwirlCommandTests()
        {
            NAR.Tests.ImageProcessing.Effects.SwirlCommandTests test = new NAR.Tests.ImageProcessing.Effects.SwirlCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingEffectsTimeWarpCommandTests()
        {
            NAR.Tests.ImageProcessing.Effects.TimeWarpCommandTests test = new NAR.Tests.ImageProcessing.Effects.TimeWarpCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingEffectsMoireCommandTests()
        {
            NAR.Tests.ImageProcessing.Effects.MoireCommandTests test = new NAR.Tests.ImageProcessing.Effects.MoireCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingEffectsWaterCommandTests()
        {
            NAR.Tests.ImageProcessing.Effects.WaterCommandTests test = new NAR.Tests.ImageProcessing.Effects.WaterCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingEffectsPixelateCommandTests()
        {
            NAR.Tests.ImageProcessing.Effects.PixelateCommandTests test = new NAR.Tests.ImageProcessing.Effects.PixelateCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingEffectsDilatationCommandTests()
        {
            NAR.Tests.ImageProcessing.Effects.DilatationCommandTests test = new NAR.Tests.ImageProcessing.Effects.DilatationCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingEffectsErosionCommandTests()
        {
            NAR.Tests.ImageProcessing.Effects.ErosionCommandTests test = new NAR.Tests.ImageProcessing.Effects.ErosionCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingEffectsZhangSuenCommandTests()
        {
            NAR.Tests.ImageProcessing.Effects.ZhangSuenCommandTests test = new NAR.Tests.ImageProcessing.Effects.ZhangSuenCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingEffectsGaussianCommandTests()
        {
            NAR.Tests.ImageProcessing.Effects.GaussianCommandTests test = new NAR.Tests.ImageProcessing.Effects.GaussianCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        #endregion

        #region Image Processing Borders Tests

        private void RunImageProcessingBorderTests()
        {
            RunImageProcessingBorderLaplaceOfGaussianCommandTests();
            RunImageProcessingBorderBooleanEdgeCommandTests(); 
            RunImageProcessingBorderRobinsonCommandTests();
            RunImageProcessingBorderSmoothCommandTests();
            RunImageProcessingBorderEdgeDetectQuickCommandTests();
            RunImageProcessingBorderSobelCommandTests();
            RunImageProcessingBorderPrewittCommandTests();
            RunImageProcessingBorderKirshCommandTests();
            RunImageProcessingBorderLaplaceCommandTests();
            RunImageProcessingBorderCannyCommandTests();
            RunImageProcessingBorderDifferenceCommandTests();
            RunImageProcessingBorderHomogenityCommandTests();
            RunImageProcessingBorderHorizontalCommandTests();
            RunImageProcessingBorderRichardsCommandTests();
            RunImageProcessingBorderEuclidianDistanceCommandTests();
            RunImageProcessingBorderHumanCommandTests();
            RunImageProcessingBorderMarrHildrethCommandTests();
            RunImageProcessingBorderMultiFlashCommandTests();
        }
       
        private void RunImageProcessingBorderRobinsonCommandTests()
        {
            NAR.Tests.ImageProcessing.Borders.RobinsonCommandTests test = new NAR.Tests.ImageProcessing.Borders.RobinsonCommandTests();
            test.Init();
            test.TestExecute();
            test.TestExecuteInGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingBorderSmoothCommandTests()
        {
            NAR.Tests.ImageProcessing.Effects.SmoothCommandTests test = new NAR.Tests.ImageProcessing.Effects.SmoothCommandTests();
            test.Init();
            test.TestExecute();          
            test.Cleanup();
        }
        private void RunImageProcessingBorderEdgeDetectQuickCommandTests()
        {
            NAR.Tests.ImageProcessing.Borders.EdgeDetectQuickCommandTests test = new NAR.Tests.ImageProcessing.Borders.EdgeDetectQuickCommandTests();
            test.Init();
            test.TestExecute();
            test.TestExecuteInGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingBorderSobelCommandTests()
        {
            NAR.Tests.ImageProcessing.Borders.SobelCommandTests test = new NAR.Tests.ImageProcessing.Borders.SobelCommandTests();
            test.Init();
            test.TestExecute();
            test.TestExecuteInGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingBorderPrewittCommandTests()
        {
            NAR.Tests.ImageProcessing.Borders.PrewittCommandTests test = new NAR.Tests.ImageProcessing.Borders.PrewittCommandTests();
            test.Init();
            test.TestExecute();
            test.TestExecuteInGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingBorderKirshCommandTests()
        {
            NAR.Tests.ImageProcessing.Borders.KirshCommandTests test = new NAR.Tests.ImageProcessing.Borders.KirshCommandTests();
            test.Init();
            test.TestExecute();
            test.TestExecuteInGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingBorderLaplaceCommandTests()
        {
            NAR.Tests.ImageProcessing.Borders.LaplaceCommandTests test = new NAR.Tests.ImageProcessing.Borders.LaplaceCommandTests();
            test.Init();
            test.TestExecute();
            test.TestExecuteInGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingBorderCannyCommandTests()
        {
            NAR.Tests.ImageProcessing.Borders.CannyCommandTests test = new NAR.Tests.ImageProcessing.Borders.CannyCommandTests();
            test.Init();
            test.TestExecute();
            test.TestExecuteInGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingBorderDifferenceCommandTests()
        {
            NAR.Tests.ImageProcessing.Borders.DifferenceCommandTests test = new NAR.Tests.ImageProcessing.Borders.DifferenceCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingBorderHomogenityCommandTests()
        {
            NAR.Tests.ImageProcessing.Borders.HomogenityCommandTests test = new NAR.Tests.ImageProcessing.Borders.HomogenityCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingBorderHorizontalCommandTests()
        {
            NAR.Tests.ImageProcessing.Borders.HorizontalCommandTests test = new NAR.Tests.ImageProcessing.Borders.HorizontalCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingBorderRichardsCommandTests()
        {
            NAR.Tests.ImageProcessing.Borders.RichardsCommandTests test = new NAR.Tests.ImageProcessing.Borders.RichardsCommandTests();
            test.Init();
            test.TestExecute();
            test.TestExecuteInGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingBorderLaplaceOfGaussianCommandTests()
        {
            NAR.Tests.ImageProcessing.Borders.LaplaceOfGaussianCommandTests test = new NAR.Tests.ImageProcessing.Borders.LaplaceOfGaussianCommandTests();
            test.Init();
            test.TestExecute();
            test.TestExecuteInGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingBorderBooleanEdgeCommandTests()
        {
            NAR.Tests.ImageProcessing.Borders.BooleanEdgeCommandTests test = new NAR.Tests.ImageProcessing.Borders.BooleanEdgeCommandTests();
            test.Init();
            test.TestExecute();
            test.TestExecuteByLimiar();
            test.Cleanup();
        }
        private void RunImageProcessingBorderEuclidianDistanceCommandTests()
        {
            NAR.Tests.ImageProcessing.Borders.EuclidianDistanceCommandTests test = new NAR.Tests.ImageProcessing.Borders.EuclidianDistanceCommandTests();
            test.Init();
            test.TestExecute();
            test.TestExecuteInGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingBorderHumanCommandTests()
        {
            NAR.Tests.ImageProcessing.Borders.HumanCommandTests test = new NAR.Tests.ImageProcessing.Borders.HumanCommandTests();
            test.Init();
            test.TestExecute();
            test.TestExecuteInGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingBorderMarrHildrethCommandTests()
        {
            NAR.Tests.ImageProcessing.Borders.MarrHildrethCommandTests test = new NAR.Tests.ImageProcessing.Borders.MarrHildrethCommandTests();
            test.Init();
            test.TestExecute();
            test.TestExecuteInGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingBorderMultiFlashCommandTests()
        {
            NAR.Tests.ImageProcessing.Borders.MultiFlashCommandTests test = new NAR.Tests.ImageProcessing.Borders.MultiFlashCommandTests();
            test.Init();
            test.TestExecute();
            test.TestExecuteInGrayScale();
            test.Cleanup();
        }

        #endregion

        #region Image Processing Images Tests

        private void RunImageProcessingImagesTests()
        {

            RunImageProcessingImagesBinarizationTests();
            RunImageProcessingImagesColorReductionTests();


            RunImageProcessingImagesAverageCommandTests();
            RunImageProcessingImagesBlackWhiteCommandTests();
            RunImageProcessingImagesGrayscaleCommandTests();
            RunImageProcessingImagesRedCommandTests();
            RunImageProcessingImagesGreenCommandTests();
            RunImageProcessingImagesBlueCommandTests();
            RunImageProcessingImagesInvertCommandTests();
            RunImageProcessingImagesLuminosityCommandTests();
            RunImageProcessingImagesRotateCommandTests();
            RunImageProcessingImagesNormalizeCommandTests();
            RunImageProcessingImagesSepiaCommandTests();
        }

        #region Image Processing Images Binarization Tests

        private void RunImageProcessingImagesBinarizationTests()
        {

            
            RunImageProcessingImagesBinarizationOrderedDitheringTests();
            RunImageProcessingImagesBinarizationBayerDitheringTests();
            RunImageProcessingImagesBinarizationBurkesDitheringTests();
            //RunImageProcessingImagesBinarizationErrorDiffusionDitheringTests();
            RunImageProcessingImagesBinarizationFloydSteinbergDitheringTests();
            RunImageProcessingImagesBinarizationJarvisJudiceNinkeDitheringTests();
            RunImageProcessingImagesBinarizationSierraDitheringTests();
            RunImageProcessingImagesBinarizationStuckiDitheringTests();
            RunImageProcessingImagesBinarizationThresholdTests();
            //RunImageProcessingImagesBinarizationThresholdWithCarryTests();
            //RunImageProcessingImagesBinarizationErrorDiffusionToAdjacentNeighborsTests();

            RunImageProcessingImagesBinarizationAdaptativeTests();
        }

        private void RunImageProcessingImagesBinarizationBayerDitheringTests()
        {
            NAR.Tests.ImageProcessing.Images.Binarization.BayerDitheringCommandTests test = new NAR.Tests.ImageProcessing.Images.Binarization.BayerDitheringCommandTests();
            test.Init();
            test.TestExecuteGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingImagesBinarizationBurkesDitheringTests()
        {
            NAR.Tests.ImageProcessing.Images.Binarization.BurkesDitheringCommandTests test = new NAR.Tests.ImageProcessing.Images.Binarization.BurkesDitheringCommandTests();
            test.Init();
            test.TestExecuteGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingImagesBinarizationErrorDiffusionDitheringTests()
        {
            NAR.Tests.ImageProcessing.Images.Binarization.ErrorDiffusionDitheringCommandTests test = new NAR.Tests.ImageProcessing.Images.Binarization.ErrorDiffusionDitheringCommandTests();
            test.Init();
            test.TestExecuteGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingImagesBinarizationErrorDiffusionToAdjacentNeighborsTests()
        {
            NAR.Tests.ImageProcessing.Images.Binarization.ErrorDiffusionToAdjacentNeighborsCommandTests test = new NAR.Tests.ImageProcessing.Images.Binarization.ErrorDiffusionToAdjacentNeighborsCommandTests();
            test.Init();
            test.TestExecuteGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingImagesBinarizationFloydSteinbergDitheringTests()
        {
            NAR.Tests.ImageProcessing.Images.Binarization.FloydSteinbergDitheringCommandTests test = new NAR.Tests.ImageProcessing.Images.Binarization.FloydSteinbergDitheringCommandTests();
            test.Init();
            test.TestExecuteGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingImagesBinarizationJarvisJudiceNinkeDitheringTests()
        {
            NAR.Tests.ImageProcessing.Images.Binarization.JarvisJudiceNinkeDitheringCommandTests test = new NAR.Tests.ImageProcessing.Images.Binarization.JarvisJudiceNinkeDitheringCommandTests();
            test.Init();
            test.TestExecuteGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingImagesBinarizationOrderedDitheringTests()
        {
            NAR.Tests.ImageProcessing.Images.Binarization.OrderedDitheringCommandTests test = new NAR.Tests.ImageProcessing.Images.Binarization.OrderedDitheringCommandTests();
            test.Init();
            test.TestExecuteGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingImagesBinarizationSierraDitheringTests()
        {
            NAR.Tests.ImageProcessing.Images.Binarization.SierraDitheringCommandTests test = new NAR.Tests.ImageProcessing.Images.Binarization.SierraDitheringCommandTests();
            test.Init();
            test.TestExecuteGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingImagesBinarizationStuckiDitheringTests()
        {
            NAR.Tests.ImageProcessing.Images.Binarization.StuckiDitheringCommandTests test = new NAR.Tests.ImageProcessing.Images.Binarization.StuckiDitheringCommandTests();
            test.Init();
            test.TestExecuteGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingImagesBinarizationThresholdTests()
        {
            NAR.Tests.ImageProcessing.Images.Binarization.ThresholdCommandTests test = new NAR.Tests.ImageProcessing.Images.Binarization.ThresholdCommandTests();
            test.Init();
            test.TestExecuteGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingImagesBinarizationThresholdWithCarryTests()
        {
            NAR.Tests.ImageProcessing.Images.Binarization.ThresholdWithCarryCommandTests test = new NAR.Tests.ImageProcessing.Images.Binarization.ThresholdWithCarryCommandTests();
            test.Init();
            test.TestExecuteGrayScale();
            test.Cleanup();
        }


        #region Image Processing Images Binarization Adaptative Tests

        private void RunImageProcessingImagesBinarizationAdaptativeTests()
        {
            RunImageProcessingImagesBinarizationAdaptativeBradleyLocalThresholdingTests();
            RunImageProcessingImagesBinarizationAdaptativeIterativeThresholdTests();
            RunImageProcessingImagesBinarizationAdaptativeOtsuThresholdTests();
            RunImageProcessingImagesBinarizationAdaptativeSISThresholdTests();
        }
        private void RunImageProcessingImagesBinarizationAdaptativeBradleyLocalThresholdingTests()
        {
            NAR.Tests.ImageProcessing.Images.Binarization.Adaptative.BradleyLocalThresholdingCommandTests test = new NAR.Tests.ImageProcessing.Images.Binarization.Adaptative.BradleyLocalThresholdingCommandTests();
            test.Init();
            test.TestExecuteGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingImagesBinarizationAdaptativeIterativeThresholdTests()
        {
            NAR.Tests.ImageProcessing.Images.Binarization.Adaptative.IterativeThresholdCommandTests test = new NAR.Tests.ImageProcessing.Images.Binarization.Adaptative.IterativeThresholdCommandTests();
            test.Init();
            test.TestExecuteGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingImagesBinarizationAdaptativeOtsuThresholdTests()
        {
            NAR.Tests.ImageProcessing.Images.Binarization.Adaptative.OtsuThresholdCommandTests test = new NAR.Tests.ImageProcessing.Images.Binarization.Adaptative.OtsuThresholdCommandTests();
            test.Init();
            test.TestExecuteGrayScale();
            test.Cleanup();
        }
        private void RunImageProcessingImagesBinarizationAdaptativeSISThresholdTests()
        {
            NAR.Tests.ImageProcessing.Images.Binarization.Adaptative.SISThresholdCommandTests test = new NAR.Tests.ImageProcessing.Images.Binarization.Adaptative.SISThresholdCommandTests();
            test.Init();
            test.TestExecuteGrayScale();
            test.Cleanup();
        }

        #endregion

        #endregion

        #region Image Processing Images ColorReduction Tests

        private void RunImageProcessingImagesColorReductionTests()
        {
            RunImageProcessingImagesColorReductionBurkesColorDitheringTests();
            RunImageProcessingImagesColorReductionColorErrorDiffusionToAdjacentNeighborsTests();
            RunImageProcessingImagesColorReductionColorImageQuantizerTests();
            RunImageProcessingImagesColorReductionErrorDiffusionColorDitheringTests();
            RunImageProcessingImagesColorReductionFloydSteinbergColorDitheringTests();
            RunImageProcessingImagesColorReductionJarvisJudiceNinkeColorDitheringTests();
            RunImageProcessingImagesColorReductionMedianCutCubeTests();
            RunImageProcessingImagesColorReductionMedianCutQuantizerTests();
            RunImageProcessingImagesColorReductionOrderedColorDitheringTests();
            RunImageProcessingImagesColorReductionSierraColorDitheringTests();
            RunImageProcessingImagesColorReductionStuckiColorDitheringTests();
        }
        
        private void RunImageProcessingImagesColorReductionBurkesColorDitheringTests()
        {
        }
        private void RunImageProcessingImagesColorReductionColorErrorDiffusionToAdjacentNeighborsTests()
        {
        }
        private void RunImageProcessingImagesColorReductionColorImageQuantizerTests()
        {
        }
        private void RunImageProcessingImagesColorReductionErrorDiffusionColorDitheringTests()
        {
        }
        private void RunImageProcessingImagesColorReductionFloydSteinbergColorDitheringTests()
        {
        }
        private void RunImageProcessingImagesColorReductionJarvisJudiceNinkeColorDitheringTests()
        {
        }
        private void RunImageProcessingImagesColorReductionMedianCutCubeTests()
        {
        }
        private void RunImageProcessingImagesColorReductionMedianCutQuantizerTests()
        {
        }
        private void RunImageProcessingImagesColorReductionOrderedColorDitheringTests()
        {
        }
        private void RunImageProcessingImagesColorReductionSierraColorDitheringTests()
        {
        }
        private void RunImageProcessingImagesColorReductionStuckiColorDitheringTests()
        {
        }

        #endregion

        private void RunImageProcessingImagesAverageCommandTests()
        {
            NAR.Tests.ImageProcessing.Images.AverageCommandTests test = new NAR.Tests.ImageProcessing.Images.AverageCommandTests();
            test.Init();
            test.TestExecute();
            test.TestExecuteGrayscale();
            test.Cleanup();

        }
        private void RunImageProcessingImagesBlackWhiteCommandTests()
        {
            NAR.Tests.ImageProcessing.Images.BlackWhiteCommandTests test = new NAR.Tests.ImageProcessing.Images.BlackWhiteCommandTests();
            test.Init();
            test.TestExecute();
            test.TestExecuteCalculatingLimiar();
            test.Cleanup();

        }
        private void RunImageProcessingImagesGrayscaleCommandTests()
        {
            NAR.Tests.ImageProcessing.Images.GrayscaleCommandTests test = new NAR.Tests.ImageProcessing.Images.GrayscaleCommandTests();
            test.Init();
            test.TestExecuteByAverage();
            test.TestExecuteByLuminance();
            test.Cleanup();

        }
        private void RunImageProcessingImagesRedCommandTests()
        {
            NAR.Tests.ImageProcessing.Images.RedCommandTests test = new NAR.Tests.ImageProcessing.Images.RedCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();

        }
        private void RunImageProcessingImagesGreenCommandTests()
        {
            NAR.Tests.ImageProcessing.Images.GreenCommandTests test = new NAR.Tests.ImageProcessing.Images.GreenCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();

        }
        private void RunImageProcessingImagesBlueCommandTests()
        {
            NAR.Tests.ImageProcessing.Images.BlueCommandTests test = new NAR.Tests.ImageProcessing.Images.BlueCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();

        }
        private void RunImageProcessingImagesInvertCommandTests()
        {
            NAR.Tests.ImageProcessing.Images.InvertCommandTests test = new NAR.Tests.ImageProcessing.Images.InvertCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();

        }
        private void RunImageProcessingImagesLuminosityCommandTests()
        {
            NAR.Tests.ImageProcessing.Images.LuminosityCommandTests test = new NAR.Tests.ImageProcessing.Images.LuminosityCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();

        }
        private void RunImageProcessingImagesRotateCommandTests()
        {
            NAR.Tests.ImageProcessing.Images.RotateCommandTests test = new NAR.Tests.ImageProcessing.Images.RotateCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();

        }
        private void RunImageProcessingImagesNormalizeCommandTests()
        {
            NAR.Tests.ImageProcessing.Images.NormalizeCommandTests test = new NAR.Tests.ImageProcessing.Images.NormalizeCommandTests();
            test.Init();
            test.TestExecuteGrayscale();
            test.TestExecuteRGB();
            test.Cleanup();

        }
        private void RunImageProcessingImagesSepiaCommandTests()
        {
            NAR.Tests.ImageProcessing.Images.SepiaCommandTests test = new NAR.Tests.ImageProcessing.Images.SepiaCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();

        }

        #endregion

        #region Image Processing Util Tests

        private void RunImageProcessingUtilTests()
        {
            RunImageProcessingUtilHistogramUtilTests();
        }

        private void RunImageProcessingUtilHistogramUtilTests()
        {
            NAR.Tests.ImageProcessing.Util.HistogramTests test = new NAR.Tests.ImageProcessing.Util.HistogramTests();
            test.Init();
            test.TestCalculateFromGrayscale();
            test.TestCalculateFromRGB();
            test.Cleanup();
        }

        #endregion

        #region Image Processing Motion Tests

        private void RunImageProcessingMotionTests()
        {
            RunImageProcessingMotionMotionDetectorTests();
            RunImageProcessingMotionDetectMotionCommandTests();
        }

        private void RunImageProcessingMotionMotionDetectorTests()
        {
            NAR.Tests.ImageProcessing.Motion.MotionDetectorTests test = new NAR.Tests.ImageProcessing.Motion.MotionDetectorTests();
            test.Init();
            test.TestDetect();
            test.Cleanup();
        }
        private void RunImageProcessingMotionDetectMotionCommandTests()
        {
            NAR.Tests.ImageProcessing.Motion.DetectMotionCommandTests test = new NAR.Tests.ImageProcessing.Motion.DetectMotionCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }

        #endregion

        #region Image Processing Distance 

        private void RunImageProcessingDistanceTests()
        {
            RunImageProcessingDistanceCalcDistanceCommandTests();
        }

        private void RunImageProcessingDistanceCalcDistanceCommandTests()
        {
            NAR.Tests.ImageProcessing.Distance.CalcDistanceCommandTests test = new NAR.Tests.ImageProcessing.Distance.CalcDistanceCommandTests();
            test.Init();
            test.TestCalcule();
            test.Cleanup();
        }

        #endregion

        #region Image Processing Size Tests

        private void RunImageProcessingSizeTests()
        {
            RunImageProcessingSizeSizeCalculatorTests();
        }

        private void RunImageProcessingSizeSizeCalculatorTests()
        {
            NAR.Tests.ImageProcessing.Size.SizeCalculatorTests test = new NAR.Tests.ImageProcessing.Size.SizeCalculatorTests();
            test.Init();
            test.TestCalculate();
            test.Cleanup();
        }
        private void RunImageProcessingSizeSizeCalculatorCommandTests()
        {
            NAR.Tests.ImageProcessing.Size.SizeCalculatorCommandTests test = new NAR.Tests.ImageProcessing.Size.SizeCalculatorCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }

        #endregion

        #region Image Processing Pixels Tests

        private void RunImageProcessingPixelsTests()
        {
            RunImageProcessingPixelsPixelCounterTests();
            RunImageProcessingPixelsCountPixelCommandTests();
        }

        private void RunImageProcessingPixelsPixelCounterTests()
        {
            NAR.Tests.ImageProcessing.Pixels.PixelCounterTests test = new NAR.Tests.ImageProcessing.Pixels.PixelCounterTests();
            test.Init();
            test.TestCount();
            test.Cleanup();
        }
        private void RunImageProcessingPixelsCountPixelCommandTests()
        {
            NAR.Tests.ImageProcessing.Pixels.CountPixelCommandTests test = new NAR.Tests.ImageProcessing.Pixels.CountPixelCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }

        #endregion

        #region Image Processing Segmentation Tests
        private void RunImageProcessingSegmentationTests()
        {
            RunImageProcessingSegmentationHoughLineTransformationTests();
            RunImageProcessingSegmentationHoughLineTransformationCommandTests();
            
            RunImageProcessingSegmentationHoughCircleTransformationTests();
            RunImageProcessingSegmentationHoughCircleTransformationCommandTests();
            
        }

        private void RunImageProcessingSegmentationHoughLineTransformationTests()
        {

            NAR.Tests.ImageProcessing.Segmentation.HoughLineTransformationTests test = new NAR.Tests.ImageProcessing.Segmentation.HoughLineTransformationTests();
            test.Init();
            test.TestHoughGraphics();
            test.TestCollectLines();
            test.Cleanup();
        }
        private void RunImageProcessingSegmentationHoughLineTransformationCommandTests()
        {

            NAR.Tests.ImageProcessing.Segmentation.HoughLineTransformationCommandTests test = new NAR.Tests.ImageProcessing.Segmentation.HoughLineTransformationCommandTests();
            test.Init();
            test.TestExecute();
            test.TestShowLines();
            test.Cleanup();
        }

        private void RunImageProcessingSegmentationHoughCircleTransformationTests()
        {

            NAR.Tests.ImageProcessing.Segmentation.HoughCircleTransformationTests test = new NAR.Tests.ImageProcessing.Segmentation.HoughCircleTransformationTests();
            test.Init();
            test.TestHoughGraphics();
            test.TestCollectCircles();
            test.Cleanup();
        }
        private void RunImageProcessingSegmentationHoughCircleTransformationCommandTests()
        {

            NAR.Tests.ImageProcessing.Segmentation.HoughCircleTransformationCommandTests test = new NAR.Tests.ImageProcessing.Segmentation.HoughCircleTransformationCommandTests();
            test.Init();
            test.TestExecute();
            //test.TestShowLines();
            test.Cleanup();
        }


        #endregion

        #region Image Processing Corner Tests

        private void RunImageProcessingCornerTests()
        {
            RunImageProcessingCornerHarrisCornersCommandTests();
            RunImageProcessingCornerSusanCornersCommandTests();
            RunImageProcessingCornerMoravecCornersCommandTests();
            RunImageProcessingCornerBeaudetCornersCommandTests();
            RunImageProcessingCornerCssCornersCommandTests();
            RunImageProcessingCornerDericheCornersCommandTests();
            RunImageProcessingCornerForstnerCornersCommandTests();
            RunImageProcessingCornerKitchenRosenfeldCornersCommandTests();
            RunImageProcessingCornerPlesseyCornersCommandTests();
            RunImageProcessingCornerShiTomasiCornersCommandTests();
            RunImageProcessingCornerTrajkovicHedley4CornersCommandTests();
            RunImageProcessingCornerTrajkovicHedley8CornersCommandTests();
            RunImageProcessingCornerWangBradCornersCommandTests();
            RunImageProcessingCornerZhengWangCornersCommandTests();
        }
        
        private void RunImageProcessingCornerHarrisCornersCommandTests()
        {

            NAR.Tests.ImageProcessing.Corner.HarrisCornersCommandTests test = new NAR.Tests.ImageProcessing.Corner.HarrisCornersCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingCornerSusanCornersCommandTests()
        {

            NAR.Tests.ImageProcessing.Corner.SusanCornersCommandTests test = new NAR.Tests.ImageProcessing.Corner.SusanCornersCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingCornerMoravecCornersCommandTests()
        {

            NAR.Tests.ImageProcessing.Corner.MoravecCornersCommandTests test = new NAR.Tests.ImageProcessing.Corner.MoravecCornersCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }

        private void RunImageProcessingCornerBeaudetCornersCommandTests()
        {

            NAR.Tests.ImageProcessing.Corner.BeaudetCornersCommandTests test = new NAR.Tests.ImageProcessing.Corner.BeaudetCornersCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingCornerCssCornersCommandTests()
        {

            NAR.Tests.ImageProcessing.Corner.CssCornersCommandTests test = new NAR.Tests.ImageProcessing.Corner.CssCornersCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingCornerDericheCornersCommandTests()
        {

            NAR.Tests.ImageProcessing.Corner.DericheCornersCommandTests test = new NAR.Tests.ImageProcessing.Corner.DericheCornersCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingCornerForstnerCornersCommandTests()
        {

            NAR.Tests.ImageProcessing.Corner.ForstnerCornersCommandTests test = new NAR.Tests.ImageProcessing.Corner.ForstnerCornersCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingCornerKitchenRosenfeldCornersCommandTests()
        {

            NAR.Tests.ImageProcessing.Corner.KitchenRosenfeldCornersCommandTests test = new NAR.Tests.ImageProcessing.Corner.KitchenRosenfeldCornersCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingCornerPlesseyCornersCommandTests()
        {

            NAR.Tests.ImageProcessing.Corner.PlesseyCornersCommandTests test = new NAR.Tests.ImageProcessing.Corner.PlesseyCornersCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingCornerShiTomasiCornersCommandTests()
        {

            NAR.Tests.ImageProcessing.Corner.ShiTomasiCornersCommandTests test = new NAR.Tests.ImageProcessing.Corner.ShiTomasiCornersCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingCornerTrajkovicHedley4CornersCommandTests()
        {

            NAR.Tests.ImageProcessing.Corner.TrajkovicHedley4CornersCommandTests test = new NAR.Tests.ImageProcessing.Corner.TrajkovicHedley4CornersCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingCornerTrajkovicHedley8CornersCommandTests()
        {

            NAR.Tests.ImageProcessing.Corner.TrajkovicHedley8CornersCommandTests test = new NAR.Tests.ImageProcessing.Corner.TrajkovicHedley8CornersCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingCornerWangBradCornersCommandTests()
        {

            NAR.Tests.ImageProcessing.Corner.WangBradCornersCommandTests test = new NAR.Tests.ImageProcessing.Corner.WangBradCornersCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingCornerZhengWangCornersCommandTests()
        {

            NAR.Tests.ImageProcessing.Corner.ZhengWangCornersCommandTests test = new NAR.Tests.ImageProcessing.Corner.ZhengWangCornersCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }

        #endregion

        #region Image Processing Contours Tests

        private void RunImageProcessingContoursTests()
        {
            RunImageProcessingContoursContourCounterCommandTests();
            RunImageProcessingContoursSquareBlobCommandTests();
            RunImageProcessingContoursMooreNeighborBlobCommandTests();
            RunImageProcessingContoursGreedContoursCommandTests();
            RunImageProcessingContoursKassBlobCommandTests();
            RunImageProcessingContoursRadialSweepBlobCommandTests();
            RunImageProcessingContoursSnakeBlobCommandTests();
            RunImageProcessingContoursTheoPavlidisBlobCommandTests();
        }
        private void RunImageProcessingContoursContourCounterCommandTests()
        {
            //NAR.Tests.ImageProcessing.Contours.ContourCounterCommandTests test = new NAR.Tests.ImageProcessing.Contours.ContourCounterCommandTests();
            //test.Init();
            //test.TestExecute();
            //test.Cleanup();
        }
        private void RunImageProcessingContoursGreedContoursCommandTests()
        {
            NAR.Tests.ImageProcessing.Contours.GreedContoursCommandTests test = new NAR.Tests.ImageProcessing.Contours.GreedContoursCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingContoursKassBlobCommandTests()
        {
            NAR.Tests.ImageProcessing.Contours.KassBlobCommandTests test = new NAR.Tests.ImageProcessing.Contours.KassBlobCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingContoursMooreNeighborBlobCommandTests()
        {
            NAR.Tests.ImageProcessing.Contours.MooreNeighborBlobCommandTests test = new NAR.Tests.ImageProcessing.Contours.MooreNeighborBlobCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingContoursRadialSweepBlobCommandTests()
        {
            NAR.Tests.ImageProcessing.Contours.RadialSweepBlobCommandTests test = new NAR.Tests.ImageProcessing.Contours.RadialSweepBlobCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingContoursSnakeBlobCommandTests()
        {
            NAR.Tests.ImageProcessing.Contours.SnakeBlobCommandTests test = new NAR.Tests.ImageProcessing.Contours.SnakeBlobCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingContoursSquareBlobCommandTests()
        {
            NAR.Tests.ImageProcessing.Contours.SquareBlobCommandTests test = new NAR.Tests.ImageProcessing.Contours.SquareBlobCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingContoursTheoPavlidisBlobCommandTests()
        {
            NAR.Tests.ImageProcessing.Contours.TheoPavlidisBlobCommandTests test = new NAR.Tests.ImageProcessing.Contours.TheoPavlidisBlobCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }

        #endregion

        #region Image Processing Textures Tests

        private void RunImageProcessingTexturesTests()
        {
            RunImageProcessingTexturesCloudsTextureCommandTests();
            RunImageProcessingTexturesLabyrinthTextureCommandTests();
            RunImageProcessingTexturesMarbleTextureCommandTests();
            RunImageProcessingTexturesTextileTextureCommandTests();
            RunImageProcessingTexturesWoodTextureCommandTests();
        }


        private void RunImageProcessingTexturesCloudsTextureCommandTests()
        {
            NAR.Tests.ImageProcessing.Textures.CloudsTextureCommandTests test = new NAR.Tests.ImageProcessing.Textures.CloudsTextureCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingTexturesLabyrinthTextureCommandTests()
        {
            NAR.Tests.ImageProcessing.Textures.LabyrinthTextureCommandTests test = new NAR.Tests.ImageProcessing.Textures.LabyrinthTextureCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingTexturesMarbleTextureCommandTests()
        {
            NAR.Tests.ImageProcessing.Textures.MarbleTextureCommandTests test = new NAR.Tests.ImageProcessing.Textures.MarbleTextureCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingTexturesTextileTextureCommandTests()
        {
            NAR.Tests.ImageProcessing.Textures.TextileTextureCommandTests test = new NAR.Tests.ImageProcessing.Textures.TextileTextureCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }
        private void RunImageProcessingTexturesWoodTextureCommandTests()
        {
            NAR.Tests.ImageProcessing.Textures.WoodTextureCommandTests test = new NAR.Tests.ImageProcessing.Textures.WoodTextureCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }


        #endregion

        #region Image Processing Camera Calibration Tests

        private void RunImageProcessingCameraCalibrationTests()
        {
            //RunImageProcessingCameraCalibrationTsaiCameraCalibrationCommandTests();
            RunImageProcessingCameraCalibrationTsaiTests();
        }

        #region Image Processing Camera Calibration Tsai Tests
        private void RunImageProcessingCameraCalibrationTsaiTests()
        {
            RunImageProcessingCameraCalibrationTsaiComputeTests();
        }
        private void RunImageProcessingCameraCalibrationTsaiComputeTests()
        {

            NAR.Tests.ImageProcessing.CameraCalibration.Tsai.ComputeTests test = new NAR.Tests.ImageProcessing.CameraCalibration.Tsai.ComputeTests();
            test.Init();
            test.TestCalibration();
            test.Cleanup();
        }
        #endregion


        private void RunImageProcessingCameraCalibrationTsaiCameraCalibrationCommandTests()
        {
            NAR.Tests.ImageProcessing.CameraCalibration.TsaiCameraCalibrationCommandTests test = new NAR.Tests.ImageProcessing.CameraCalibration.TsaiCameraCalibrationCommandTests();
            test.Init();
            test.TestExecute();
            test.Cleanup();
        }


        #endregion

        #region Image Processing

        private void RunImageProcessingImageInvokerTests()
        {
            NAR.Tests.ImageProcessing.ImageInvokerTests test = new NAR.Tests.ImageProcessing.ImageInvokerTests();
            test.Init();
            test.TestAddCommands();
            test.TestRemoveCommands();
            test.TestExecuteCommand();
            test.Cleanup();
        }


        #endregion

        #endregion

        #region Model Tests

        private void RunModelTests()
        {
            RunModelImageBitmapTests();
        }

        #region Model Tests
        private void RunModelImageBitmapTests()
        {
            NAR.Tests.Model.ImageBitmapTests test = new NAR.Tests.Model.ImageBitmapTests();
            test.Init();
            test.TestBytesFromConstructorByImage();
            test.TestImageFromConstructorByByteArray();
            test.TestImageAndByteArrayFromConstryctorByImageAndByteArray();
            test.Cleanup();
        }
        #endregion

        #endregion

        #region ArManagement Tests

        private void RunArManagementTests()
        {
            RunArManagementPatternTests();
        }

        private void RunArManagementPatternTests()
        {
            NAR.Tests.ArManagement.PatternTests test = new NAR.Tests.ArManagement.PatternTests();
            test.Init();

            test.TestLoadFile();

            test.Cleanup();
        }
        #endregion


        #endregion

        #endregion

    }
}
