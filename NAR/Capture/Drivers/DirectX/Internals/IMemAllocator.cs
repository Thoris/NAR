using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NAR.Capture.Drivers.DirectX.Internals
{
    [ComVisible(true), ComImport,
        Guid("56a8689c-0ad4-11ce-b03a-0020af0ba770"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMemAllocator 
    {
        /// <summary>
        /// The SetProperties method specifies the number of buffers to allocate and the size of each buffer.
        /// </summary>
        /// <remarks>
        /// 
        /// This method specifies the buffer requirements, but does not allocate any buffers. Call the IMemAllocator::Commit method to allocate buffers.
        /// The caller allocates two ALLOCATOR_PROPERTIES structures. The pRequest parameter contains the caller's buffer requirements, including the number of buffers and the size of each buffer. When the method returns, the pActual parameter contains the actual buffer properties, as set by the allocator.
        /// When this method is called, the allocator must not be committed or have outstanding buffers.
        /// </remarks>
        /// <param name="pRequest">Pointer to an ALLOCATOR_PROPERTIES structure that contains the buffer requirements.</param>
        /// <param name="pActual">Pointer to an ALLOCATOR_PROPERTIES structure that receives the actual buffer properties.</param>
        /// <returns>Returns an HRESULT value. Possible values include those shown in the following table.</returns>
        long SetProperties( 
            /* [annotation][in] */ 
            [In]  AllocatorProperties pRequest,
            /* [annotation][out] */ 
            [Out] out AllocatorProperties pActual);
        
        /// <summary>
        /// The GetProperties method retrieves the number of buffers that the allocator will create, and the buffer 
        /// </summary>
        /// <param name="pProps">Pointer to an ALLOCATOR_PROPERTIES structure that receives the allocator properties.</param>
        /// <returns>Returns S_OK if successful, or an HRESULT value indicating the cause of the error.</returns>
        /// <remarks>
        /// Calls to this method might not succeed until the IMemAllocator::Commit method is called.
        /// </remarks>
        long GetProperties( 
            /* [annotation][out] */ 
            [Out] out  AllocatorProperties pProps) ;
        
        /// <summary>
        /// The Commit method allocates the buffer memory.
        /// </summary>
        /// <returns>Returns an HRESULT value. Possible values include those shown in the following table.</returns>
        /// <remarks>
        /// Before calling this method, call the IMemAllocator::SetProperties method to specify the buffer requirements.
        /// You must call this method before calling the IMemAllocator::GetBuffer method.
        /// </remarks>
        long Commit( ) ;
        
        /// <summary>
        /// The Decommit method releases the buffer memory.
        /// </summary>
        /// <returns>Returns S_OK if successful, or an HRESULT value indicating the cause of the error.</returns>
        /// <remarks>
        /// Any threads waiting in the IMemAllocator::GetBuffer method return with an error. Further calls to GetBuffer fail, until the IMemAllocator::Commit method is called.
        /// The purpose of the Decommit method is to prevent filters from getting any more samples from the allocator. Filters that already hold a reference count on a sample are not affected. After a filter releases a sample and the reference count goes to zero, however, the sample is no longer available.
        /// The allocator may free the memory belonging to any sample with a reference count of zero. Thus, the Decommit method "releases" the memory in the sense that filters stop having access to it. Whether the memory actually returns to the heap depends on the implementation of the allocator. Some allocators wait until their own destructor method. However, an allocator must not leave any allocated memory behind when it deletes itself. Therefore, an allocator's destructor must wait until all of its samples are released.
        /// </remarks>
        long Decommit( ) ;
        
        /// <summary>
        /// The GetBuffer method retrieves a media sample that contains an empty buffer.
        /// </summary>
        /// <param name="ppBuffer">Receives a pointer to the buffer's IMediaSample interface. The caller must release the interface.</param>
        /// <param name="pStartTime">Pointer to the start time of the sample, or NULL.</param>
        /// <param name="pEndTime">Pointer to the ending time of the sample, or NULL.</param>
        /// <param name="dwFlags">Bitwise combination of zero</param>
        /// <returns>Returns an HRESULT value. Possible values include those shown in the following table.</returns>
        /// <remarks>
        /// By default, this method blocks until a free sample is available or the allocator is decommitted. If the caller specifies the AM_GBF_NOWAIT flag and no sample is available, the allocator can return immediately with a return value of VFW_E_TIMEOUT. However, allocators are not required to support this flag.
        /// The sample returned in ppBuffer has a valid buffer pointer. The caller is responsible for setting any other properties on the sample, such as the time stamps, the media times, or the sync-point property. (For more information, see IMediaSample.)
        /// The pStartTime and pEndTime parameters are not applied to the sample. The allocator might use these values to determine which buffer it retrieves. For example, the Video Renderer filter uses these values to synchronize switching between DirectDraw surfaces. To set the time stamp on the sample, call the IMediaSample::SetTime method.
        /// You must call the IMemAllocator::Commit method before calling this method. This method fails after the IMemAllocator::Decommit method is called.
        /// </remarks>
        long GetBuffer( 
            /* [annotation][out] */ 
            [Out] out  IMediaSample ppBuffer,
            /* [annotation][unique][in] */ 
            [In]  long pStartTime,
            /* [annotation][unique][in] */ 
            [In]  long pEndTime,
            /* [in] */ 
            [In]int dwFlags) ;
        
        /// <summary>
        /// The ReleaseBuffer method releases a media sample.
        /// </summary>
        /// <param name="pBuffer">Pointer to the media sample's IMediaSample interface.</param>
        /// <returns>Returns S_OK if successful, or an HRESULT value indicating the cause of the error.</returns>
        /// <remarks>
        /// When a media sample's reference count reaches zero, it calls this method with itself as the pBuffer parameter. This method releases the sample back to the allocator's list of available samples.
        /// </remarks>
        long ReleaseBuffer( 
            /* [in] */ [In] IMediaSample pBuffer) ;
        
    };
}
