using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace NAR.ImageProcessing.Contours
{
    #region Enumerations / Constants
    /// <summary>
    /// Possible object orders.
    /// </summary>
    /// 
    /// <remarks>The enumeration defines possible sorting orders of objects, found by blob
    /// counting classes.</remarks>
    /// 
    public enum ObjectsOrder
    {
        /// <summary>
        /// Unsorted order (as it is collected by algorithm).
        /// </summary>
        None,

        /// <summary>
        /// Objects are sorted by size in descending order (bigger objects go first).
        /// Size is calculated as <b>Width * Height</b>.
        /// </summary>
        Size,

        /// <summary>
        /// Objects are sorted by area in descending order (bigger objects go first).
        /// </summary>
        Area,

        /// <summary>
        /// Objects are sorted by Y coordinate, then by X coordinate in ascending order
        /// (smaller coordinates go first).
        /// </summary>
        YX,

        /// <summary>
        /// Objects are sorted by X coordinate, then by Y coordinate in ascending order
        /// (smaller coordinates go first).
        /// </summary>
        XY
    }
    #endregion



    public class ContourCounterBase
    {

        #region Variables

        /// <summary>
        /// found blobs 
        /// </summary>
        protected List<Blob> _blobs = new List<Blob>();
        /// <summary>
        /// Objects count.
        /// </summary>
        protected int _objectsCount;
        /// <summary>
        /// filtering by size is required or not
        /// </summary>
        private bool _filterBlobs = false;
        /// <summary>
        /// coupled size filtering or not
        /// </summary>
        private bool _coupledSizeFiltering = false;
        /// <summary>
        /// objects' sort order
        /// </summary>
        private ObjectsOrder _objectsOrder = ObjectsOrder.None;

        /// <summary>
        /// Objects' labels.
        /// </summary>
        protected int[] _objectLabels;

        private int _minWidth = 1;
        private int _minHeight = 1;
        private int _maxWidth = int.MaxValue;
        private int _maxHeight = int.MaxValue;


        #endregion

        #region Properties

        /// <summary>
        /// Objects count.
        /// </summary>
        /// 
        /// <remarks><para>Number of objects (blobs) found by <see cref="Execute(Model.IImage)"/> method.
        /// </para></remarks>
        /// 
        public int ObjectsCount
        {
            get { return _objectsCount; }
        }
        /// <summary>
        /// Objects sort order.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies objects' sort order, which are provided
        /// by <see cref="GetObjectsRectangles"/>, <see cref="GetObjectsInformation"/>, etc.
        /// </para></remarks>
        /// 
        public ObjectsOrder ObjectsOrder
        {
            get { return _objectsOrder; }
            set { _objectsOrder = value; }
        }
        /// <summary>
        /// Specifies if blobs should be filtered.
        /// </summary>
        /// 
        /// <remarks><para>If the property is equal to <b>false</b>, then there is no any additional
        /// post processing after image was processed. If the property is set to <b>true</b>, then
        /// blobs filtering is done right after image processing routine. If <see cref="BlobsFilter"/>
        /// is set, then custom blobs' filtering is done, which is implemented by user. Otherwise
        /// blobs are filtered according to dimensions specified in <see cref="MinWidth"/>,
        /// <see cref="MinHeight"/>, <see cref="MaxWidth"/> and <see cref="MaxHeight"/> properties.</para>
        /// 
        /// <para>Default value is set to <see langword="false"/>.</para></remarks>
        /// 
        public bool FilterBlobs
        {
            get { return _filterBlobs; }
            set { _filterBlobs = value; }
        }

        /// <summary>
        /// Minimum allowed width of blob.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies minimum object's width acceptable by blob counting
        /// routine and has power only when <see cref="FilterBlobs"/> property is set to
        /// <see langword="true"/> and <see cref="BlobsFilter">custom blobs' filter</see> is
        /// set to <see langword="null"/>.</para>
        /// 
        /// <para>See documentation to <see cref="CoupledSizeFiltering"/> for additional information.</para>
        /// </remarks>
        /// 
        public int MinWidth
        {
            get { return _minWidth; }
            set { _minWidth = value; }
        }

        /// <summary>
        /// Minimum allowed height of blob.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies minimum object's height acceptable by blob counting
        /// routine and has power only when <see cref="FilterBlobs"/> property is set to
        /// <see langword="true"/> and <see cref="BlobsFilter">custom blobs' filter</see> is
        /// set to <see langword="null"/>.</para>
        /// 
        /// <para>See documentation to <see cref="CoupledSizeFiltering"/> for additional information.</para>
        /// </remarks>
        /// 
        public int MinHeight
        {
            get { return _minHeight; }
            set { _minHeight = value; }
        }

        /// <summary>
        /// Maximum allowed width of blob.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies maximum object's width acceptable by blob counting
        /// routine and has power only when <see cref="FilterBlobs"/> property is set to
        /// <see langword="true"/> and <see cref="BlobsFilter">custom blobs' filter</see> is
        /// set to <see langword="null"/>.</para>
        /// 
        /// <para>See documentation to <see cref="CoupledSizeFiltering"/> for additional information.</para>
        /// </remarks>
        /// 
        public int MaxWidth
        {
            get { return _maxWidth; }
            set { _maxWidth = value; }
        }

        /// <summary>
        /// Maximum allowed height of blob.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies maximum object's height acceptable by blob counting
        /// routine and has power only when <see cref="FilterBlobs"/> property is set to
        /// <see langword="true"/> and <see cref="BlobsFilter">custom blobs' filter</see> is
        /// set to <see langword="null"/>.</para>
        /// 
        /// <para>See documentation to <see cref="CoupledSizeFiltering"/> for additional information.</para>
        /// </remarks>
        /// 
        public int MaxHeight
        {
            get { return _maxHeight; }
            set { _maxHeight = value; }
        }
        #endregion

        #region Constructors/Destructors
        public ContourCounterBase()
        {
        }
        #endregion

        #region Methods

        /// <summary>
        /// Actual objects map building.
        /// </summary>
        /// 
        /// <param name="image">Unmanaged image to process.</param>
        /// 
        /// <remarks><note>By the time this method is called bitmap's pixel format is not
        /// yet checked, so this should be done by the class inheriting from the base class.
        /// <see cref="imageWidth"/> and <see cref="imageHeight"/> members are initialized
        /// before the method is called, so these members may be used safely.</note></remarks>
        /// 
        protected virtual void BuildObjectsMap(Model.IImage image, int backgroundThreshold)
        {
            
        }

        /// <summary>
        /// Collect objects' rectangles
        /// </summary>
        /// <param name="image"></param>
        /// <param name="result"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void CollectObjectsInfo(Model.IImage image)
        {
            int i = 0, label;
            int bytesPerPixel = 3;
            
            
            int width = image.Image.Width;
            int height = image.Image.Height;
            int size = width * height * bytesPerPixel;
            byte[] result = new byte[size];
            Array.Copy(image.Bytes, result, size);

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


            // create blobs
            _blobs.Clear();

            for (int j = 1; j <= _objectsCount; j++)
            {
                int blobArea = area[j];

                Blob blob = new Blob(j, new Rectangle(x1[j], y1[j], x2[j] - x1[j] + 1, y2[j] - y1[j] + 1));
                blob.Area = blobArea;
                blob.Fullness = (double)blobArea / ((x2[j] - x1[j] + 1) * (y2[j] - y1[j] + 1));
                //blob.CenterOfGravity = new Point((float)xc[j] / blobArea, (float)yc[j] / blobArea);
                blob.ColorMean = Color.FromArgb((byte)(meanR[j] / blobArea), (byte)(meanG[j] / blobArea), (byte)(meanB[j] / blobArea));
                blob.ColorStdDev = Color.FromArgb(
                    (byte)(Math.Sqrt(stdDevR[j] / blobArea - blob.ColorMean.R * blob.ColorMean.R)),
                    (byte)(Math.Sqrt(stdDevG[j] / blobArea - blob.ColorMean.G * blob.ColorMean.G)),
                    (byte)(Math.Sqrt(stdDevB[j] / blobArea - blob.ColorMean.B * blob.ColorMean.B)));

                _blobs.Add(blob);
            }
        }


        public void ProcessImage(Model.IImage image, int threshold)
        {
            int imageWidth = image.Image.Width;
            int imageHeight = image.Image.Height;

            // do actual objects map building
            BuildObjectsMap(image, threshold);

            // collect information about blobs
            CollectObjectsInfo(image);

            // filter blobs by size if required
            if (_filterBlobs)
            {
                // labels remapping array
                int[] labelsMap = new int[_objectsCount + 1];
                for (int i = 1; i <= _objectsCount; i++)
                {
                    labelsMap[i] = i;
                }

                // check dimension of all objects and filter them
                int objectsToRemove = 0;


                for (int i = _objectsCount - 1; i >= 0; i--)
                {
                    int blobWidth = _blobs[i].Rectangle.Width;
                    int blobHeight = _blobs[i].Rectangle.Height;

                    if (_coupledSizeFiltering == false)
                    {
                        // uncoupled filtering
                        if (
                            (blobWidth < _minWidth) || (blobHeight < _minHeight) ||
                            (blobWidth > _maxWidth) || (blobHeight > _maxHeight))
                        {
                            labelsMap[i + 1] = 0;
                            objectsToRemove++;
                            _blobs.RemoveAt(i);
                        }
                    }
                    else
                    {
                        // coupled filtering
                        if (
                            ((blobWidth < _minWidth) && (blobHeight < _minHeight)) ||
                            ((blobWidth > _maxWidth) && (blobHeight > _maxHeight)))
                        {
                            labelsMap[i + 1] = 0;
                            objectsToRemove++;
                            _blobs.RemoveAt(i);
                        }
                    }
                }


                // update labels remapping array
                int label = 0;
                for (int i = 1; i <= _objectsCount; i++)
                {
                    if (labelsMap[i] != 0)
                    {
                        label++;
                        // update remapping array
                        labelsMap[i] = label;
                    }
                }

                // repair object labels
                for (int i = 0, n = _objectLabels.Length; i < n; i++)
                {
                    _objectLabels[i] = labelsMap[_objectLabels[i]];
                }

                _objectsCount -= objectsToRemove;

                // repair IDs
                for (int i = 0, n = _blobs.Count; i < n; i++)
                {
                    _blobs[i].ID = i + 1;
                }
            }

            //// do we need to sort the list?
            //if (_objectsOrder != ObjectsOrder.None)
            //{
            //    _blobs.Sort(new BlobsSorter(objectsOrder));
            //}
        }
        #endregion

    }
}
