using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace NAR.ImageProcessing.Contours
{
    /// <summary>
    /// Image's blob.
    /// </summary>
    /// 
    /// <remarks><para>The class represents a blob - part of another images. The
    /// class encapsulates the blob itself and information about its position
    /// in parent image.</para>
    /// 
    /// <para><note>The class is not responsible for blob's image disposing, so it should be
    /// done manually when it is required.</note></para>
    /// </remarks>
    /// 
    public class Blob
    {
        #region Variables

        /// <summary>
        /// blob's image size - as original image or not 
        /// </summary>
        private bool _originalSize = false;
        /// <summary>
        /// blob's rectangle in the original image 
        /// </summary>
        private Rectangle _rect;
        /// <summary>
        /// blob's ID in the original image
        /// </summary>
        private int _id;
        /// <summary>
        /// area of the blob 
        /// </summary>
        private int _area;
        /// <summary>
        /// center of gravity 
        /// </summary>
        private Point _cog;
        /// <summary>
        /// fullness of the blob ( area / ( width * height ) ) 
        /// </summary>
        private double _fullness;
        /// <summary>
        /// mean color of the blob
        /// </summary>
        private Color _colorMean = Color.Black;
        /// <summary>
        /// color's standard deviation of the blob
        /// </summary>
        private Color _colorStdDev = Color.Black;

        #endregion

        #region Properties

        /// <summary>
        /// Blob's image size.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies size of the <see cref="Image">blob's image</see>.
        /// If the property is set to <see langword="true"/>, the blob's image size equals to the
        /// size of original image. If the property is set to <see langword="false"/>, the blob's
        /// image size equals to size of actual blob.</para></remarks>
        /// 
        [Browsable(false)]
        public bool OriginalSize
        {
            get { return _originalSize; }
            internal set { _originalSize = value; }
        }
        /// <summary>
        /// Blob's rectangle in the original image.
        /// </summary>
        /// 
        /// <remarks><para>The property specifies position of the blob in the original image
        /// and its size.</para></remarks>
        /// 
        public Rectangle Rectangle
        {
            get { return _rect; }
        }
        /// <summary>
        /// Blob's ID in the original image.
        /// </summary>
        [Browsable(false)]
        public int ID
        {
            get { return _id; }
            internal set { _id = value; }
        }
        /// <summary>
        /// Blob's area.
        /// </summary>
        /// 
        /// <remarks><para>The property equals to blob's area measured in number of pixels
        /// contained by the blob.</para></remarks>
        /// 
        public int Area
        {
            get { return _area; }
            internal set { _area = value; }
        }
        /// <summary>
        /// Blob's fullness, [0, 1].
        /// </summary>
        /// 
        /// <remarks><para>The property equals to blob's fullness, which is calculated
        /// as <b>Area / ( Width * Height )</b>. If it equals to <b>1</b>, then
        /// it means that entire blob's rectangle is filled by blob's pixel (no
        /// blank areas), which is true only for rectangles. If it equals to <b>0.5</b>,
        /// for example, then it means that only half of the bounding rectangle is filled
        /// by blob's pixels.</para></remarks>
        /// 
        public double Fullness
        {
            get { return _fullness; }
            internal set { _fullness = value; }
        }
        /// <summary>
        /// Blob's center of gravity point.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps center of gravity point, which is calculated as
        /// mean value of X and Y coordinates of blob's points.</para></remarks>
        /// 
        public Point CenterOfGravity
        {
            get { return _cog; }
            internal set { _cog = value; }
        }
        /// <summary>
        /// Blob's mean color.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps mean color of pixels comprising the blob.</para></remarks>
        /// 
        public Color ColorMean
        {
            get { return _colorMean; }
            internal set { _colorMean = value; }
        }
        /// <summary>
        /// Blob color's standard deviation.
        /// </summary>
        /// 
        /// <remarks><para>The property keeps standard deviation of pixels' colors comprising the blob.</para></remarks>
        /// 
        public Color ColorStdDev
        {
            get { return _colorStdDev; }
            internal set { _colorStdDev = value; }
        }

        #endregion

        #region Constructors/Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Blob"/> class.
        /// </summary>
        /// 
        /// <param name="id">Blob's ID in the original image.</param>
        /// <param name="rect">Blob's rectangle in the original image.</param>
        /// 
        /// <remarks><para>This constructor leaves <see cref="Image"/> property not initialized. The blob's
        /// image may be extracted later using <see cref="BlobCounterBase.ExtractBlobsImage( Bitmap, Blob, bool )"/>
        /// or <see cref="BlobCounterBase.ExtractBlobsImage( UnmanagedImage, Blob, bool )"/> method.</para></remarks>
        /// 
        public Blob(int id, Rectangle rect)
        {
            _id = id;
            _rect = rect;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Blob"/> class.
        /// </summary>
        /// 
        /// <param name="source">Source blob to copy.</param>
        /// 
        /// <remarks><para>This copy constructor leaves <see cref="Image"/> property not initialized. The blob's
        /// image may be extracted later using <see cref="BlobCounterBase.ExtractBlobsImage( Bitmap, Blob, bool )"/>
        /// or <see cref="BlobCounterBase.ExtractBlobsImage( UnmanagedImage, Blob, bool )"/> method.</para></remarks>
        /// 
        public Blob(Blob source)
        {
            // copy everything except image
            _id = source._id;
            _rect = source._rect;
            _cog = source._cog;
            _area = source._area;
            _fullness = source._fullness;
            _colorMean = source._colorMean;
            _colorStdDev = source._colorStdDev;
        }

        #endregion
    }
}
