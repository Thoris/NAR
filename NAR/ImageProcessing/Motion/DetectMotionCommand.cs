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
using System.Collections;
using NAR.Model;

namespace NAR.ImageProcessing.Motion
{
    public class DetectMotionCommand : MotionDetector, ICommand
    {
        #region Variables
        
        private bool _showInImage;
        private IList<MotionPosition> _list;

        #endregion

        #region Constructors/Destructors
        public DetectMotionCommand(int resX, int resY, byte relevance, int qtdPixel, bool showInImage)
            : base (resX, resY, relevance, qtdPixel)
        {
            _showInImage = showInImage;
        }
        #endregion
        
        #region ICommand Members
        public Model.IImage Execute(Model.IImage image)
        {
            Model.IImage result;
            _list = base.Detect(image);

            if (_showInImage)
            {
                result = base.FillImageMotion(image, _list);
                return result;
            }
            else
            {
                return image;
            }
        }
        #endregion
    }
}
