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
using HL2UnityPlugin;
using Windows.Graphics.Imaging;
using Windows.Perception.Spatial;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
#endif

public class CamTest : MonoBehaviour
{
#if ENABLE_WINMD_SUPPORT
    HL2ResearchMode researchMode;
    OpenCVRuntimeComponent.CvUtils CvUtils;
#endif

    public ArUcoUtils.ArUcoDictionaryName ArUcoDictionaryName = ArUcoUtils.ArUcoDictionaryName.DICT_6X6_50;
    public ArUcoUtils.ArUcoTrackingType ArUcoTrackingType = ArUcoUtils.ArUcoTrackingType.Markers;

    public ArUcoUtils.CameraCalibrationParameterType CalibrationParameterType = ArUcoUtils.CameraCalibrationParameterType.UserDefined;

    public UserDefinedCameraCalibrationParams UserDefinedCalibParamsLeft;
    public UserDefinedCameraCalibrationParams UserDefinedCalibParamsRight;

    public ArUcoBoardPositions ArUcoBoardPositions;

    public GameObject LFPreviewPlane = null;
    private Material LFMediaMaterial = null;
    private Texture2D LFMediaTexture = null;
    private byte[] LFFrameData = null;

    public GameObject RFPreviewPlane = null;
    private Material RFMediaMaterial = null;
    private Texture2D RFMediaTexture = null;
    private byte[] RFFrameData = null;

    public UnityEngine.UI.Text text;
    public UnityEngine.UI.Text TextLeft;
    public Canvas canvas;

    public Camera MainCamera;
    public int numOfTrackedObj;

    private GameObject leftBar;
    private GameObject rightBar;
    private GameObject[] markers;

    //private Matrix4x4 K_left = new Matrix4x4();

    // Start is called before the first frame update
    void Start()
    {
        /*K_left.SetRow(0, new Vector4(387.07f, 0.0f, 0.0f, 0.0f));
        K_left.SetRow(1, new Vector4(0.0f, 385.18f, 0.0f, 0.0f));
        K_left.SetRow(2, new Vector4(240.14f, 317.46f, 0.0f, 0.0f));
        K_left.SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 0.0f));*/
        canvas.transform.SetParent(MainCamera.transform);

        if (LFPreviewPlane != null)
        {
            LFMediaMaterial = LFPreviewPlane.GetComponent<MeshRenderer>().material;
            LFMediaTexture = new Texture2D(640, 480, TextureFormat.Alpha8, false);
            LFMediaMaterial.mainTexture = LFMediaTexture;
            LFPreviewPlane.transform.SetParent(MainCamera.transform);
        }

        if (RFPreviewPlane != null)
        {
            RFMediaMaterial = RFPreviewPlane.GetComponent<MeshRenderer>().material;
            RFMediaTexture = new Texture2D(640, 480, TextureFormat.Alpha8, false);
            RFMediaMaterial.mainTexture = RFMediaTexture;
            RFPreviewPlane.transform.SetParent(MainCamera.transform);
        }

        //Create left and right slide bars
        leftBar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        leftBar.name = "Left Bar";
        leftBar.transform.SetParent(MainCamera.transform);
        leftBar.transform.localScale = new Vector3(0.01f, 0.1f, 0.01f);
        leftBar.GetComponent<MeshRenderer>().material.color = Color.blue;
        leftBar.transform.localPosition = new Vector3(-0.15f, 0f, 0.65f);
        leftBar.GetComponent<Renderer>().enabled = true;

        rightBar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rightBar.name = "Right Bar";
        rightBar.transform.SetParent(MainCamera.transform);
        rightBar.transform.localScale = new Vector3(0.01f, 0.1f, 0.01f);
        rightBar.GetComponent<MeshRenderer>().material.color = Color.blue;
        rightBar.transform.localPosition = new Vector3(0.1f, 0f, 0.65f);
        leftBar.GetComponent<Renderer>().enabled = true;

        markers = new GameObject[numOfTrackedObj];
        for (int i = 0; i < numOfTrackedObj; i++)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = "tracked object " + i;
            marker.transform.SetParent(MainCamera.transform);
            marker.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            marker.GetComponent<MeshRenderer>().material.color = Color.red;
            markers[i] = marker;
        }


#if ENABLE_WINMD_SUPPORT
    try
    {
        researchMode = new HL2ResearchMode();
        text.text = "start initialize cams";
        researchMode.InitializeSpatialCamerasFront();
        researchMode.StartSpatialCamerasFrontLoop();
        
        text.text = "Initialization finished";
        Debug.Log("Initialization finished");
    }
    catch (System.Exception e) 
    {
        text.text = "failed to initialize cams";
    }

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
        text.text = "cvutil finished";
        Debug.Log("cvutil finished");
    }
    catch (System.Exception e) 
    {
        text.text = "cvutil failed";
    }

    
#endif
    }

    // Update is called once per frame
    void LateUpdate()
    {
#if ENABLE_WINMD_SUPPORT
        if (LFPreviewPlane != null && researchMode.LFImageUpdated())
        {
            long ts;
            byte[] frameTexture = researchMode.GetLFCameraBuffer(out ts);
            if (frameTexture.Length > 0)
            {
                if (LFFrameData == null)
                {
                    LFFrameData = frameTexture;
                }
                else
                {
                    System.Buffer.BlockCopy(frameTexture, 0, LFFrameData, 0, LFFrameData.Length);
                }

                LFMediaTexture.LoadRawTextureData(LFFrameData);
                LFMediaTexture.Apply();
                IBuffer buffer = LFFrameData.AsBuffer();
                Debug.Log($"buffer length: {buffer.Length}");
                //text.text = "Build bit map";
                //SoftwareBitmap bitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, 640, 480);
                SoftwareBitmap bitmap_gray = SoftwareBitmap.CreateCopyFromBuffer(buffer, BitmapPixelFormat.Gray8, 640, 480);
                bitmap_gray.CopyFromBuffer(buffer);
                SoftwareBitmap bitmap = SoftwareBitmap.Convert(bitmap_gray, BitmapPixelFormat.Bgra8);
                Debug.Log("Start handle track");
                HandleArUcoTracking(bitmap, true);
                //text.text = "end handle track";
            }
        }

        if (RFPreviewPlane != null && researchMode.RFImageUpdated())
        {
            long ts;
            byte[] frameTexture = researchMode.GetRFCameraBuffer(out ts);
            if (frameTexture.Length > 0)
            {
                if (RFFrameData == null)
                {
                    RFFrameData = frameTexture;
                }
                else
                {
                    System.Buffer.BlockCopy(frameTexture, 0, RFFrameData, 0, RFFrameData.Length);
                }

                RFMediaTexture.LoadRawTextureData(RFFrameData);
                RFMediaTexture.Apply();
                IBuffer buffer = RFFrameData.AsBuffer();
                Debug.Log($"buffer length: {buffer.Length}");
                //text.text = "Build bit map";
                //SoftwareBitmap bitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, 640, 480);
                SoftwareBitmap bitmap_gray = SoftwareBitmap.CreateCopyFromBuffer(buffer, BitmapPixelFormat.Gray8, 640, 480);
                bitmap_gray.CopyFromBuffer(buffer);
                SoftwareBitmap bitmap = SoftwareBitmap.Convert(bitmap_gray, BitmapPixelFormat.Bgra8);
                Debug.Log("Start handle track");
                HandleArUcoTracking(bitmap, false);
                //text.text = "end handle track";
            }
        }
#endif
    }

#if ENABLE_WINMD_SUPPORT
    private void HandleArUcoTracking(SoftwareBitmap bitmap, bool is_left)
    {
        OpenCVRuntimeComponent.CameraCalibrationParams calibParams = 
              new OpenCVRuntimeComponent.CameraCalibrationParams(System.Numerics.Vector2.Zero, System.Numerics.Vector2.Zero, System.Numerics.Vector3.Zero, System.Numerics.Vector2.Zero, 0, 0);
        if (bitmap != null)
        {
            //text.text = "bitmap is not null";
            //System.Threading.Thread.Sleep(1000);
             switch (CalibrationParameterType)
            {
                // Cache from user-defined parameters 
                case ArUcoUtils.CameraCalibrationParameterType.UserDefined:
                    //text.text = "user defined param found";
                    Debug.Log("user defined param found");
                    //System.Threading.Thread.Sleep(1000);
                    if (is_left)
                    {
                        calibParams = new OpenCVRuntimeComponent.CameraCalibrationParams(
                            new System.Numerics.Vector2(UserDefinedCalibParamsLeft.focalLength.x, UserDefinedCalibParamsLeft.focalLength.y), // Focal length
                            new System.Numerics.Vector2(UserDefinedCalibParamsLeft.principalPoint.x, UserDefinedCalibParamsLeft.principalPoint.y), // Principal point
                            new System.Numerics.Vector3(UserDefinedCalibParamsLeft.radialDistortion.x, UserDefinedCalibParamsLeft.radialDistortion.y, UserDefinedCalibParamsLeft.radialDistortion.z), // Radial distortion
                            new System.Numerics.Vector2(UserDefinedCalibParamsLeft.tangentialDistortion.x, UserDefinedCalibParamsLeft.tangentialDistortion.y), // Tangential distortion
                            UserDefinedCalibParamsLeft.imageWidth, // Image width
                            UserDefinedCalibParamsLeft.imageHeight); // Image height
                            Debug.Log($"User-defined calibParams: [{calibParams}]");
                    }
                    else
                    {
                        calibParams = new OpenCVRuntimeComponent.CameraCalibrationParams(
                            new System.Numerics.Vector2(UserDefinedCalibParamsRight.focalLength.x, UserDefinedCalibParamsRight.focalLength.y), // Focal length
                            new System.Numerics.Vector2(UserDefinedCalibParamsRight.principalPoint.x, UserDefinedCalibParamsRight.principalPoint.y), // Principal point
                            new System.Numerics.Vector3(UserDefinedCalibParamsRight.radialDistortion.x, UserDefinedCalibParamsRight.radialDistortion.y, UserDefinedCalibParamsRight.radialDistortion.z), // Radial distortion
                            new System.Numerics.Vector2(UserDefinedCalibParamsRight.tangentialDistortion.x, UserDefinedCalibParamsRight.tangentialDistortion.y), // Tangential distortion
                            UserDefinedCalibParamsRight.imageWidth, // Image width
                            UserDefinedCalibParamsRight.imageHeight); // Image height
                            Debug.Log($"User-defined calibParams: [{calibParams}]");
                    }
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
                    //System.Threading.Thread.Sleep(1000);
                    Debug.Log("start detect marker");
                    DetectMarkers(bitmap, calibParams, is_left);
                    break;

                case ArUcoUtils.ArUcoTrackingType.None:
                    //text.text = $"Not running tracking...";
                    break;

                default:
                    //text.text = $"No option selected for tracking...";
                    break;
            }
        }
        bitmap?.Dispose();
    }

    private void DetectMarkers(SoftwareBitmap softwareBitmap, OpenCVRuntimeComponent.CameraCalibrationParams calibParams, bool is_left)
    {
        // Get marker detections from opencv component
        var detected_markers = CvUtils.DetectMarkers(softwareBitmap, calibParams);
        Vector3 pos = new Vector3(0.0f, 0.0f, 0.0f);

        if (is_left)
        {
            foreach (var marker in markers)
            {
                marker.GetComponent<Renderer>().enabled = false;
            }
            foreach (var det_marker in detected_markers)
            {
                int id = det_marker.Id;
                pos = ArUcoUtils.Vec3FromFloat3(det_marker.Position);
                float portion = ((pos.x + 0.35f) / 0.85f) * 2 - 1;

                markers[id].transform.localPosition = new Vector3(leftBar.transform.localPosition.x, leftBar.transform.localPosition.y + portion * leftBar.transform.localScale.y, leftBar.transform.localPosition.z);
                //markers[id].transform.localPosition = findScaledPosYOfTrackedObj(pos, is_left);
                markers[id].GetComponent<Renderer>().enabled = true;
            }
            TextLeft.text = $"Detected: {detected_markers.Count} markers.";// Pos : {pos.x}, {pos.y}, {pos.z}";
        }
        else
        {
            foreach (var det_marker in detected_markers)
            {
                int id = det_marker.Id;
                pos = ArUcoUtils.Vec3FromFloat3(det_marker.Position);
                float portion = (1 - ((pos.x + 0.2f) / 1.0f)) * 2 - 1;

                markers[id].transform.localPosition = new Vector3(rightBar.transform.localPosition.x, rightBar.transform.localPosition.y + portion * rightBar.transform.localScale.y, rightBar.transform.localPosition.z);
                //markers[id].transform.localPosition = findScaledPosYOfTrackedObj(pos, is_left);
                markers[id].GetComponent<Renderer>().enabled = true;
            }
            text.text = $"Detected: {detected_markers.Count} markers."; //Pos : {pos.x}, {pos.y}, {pos.z}";
        }
        
    }
#endif

    float findScaledPosYOfTrackedObj(Vector3 trackingPos, bool isLeft)
    {
        Vector3 result;
        //Find pixel size of left camera screen
        float screenHeight = 640; //leftCam.GetComponent<Camera>().pixelHeight; //832 pixels

        //Find pixel size of right camera screen
        float screenHeightR = 640;  //rightCam.GetComponent<Camera>().pixelHeight; //832 pixels

        //Find pixel size of bars - both are equal in size
        Vector3 posStart = MainCamera.WorldToScreenPoint(leftBar.GetComponent<Renderer>().bounds.min);
        Vector3 posEnd = MainCamera.WorldToScreenPoint(leftBar.GetComponent<Renderer>().bounds.max);
        int barHeight = (int)(posEnd.y - posStart.y);
        Vector3 left_interval = leftBar.GetComponent<Renderer>().bounds.max - leftBar.GetComponent<Renderer>().bounds.min;
        Vector3 right_interval = rightBar.GetComponent<Renderer>().bounds.max - rightBar.GetComponent<Renderer>().bounds.min;
        //Debug.Log("Quadspace: (" + barHeight + ")");

        //screen location of the quads:
        Vector3 leftBarScreenPos = MainCamera.WorldToScreenPoint(leftBar.transform.position);
        Debug.Log("quad screen coordinates (main cam): " + leftBarScreenPos);
        Vector3 rightBarScreenPos = MainCamera.WorldToScreenPoint(rightBar.transform.position);

        if (isLeft == true)
        {
            //Find position of object on camera screen view (2D)
            //float y = 387.07f * trackingPos.x / (240.14f * trackingPos.x + 317.46f * trackingPos.y + trackingPos.z); 
            //Vector3 marker1ScreenPos = leftCam.GetComponent<Camera>().WorldToScreenPoint(trackingPos);
            //Debug.Log("target screen coordinates (left cam): " + marker1ScreenPos);

            //Convert object's position to its position relative to the quad
            float posY = ((trackingPos.x + 0.5f) / 1.5f) * barHeight;

            //Find actual position of object on the quad
            result = MainCamera.ScreenToWorldPoint(new Vector3(leftBarScreenPos.x, leftBarScreenPos.y - (barHeight / 2f) + posY, leftBarScreenPos.z));
            //result = leftBar.GetComponent<Renderer>().bounds.min + portion * left_interval;
            return result.y - left_interval.y / 2;
        }

        else
        {
            float posY = (1 - (trackingPos.x + 0.5f) / 1.5f) * barHeight;

            //Find actual position of object on the quad
            result = MainCamera.ScreenToWorldPoint(new Vector3(rightBarScreenPos.x, rightBarScreenPos.y - (barHeight / 2f) + posY, rightBarScreenPos.z));
            //result = leftBar.GetComponent<Renderer>().bounds.min + portion * left_interval;
            return result.y - right_interval.y / 2;
        }
    }
}
