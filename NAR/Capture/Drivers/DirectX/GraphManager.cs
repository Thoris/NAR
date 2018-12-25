
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Capture.Drivers.DirectX
{
    using Internals;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Runtime.InteropServices.ComTypes;

    public class GraphManager : ISampleGrabberCB
    {
        #region External Methods

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        static extern void MoveMemory(IntPtr dest, IntPtr src, int size);

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        static extern void CopyMemory(DS_MEDIA_FORMAT dest, ref DS_MEDIA_FORMAT src, int size);


        #endregion

        #region Enumerations / Constants


        public const uint MIN_ALLOCATOR_BUFFERS_PER_CLIENT = 3;
        public const int MAX_GUID_STRING_LENGTH = 72;
        public const VIDEO_INPUT_DEVICE default_VIDEO_INPUT_DEVICE = VIDEO_INPUT_DEVICE.WDM_VIDEO_CAPTURE_FILTER;

        public const PIXELFORMAT default_PIXELFORMAT = PIXELFORMAT.PIXELFORMAT_RGB32;



        public readonly string[] VIDEO_INPUT_DEVICE_names =
        {
	        "WDM_CAP",
	        "AVI_FILE",
	        "invalid" // do not use
        };

        #endregion

        #region Variables

        MemoryBufferHandle g_Handle;
        public bool bufferCheckedOut;
        private IGraphBuilder _graphBuilder;
        private IMediaControl _mediaControl;
        private ICaptureGraphBuilder2 _captureGraphBuilder;
        private IMediaEventEx _mediaEvent;
        private IMediaSeeking _mediaSeeking;
        private IAMCameraControl _cameraControl;
        //private IAMDroppedFrames _droppedFrames;
        private IAMVideoProcAmp _videoProcAmp;
        private IAMVideoControl _videoControl;
        private IBaseFilter _sourceFilter;
        private IPin _capturePin;
        private IBaseFilter _decoderFilter;
        private IBaseFilter _rendererFilter;
        private IBaseFilter _grabberFilter;
        private ISampleGrabber _sampleGrabber;

        private string _deviceMoniker;

        private IFilterGraph2 _graph;

        private int _width;
        private int _height;


        ulong fRefCount;

        long current_timestamp;
        IDictionary<long, MemoryBufferEntry> mb;
        long sample_counter;
        uint m_currentAllocatorBuffers;



        bool m_bGraphIsInitialized;


#if DEBUG
        int dwRegisterROT;
#endif

        DS_MEDIA_FORMAT media_format;

        IntPtr m_ESync;
        string m_ESyncName;

        #endregion

        #region Constructors/Destructors

        public GraphManager()
        {
            //captureGraphBuilder(NULL),
            //graphBuilder(NULL),
            //mediaControl(NULL),
            //mediaEvent(NULL),
            //mediaSeeking(NULL),
            //cameraControl(NULL),
            //droppedFrames(NULL),
            //videoControl(NULL),
            //videoProcAmp(NULL),
            //sourceFilter(NULL),
            //decoderFilter(NULL),
            //rendererFilter(NULL),
            //grabberFilter(NULL),
            //capturePin(NULL),
            //m_ESync(NULL),
            //m_ESyncName("SyncEvent")


            //m_ESync = CreateEvent(NULL, TRUE, 0, _T("SyncEvent"));

            media_format = default_DS_MEDIA_FORMAT();
            media_format.subtype = Guid.Empty;

            current_timestamp = 0;
            sample_counter = 0;

            g_Handle = new MemoryBufferHandle() ;
            mb = new Dictionary<long, MemoryBufferEntry>();

        }
        ~GraphManager()
        {
            long hr;
            hr = ReleaseGraph();
        }

        #endregion

        #region Methods


        private void SetFrameSizeAndRate(IAMStreamConfig streamConfig, Size size, int frameRate)
        {
            bool sizeSet = false;
            AMMediaType mediaType;

            // get current format
            streamConfig.GetFormat(out mediaType);

            // change frame size if required
            if ((size.Width != 0) && (size.Height != 0))
            {
                // iterate through device's capabilities to find mediaType for desired resolution
                int capabilitiesCount = 0, capabilitySize = 0;
                AMMediaType newMediaType = null;
                VideoStreamConfigCaps caps = new VideoStreamConfigCaps();

                streamConfig.GetNumberOfCapabilities(out capabilitiesCount, out capabilitySize);

                for (int i = 0; i < capabilitiesCount; i++)
                {
                    if (streamConfig.GetStreamCaps(i, out newMediaType, caps) == 0)
                    {
                        if (caps.InputSize == size)
                        {
                            mediaType.Dispose();
                            mediaType = newMediaType;
                            sizeSet = true;
                            break;
                        }
                        else
                        {
                            newMediaType.Dispose();
                        }
                    }
                }
            }

            VideoInfoHeader infoHeader = (VideoInfoHeader)Marshal.PtrToStructure(mediaType.FormatPtr, typeof(VideoInfoHeader));

            // try changing size manually if failed finding mediaType before
            if ((size.Width != 0) && (size.Height != 0) && (!sizeSet))
            {
                infoHeader.BmiHeader.Width = size.Width;
                infoHeader.BmiHeader.Height = size.Height;
            }
            // change frame rate if required
            if (frameRate != 0)
            {
                infoHeader.AverageTimePerFrame = 10000000 / frameRate;
            }

            // copy the media structure back
            Marshal.StructureToPtr(infoHeader, mediaType.FormatPtr, false);

            // set the new format
            streamConfig.SetFormat(mediaType);

            _width = infoHeader.BmiHeader.Width;
            _height = infoHeader.BmiHeader.Height;

            mediaType.Dispose();
        }
        private byte[] FlipImageRGB32(IntPtr buffer, int len, int width, int height, bool flipImageH, bool flipImageV)
        {
            byte[] ptr = new byte [len];
            Marshal.Copy(buffer,ptr, 0, len);

            return ptr;

            int pixelCount = len;


            

            if (flipImageV)
            {
                if (flipImageH)
                {
                    // both flips set -> rotation about 180 degree
                    for (int index = 0; index < pixelCount / 2; index++)
                    {
                        ptr[index] = (byte)(ptr[index] ^ ptr[pixelCount - index - 1]);
                        ptr[pixelCount - index - 1] = (byte)(ptr[index] ^ ptr[pixelCount - index - 1]);
                        ptr[index] = (byte)(ptr[index] ^ ptr[pixelCount - index - 1]);
                    }
                }
                else
                {
                    // only vertical flip 
                    for (int line = 0; line < height / 2; line++)
                        for (int pixel = 0; pixel < width; pixel++)
                        {
                            ptr[line * width + pixel] = (byte)(ptr[line * width + pixel] ^ ptr[pixelCount - line * width - (width - pixel)]);
                            ptr[pixelCount - line * width - (width - pixel)] = (byte)(ptr[line * width + pixel] ^ ptr[pixelCount - line * width - (width - pixel)]);
                            ptr[line * width + pixel] = (byte)(ptr[line * width + pixel] ^ ptr[pixelCount - line * width - (width - pixel)]);
                        }
                }
            }
            else
            {
                if (flipImageH)
                {
                    // only horizontal flip
                    for (int line = 0; line < height; line++)
                        for (int pixel = 0; pixel < width / 2; pixel++)
                        {
                            ptr[line * width + pixel] = (byte)(ptr[line * width + pixel] ^ ptr[line * width + (width - pixel)]);
                            ptr[line * width + (width - pixel)] = (byte)(ptr[line * width + pixel] ^ ptr[line * width + (width - pixel)]);
                            ptr[line * width + pixel] = (byte)(ptr[line * width + pixel] ^ ptr[line * width + (width - pixel)]);
                        }
                }
            }


            return ptr;
        }
        private long FindCaptureDevice(ref IBaseFilter ppSrcFilter, string filterNameSubstring, bool matchDeviceName, string ieee1394id_str)
        {
            object comObj = null;
            ICreateDevEnum enumDev = null;
            IEnumMoniker enumMon = null;
            IMoniker[] devMon = new IMoniker[1];
            int hr;

            try
            {

                Guid category = FilterCategory.VideoInputDevice;

                // Get the system device enumerator
                Type srvType = Type.GetTypeFromCLSID(Clsid.SystemDeviceEnum);
                if (srvType == null)
                    throw new ApplicationException("Failed creating device enumerator");

                // create device enumerator
                comObj = Activator.CreateInstance(srvType);
                enumDev = (ICreateDevEnum)comObj;

                // Create an enumerator to find filters of specified category
                hr = enumDev.CreateClassEnumerator(ref category, out enumMon, 0);
                if (hr != 0)
                    throw new ApplicationException("No devices of the category");

                // Collect all filters
                IntPtr n = IntPtr.Zero;
                while (true)
                {
                    // Get next filter
                    hr = enumMon.Next(1, devMon, n);
                    if ((hr != 0) || (devMon[0] == null))
                        break;

                    // Add the filter
                    FilterInfo2 filter = new FilterInfo2(devMon[0]);


                    object result = null;
                    //InnerList.Add(filter);
                    devMon[0].BindToObject(null, null, typeof(IBaseFilter).GUID, out result);

                    ppSrcFilter = (IBaseFilter) result;

                    // Release COM object
                    Marshal.ReleaseComObject(devMon[0]);
                    devMon[0] = null;


                    return 0;
                
                }

                // Sort the collection
                //InnerList.Sort();
            }
            catch
            {
            }
            finally
            {
                // release all COM objects
                enumDev = null;
                if (comObj != null)
                {
                    Marshal.ReleaseComObject(comObj);
                    comObj = null;
                }
                if (enumMon != null)
                {
                    Marshal.ReleaseComObject(enumMon);
                    enumMon = null;
                }
                if (devMon[0] != null)
                {
                    Marshal.ReleaseComObject(devMon[0]);
                    devMon[0] = null;
                }
            }

            return -2;

        }
        private ASYNC_INPUT_FLAGS default_ASYNC_INPUT_FLAGS()
        {
            return 0;
        }
        private VIDEO_INPUT_FLAGS default_VIDEO_INPUT_FLAGS()
        {
            return VIDEO_INPUT_FLAGS.WDM_SHOW_FORMAT_DIALOG;
        }
        private DS_MEDIA_FORMAT default_DS_MEDIA_FORMAT()
        {
            DS_MEDIA_FORMAT mf = new DS_MEDIA_FORMAT();
            //ZeroMemory(&mf, sizeof(DS_MEDIA_FORMAT));
            mf.inputDevice = default_VIDEO_INPUT_DEVICE;
            mf.pixel_format = default_PIXELFORMAT;
            mf.inputFlags = 0; // clear flags
            mf.defaultInputFlags = true;
            return (mf);
        }
        public void Init(string deviceMoniker)
        {
            int hr;

            try
            {


                //-----------------------------------------------------------------------------

                //captureGraphBuilder(NULL),

                // get type of capture graph builder
                Type type = Type.GetTypeFromCLSID(Clsid.CaptureGraphBuilder2);
                if (type == null)
                    throw new ApplicationException("Failed creating capture graph builder");

                // create capture graph builder
                object captureGraphObject = Activator.CreateInstance(type);
                _captureGraphBuilder = (ICaptureGraphBuilder2)captureGraphObject;

                //-----------------------------------------------------------------------------

                // get type of filter graph
                type = Type.GetTypeFromCLSID(Clsid.FilterGraph);
                if (type == null)
                    throw new ApplicationException("Failed creating filter graph");

                // create filter graph
                object graphObject = Activator.CreateInstance(type);
                _graph = (IFilterGraph2)graphObject;

                // set filter graph to the capture graph builder
                hr = _captureGraphBuilder.SetFiltergraph((IGraphBuilder)_graph);

                //-----------------------------------------------------------------------------

                //graphBuilder(NULL),

                // Make a new filter graph
                _graphBuilder = (IGraphBuilder)Activator.CreateInstance(Type.GetTypeFromCLSID(Clsid.FilterGraph, true));

                hr = _captureGraphBuilder.SetFiltergraph(_graphBuilder);

                //-----------------------------------------------------------------------------

                //mediaControl(NULL),

                // Retreive the media control interface (for starting/stopping graph)
                _mediaControl = (IMediaControl)_graphBuilder;

                //-----------------------------------------------------------------------------

                //mediaEvent(NULL),

                // get media events' interface
                _mediaEvent = (IMediaEventEx)graphObject;

                //-----------------------------------------------------------------------------


                //mediaSeeking(NULL),
                _mediaSeeking = (IMediaSeeking)graphObject;


                //-----------------------------------------------------------------------------


                //cameraControl(NULL),
                _cameraControl = (IAMCameraControl)_graph;



                //-----------------------------------------------------------------------------


                //droppedFrames(NULL),

                //-----------------------------------------------------------------------------


                //videoControl(NULL),
                // get filter info
                FilterInfo filterInfo = new FilterInfo();


                //// create source device's object
                //sourceObject = FilterInfo.CreateFilter(deviceMoniker);
                //if (sourceObject == null)
                //    throw new ApplicationException("Failed creating device object for moniker");

                //// get base filter interface of source device
                //sourceBase = (IBaseFilter)sourceObject;

                //// get video control interface of the device
                //try
                //{
                //    videoControl = (IAMVideoControl)sourceObject;
                //}
                //catch
                //{
                //    // some camera drivers may not support IAMVideoControl interface
                //}


                //-----------------------------------------------------------------------------


                //videoProcAmp(NULL),

                //-----------------------------------------------------------------------------

                //sourceFilter(NULL),

                //-----------------------------------------------------------------------------

                //decoderFilter(NULL),

                //-----------------------------------------------------------------------------



                //rendererFilter(NULL),
                //-----------------------------------------------------------------------------



                //grabberFilter(NULL),

                //-----------------------------------------------------------------------------

                //capturePin(NULL),

                //-----------------------------------------------------------------------------

                //m_ESync(NULL),

                //-----------------------------------------------------------------------------


                //m_ESyncName("SyncEvent")






            }
            finally
            {
            }
        }
        public long BuildGraphFromXMLString(string xml_string)
        {

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml_string);

            return (BuildGraphFromXMLHandle(xmlDoc));
        }
        private long DisplayFilterProperties(IBaseFilter pFilter, IntPtr hWnd)
        {

            return 0;
            ISpecifyPropertyPages pProp;

            pProp = (ISpecifyPropertyPages)pFilter;

            
            //HRESULT hr = pFilter->QueryInterface(IID_ISpecifyPropertyPages, (void**)&pProp);
            //if (SUCCEEDED(hr))
            {
                // Get the filter's name and IUnknown pointer.
                Internals.FilterInfo FilterInfo;
                pFilter.QueryFilterInfo(out FilterInfo);
                //FilterInfo = (IBaseFilter)pFilter;

                // Show the page. 
                CAUUID caGUID;
                pProp.GetPages(out caGUID);
                //OleCreatePropertyFrame(
                //    hWnd,                   // Parent window
                //    0, 0,                   // (Reserved)
                //    FilterInfo.achName,     // Caption for the dialog box
                //    1,                      // Number of objects (just the filter)
                //    (IUnknown**)&pFilter,  // Array of object pointers. 
                //    caGUID.cElems,          // Number of property pages
                //    caGUID.pElems,          // Array of property page CLSIDs
                //    0,                      // Locale identifier
                //    0, NULL);               // Reserved

                //// Clean up.
                //if (FilterInfo.pGraph != NULL) FilterInfo.pGraph->Release();
                //CoTaskMemFree(caGUID.pElems);
            }
            return (0);
        }
        private bool CanDeliverDV(IPin pin)
        {

            return false;







            long hr;
            

            IEnumMediaTypes enum_mt = null;
            pin.EnumMediaTypes(enum_mt);

            enum_mt.Reset();
            AMMediaType[] p_mt = null;
            int fetch = 0;
            int count = 0;
            //while (0 == (hr = enum_mt.Next(1, p_mt, out fetch)))
            //{
            //    if ((p_mt[count].SubType == MediaSubType.DVSD))// ||
            //        //(p_mt[count].SubType == MEDIASUBTYPE_dvsl) ||
            //        //(p_mt[count].SubType == MediaSubType.d MEDIASUBTYPE_dvhd))
            //    {
            //        DeleteMediaType(p_mt);
            //        return (true);
            //    }
            //    else DeleteMediaType(p_mt);

            //    count++;
            //}

            return (false);
        }
        private long MatchMediaTypes(IPin pin, DS_MEDIA_FORMAT mf, AMMediaType req_mt)
        {
            long hr;
            IEnumMediaTypes enum_mt = null;
            pin.EnumMediaTypes(enum_mt);
            enum_mt.Reset();
            //AM_MEDIA_TYPE p_mt = NULL;
            //while (S_OK == (hr = enum_mt->Next(1, &p_mt, NULL)))
            //{
            //    VIDEOINFOHEADER* pvi = (VIDEOINFOHEADER*)p_mt->pbFormat;
            //    if (((p_mt->subtype == mf->subtype) || (mf->subtype == GUID_NULL)) &&
            //        ((pvi->bmiHeader.biHeight == mf->biHeight) || (mf->biHeight == 0)) &&
            //        ((pvi->bmiHeader.biWidth == mf->biWidth) || (mf->biWidth == 0)) &&
            //        ((avg2fps(pvi->AvgTimePerFrame, 3) == RoundDouble(mf->frameRate, 3)) || (mf->frameRate == 0.0))
            //        )
            //    {
            //        // found a match!
            //        CopyMediaType(req_mt, p_mt);
            //        DeleteMediaType(p_mt);
            //        return (S_OK);
            //    }
            //    else DeleteMediaType(p_mt);
            //}

            return (-1);
        }
        private bool CanDeliverVideo(IPin pin)
        {
            long hr;
            IEnumMediaTypes enum_mt = null;
            pin.EnumMediaTypes(enum_mt);
            enum_mt.Reset();
            //AM_MEDIA_TYPE* p_mt = NULL;
            //while (S_OK == (hr = enum_mt->Next(1, &p_mt, NULL)))
            //{
            //    if (p_mt->majortype == MEDIATYPE_Video)
            //    {
            //        DeleteMediaType(p_mt);
            return (true);
            //    }
            //    else DeleteMediaType(p_mt);
            //}

            return (false);
        }
        private long ConnectFilters(IBaseFilter filter_out, int out_pin_nr, IBaseFilter in_filter, int in_pin_nr)
        {
            long hr;
            IPin OutPin = null;
            IPin InPin = null;
            hr = getPin(filter_out, PinDirection.Output, out_pin_nr, ref OutPin);
            if (hr != 0) return (hr);
            hr = getPin(in_filter, PinDirection.Input, in_pin_nr, ref InPin);
            if (hr != 0) return (hr);
            if (OutPin == null || InPin == null) return -1; // (E_FAIL);

            hr = OutPin.Connect(InPin, null);
            if (hr != 0) return -1;//(E_FAIL);
            else return (0);
        }
        private long AutoConnectFilters(IBaseFilter filter_out, int out_pin_nr, IBaseFilter in_filter, int in_pin_nr, IGraphBuilder pGraphBuilder)
        {
            long hr;
            IPin OutPin = null;
            IPin InPin = null;

            hr = getPin(filter_out, PinDirection.Output, out_pin_nr, ref OutPin);
            if (hr != 0) return hr;

            hr = getPin(filter_out, PinDirection.Output, out_pin_nr, ref OutPin);
            if (hr != 0) return (hr);

            hr = getPin(in_filter, PinDirection.Input, in_pin_nr, ref InPin);
            if (hr != 0) return (hr);

            if (OutPin == null || InPin == null) return -1;//(E_FAIL);

            hr = pGraphBuilder.Connect(OutPin, InPin);
            if (hr != 0) return -1;//(E_FAIL);
            else return (0);
        }
        private long BuildGraphFromXMLHandle(XmlDocument xml_h)
        {
            if (m_bGraphIsInitialized) return -1; //return(E_FAIL); // call ReleaseGraph() first!

            DS_MEDIA_FORMAT mf = default_DS_MEDIA_FORMAT();
            mf.defaultInputFlags = false;
            mf.sourceFilterName = "";

            XmlNode doc_handle = xml_h;

            XmlNode e_avi = doc_handle.SelectSingleNode("dsvl_input").SelectSingleNode("avi_file");
            XmlNode e_camera = doc_handle.SelectSingleNode("dsvl_input").SelectSingleNode("camera");
            XmlNode e_pixel_format = null;

            if (e_camera != null)
            {
                mf.inputDevice = VIDEO_INPUT_DEVICE.WDM_VIDEO_CAPTURE_FILTER;

                if (e_camera.Attributes["device_name"] != null && !string.IsNullOrEmpty(e_camera.Attributes["device_name"].Value))
                {
                    mf.sourceFilterName = e_camera.Attributes["device_name"].ToString();
                    mf.isDeviceName = true;
                    mf.inputFlags |= (int)VIDEO_INPUT_FLAGS.WDM_MATCH_FILTER_NAME;
                }
                else if (e_camera.Attributes["friendly_name"] != null && !string.IsNullOrEmpty(e_camera.Attributes["friendly_name"].Value))
                {
                    mf.sourceFilterName = e_camera.Attributes["friendly_name"].ToString();
                    mf.isDeviceName = false;
                    mf.inputFlags |= (int)VIDEO_INPUT_FLAGS.WDM_MATCH_FILTER_NAME;
                }

                if ((e_camera.Attributes["show_format_dialog"] != null) &&
                    e_camera.Attributes["show_format_dialog"].Value.CompareTo("true") == 0)
                    mf.inputFlags |= (int)VIDEO_INPUT_FLAGS.WDM_SHOW_FORMAT_DIALOG;

                if (e_camera.Attributes["ieee1394id"] != null && !string.IsNullOrEmpty(e_camera.Attributes["ieee1394id"].Value))
                {
                    mf.ieee1394_id = e_camera.Attributes["ieee1394id"].Value;
                    mf.inputFlags |= (int)VIDEO_INPUT_FLAGS.WDM_MATCH_IEEE1394_ID;
                }

                if (e_camera.Attributes["frame_width"] != null && !string.IsNullOrEmpty(e_camera.Attributes["frame_width"].Value))
                {
                    mf.biWidth = int.Parse(e_camera.Attributes["frame_width"].Value);
                    if (mf.biWidth < 0) return -2;//return(E_INVALIDARG);
                    mf.inputFlags |= (int)VIDEO_INPUT_FLAGS.WDM_MATCH_FORMAT;
                }

                if (e_camera.Attributes["frame_height"] != null && !string.IsNullOrEmpty(e_camera.Attributes["frame_height"].Value))
                {
                    mf.biHeight = int.Parse(e_camera.Attributes["frame_height"].Value);
                    if (mf.biHeight < 0) return -2;//return(E_INVALIDARG);
                    mf.inputFlags |= (int)VIDEO_INPUT_FLAGS.WDM_MATCH_FORMAT;
                }

                if (e_camera.Attributes["frame_rate"] != null && !string.IsNullOrEmpty(e_camera.Attributes["frame_rate"].Value))
                {

                    mf.frameRate = double.Parse(e_camera.Attributes["frame_rate"].Value);
                    if (mf.frameRate <= 0) return -2;// return(E_INVALIDARG);
                    mf.inputFlags |= (int)VIDEO_INPUT_FLAGS.WDM_MATCH_FORMAT;
                }

                e_pixel_format = doc_handle.SelectSingleNode("dsvl_input").SelectSingleNode("camera").SelectSingleNode("pixel_format").FirstChild;
                if (e_pixel_format == null) return -2;//return(E_INVALIDARG);


                mf.pixel_format = (PIXELFORMAT)Enum.Parse(typeof(PIXELFORMAT), "PIXELFORMAT_" + e_pixel_format.Name);


                if (mf.pixel_format == PIXELFORMAT.PIXELFORMAT_RGB32)
                {
                    if ((e_pixel_format.Attributes["flip_h"] != null) && (e_pixel_format.Attributes["flip_h"].Value.ToString().CompareTo("true") == 0))
                        media_format.flipH = true;
                    if ((e_pixel_format.Attributes["flip_v"] != null) && (e_pixel_format.Attributes["flip_v"].Value.ToString().CompareTo("true") == 0))
                        media_format.flipV = true;
                }
            }
            else if (e_avi != null)
            {
                mf.inputDevice = VIDEO_INPUT_DEVICE.ASYNC_FILE_INPUT_FILTER;
                mf.sourceFilterName = e_avi.Attributes["file_name"].ToString();


                if ((e_avi.Attributes["use_reference_clock"] != null) &&
                    (e_avi.Attributes["use_reference_clock"].Value.CompareTo("false") == 0))
                    mf.inputFlags |= (int)ASYNC_INPUT_FLAGS.ASYNC_INPUT_DO_NOT_USE_CLOCK;

                if ((e_avi.Attributes["loop_avi"] != null) && (e_avi.Attributes["loop_avi"].Value.CompareTo("true") == 0))
                    mf.inputFlags |= (int)ASYNC_INPUT_FLAGS.ASYNC_LOOP_VIDEO;

                if ((e_avi.Attributes["render_secondary"] != null) && (e_avi.Attributes["render_secondary"].Value.CompareTo("true") == 0))
                    mf.inputFlags |= (int)ASYNC_INPUT_FLAGS.ASYNC_RENDER_SECONDARY_STREAMS;

                e_pixel_format = doc_handle.SelectSingleNode("dsvl_input").SelectSingleNode("avi_file").SelectSingleNode("pixel_format");
                if (e_pixel_format == null) return -2;//return(E_INVALIDARG);

                mf.pixel_format = (PIXELFORMAT)Enum.Parse(typeof(PIXELFORMAT), "PIXELFORMAT_" + e_pixel_format.Name);


                if (mf.pixel_format == PIXELFORMAT.PIXELFORMAT_RGB32)
                {
                    if ((e_pixel_format.Attributes["flip_h"] != null) && (e_pixel_format.Attributes["flip_h"].Value.CompareTo("true") == 0))
                        media_format.flipH = true;
                    if ((e_pixel_format.Attributes["flip_v"] != null) && (e_pixel_format.Attributes["flip_v"].Value.CompareTo("true") == 0))
                        media_format.flipV = true;
                }
            }
            else return -2; //return(E_INVALIDARG);


            // check for mf.defaultInputFlags and set default flags if necessary
            if (mf.defaultInputFlags)
            {
                switch (mf.inputDevice)
                {
                    case (VIDEO_INPUT_DEVICE.WDM_VIDEO_CAPTURE_FILTER): mf.inputFlags = (int)default_VIDEO_INPUT_FLAGS();
                        break;
                    case (VIDEO_INPUT_DEVICE.ASYNC_FILE_INPUT_FILTER): mf.inputFlags = (int)default_ASYNC_INPUT_FLAGS();
                        break;
                    default: return -2;//return(E_INVALIDARG);
                        break;
                };
            }


            long hr;

            IBaseFilter pVideoSource = null;
            IBaseFilter pStreamSplitter = null; // used in conjunction with the Async File Source filter
            IBaseFilter pVideoDecoder = null;	// used for changing DV resolution
            IBaseFilter pVideoRenderer = null;
            IAMStreamConfig pStreamConfig = null;
            object objStreamConfig = null;


            // OT-FIX 11/22/04 [thp]
            IBaseFilter pSampleGrabber = null;

            FilterInfo filter_info;

            _graphBuilder = (IGraphBuilder)Activator.CreateInstance(Type.GetTypeFromCLSID(Clsid.FilterGraph, true));


#if (DEBUG)
            //hr = AddToRot(graphBuilder, &dwRegisterROT);
#endif


            // Create the capture graph builder
            Type type = Type.GetTypeFromCLSID(Clsid.CaptureGraphBuilder2);
            if (type == null)
                throw new ApplicationException("Failed creating capture graph builder");

            // create capture graph builder
            object captureGraphObject = Activator.CreateInstance(type);
            _captureGraphBuilder = (ICaptureGraphBuilder2)captureGraphObject;




            // OT-FIX 11/22/04 [thp]

            // Add Null Renderer.
            pVideoRenderer = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(Clsid.NullRender));


            //hr = CoCreateInstance(CLSID_NullRenderer, NULL, CLSCTX_INPROC_SERVER, IID_IBaseFilter, (void**)&(pVideoRenderer));
            //if (FAILED(hr))
            //    return hr;

            // get type for sample grabber
            type = Type.GetTypeFromCLSID(Clsid.SampleGrabber);
            if (type == null)
                throw new ApplicationException("Failed creating sample grabber");

            // create sample grabber
            object sampleGrabberObj = Activator.CreateInstance(type);
            _sampleGrabber = (ISampleGrabber)sampleGrabberObj;
            pSampleGrabber = (IBaseFilter)sampleGrabberObj;



            AMMediaType _mt = new AMMediaType();

            _mt.MajorType = MediaType.Video;
            _mt.FormatType = Guid.Empty;
            //_mt.SubType =  PXtoMEDIASUBTYPE(mf.pixel_format);
            hr = _sampleGrabber.SetMediaType(_mt);
            if (hr != 0)
                return hr;

            // Obtain interfaces for media control
            _mediaControl = (IMediaControl)_graphBuilder;

            _mediaEvent = (IMediaEventEx)_graphBuilder;


            hr = _captureGraphBuilder.SetFiltergraph(_graphBuilder);
            if (hr != 0)
                return hr;

            // ###########################################################################################
            if (mf.inputDevice == VIDEO_INPUT_DEVICE.WDM_VIDEO_CAPTURE_FILTER)
            {
                // ###########################################################################################

                hr = FindCaptureDevice(ref pVideoSource,
                    ((mf.inputFlags & (int)VIDEO_INPUT_FLAGS.WDM_MATCH_FILTER_NAME) == (int)VIDEO_INPUT_FLAGS.WDM_MATCH_FILTER_NAME) ? mf.sourceFilterName : null,
                    (mf.isDeviceName),
                    ((mf.inputFlags & (int)VIDEO_INPUT_FLAGS.WDM_MATCH_IEEE1394_ID) == (int)VIDEO_INPUT_FLAGS.WDM_MATCH_IEEE1394_ID) ? mf.ieee1394_id : "0");

                if (hr != 0)
                {
                    // Don't display a message because FindCaptureDevice will handle it
                    return hr;
                }

                hr = _graphBuilder.AddFilter(pVideoSource, "Video Source");

                if (hr != 0)
                {
                    //AMErrorMessage(hr,"Couldn't add capture filter to graph!");
                    return hr;
                }

                _sourceFilter = pVideoSource;

                hr = getPin(pVideoSource, PinDirection.Output, 1, ref _capturePin);
                if (hr != 0)
                    return (hr);

                // -------------
                //hr = _captureGraphBuilder.FindInterface( PinCategory.Capture, Guid.Empty, pVideoSource, typeof( IAMStreamConfig ).GUID, out objStreamConfig);


                hr = _captureGraphBuilder.FindInterface(PinCategory.Capture, MediaType.Video, pVideoSource, typeof(IAMStreamConfig).GUID, out objStreamConfig);

                if (hr != 0)
                {
                    //AMErrorMessage(hr,"Couldn't find IAMStreamConfig interface.");
                    return hr;
                }
                pStreamConfig = (IAMStreamConfig)objStreamConfig;

                

                // ---------------------------------------------------------------------------------
                // Unfortunately, WDM DV Video Capture Devices (such as consumer miniDV camcorders)
                // require special handling. Since the WDM source will only offer DVRESOLUTION_FULL,
                // any changes must be made through IIPDVDec on the DV Decoder filter.
                bool pinSupportsDV = CanDeliverDV(_capturePin);

                // ---------------------------------------------------------------------------------
                if (pinSupportsDV)
                {
                    IIPDVDec pDVDec;
                    // insert a DV decoder (CLSID_DVVideoCodec) into our graph.
                    //hr = CoCreateInstance(CLSID_DVVideoCodec, NULL, CLSCTX_INPROC_SERVER, IID_IBaseFilter, (void**)&(pVideoDecoder));
                    if (hr != 0) return (hr);
                    _decoderFilter = pVideoDecoder;

                    hr = _graphBuilder.AddFilter(pVideoDecoder, "Video Decoder");

                    if ((mf.inputFlags & (int)VIDEO_INPUT_FLAGS.WDM_MATCH_FORMAT) == (int)VIDEO_INPUT_FLAGS.WDM_MATCH_FORMAT)
                    // since we're dealing with DV, there's only a limited range of possible resolutions:
                    //
                    //	 DVDECODERRESOLUTION_720x480 (PAL: 720x576)  = DVRESOLUTION_FULL 
                    //	 DVDECODERRESOLUTION_360x240 (PAL: 360x288)  = DVRESOLUTION_HALF
                    //	 DVDECODERRESOLUTION_180x120 (PAL: 180x144)	 = DVRESOLUTION_QUARTER
                    //	 DVDECODERRESOLUTION_88x60   (PAL: 88x72)	 = DVRESOLUTION_DC
                    {
                        pDVDec = (IIPDVDec)pVideoDecoder;

                        int dvRes;
                        //if(FAILED(hr = pDVDec->get_IPDisplay(&dvRes))) return(hr); // get default resolution
                        hr = pDVDec.get_IPDisplay(out dvRes);
                        if (hr != 0)
                            return hr;

                        if ((mf.biWidth == 720) && ((mf.biHeight == 480) || (mf.biHeight == 576))) dvRes = (int)DVRESOLUTION.DVRESOLUTION_FULL;
                        else if ((mf.biWidth == 360) && ((mf.biHeight == 240) || (mf.biHeight == 288))) dvRes = (int)DVRESOLUTION.DVRESOLUTION_HALF;
                        else if ((mf.biWidth == 180) && ((mf.biHeight == 120) || (mf.biHeight == 144))) dvRes = (int)DVRESOLUTION.DVRESOLUTION_QUARTER;
                        else if ((mf.biWidth == 88) && ((mf.biHeight == 60) || (mf.biHeight == 72))) dvRes = (int)DVRESOLUTION.DVRESOLUTION_DC;

                        hr = pDVDec.put_IPDisplay(dvRes);
                        if (hr != 0)
                            return hr;
                    }

                    if (((mf.inputFlags & (int)VIDEO_INPUT_FLAGS.WDM_SHOW_FORMAT_DIALOG)) == (int)VIDEO_INPUT_FLAGS.WDM_SHOW_FORMAT_DIALOG) // displaying the DV decoder's FILTER property page amounts to
                    // the same as showing the WDM capture PIN property page.
                    {
                        hr = DisplayFilterProperties(pVideoDecoder, IntPtr.Zero);
                        if (hr != 0)
                        {
                            //AMErrorMessage(hr,"Can't display filter properties.");
                            //  non-critical error, no need to abort
                        }
                    }

                } // pinSupportsDV


                // ---------------------------------------------------------------------------------
                else // !pinSupportsDV
                {
                    if ((mf.inputFlags & (int)VIDEO_INPUT_FLAGS.WDM_MATCH_FORMAT) == (int)VIDEO_INPUT_FLAGS.WDM_MATCH_FORMAT)
                    {
                        AMMediaType mt = null;
                        hr = MatchMediaTypes(_capturePin, mf, mt);
                        if (hr != 0)
                        {
                            // automated media type selection failed -- display property page!
                            mf.inputFlags &= (int)VIDEO_INPUT_FLAGS.WDM_SHOW_FORMAT_DIALOG;
                        }
                        else pStreamConfig.SetFormat(mt);
                    }

                    if (((mf.inputFlags & (int)VIDEO_INPUT_FLAGS.WDM_SHOW_FORMAT_DIALOG)) == (int)VIDEO_INPUT_FLAGS.WDM_SHOW_FORMAT_DIALOG && !pinSupportsDV)
                    {
                        //hr = DisplayPinProperties(_capturePin);
                        if (hr != 0)
                            return hr;
                    }


                } // !pinSupportsDV

                // ---------------------------------------------------------------------------------

                if ((mf.inputFlags & (int)VIDEO_INPUT_FLAGS.WDM_SHOW_CONTROL_DIALOG) == (int)VIDEO_INPUT_FLAGS.WDM_SHOW_CONTROL_DIALOG)
                {
                    //hr = DisplayFilterProperties(_sourceFilter);
                    if (hr != 0)
                        return hr;
                }
                // ---------------------------------------------------------------------------------


                _cameraControl = (IAMCameraControl)pVideoSource;
                //if(FAILED(pVideoSource->QueryInterface(IID_IAMCameraControl,(void**)&cameraControl)))
                //    cameraControl = NULL; // will be NULL anyway (replace with something intelligent)

                //_droppedFrames = (IAMDroppedFrames)pVideoSource;
                //if(FAILED(pVideoSource->QueryInterface(IID_IAMDroppedFrames,(void**)&droppedFrames)))
                //	droppedFrames = NULL; // will be NULL anyway (replace with something intelligent)

                _videoControl = (IAMVideoControl)pVideoSource;
                //if(FAILED(pVideoSource->QueryInterface(IID_IAMVideoControl,(void**)&videoControl)))
                //	videoControl = NULL; // will be NULL anyway (replace with something intelligent)

                _videoProcAmp = (IAMVideoProcAmp)pVideoSource;
                //if(FAILED(pVideoSource->QueryInterface(IID_IAMVideoProcAmp,(void**)&videoProcAmp)))
                //	videoProcAmp = NULL; // will be NULL anyway (replace with something intelligent)


                // ###########################################################################################
            }
            else if (mf.inputDevice == VIDEO_INPUT_DEVICE.ASYNC_FILE_INPUT_FILTER)
            {
                // ###########################################################################################



                //if(FAILED(hr = CoCreateInstance(CLSID_AsyncReader, NULL, CLSCTX_INPROC_SERVER, 
                //	IID_IBaseFilter, (void**)&(pVideoSource)))) return(hr);

                IFileSourceFilter pFileSource = null;

                pFileSource = (IFileSourceFilter)pVideoSource;
                //if(FAILED(pVideoSource->QueryInterface(IID_IFileSourceFilter,(void**)&pFileSource))) return(hr);

                hr = pFileSource.Load(mf.sourceFilterName, null);
                if (hr != 0)
                {
                    string path;
                    string path_offset;
                    //if(SearchPath(NULL,_bstr_t(mf.sourceFilterName),NULL,MAX_PATH,path,&path_offset) > 0)
                    //{
                    //    if(FAILED(hr = pFileSource->Load(_bstr_t(path),NULL)))
                    //    {
                    //        // file not found
                    //        AMErrorMessage(hr,"Input file not found.");
                    //        return(hr);
                    //    }
                    //}
                }

                hr = _graphBuilder.AddFilter(pVideoSource, "File Reader");
                if (hr != 0)
                {
                    //AMErrorMessage(hr,"Couldn't add async file source filter to graph!");
                    return hr;
                }

                //if(FAILED(hr = CoCreateInstance(CLSID_AviSplitter, NULL, CLSCTX_INPROC_SERVER, 
                //    IID_IBaseFilter, (void**)&(pStreamSplitter)))) return(hr);


                hr = _graphBuilder.AddFilter(pStreamSplitter, "Stream Splitter");
                if (hr != 0)
                    return hr;


                hr = ConnectFilters(pVideoSource, 1, pStreamSplitter, 1);
                if (hr != 0)
                {
                    //AMErrorMessage(hr,"Couldn't connect Async File Source to AVI Splitter!");
                    return hr;
                }

                IPin pStreamPin00 = null;
                hr = getPin(pStreamSplitter, PinDirection.Output, 1, ref pStreamPin00);
                if (hr != 0)
                    return hr;

                //CComPtr<IPin> pStreamPin00 = getPin(pStreamSplitter,PINDIR_OUTPUT,1);
                if (pStreamPin00 == null || !CanDeliverVideo(pStreamPin00))
                {
                    //AMErrorMessage(hr,"AVI file format error. Substream 0x00 does not deliver MEDIATYPE_Video.");
                    return -1; //(E_FAIL);
                }

                // -------------------------------------------------------------------------------------------
            }
            else return -2;//(E_INVALIDARG);
            // ###########################################################################################


            // OT-FIX 11/22/04 [thp]
            hr = _graphBuilder.AddFilter(pSampleGrabber, "Sample Grabber");

            // -------------
            hr = _graphBuilder.AddFilter(pVideoRenderer, "Video Renderer");
            // -------------
            // Render the capture pin on the video capture filter
            // Use this instead of g_pGraph->RenderFile


            // -------------------------------------------------------------------------------------------
            if (mf.inputDevice == VIDEO_INPUT_DEVICE.WDM_VIDEO_CAPTURE_FILTER)
            {

                // OT-FIX 11/22/04 [thp]
                hr = AutoConnectFilters(pVideoSource, 1, pSampleGrabber, 1, _graphBuilder);
                if (hr != 0) return (hr);
                hr = AutoConnectFilters(pSampleGrabber, 1, pVideoRenderer, 1, _graphBuilder);
                if (hr != 0) return (hr);

                _graphBuilder.SetDefaultSyncSource();

                // -------------------------------------------------------------------------------------------
            }
            else if (mf.inputDevice == VIDEO_INPUT_DEVICE.ASYNC_FILE_INPUT_FILTER)
            {

                //hr = AutoConnectFilters(pStreamSplitter,1,pVideoRenderer,1,graphBuilder);

                // OT-FIX 11/22/04 [thp]
                hr = AutoConnectFilters(pStreamSplitter, 1, pSampleGrabber, 1, _graphBuilder);

                if (hr != 0)
                {
                    //AMErrorMessage(hr,"Couldn't find a matching decoder filter for stream 0x00.\n"
                    //    "Check if the required AVI codec is installed.");
                    return hr;
                }
                // OT-FIX 11/22/04 [thp]
                hr = AutoConnectFilters(pSampleGrabber, 1, pVideoRenderer, 1, _graphBuilder);
                if (hr != 0) return (hr);

                if ((mf.inputFlags & (int)ASYNC_INPUT_FLAGS.ASYNC_INPUT_DO_NOT_USE_CLOCK) == (int)ASYNC_INPUT_FLAGS.ASYNC_INPUT_DO_NOT_USE_CLOCK)
                {
                    pVideoSource.SetSyncSource(IntPtr.Zero);
                    pStreamSplitter.SetSyncSource(IntPtr.Zero);
                    pVideoRenderer.SetSyncSource(IntPtr.Zero);

                    // OT-FIX 11/22/04 [thp]
                    pSampleGrabber.SetSyncSource(IntPtr.Zero);
                }

                if ((mf.inputFlags & (int)ASYNC_INPUT_FLAGS.ASYNC_RENDER_SECONDARY_STREAMS) == (int)ASYNC_INPUT_FLAGS.ASYNC_RENDER_SECONDARY_STREAMS)
                {
                    IPin[] pPin = null;
                    IEnumPins EnumPins;
                    int fetched;
                    Internals.PinInfo pinfo;

                    pStreamSplitter.EnumPins(out EnumPins);
                    EnumPins.Reset();
                    EnumPins.Next(1, pPin, out fetched);
                    pPin[0].QueryPinInfo(out pinfo);

                    //if(fetched > 0) do
                    //{
                    //    if(pinfo.Direction == PinDirection.Output)
                    //    {
                    //        IPin pConnectedPin = null;
                    //        if(pPin[0].ConnectedTo(out pConnectedPin) == 0x80040209L)//VFW_E_NOT_CONNECTED)
                    //            hr = _graphBuilder.Render(pPin[0]);
                    //        if(pConnectedPin != null) pConnectedPin.Release();
                    //    }
                    //    pPin[0].Release();
                    //    EnumPins.Next(1, pPin[0], out fetched);
                    //    pPin[0].QueryPinInfo(&pinfo);

                    //} while(fetched > 0);

                }

                // -------------------------------------------------------------------------------------------
            }
            else return -2;//(E_INVALIDARG);



            AMMediaType mediaType = null;
            IPin rendererPin = null;
            hr = getPin(pVideoRenderer, PinDirection.Input, 1, ref rendererPin);
            if (hr != 0) return (hr);

            
            if (mediaType == null)
                mediaType = new AMMediaType();

            hr = pStreamConfig.GetFormat(out mediaType);
            


            //hr = rendererPin.ConnectionMediaType(mediaType);
            //VideoInfoHeader pvi = new VideoInfoHeader();// (VideoInfoHeader) mediaType.pbFormat;

            pVideoSource.QueryFilterInfo(out filter_info);


            //THORIS
            SetFrameSizeAndRate(pStreamConfig, new Size(640, 480), 30);


            // ISSUE: filter_info.achName will just contain an integer (e.g. '0001', probably in order of creation)
            // instead of the filter's "friendly name"
            media_format.sourceFilterName = filter_info.Name;// (LPWSTR) CoTaskMemAlloc(sizeof(WCHAR)*(wcslen(filter_info.achName)+1));
            //if(filter_info.pGraph != NULL) filter_info.pGraph->Release(); 

            media_format.subtype = mediaType.SubType;
            media_format.biWidth = 640;
            media_format.biHeight = 480;
            media_format.frameRate = 30;
            //media_format.pixel_format = MEDIASUBTYPEtoPX(mediaType.subtype);
            //FreeMediaType(mediaType);
            // -------------

            //long _num_alloc = (3 * 3);
            IPin sgPin = null;
            hr = getPin(pSampleGrabber, PinDirection.Input, 1, ref sgPin);
            if (hr != 0)
                return hr;

            IMemAllocator pAllocator = null;

            IMemInputPin sgmiPin = null;
            sgmiPin = (IMemInputPin)sgPin;

            hr = sgmiPin.GetAllocator(out pAllocator);

            if (hr != 0)
                return hr;

            hr = pAllocator.Decommit();
            if (hr != 0) return (hr);
            AllocatorProperties requestedProperties;
            AllocatorProperties actualProperties;
            //pAllocator.GetProperties(out requestedProperties);
            //if (requestedProperties.cBuffers != _num_alloc) requestedProperties.cBuffers = _num_alloc;
            //hr = pAllocator.SetProperties(requestedProperties, out actualProperties);

            // -------------
            m_bGraphIsInitialized = true;

            _grabberFilter = pSampleGrabber;
            return 0;//(S_OK);
        }
        private long BuildGraphFromXMLFile(string xml_filename)
        {
            return 0;
        }
        private long ReleaseGraph()
        {

            //CAutoLock cObjectLock(&m_CSec);
            if (!m_bGraphIsInitialized) return -1;//return(E_FAIL); // call BuildGraph() first!
            long hr = _mediaControl.Stop();

            // Enumerate the filters in the graph.
            IEnumFilters pEnum = null;
            hr = _graphBuilder.EnumFilters(out pEnum);
            if (hr == 0)
            {
                IBaseFilter[] pFilter = null;
                int res = 0;
                while (0 == pEnum.Next(1, pFilter, out res))
                {
                    FilterInfo f_info;

                    long hr_f = pFilter[0].QueryFilterInfo(out f_info);

                    // Remove the filter.
                    hr = _graphBuilder.RemoveFilter(pFilter[0]);
#if DEBUG
                    //_ASSERT(SUCCEEDED(hr));
#endif

                    if (hr_f == 0)
                    {
                        //f_info.FilterGraph.Release();
                    }
                    else
                    {
#if DEBUG
                        //_ASSERT(FALSE);
#endif
                    }

                    // Reset the enumerator.
                    pEnum.Reset();
                    //pFilter[0].Release();
                }
                //pEnum.Release();
            }

            //_mediaControl.Release();
            //_graphBuilder.Release();

            //if(_captureGraphBuilder!=null) _captureGraphBuilder.Release();
            //if(_mediaEvent!=null) _mediaEvent.Release();
            //if(_mediaSeeking!=null) _mediaSeeking.Release();
            //if(_cameraControl!=null) _cameraControl.Release();
            //if(_droppedFrames!=null) _droppedFrames.Release();
            //if(_videoControl!=null) _videoControl.Release();
            //if(_videoProcAmp!=null) _videoProcAmp.Release();
            //if(_sourceFilter!=null) _sourceFilter.Release();
            //if(_capturePin!=null) _capturePin.Release();
            //if(_decoderFilter!=null) _decoderFilter.Release();
            //if(_rendererFilter!=null) _rendererFilter.Release();
            //if(_grabberFilter!=null) _grabberFilter.Release();
            //if(_sampleGrabber!=null) _sampleGrabber.Release();


#if DEBUG
            //RemoveFromRot(dwRegisterROT);
            dwRegisterROT = 0;
#endif

            m_bGraphIsInitialized = false;


            return 0;
        }
        private bool IsGraphInitialized()
        {
            return false;
        }
        private long GetCurrentMediaFormatEx(ref DS_MEDIA_FORMAT mf)
        {
            int size = Marshal.SizeOf(typeof(DS_MEDIA_FORMAT));
            CopyMemory(mf, ref media_format, size);
            return 0;
        }
        private long GetCurrentMediaFormat(ref long frame_width, ref  long frame_height, ref double frames_per_second, ref PIXELFORMAT pixel_format)
        {

            DS_MEDIA_FORMAT mf = null;
            long hr = GetCurrentMediaFormatEx(ref mf);
            if (hr != 0) return (hr);
            if (frame_width != null) frame_width = mf.biWidth;
            if (frame_height != null) frame_height = mf.biHeight;
            if (frames_per_second != null) frames_per_second = mf.frameRate;
            if (pixel_format != null) pixel_format = mf.pixel_format;
            return 0;
        }
        private long GetCurrentTimestamp(ref long Timestamp)
        {
            Timestamp = current_timestamp;
            return 0;
        }
        private long GetCurrentTimestamp()
        {
            return current_timestamp;
        }
        public int WaitForNextSample(long dwMilliseconds)
        {
            return 0;
        }
        public long EnableMemoryBuffer(uint _maxConcurrentClients, uint _allocatorBuffersPerClient)
        {

            //CAutoLock cObjectLock(&m_CSec);
            long hr;

            if (_allocatorBuffersPerClient < MIN_ALLOCATOR_BUFFERS_PER_CLIENT)
                _allocatorBuffersPerClient = MIN_ALLOCATOR_BUFFERS_PER_CLIENT;
            if (_maxConcurrentClients <= 0) _maxConcurrentClients = 1;

            m_currentAllocatorBuffers = _maxConcurrentClients * _allocatorBuffersPerClient;

            // ------
            IPin sgPin = null;
            hr = getPin(_grabberFilter, PinDirection.Input, 1, ref sgPin);
            if (hr != 0)
                return (hr);

            IMemAllocator pAllocator = null;

            IMemInputPin sgmiPin = null;
            sgmiPin = (IMemInputPin)sgPin;
            //hr = sgPin.QueryInterface(IID_IMemInputPin, ref sgmiPin);
            //if (hr != 0) return (hr);

            hr = sgmiPin.GetAllocator(out pAllocator);
            if (hr != 0) return (hr);
            hr = pAllocator.Decommit();
            if (hr != 0) return (hr);
            AllocatorProperties requestedProperties = null;
            AllocatorProperties actualProperties = null;
            //pAllocator.GetProperties(out requestedProperties);
            //if (requestedProperties.cBuffers != m_currentAllocatorBuffers)
            //    requestedProperties.cBuffers = m_currentAllocatorBuffers;
            //hr = pAllocator.SetProperties(requestedProperties, out actualProperties);
            // ------

            _sampleGrabber.SetBufferSamples(false);
            _sampleGrabber.SetOneShot(false);

            ISampleGrabberCB custom_sgCB = null;

            custom_sgCB = (ISampleGrabberCB)this;
            
            if (hr != 0) return (hr);
            hr = _sampleGrabber.SetCallback(custom_sgCB, 0);
            //hr = _sampleGrabber.SetCallback(custom_sgCB, 1);
            return (hr);

        }
        public long DisableMemoryBuffer()
        {
            //CAutoLock cObjectLock(&m_CSec);

            //if (mb.Count > 0)
            //{
            //    for (int c = 0; c < mb.Count; c++)
            //    {
            //        //mb[c].media_sample.Release();
                    
            //    }
            //}


            //if(mb.size() > 0)
            //{
            //    std::map<unsigned long, MemoryBufferEntry>::iterator iter;
            //    for(iter = mb.begin();
            //        iter != mb.end();
            //        iter++)
            //    {
            //        // ignore use_count
            //        (*iter).second.media_sample->Release();
            //        std::map<unsigned long, MemoryBufferEntry>::iterator iter2 =
            //            mb.erase(iter);
            //        iter = iter2;
            //    }
            //}

            m_currentAllocatorBuffers = 0;
            return (_sampleGrabber.SetCallback(null, 0));

        }
        public int CheckoutMemoryBuffer(ref byte [] buffer, ref int Width, ref int Height, ref PIXELFORMAT PixelFormat, ref TimeSpan Timestamp)
        {
            //CAutoLock cObjectLock(&m_CSec);
            long hr;
            MemoryBufferEntry iter = null;

            if (mb.ContainsKey(sample_counter))
            {

                iter = mb[sample_counter];
                
                iter.use_count++;
                buffer = iter.ptr;

                //if (Width != 0)
                    Width = media_format.biWidth;
                //if (Height != 0)
                    Height = media_format.biHeight;
                //if (PixelFormat != 0)
                    PixelFormat = media_format.pixel_format;
                //if (Timestamp != null)
                    Timestamp = iter.timestamp;

                g_Handle.n = mb.Count;
                g_Handle.t = iter.timestamp.Ticks;
                return (0);

            }
            else
                return -1;

            
            


        }
        public int CheckinMemoryBuffer(bool ForceRelease = false)
        {
            MemoryBufferEntry iter = null;

            if (mb.ContainsKey(g_Handle.n))
            {
                iter = mb[g_Handle.n];
            }
            else
                return 0;


            if (ForceRelease) 
                iter.use_count = 0;
            else
            {
                if (iter.use_count > 0)
                    iter.use_count--;
            }

            return 0;
        }
        private long ShowFilterProperties(IntPtr hWnd)
        {
            return 0;
        }
        private long ShowPinProperties(IntPtr hWnd)
        {
            return 0;
        }
        private long BuildGraphFromXMLHandle(IntPtr xml_h)
        {
            return 0;
        }
        private long GetCameraParameterRange(CP_INTERFACE interface_type, long property, long pMin, long pMax, long pSteppingDelta, long pDefault, long pCapsFlags)
        {
            return 0;
        }
        private long GetCameraParameter(CP_INTERFACE interface_type, long Property, long lValue, bool bAuto)
        {
            return 0;
        }
        private long SetCameraParameter(CP_INTERFACE interface_type, long Property, long lValue, bool bAuto)
        {
            return 0;
        }
        private long GetCameraPropertyAUTOFlag(CP_INTERFACE interface_type, bool bAUTO = true)
        {
            return 0;
        }
        private long GetCameraParameter(CP_INTERFACE interface_type, long Property, long lValue, long Flags)
        {
            return 0;
        }
        private long SetCameraParameter(CP_INTERFACE interface_type, long Property, long lValue, long Flags)
        {
            return 0;
        }
        private long GetCameraParameterN(CP_INTERFACE interface_type, long Property, double dValue)
        {
            return 0;
        }
        private long SetCameraParameterN(CP_INTERFACE interface_type, long Property, double dValue) // dValue will be clamped to [0..1]
        {
            return 0;
        }
        private long SetCameraParameterToDefault(CP_INTERFACE interface_type, long Property, bool bAuto)
        {
            return 0;
        }
        private long ResetCameraParameters(bool bAuto) // reset all parameters to their defaults
        {
            return 0;
        }
        public long Run()
        {
            long hr;

            hr = _mediaControl.Run();

            if (hr != 0) return (hr);
            else return (0);

        }
        public long Pause()
        {
            long  hr;
            hr = _mediaControl.Pause();
            if (hr != 0) return (hr);
            else return (0);
 
        }
        public long Stop(bool forcedStop = false)
        {
            long hr;

            hr = (forcedStop ? _mediaControl.Stop() : _mediaControl.StopWhenReady());

            if (hr != 0)
                return (hr);
            else return (0);            
        }
        private void Lock() { }
        private void Unlock() { }
        private long getPin(IBaseFilter flt, PinDirection dir, int number, ref IPin pRetPin)
        {
            IPin[] pin = new IPin[1];
            IEnumPins pinsEnum = null;

            // enum filter pins
            if (flt.EnumPins(out pinsEnum) == 0)
            {
                //PinDirection pinDir;
                int n;
                PinInfo info;

                try
                {
                    pinsEnum.Reset();
                    pinsEnum.Next(1, pin, out n);
                    pin[0].QueryPinInfo(out info);

                    // query pin`s direction
                    //pin[0].QueryDirection(out pinDir);

                    // get next pin
                    while (pin != null)
                    {

                        if (info.Direction == dir)
                        {
                            //n++;
                            if (number == n)
                            {
                                pRetPin = pin[0];
                                return 0;
                            }
                            else
                            {
                                pinsEnum.Next(1, pin, out n);
                                if (n == 0)
                                {
                                    Marshal.ReleaseComObject(pin[0]);
                                    return -1;
                                }
                                pin[0].QueryPinInfo(out info);

                            }
                            number--;
                        }
                        else
                        {
                            pinsEnum.Next(1, pin, out n);
                            if (n == 0)
                            {
                                Marshal.ReleaseComObject(pin[0]);
                                return -1;
                            }
                            pin[0].QueryPinInfo(out info);
                        }

                        //Marshal.ReleaseComObject(pin[0]);
                        //pin[0] = null;

                    }
                }
                finally
                {
                    Marshal.ReleaseComObject(pinsEnum);
                }
            }
            return -1;
        }



        #endregion

        #region ISampleGrabberCB members

        public int SampleCB(double sampleTime, IMediaSample sample)
        {
            //return 0;

            try
            {
                if (mb.Count > 0)
                {
                    IList<long> keysToDelete = new List<long>();

                    foreach (var item in mb)
                    {
                        MemoryBufferEntry entry = (MemoryBufferEntry)item.Value;
                        if (entry.use_count == 0)
                        {
                            entry.media_sample = null;
                            keysToDelete.Add(item.Key);
                        }
                    }

                    for (int c = 0; c < keysToDelete.Count; c++)
                        mb.Remove(keysToDelete[c]);
                }
                

                long hr;
                long t_start, t_end;
                hr = sample.GetMediaTime(out t_start, out t_end);   // we just care about the start
                current_timestamp = t_start;
                sample_counter++;
                MemoryBufferEntry mb_entry = new MemoryBufferEntry();
                mb_entry.use_count = 0;
                mb_entry.timestamp = new TimeSpan(current_timestamp);

                mb_entry.media_sample = sample;
                //sample.AddRef();


                // flipping
                if (media_format.flipH || media_format.flipV)
                {
                    IntPtr bufferPtr = IntPtr.Zero;
                    hr = sample.GetPointer(out bufferPtr);

                    if (hr == 0)
                    {
                        int lenght = sample.GetActualDataLength();
                        mb_entry.ptr = FlipImageRGB32(bufferPtr, lenght, media_format.biWidth, media_format.biHeight, media_format.flipH, media_format.flipV);
                    }
                }

                mb.Add(sample_counter, mb_entry);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                }
            }

            Marshal.ReleaseComObject(sample);

            // --------
            //SetEvent(m_ESync);

            return (0);















            ////int hr;
            //IntPtr buffer;
            //AMMediaType mediaType;
            //VideoInfoHeader videoInfo;
            //int frameWidth;
            //int frameHeight;
            //int stride;
            //int bufferLength;


            //hr = sample.GetPointer(out buffer);


            //hr = sample.GetMediaType(out mediaType);


            //bufferLength = sample.GetSize();

            //try
            //{
            //    videoInfo = new VideoInfoHeader();
            //    Marshal.PtrToStructure(mediaType.FormatPtr, videoInfo);

            //    frameWidth = videoInfo.BmiHeader.Width;
            //    frameHeight = videoInfo.BmiHeader.Height;
            //    stride = frameWidth * (videoInfo.BmiHeader.BitCount / 8);

            //    //CopyMemory(imageBuffer, buffer, bufferLength);

            //    //Bitmap bitmapOfFrame = new Bitmap(frameWidth, frameHeight, stride, PixelFormat.Format24bppRgb, buffer);
            //    //bitmapOfFrame.Save("C:\\Users\\...\\...\\...\\....jpg");


            //}
            //catch (Exception ex)
            //{
            //    //Console.WriteLine(ex.ToString());
            //}

            return 0;
        }     
        public int BufferCB(double sampleTime, IntPtr buffer, int bufferLen)
        {
            return 0;

            //if (bufferLen == 0)
            //    return 0;

            
            //if (mb.Count > 0)
            //{
            //    IList<long> keysToDelete = new List<long>();

            //    foreach (var item in mb)
            //    {
            //        MemoryBufferEntry entry = (MemoryBufferEntry)item.Value;
            //        if (entry.use_count == 0)
            //        {
            //            entry.media_sample = null;
            //            keysToDelete.Add(item.Key);
            //        }
            //    }

            //    for (int c = 0; c < keysToDelete.Count; c++)
            //        mb.Remove(keysToDelete[c]);
            //}

            //sample_counter++;

            //MemoryBufferEntry mb_entry = new MemoryBufferEntry();
            //mb_entry.use_count = 0;
            //mb_entry.timestamp = new TimeSpan(current_timestamp);

            

            //// flipping
            //if (media_format.flipH || media_format.flipV)
            //{
            //    mb_entry.ptr = FlipImageRGB32(buffer, bufferLen, media_format.biWidth, media_format.biHeight, media_format.flipH, media_format.flipV);
            //}

            //mb.Add(sample_counter, mb_entry);
            


            //return 0;
        }

        #endregion
    }
}
