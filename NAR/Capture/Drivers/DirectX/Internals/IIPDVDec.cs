using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NAR.Capture.Drivers.DirectX.Internals
{

     [ComImport,
    Guid( "b8e8bd60-0bfe-11d0-af91-00aa00b67a42" ),
    InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    public interface IIPDVDec 
    {

         long get_IPDisplay( 
            /* [annotation][out] */ 
            [Out] out int displayPix) ;
        
        long put_IPDisplay( 
            /* [in] */ [In] int displayPix) ;
        
    };
}
