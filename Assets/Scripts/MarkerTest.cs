using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Input;

#if ENABLE_WINMD_SUPPORT
using Windows.Graphics.Imaging;
using Windows.Perception.Spatial;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
#endif

public class MarkerTest : MonoBehaviour
{
#if ENABLE_WINMD_SUPPORT
    OpenCVRuntimeComponent.CvUtils CvUtils;
#endif

    public ArUcoUtils.ArUcoDictionaryName ArUcoDictionaryName = ArUcoUtils.ArUcoDictionaryName.DICT_6X6_50;
    public ArUcoUtils.ArUcoTrackingType ArUcoTrackingType = ArUcoUtils.ArUcoTrackingType.Markers;

    public ArUcoUtils.CameraCalibrationParameterType CalibrationParameterType = ArUcoUtils.CameraCalibrationParameterType.UserDefined;

    public UserDefinedCameraCalibrationParams UserDefinedCalibParams;

    public ArUcoBoardPositions ArUcoBoardPositions;

    public UnityEngine.UI.Text text;
    // Start is called before the first frame update
    void Start()
    {
#if ENABLE_WINMD_SUPPORT
    try
    {
        CvUtils = new OpenCVRuntimeComponent.CvUtils(
                    ArUcoBoardPositions.ComputeMarkerSizeForTrackingType(
                        ArUcoTrackingType, 
                        ArUcoBoardPositions.markerSizeForSingle,
                        ArUcoBoardPositions.markerSizeForBoard),
                    ArUcoBoardPositions.numMarkers,
                    (int)ArUcoDictionaryName,
                    ArUcoBoardPositions.FillCustomObjectPointsFromUnity());
        text.text = "Initialization done";
    }
    catch (System.Exception e) 
    {
        text.text = "failed to initialize cvutil";
    }
    
#endif     
    }

    // Update is called once per frame
    void Update()
    {
#if ENABLE_WINMD_SUPPORT
    byte[] pvrtcBytes = new byte[]
        {
            0x30, 0x32, 0x32, 0x32, 0xe7, 0x30, 0xaa, 0x7f, 0x32, 0x32, 0x32, 0x32, 0xf9, 0x40, 0xbc, 0x7f,
            0x03, 0x03, 0x03, 0x03, 0xf6, 0x30, 0x02, 0x05, 0x03, 0x03, 0x03, 0x03, 0xf4, 0x30, 0x03, 0x06,
            0x32, 0x32, 0x32, 0x32, 0xf7, 0x40, 0xaa, 0x7f, 0x32, 0xf2, 0x02, 0xa8, 0xe7, 0x30, 0xff, 0xff,
            0x03, 0x03, 0x03, 0xff, 0xe6, 0x40, 0x00, 0x0f, 0x00, 0xff, 0x00, 0xaa, 0xe9, 0x40, 0x9f, 0xff,
            0x5b, 0x03, 0x03, 0x03, 0xca, 0x6a, 0x0f, 0x30, 0x03, 0x03, 0x03, 0xff, 0xca, 0x68, 0x0f, 0x30,
            0xaa, 0x94, 0x90, 0x40, 0xba, 0x5b, 0xaf, 0x68, 0x40, 0x00, 0x00, 0xff, 0xca, 0x58, 0x0f, 0x20,
            0x00, 0x00, 0x00, 0xff, 0xe6, 0x40, 0x01, 0x2c, 0x00, 0xff, 0x00, 0xaa, 0xdb, 0x41, 0xff, 0xff,
            0x00, 0x00, 0x00, 0xff, 0xe8, 0x40, 0x01, 0x1c, 0x00, 0xff, 0x00, 0xaa, 0xbb, 0x40, 0xff, 0xff,
            0x30, 0x32, 0x32, 0x32, 0xe7, 0x30, 0xaa, 0x7f, 0x32, 0x32, 0x32, 0x32, 0xf9, 0x40, 0xbc, 0x7f,
            0x03, 0x03, 0x03, 0x03, 0xf6, 0x30, 0x02, 0x05, 0x03, 0x03, 0x03, 0x03, 0xf4, 0x30, 0x03, 0x06,
            0x32, 0x32, 0x32, 0x32, 0xf7, 0x40, 0xaa, 0x7f, 0x32, 0xf2, 0x02, 0xa8, 0xe7, 0x30, 0xff, 0xff,
            0x03, 0x03, 0x03, 0xff, 0xe6, 0x40, 0x00, 0x0f, 0x00, 0xff, 0x00, 0xaa, 0xe9, 0x40, 0x9f, 0xff,
            0x5b, 0x03, 0x03, 0x03, 0xca, 0x6a, 0x0f, 0x30, 0x03, 0x03, 0x03, 0xff, 0xca, 0x68, 0x0f, 0x30,
            0xaa, 0x94, 0x90, 0x40, 0xba, 0x5b, 0xaf, 0x68, 0x40, 0x00, 0x00, 0xff, 0xca, 0x58, 0x0f, 0x20,
            0x00, 0x00, 0x00, 0xff, 0xe6, 0x40, 0x01, 0x2c, 0x00, 0xff, 0x00, 0xaa, 0xdb, 0x41, 0xff, 0xff,
            0x00, 0x00, 0x00, 0xff, 0xe8, 0x40, 0x01, 0x1c, 0x00, 0xff, 0x00, 0xaa, 0xbb, 0x40, 0xff, 0xff,
        };
    IBuffer buffer = pvrtcBytes.AsBuffer();
    Debug.Log($"buffer length: {buffer.Length}");
    //text.text = $"buffer length: {buffer.Length}";
    SoftwareBitmap bitmap = new SoftwareBitmap(BitmapPixelFormat.Gray8, 16, 16);
    bitmap.CopyFromBuffer(buffer);
    HandleArUcoTracking(bitmap);
    
#endif
    }

#if ENABLE_WINMD_SUPPORT
    private void HandleArUcoTracking(SoftwareBitmap bitmap)
    {
         OpenCVRuntimeComponent.CameraCalibrationParams calibParams = 
                new OpenCVRuntimeComponent.CameraCalibrationParams(System.Numerics.Vector2.Zero, System.Numerics.Vector2.Zero, System.Numerics.Vector3.Zero, System.Numerics.Vector2.Zero, 0, 0);
        //text.text = "OpenCV object created";
        if (bitmap != null)
        {
            //text.text = "bitmap is not null";
            switch (CalibrationParameterType)
            {
                // Cache from user-defined parameters 
                case ArUcoUtils.CameraCalibrationParameterType.UserDefined:
                    //text.text = "user defined param found";
                    Debug.Log("user defined param found");
                    calibParams = new OpenCVRuntimeComponent.CameraCalibrationParams(
                        new System.Numerics.Vector2(UserDefinedCalibParams.focalLength.x, UserDefinedCalibParams.focalLength.y), // Focal length
                        new System.Numerics.Vector2(UserDefinedCalibParams.principalPoint.x, UserDefinedCalibParams.principalPoint.y), // Principal point
                        new System.Numerics.Vector3(UserDefinedCalibParams.radialDistortion.x, UserDefinedCalibParams.radialDistortion.y, UserDefinedCalibParams.radialDistortion.z), // Radial distortion
                        new System.Numerics.Vector2(UserDefinedCalibParams.tangentialDistortion.x, UserDefinedCalibParams.tangentialDistortion.y), // Tangential distortion
                        UserDefinedCalibParams.imageWidth, // Image width
                        UserDefinedCalibParams.imageHeight); // Image height
                        Debug.Log($"User-defined calibParams: [{calibParams}]");
                    break;
                default:
                    //text.text = "user defined param not found";
                    Debug.Log("user defined param not found");
                    break;
            }

            switch (ArUcoTrackingType)
            {
                case ArUcoUtils.ArUcoTrackingType.Markers:
                    //text.text = "start detect marker";
                    System.Threading.Thread.Sleep(1000);
                    Debug.Log("start detect marker");
                    DetectMarkers(bitmap, calibParams);
                    break;

                /*case ArUcoUtils.ArUcoTrackingType.CustomBoard:
                    DetectBoard(bitmap, calibParams);
                    break;*/

                case ArUcoUtils.ArUcoTrackingType.None:
                    text.text = $"Not running tracking...";
                    break;

                default:
                    text.text = $"No option selected for tracking...";
                    break;
            }
        }
        bitmap?.Dispose();
    }

    private void DetectMarkers(SoftwareBitmap softwareBitmap, OpenCVRuntimeComponent.CameraCalibrationParams calibParams)
    {
        // Get marker detections from opencv component
        var markers = CvUtils.DetectMarkers(softwareBitmap, calibParams);

        text.text = $"Detected: {markers.Count} markers";
    }
#endif
}
