﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing.Operators
{
    [TestFixture]
    public class OrCommandTests : BaseCommand
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
        public void TestExecute()
        {
            NAR.ImageProcessing.Operators.OrCommand test = new NAR.ImageProcessing.Operators.OrCommand();

            test.ImageBase = base.Model2;
            NAR.Model.IImage result = test.Execute(base.Model);


            //result.Image.Save("C:\\temp\\cap.bmp");

            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\OrCommand.bmp"));

            base.CheckAllBytes(result.Bytes, bitmap.Bytes);
        }

        #endregion

    }
}
