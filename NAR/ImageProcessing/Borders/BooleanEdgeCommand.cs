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
    public class BooleanEdgeCommand : Images.BlackWhiteCommand, ICommand, IBorderDetector
    {
        #region Variables

        private IList<byte[,]> _compare = new  List<byte[,]>(); 
            
            
        #endregion

        #region Constructors/Destructors
        public BooleanEdgeCommand(bool calculateLimiar)
            : base (calculateLimiar)
        {

            _compare.Add (new byte[3,3]        
                {
                    {255,  0,  0},
                    {255,  0,  0},
                    {255,  0,  0}               
                });
            _compare.Add (new byte[3,3]        
            
                {
                    {255,255,  0},
                    {255,  0,  0},
                    {  0,  0,  0}               
                });
            _compare.Add (new byte[3,3]                    
                {
                    {255,255,255},
                    {255,  0,  0},
                    {  0,  0,  0}               
                });
            _compare.Add (new byte[3,3]                    
                {
                    {255,255,255},
                    {  0,  0,255},
                    {  0,  0,  0}               
                });
            _compare.Add (new byte[3,3]                    
                {
                    {255,255,255},
                    {  0,  0,  0},
                    {  0,  0,  0}               
                });
            _compare.Add (new byte[3,3]                 
                {
                    {  0,255,255},
                    {  0,  0,255},
                    {  0,  0,  0}               
                });
            _compare.Add (new byte[3,3]                   
                {
                    {  0,255,255},
                    {  0,  0,255},
                    {  0,  0,255}               
                });
            _compare.Add (new byte[3,3]                    
                {
                    {  0,  0,255},
                    {  0,  0,255},
                    {  0,255,255}               
                });
            _compare.Add (new byte[3,3]                    
                {
                    {  0,  0,255},
                    {  0,  0,255},
                    {  0,  0,255}               
                });
            _compare.Add (new byte[3,3]                    
                {
                    {  0,  0,  0},
                    {  0,  0,255},
                    {  0,255,255}               
                });
            _compare.Add (new byte[3,3]                    
                {
                    {  0,  0,  0},
                    {  0,  0,255},
                    {255,255,255}               
                });
            _compare.Add (new byte[3,3]                    
                {
                    {  0,  0,  0},
                    {255,  0,  0},
                    {255,255,255}               
                });
            _compare.Add (new byte[3,3]                    
                {
                    {  0,  0,  0},
                    {  0,  0,  0},
                    {255,255,255}               
                });
            _compare.Add (new byte[3,3]                    
                {
                    {  0,  0,  0},
                    {255,  0,  0},
                    {255,255,  0}               
                });
            _compare.Add (new byte[3,3]                    
                {
                    {255,  0,  0},
                    {255,  0,  0},
                    {255,255,  0}               
                });            
            _compare.Add (new byte[3,3]        
                {
                    {255,255,  0},
                    {255,  0,  0},
                    {255,  0,  0}                               
                });


        }
        #endregion

        #region Methods

        private bool CompareBoolean(byte[,] currentImage)
        {
            for (int c = 0; c < _compare.Count; c++)
            {
                if (CompareMatrix(currentImage, _compare[c]))
                    return true;
            }

            return false;
               
        }
        private bool CompareMatrix(byte[,] currentImage, byte[,] compared)
        {
            for (int c = 0; c < 3; c++)
            {
                for (int l = 0; l < 3; l++)
                {
                    if (currentImage[c, l] != compared[c, l])
                        return false;
                }
            }

            return true;
        }

        #endregion

        #region ICommand Members

        public new Model.IImage Execute(Model.IImage image)
        {
            Model.IImage bw = base.Execute(image);


            //Getting the info related to the image
            int width = image.Image.Width;
            int height = image.Image.Height;
            int size = width * height * 3;

            //Creating a new vector to apply the image
            byte[] result = new byte[size];
            byte[] newBytes = new byte[size];

            Array.Copy(bw.Bytes, result, size);
            
            //Calculating the end of the column from the byte array
            long colEnd = (width * 3) - 3;

            //Auxiliar lines used to identify the entry in the vector
            int [] lines = new int[3];

            //Matrix of the current image
            byte [,] currentImage = new byte[3,3];
            
            //Foreach line in the byte array
            for (int line = 1; line < height - 1; line++)
            {
                //Foreach column in the byte array
                for (int col = 3; col < colEnd; col += 3)
                {
                    //Calculating the position of near to the focal point
                    lines[0] = (col) + width * (line - 1) * 3;
                    lines[1] = (col) + width * (line) * 3;
                    lines[2] = (col) + width * (line + 1) * 3;

                    //Checking the pixels of the current matrix to be compared
                    currentImage[0, 0] = result[lines[0] - 3];
                    currentImage[0, 1] = result[lines[0]];
                    currentImage[0, 2] = result[lines[0] + 3];
                    currentImage[1, 0] = result[lines[0] - 3];
                    currentImage[1, 1] = result[lines[0] ];
                    currentImage[1, 2] = result[lines[0] + 3];
                    currentImage[2, 0] = result[lines[0] - 3];
                    currentImage[2, 1] = result[lines[0]];
                    currentImage[2, 2] = result[lines[0] + 3];


                    if (CompareBoolean(currentImage))
                    {
                        newBytes[lines[1] + Model.ImageBitmap.COLOR_BLUE] = 255;
                        newBytes[lines[1] + Model.ImageBitmap.COLOR_GREEN] = 255;
                        newBytes[lines[1] + Model.ImageBitmap.COLOR_RED] = 255;
                    }
                    else
                    {
                        newBytes[lines[1] + Model.ImageBitmap.COLOR_BLUE] = 0;
                        newBytes[lines[1] + Model.ImageBitmap.COLOR_GREEN] = 0;
                        newBytes[lines[1] + Model.ImageBitmap.COLOR_RED] = 0;
                    }

                }//end for each pixel from the mask

            } //end for line


            return new Model.ImageBitmap(width, height, newBytes);
        }

        #endregion
    
    }
}
