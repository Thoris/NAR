using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace NAR.ImageProcessing.Contours
{
    public class ContourCounterCommand : ContourCounterBase, ICommand
    {
        #region Variables

        private int _threshold = 128;
        
        #endregion

        #region Constructors/Destructors
        public ContourCounterCommand()
            : base ()
        {
        }
        #endregion

        #region Methods

        /// <summary>
        /// Actual objects map building.
        /// </summary>
        protected override void BuildObjectsMap(Model.IImage image, int backgroundThreshold)
        {


            int width = image.Image.Width;
            int height = image.Image.Height;
            int bytesPerPixel = 3;
            int size = width * height * bytesPerPixel;
            byte[] result = new byte[size];
            Array.Copy(image.Bytes, result, size);


            int stride = height * bytesPerPixel;

            int posIndex = 0;

            int imageWidthM1 = image.Image.Width - 1;

            // allocate labels array
            _objectLabels = new int[width * height];

            // initial labels count
            int labelsCount = 0;

            // create map
            int maxObjects = ((width / 2) + 1) * ((height / 2) + 1) + 1;
            int[] map = new int[maxObjects];

            // initially map all labels to themself
            for (int i = 0; i < maxObjects; i++)
            {
                map[i] = i;
            }

            byte src = result[posIndex];
            int p = 0;


            int offset = 0;// stride - width;

            // 1 - for pixels of the first row
            if (src > backgroundThreshold)
            {
                _objectLabels[p] = ++labelsCount;
            }

            posIndex += bytesPerPixel;

            src = result[posIndex];

            ++p;

            // process the rest of the first row
            for (int x = 1; x < width; x++, posIndex += bytesPerPixel, p++)
            {
                src = result[posIndex];

                // check if we need to label current pixel
                if (src > backgroundThreshold)
                {
                    // check if the previous pixel already was labeled
                    if (result[posIndex - 1] > backgroundThreshold)
                    {
                        // label current pixel, as the previous
                        _objectLabels[p] = _objectLabels[p - 1];
                    }
                    else
                    {
                        // create new label
                        _objectLabels[p] = ++labelsCount;
                    }
                }
            }
            posIndex += offset;

            src = result[posIndex];

            // 2 - for other rows
            // for each row
            for (int y = 1; y < height; y++)
            {
                // for the first pixel of the row, we need to check
                // only upper and upper-right pixels
                if (src > backgroundThreshold)
                {
                    // check surrounding pixels
                    if (result[posIndex - stride] > backgroundThreshold)
                    {
                        // label current pixel, as the above
                        _objectLabels[p] = _objectLabels[p - width];
                    }
                    else if (result[posIndex + 1 - stride] > backgroundThreshold)
                    {
                        // label current pixel, as the above right
                        _objectLabels[p] = _objectLabels[p + 1 - width];
                    }
                    else
                    {
                        // create new label
                        _objectLabels[p] = ++labelsCount;
                    }
                }
                src = result[++posIndex];
                ++p;

                // check left pixel and three upper pixels for the rest of pixels
                for (int x = 1; x < imageWidthM1; x++, posIndex += bytesPerPixel, p++)
                {
                    src = result[posIndex];
                    if (src > backgroundThreshold)
                    {
                        // check surrounding pixels
                        if (result[posIndex - 1] > backgroundThreshold)
                        {
                            // label current pixel, as the left
                            _objectLabels[p] = _objectLabels[p - 1];
                        }
                        else if (result[posIndex - 1 - stride] > backgroundThreshold)
                        {
                            // label current pixel, as the above left
                            _objectLabels[p] = _objectLabels[p - 1 - width];
                        }
                        else if (result[posIndex - stride] > backgroundThreshold)
                        {
                            // label current pixel, as the above
                            _objectLabels[p] = _objectLabels[p - width];
                        }

                        if (result[posIndex + 1 - stride] > backgroundThreshold)
                        {
                            if (_objectLabels[p] == 0)
                            {
                                // label current pixel, as the above right
                                _objectLabels[p] = _objectLabels[p + 1 - width];
                            }
                            else
                            {
                                int l1 = _objectLabels[p];
                                int l2 = _objectLabels[p + 1 - width];

                                if ((l1 != l2) && (map[l1] != map[l2]))
                                {
                                    // merge
                                    if (map[l1] == l1)
                                    {
                                        // map left value to the right
                                        map[l1] = map[l2];
                                    }
                                    else if (map[l2] == l2)
                                    {
                                        // map right value to the left
                                        map[l2] = map[l1];
                                    }
                                    else
                                    {
                                        // both values already mapped
                                        map[map[l1]] = map[l2];
                                        map[l1] = map[l2];
                                    }

                                    //// reindex
                                    //for (int i = 1; i <= labelsCount; i++)
                                    //{
                                    //    if (map[i] != i)
                                    //    {
                                    //        // reindex
                                    //        int j = map[i];
                                    //        while (j != map[j])
                                    //        {
                                    //            j = map[j];
                                    //        }
                                    //        map[i] = j;
                                    //    }
                                    //}
                                }
                            }
                        }

                        // label the object if it is not yet
                        if (_objectLabels[p] == 0)
                        {
                            // create new label
                            _objectLabels[p] = ++labelsCount;
                        }
                    }
                }

                // for the last pixel of the row, we need to check
                // only upper and upper-left pixels
                if (src > backgroundThreshold)
                {
                    // check surrounding pixels
                    if (result[posIndex - 1] > backgroundThreshold)
                    {
                        // label current pixel, as the left
                        _objectLabels[p] = _objectLabels[p - 1];
                    }
                    else if (result[posIndex - 1 - stride] > backgroundThreshold)
                    {
                        // label current pixel, as the above left
                        _objectLabels[p] = _objectLabels[p - 1 - width];
                    }
                    else if (result[posIndex - stride] > backgroundThreshold)
                    {
                        // label current pixel, as the above
                        _objectLabels[p] = _objectLabels[p - width];
                    }
                    else
                    {
                        // create new label
                        _objectLabels[p] = ++labelsCount;
                    }
                }
                src = result[++posIndex];
                ++p;

                posIndex += offset;

                src = result[posIndex];

            }




            // allocate remapping array
            int[] reMap = new int[map.Length];

            // count objects and prepare remapping array
            _objectsCount = 0;
            for (int i = 1; i <= labelsCount; i++)
            {
                if (map[i] == i)
                {
                    // increase objects count
                    reMap[i] = ++_objectsCount;
                }
            }
            // second pass to complete remapping
            for (int i = 1; i <= labelsCount; i++)
            {
                if (map[i] != i)
                {
                    reMap[i] = reMap[map[i]];
                }
            }

            // repair object labels
            for (int i = 0, n = _objectLabels.Length; i < n; i++)
            {
                _objectLabels[i] = reMap[_objectLabels[i]];
            }

        }
 
        #endregion

        #region ICommand Members

        public Model.IImage Execute(Model.IImage image)
        {
            base.ProcessImage(image, _threshold);

            return image;
        }

        #endregion
    }
}
