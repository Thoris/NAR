using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.ImageProcessing.Images.Binarization
{
    public class OrderedDitheringCommand : IBinarization, ICommand
    {
        
        #region Variables
        private int _rows = 4;
        private int _cols = 4;

        private byte[,] _matrix = new byte[4, 4]
		{
			{  15, 143,  47, 175 },
			{ 207,  79, 239, 111 },
			{  63, 191,  31, 159 },
			{ 255, 127, 223,  95 }
		};
        #endregion

        #region Constructors/Destructors
        public OrderedDitheringCommand()
        {
        }
        public OrderedDitheringCommand(byte[,] matrix)
        {
            _rows = matrix.GetLength(0);
            _cols = matrix.GetLength(1);

            this._matrix = matrix;
        }
        #endregion

        #region ICommand Members

        public Model.IImage Execute(Model.IImage image)
        {
            
            int height = image.Image.Height;
            int width = image.Image.Width;
            int size = width * height * 3;
            int stride = width * 3;

            //Creating the size of bytes similar to the image considering 3 bytes of each pixel (RGB)
            byte[] result = new byte[size];
            
            //Array.Copy(image.Bytes, result, size);

            int pos = 0;

            //for each line
            for (int line = 0; line < height; line++)
            {
                //for each pixel
                for (int col = 0; col < width; col++)
                {
                    byte value = (byte)((image.Bytes[pos] <= _matrix[(line % _rows), (col % _cols)]) ? 0 : 255);
                    result[pos + (int)ColorCommand.Colors.Red] = value;
                    result[pos + (int)ColorCommand.Colors.Green] = value;
                    result[pos + (int)ColorCommand.Colors.Blue] = value;

                    pos += 3;

                }//end for pixel

            }//end for line


            return new Model.ImageBitmap(width, height, result );
        }

        #endregion
    }
    
}
