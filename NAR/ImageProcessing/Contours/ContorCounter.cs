using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NAR.ImageProcessing.Contours
{
    public class ContorCounter : ContourCounterBase
    {
        #region Constructors/Destructors
        public ContorCounter()
        {
        }
        #endregion

        #region Methods

        /// <summary>
        /// Actual objects map building.
        /// </summary>
        protected override void BuildObjectsMap(Model.IImage image, int backgroundThreshold)
        {
            int stride = 0;

            int width = image.Image.Width;
            int height = image.Image.Height;
            int bytesPerPixel = 3;
            int size = width * height * bytesPerPixel;
            byte[] result = new byte[size];
            Array.Copy(image.Bytes, result, size);


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


            int offset = stride - width;

            // 1 - for pixels of the first row
            if (src > backgroundThreshold)
            {
                _objectLabels[p] = ++labelsCount;
            }
            src = result[++posIndex];

            ++p;

            // process the rest of the first row
            for (int x = 1; x < width; x++, src++, p++)
            {
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
                for (int x = 1; x < imageWidthM1; x++, src++, p++)
                {
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

                                    // reindex
                                    for (int i = 1; i <= labelsCount; i++)
                                    {
                                        if (map[i] != i)
                                        {
                                            // reindex
                                            int j = map[i];
                                            while (j != map[j])
                                            {
                                                j = map[j];
                                            }
                                            map[i] = j;
                                        }
                                    }
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

        private void CollectObjectsInfo(Model.IImage image, byte[] result, int width, int height)
        {
            int i = 0, label;
            int bytesPerPixel = 3;
            int size = width * height * bytesPerPixel;

            // create object coordinates arrays
            int[] x1 = new int[_objectsCount + 1];
            int[] y1 = new int[_objectsCount + 1];
            int[] x2 = new int[_objectsCount + 1];
            int[] y2 = new int[_objectsCount + 1];

            int[] area = new int[_objectsCount + 1];
            long[] xc = new long[_objectsCount + 1];
            long[] yc = new long[_objectsCount + 1];

            long[] meanR = new long[_objectsCount + 1];
            long[] meanG = new long[_objectsCount + 1];
            long[] meanB = new long[_objectsCount + 1];

            long[] stdDevR = new long[_objectsCount + 1];
            long[] stdDevG = new long[_objectsCount + 1];
            long[] stdDevB = new long[_objectsCount + 1];

            for (int j = 1; j <= _objectsCount; j++)
            {
                x1[j] = width;
                y1[j] = height;
            }

            int posIndex = 0;

            byte src = result[posIndex];

            int offset = width - width;
            byte g; // pixel's grey value

            // walk through labels array
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, i++, src++)
                {
                    // get current label
                    label = _objectLabels[i];

                    // skip unlabeled pixels
                    if (label == 0)
                        continue;

                    // check and update all coordinates

                    if (x < x1[label])
                    {
                        x1[label] = x;
                    }
                    if (x > x2[label])
                    {
                        x2[label] = x;
                    }
                    if (y < y1[label])
                    {
                        y1[label] = y;
                    }
                    if (y > y2[label])
                    {
                        y2[label] = y;
                    }

                    area[label]++;
                    xc[label] += x;
                    yc[label] += y;

                    g = src;
                    meanG[label] += g;
                    stdDevG[label] += g * g;
                }

                posIndex += offset;
                src = result[posIndex];
            }

            for (int j = 1; j <= _objectsCount; j++)
            {
                meanR[j] = meanB[j] = meanG[j];
                stdDevR[j] = stdDevB[j] = meanG[j];
            }


            //// create blobs
            //blobs.Clear();

            //for (int j = 1; j <= _objectsCount; j++)
            //{
            //    int blobArea = area[j];

            //    Blob blob = new Blob(j, new Rectangle(x1[j], y1[j], x2[j] - x1[j] + 1, y2[j] - y1[j] + 1));
            //    blob.Area = blobArea;
            //    blob.Fullness = (double)blobArea / ((x2[j] - x1[j] + 1) * (y2[j] - y1[j] + 1));
            //    blob.CenterOfGravity = new AForge.Point((float)xc[j] / blobArea, (float)yc[j] / blobArea);
            //    blob.ColorMean = Color.FromArgb((byte)(meanR[j] / blobArea), (byte)(meanG[j] / blobArea), (byte)(meanB[j] / blobArea));
            //    blob.ColorStdDev = Color.FromArgb(
            //        (byte)(Math.Sqrt(stdDevR[j] / blobArea - blob.ColorMean.R * blob.ColorMean.R)),
            //        (byte)(Math.Sqrt(stdDevG[j] / blobArea - blob.ColorMean.G * blob.ColorMean.G)),
            //        (byte)(Math.Sqrt(stdDevB[j] / blobArea - blob.ColorMean.B * blob.ColorMean.B)));

            //    blobs.Add(blob);
            //}
        }


        #endregion
    }
}
