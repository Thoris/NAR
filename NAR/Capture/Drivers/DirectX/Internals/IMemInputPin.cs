using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NAR.Capture.Drivers.DirectX.Internals
{

    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid( "56a8689d-0ad4-11ce-b03a-0020af0ba770" ),
    InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IMemInputPin 
    {
    
        long GetAllocator( 
            /* [annotation][out] */ 
            [Out] out  IMemAllocator ppAllocator) ;
        
        long NotifyAllocator( 
            /* [in] */ [In] IMemAllocator pAllocator,
            /* [in] */ [In] bool bReadOnly) ;
        
        long GetAllocatorRequirements( 
            /* [annotation][out] */ 
            [Out] out AllocatorProperties pProps) ;
        
        long Receive( 
            /* [in] */ [In] IMediaSample pSample) ;
        
        long ReceiveMultiple( 
            /* [annotation][size_is][in] */ 
            [In] IMediaSample pSamples,
            /* [in] */ [In] long nSamples,
            /* [annotation][out] */ 
            [Out]  out long nSamplesProcessed) ;
        
        long ReceiveCanBlock() ;
        
    };
    
}
