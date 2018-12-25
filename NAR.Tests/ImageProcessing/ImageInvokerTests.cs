using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing
{
    [TestFixture]
    public class ImageInvokerTests : BaseCommand
    {
        #region Constructors/Destructors

        [TestFixtureSetUp]
        public void Init()
        {
        }
        [TearDown]
        public void Cleanup()
        {
        }

        #endregion

        #region Methods
        [Test]
        public void TestAddCommands()
        {
            NAR.ImageProcessing.ICommand commandGray = new NAR.ImageProcessing.Images.GrayscaleCommand(false);
            NAR.ImageProcessing.ICommand commandBW = new NAR.ImageProcessing.Images.BlackWhiteCommand(false);


            NAR.ImageProcessing.ImageInvoker invoker = new NAR.ImageProcessing.ImageInvoker();
            invoker.Commands.Add(commandGray);
            invoker.Commands.Add(commandBW);

            Assert.AreEqual(2, invoker.Commands.Count);

        }
        [Test]
        public void TestRemoveCommands()
        {
            NAR.ImageProcessing.ICommand commandGray = new NAR.ImageProcessing.Images.GrayscaleCommand(false);
            NAR.ImageProcessing.ICommand commandBW = new NAR.ImageProcessing.Images.BlackWhiteCommand(false);


            NAR.ImageProcessing.ImageInvoker invoker = new NAR.ImageProcessing.ImageInvoker();
            invoker.Commands.Add(commandGray);
            invoker.Commands.Add(commandBW);

            invoker.Commands.RemoveAt(0);

            Assert.AreEqual(1, invoker.Commands.Count);

        }
        [Test]
        public void TestExecuteCommand()
        {            
            NAR.ImageProcessing.ICommand commandGray = new NAR.ImageProcessing.Images.GrayscaleCommand(false);
            NAR.ImageProcessing.ICommand commandLaplace = new NAR.ImageProcessing.Borders.LaplaceCommand(false);
            NAR.ImageProcessing.ICommand commandBW = new NAR.ImageProcessing.Images.BlackWhiteCommand(false);


            NAR.ImageProcessing.ImageInvoker invoker = new NAR.ImageProcessing.ImageInvoker();
            invoker.Commands.Add(commandGray);
            invoker.Commands.Add(commandLaplace);
            invoker.Commands.Add(commandBW);

            NAR.Model.IImage result = invoker.ExecuteCommand(base.Model);

            //result.Image.Save("C:\\temp\\cap.bmp");

            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\Invoker.bmp"));

            base.CheckAllBytes(result.Bytes, bitmap.Bytes);
        }

        #endregion

    }
}
