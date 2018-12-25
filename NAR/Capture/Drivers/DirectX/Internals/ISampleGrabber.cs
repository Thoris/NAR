// AForge Direct Show Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace NAR.Capture.Drivers.DirectX.Internals
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The interface is exposed by the Sample Grabber Filter. It enables an application to retrieve
    /// individual media samples as they move through the filter graph.
    /// </summary>
    /// 
	[ComImport,
	Guid("6B652FFF-11FE-4FCE-92AD-0266B5D7C78F"),
	InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISampleGrabber
	{
        /// <summary>
        /// Specifies whether the filter should stop the graph after receiving one sample.
        /// </summary>
        /// 
        /// <param name="oneShot">Boolean value specifying whether the filter should stop the graph after receiving one sample.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        /// <remarks>
        /// 
        /// Use this method to get a single sample from the stream, as follows:
        /// Call SetOneShot with the value TRUE. Optionally, use the IMediaSeeking interface to seek to a position in the stream.
        /// Call IMediaControl::Run to run the filter graph.
        /// Call IMediaEvent::WaitForCompletion to wait for the graph to halt. 
        ///     Alternatively, call IMediaEvent::GetEvent to get graph events, until you receive the EC_COMPLETE event.
        /// After the Sample Grabber halts, the filter graph is still in a running state. 
        /// You can seek or pause the graph to get another sample. 
        /// 
        /// <b>Note</b>
        /// An earlier version of the documentation stated that the filter graph stops after the
        /// sample is received. That is not accurate. The stream ends, but the graph remains in the running state.
        /// The Sample Grabber implements one-shot mode by calling IPin::EndOfStream on the downstream 
        /// filter and returning S_FALSE from the IMemInputPin::Receive method of it.
        /// 
        /// </remarks>
        /// 
        [PreserveSig]
        int SetOneShot( [In, MarshalAs( UnmanagedType.Bool )] bool oneShot );

        /// <summary>
        /// Specifies the media type for the connection on the Sample Grabber's input pin.
        /// </summary>
        /// 
        /// <param name="mediaType">Specifies the required media type.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int SetMediaType( [In, MarshalAs( UnmanagedType.LPStruct )] AMMediaType mediaType );

        /// <summary>
        /// Retrieves the media type for the connection on the Sample Grabber's input pin.
        /// </summary>
        /// 
        /// <param name="mediaType"><see cref="AMMediaType"/> structure, which receives media type.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetConnectedMediaType( [Out, MarshalAs( UnmanagedType.LPStruct )] AMMediaType mediaType );

        /// <summary>
        /// Specifies whether to copy sample data into a buffer as it goes through the filter.
        /// </summary>
        /// 
        /// <param name="bufferThem">Boolean value specifying whether to buffer sample data.
        /// If <b>true</b>, the filter copies sample data into an internal buffer.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int SetBufferSamples( [In, MarshalAs( UnmanagedType.Bool )] bool bufferThem );

        /// <summary>
        /// Retrieves a copy of the sample that the filter received most recently.
        /// </summary>
        /// 
        /// <param name="bufferSize">Pointer to the size of the buffer. If pBuffer is NULL, this parameter receives the required size.</param>
        /// <param name="buffer">Pointer to a buffer to receive a copy of the sample, or NULL.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        /// <remarks>
        /// 
        /// To activate buffering, call ISampleGrabber::SetBufferSamples with a value of TRUE.
        /// Call this method twice. On the first call, set pBuffer to NULL. 
        /// The size of the buffer is returned in pBufferSize.
        /// Then allocate an array and call the method again.
        /// On the second call, pass the size of the array in pBufferSize, 
        /// and pass the address of the array in pBuffer. 
        /// If the array is not large enough, the method returns E_OUTOFMEMORY.
        /// The pBuffer parameter is typed as a long pointer, but the contents of the buffer depend on the
        /// format of the data. Call ISampleGrabber::GetConnectedMediaType to get the media type of the format.
        /// Do not call this method while the filter graph is running. 
        /// While the filter graph is running, the Sample Grabber filter overwrites the contents
        /// of the buffer whenever it receives a new sample. 
        /// The best way to use this method is to use "one-shot mode," which stops the graph after 
        /// receiving the first sample. To set one-shot mode, call ISampleGrabber::SetOneShot.
        /// The filter does not buffer preroll samples, or samples in which the dwStreamId member of 
        /// the AM_SAMPLE2_PROPERTIES structure is anything other than AM_STREAM_MEDIA.
        /// 
        /// </remarks>
        /// 
        [PreserveSig]
        int GetCurrentBuffer( ref int bufferSize, IntPtr buffer );

        /// <summary>
        /// Not currently implemented.
        /// </summary>
        /// 
        /// <param name="sample"></param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int GetCurrentSample( IntPtr sample );

        /// <summary>
        /// Specifies a callback method to call on incoming samples.
        /// </summary>
        /// 
        /// <param name="callback"><see cref="ISampleGrabberCB"/> interface containing the callback method, or NULL to cancel the callback.</param>
        /// <param name="whichMethodToCallback">Index specifying the callback method.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int SetCallback( ISampleGrabberCB callback, int whichMethodToCallback );
    }
}
