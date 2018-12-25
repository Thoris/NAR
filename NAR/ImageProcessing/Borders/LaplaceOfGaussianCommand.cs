/******************************************************************************
* Copyright (c) 2012, TAP Consulting Group
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither TAP Consulting Group nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY TAP Consulting Group ''AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL TAP Consulting Group BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.Borders
{
    public class LaplaceOfGaussianCommand : ICommand, IBorderDetector
        //Base.Mask9x9Command, ICommand, IBorderDetector
    {
        #region Variables
        private bool _grayscale;
        private NAR.ImageProcessing.Effects.GaussianBlurCommand _gaussian;
        private NAR.ImageProcessing.Borders.LaplaceCommand _laplace;
        private NAR.ImageProcessing.Images.GrayscaleCommand _gray;
        #endregion

        #region Constructors/Destructors
        public LaplaceOfGaussianCommand(bool grayscale)
            //: base (grayscale, 140, 0, 0, 
            //        //------------------------------
            //        // Mask X
            //        new short[9, 9]{
            //                        { 0,  1,  1,  2,  2,  2,  1,  1,  0},
            //                        { 1,  2,  4,  5,  5,  5,  4,  2,  1},
            //                        { 1,  4,  5,  3,  0,  3,  5,  4,  1},
            //                        { 2,  5,  3,-12,-24,-12,  3,  5,  1},
            //                        { 2,  5,  0,-24,-40,-24,  0,  5,  2},
            //                        { 2,  5,  3,-12,-24,-12,  3,  5,  1},
            //                        { 1,  4,  5,  3,  0,  3,  5,  4,  1},
            //                        { 1,  2,  4,  5,  5,  5,  4,  2,  1},
            //                        { 0,  1,  1,  2,  2,  2,  1,  1,  0},
            //                       }
            //        //------------------------------
            //        )
        {
            _grayscale = grayscale;

            _gaussian = new Effects.GaussianBlurCommand(_grayscale);
            _laplace = new LaplaceCommand(_grayscale);
            _gray = new Images.GrayscaleCommand(false);

        }
        #endregion

        #region ICommand Members
        public new Model.IImage Execute(Model.IImage image)
        {
            Model.IImage result = _gaussian.Execute(image);

            if (!_grayscale)
                result = _gray.Execute(result);
            
            result = _laplace.Execute(result);

            return result;
        }
        #endregion
    }
}