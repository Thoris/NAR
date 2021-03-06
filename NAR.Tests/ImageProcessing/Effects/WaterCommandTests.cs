﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NAR.Tests.ImageProcessing.Effects
{
    [TestFixture]
    public class WaterCommandTests : BaseCommand
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
            short water = 5;

            NAR.ImageProcessing.Effects.WaterCommand test = new NAR.ImageProcessing.Effects.WaterCommand(water);

            NAR.Model.IImage result = test.Execute(base.Model);

            NAR.Model.IImage bitmap = new NAR.Model.ImageBitmap(base.ReadBitmap(".\\Resources\\Commands\\WaterCommand.bmp"));

            base.CheckAllBytes(result.Bytes, bitmap.Bytes);
        }

        #endregion
    }
}
