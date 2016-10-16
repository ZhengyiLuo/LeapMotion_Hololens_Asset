using UnityEngine;
using System.Collections.Generic;
using System;
using MiniJSON;
using Leap;
//using Windows.Networking.Sockets;
//using Windows.Storage.Streams;
//using System.Threading.Tasks;


public class LeapWebProcessor : MonoBehaviour
{
    const string WEBSOCKET_URI = "ws://localhost:6437/v6.json";
    const string WEBSOCKET_URI_REMOTE = "ws://158.130.108.10:6437/v6.json";
    const string GET_FOCUS = "{\"focused\" : \"true\"}";
    const string LOST_FOCUS = "{\"focused\" : \"false\"}";
    const string BACKGROUND_ON = "{\"background\" : \"true\"";
    const string BACKGROUND_OFF = "{\"background\" : \"false\"";
    const string GESTURES_ON = "{\"enableGestures\" : \"true\"";
    const string GESTURES_OFF = "{\"enableGestures\" : \"false\"";
    const string HMD_ON = "{\"optimizeHMD\" : \"true\"";
    const string HMD_OFF = "{\"optimizeHMD\" : \"false\"";

    public String res;

    public Dictionary<System.String, System.Object> currentFrame;

    //public MessageWebSocket w;

    public float rotation;

    public float width;

    public Frame frame;

    public long timestamp;

    public string flag = BACKGROUND_ON;

    public bool IsConnected { get; internal set; }

    public GameObject LeapHandController;

    public bool hasHand;


    void Start()
    {
        IsConnected = false;
        //webSocketSetup();
        LeapHandController = GetComponent<GameObject>();
    }


    //async void webSocketSetup()
    //{
    //    Debug.Log("Setting up websocket");
    //    w = new MessageWebSocket();

    //    //In this case we will be sending/receiving a string so we need to set the MessageType to Utf8.
    //    w.Control.MessageType = SocketMessageType.Utf8;

    //    //Add the MessageReceived event handler.
    //    w.MessageReceived += WebSock_MessageReceived;

    //    //Add the Closed event handler.
    //    w.Closed += WebSock_Closed;

    //    Uri serverUri = new Uri(WEBSOCKET_URI_REMOTE);

    //    try
    //    {
    //        //Connect to the server.
    //        Debug.Log("Before Connect to Server");
    //        await w.ConnectAsync(serverUri);
    //        Debug.Log("Connect to Server");

    //        //Send a message to the server.
    //        await WebSock_SendMessage(w, flag);
    //        await WebSock_SendMessage(w, GET_FOCUS);
    //    }
    //    catch (Exception ex)
    //    {
    //        //Add code here to handle any exceptions
    //        Debug.Log(ex.StackTrace);
    //    }



    //}

    ////Send a message to the server.
    //private async Task WebSock_SendMessage(MessageWebSocket webSock, string message)
    //{
    //    DataWriter messageWriter = new DataWriter(webSock.OutputStream);
    //    messageWriter.WriteString(message);
    //    await messageWriter.StoreAsync();
    //}

    //private void WebSock_Closed(IWebSocket sender, WebSocketClosedEventArgs args)
    //{
    //    IsConnected = true;
    //    LeapHandController.SetActive(false);
    //}


    ////The MessageReceived event handler.
    //private void WebSock_MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
    //{
    //    DataReader messageReader = args.GetDataReader();
    //    res = messageReader.ReadString(messageReader.UnconsumedBufferLength);
    //    Dictionary<string, object> frameData = (Dictionary<string, object>)Json.Deserialize(res);
    //    Debug.Log(frameData);
    //    if (frameData.ContainsKey("id"))
    //    {

    //        frame = createFrame(frameData);
    //        if (IsConnected != true)
    //        {
    //            LeapHandController.SetActive(true);
    //            IsConnected = true;
    //        }

    //    }
    //}

    private Frame createFrame(Dictionary<string, object> frameData)
    {
        long id = (long)frameData["id"];

        //timestamp
        long timestamp = (long)frameData["timestamp"];

        //FrameRate
        Double fps = (Double)frameData["currentFrameRate"];

        //Interaction Box Object
        Dictionary<String, object> box = (Dictionary<String, object>)frameData["interactionBox"];
        List<object> centerList = (List<object>)box["center"];
        List<object> sizeList = (List<object>)box["size"];

        Vector center = creatVector(centerList);
        Vector size = creatVector(sizeList);

        InteractionBox interactionBox = new InteractionBox(center, size);

        //HandObject
        List<object> handsobject = (List<object>)frameData["hands"];
        List<Hand> hands = new List<Hand>();

        List<object> pointables = (List<object>)frameData["pointables"];
        foreach (var item in handsobject)
        {
            //Debug.Log(item);
            hands.Add(creatHand(id, item, pointables));
        }
        return new Frame(id, timestamp, (float)fps, interactionBox, hands);

    }

    private Vector creatVector(List<object> List)
    {
        //Debug.Log(List.Count);
        double x = (double)List[0];
        double y = (double)List[1];
        double z = (double)List[2];
        return new Vector((float)x, (float)y, (float)z);
    }

    private Vector4 creatVector4(List<object> List)
    {
        double x = (double)List[0];
        double y = (double)List[1];
        double z = (double)List[2];
        double w = (double)List[3];
        return new Vector4((float)x, (float)y, (float)z, (float)w);
    }

    private Hand creatHand(long frameId, object hand, object pointables_object)
    {
        bool isLeft = false;
        Dictionary<string, object> handsobject = (Dictionary<string, object>)hand;
        List<object> pointables_list = (List<object>)pointables_object;
        List<object> armbasislist = (List<object>)handsobject["armBasis"];
        Vector arm_1 = creatVector((List<object>)armbasislist[0]);
        Vector arm_2 = creatVector((List<object>)armbasislist[1]);
        Vector arm_3 = creatVector((List<object>)armbasislist[2]);

        //arm_1 = normalizeVector(arm_1);
        //arm_1 = normalizeVector(arm_2);
        //arm_1 = normalizeVector(arm_3);

        double armWidth = (double)handsobject["armWidth"];

        double confidence = (double)handsobject["confidence"];

        List<object> direction_list = (List<object>)handsobject["direction"];
        Vector direction = creatVector((List<object>)direction_list);

        List<object> elbow_list = (List<object>)handsobject["elbow"];
        Vector elbow = creatVector((List<object>)elbow_list);

        double grabStrength = (double)handsobject["grabStrength"];

        long Handid = (long)handsobject["id"];

        List<object> palmNormal_list = (List<object>)handsobject["palmNormal"];
        Vector palmNormal = creatVector((List<object>)palmNormal_list);

        List<object> palmPosition_list = (List<object>)handsobject["palmPosition"];
        Vector palmPosition = creatVector((List<object>)palmPosition_list);

        List<object> palmVelocity_list = (List<object>)handsobject["palmVelocity"];
        Vector palmVelocity = creatVector((List<object>)palmVelocity_list);

        double pinchStrength = (double)handsobject["pinchStrength"];

        List<object> r_list = (List<object>)handsobject["r"];

        Matrix4x4 rotationMatrix = new Matrix4x4();
        List<List<object>> r = new List<List<object>>();
        int i = 0;
        foreach (var item in r_list)
        {
            List<object> current = new List<object>();
            foreach (var array in current)
            {
                rotationMatrix.SetRow(i, creatVector4((List<object>)array));
            }
        }

        double s = (double)handsobject["s"];

        List<object> sphereCenter_list = (List<object>)handsobject["sphereCenter"];
        Vector sphereCenter = creatVector((List<object>)sphereCenter_list);

        double sphereRadius = (double)handsobject["sphereRadius"];

        List<object> stabilizedPalmPosition_list = (List<object>)handsobject["stabilizedPalmPosition"];
        Vector stabilizedPalmPosition = creatVector((List<object>)stabilizedPalmPosition_list);

        List<object> t_list = (List<object>)handsobject["t"];
        Vector t = creatVector((List<object>)t_list);

        double timeVisible = (double)handsobject["timeVisible"];

        string type = (string)handsobject["type"];
        if (type.Equals("left"))
        {
            isLeft = true;
        }
        else
        {
            isLeft = false;
        }

        List<object> wrist_list = (List<object>)handsobject["wrist"];
        Vector wrist = creatVector((List<object>)wrist_list);
        Vector midpoint = new Vector((elbow.x + wrist.x) / 2, (elbow.y + wrist.y) / 2, (elbow.z + wrist.z) / 2);

        //Debug.Log("Handdddddd");
        //Debug.Log(armWidth);


        LeapQuaternion basis = createQuaternion(arm_1, arm_2, arm_3);
        //LeapQuaternion basis = createQuaternion(arm_1, arm_2, arm_3);

        Arm arm = new Arm(elbow, wrist, midpoint, direction, 245, (float)armWidth, basis);
        // public Hand(long frameID, int id, float confidence, float grabStrength, float grabAngle, float pinchStrength, float pinchDistance, float palmWidth, bool isLeft, float timeVisible, Arm arm, List<Finger> fingers, Vector palmPosition, Vector stabilizedPalmPosition, Vector palmVelocity, Vector palmNormal, Vector direction, Vector wristPosition);

        List<Finger> fingerlist = createFingerlist(Handid, frameId, pointables_list);
        //Debug.Log(fingerlist.Count);
        Hand result = new Hand(frameId, (int)Handid, (float)confidence, (float)grabStrength, 0, (float)pinchStrength, 0, (float)s, isLeft, (float)timeVisible, arm, fingerlist, palmPosition, stabilizedPalmPosition, palmVelocity, palmNormal, direction, wrist);

        return result;
    }

    private Vector normalizeVector(Vector v)
    {
        float scale = (float)Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
        return new Vector(v.x / scale, v.y / scale, v.z / scale);
    }

    private List<Finger> createFingerlist(long Handid, long frameId, List<object> pointables_list)
    {
        List<Finger> result = new List<Finger>();
        foreach (var item in pointables_list)
        {
            Dictionary<string, object> pointables = (Dictionary<string, object>)item;

            List<object> btipPosition_list = (List<object>)pointables["btipPosition"];
            Vector btipPosition = creatVector((List<object>)btipPosition_list);

            List<object> carpPosition_list = (List<object>)pointables["carpPosition"];
            Vector carpPosition = creatVector((List<object>)carpPosition_list);

            List<object> dipPosition_list = (List<object>)pointables["dipPosition"];
            Vector dipPosition = creatVector((List<object>)dipPosition_list);

            List<object> direction_list = (List<object>)pointables["direction"];
            Vector direction = creatVector((List<object>)direction_list);

            bool extended = (bool)pointables["extended"];

            long handId = (long)pointables["handId"];

            long id = (long)pointables["id"];

            double length = (double)pointables["length"];
            //Debug.Log(length);

            List<object> mcpPosition_list = (List<object>)pointables["mcpPosition"];
            Vector mcpPosition = creatVector((List<object>)mcpPosition_list);

            List<object> pipPosition_list = (List<object>)pointables["pipPosition"];
            Vector pipPosition = creatVector((List<object>)pipPosition_list);

            List<object> stabilizedTipPosition_list = (List<object>)pointables["stabilizedTipPosition"];
            Vector stabilizedTipPosition = creatVector((List<object>)stabilizedTipPosition_list);

            double timeVisible = (double)pointables["timeVisible"];

            List<object> tipPosition_list = (List<object>)pointables["tipPosition"];
            Vector tipPosition = creatVector((List<object>)tipPosition_list);

            List<object> tipVelocity_list = (List<object>)pointables["tipVelocity"];
            Vector tipVelocity = creatVector((List<object>)tipVelocity_list);

            bool tool = (bool)pointables["tool"];

            double touchDistance = (double)pointables["touchDistance"];

            String touchZone = (String)pointables["touchZone"];

            long type_index = (long)pointables["type"];
            Finger.FingerType type;
            switch (type_index)
            {
                case 0: type = Finger.FingerType.TYPE_THUMB; break;
                case 1: type = Finger.FingerType.TYPE_INDEX; break;
                case 2: type = Finger.FingerType.TYPE_MIDDLE; break;
                case 3: type = Finger.FingerType.TYPE_RING; break;
                case 4: type = Finger.FingerType.TYPE_PINKY; break;

                default: type = Finger.FingerType.TYPE_UNKNOWN; break;
            }

            double width = (double)pointables["width"];

            float fingerId = handId + type_index;

            List<object> boneBasis = (List<object>)pointables["bases"];
            //Debug.Log(boneBasis.Count);
            List<object> bonelist_1 = (List<object>)boneBasis[0];
            List<object> bonelist_2 = (List<object>)boneBasis[1];
            List<object> bonelist_3 = (List<object>)boneBasis[2];
            List<object> bonelist_4 = (List<object>)boneBasis[3];

            Bone metacarpal = createBone(bonelist_1, carpPosition, mcpPosition, (float)width, Bone.BoneType.TYPE_METACARPAL);
            Bone proximal = createBone(bonelist_2, mcpPosition, pipPosition, (float)width, Bone.BoneType.TYPE_PROXIMAL);
            Bone intermediate = createBone(bonelist_3, pipPosition, dipPosition, (float)width, Bone.BoneType.TYPE_INTERMEDIATE);
            Bone distal = createBone(bonelist_4, dipPosition, btipPosition, (float)width, Bone.BoneType.TYPE_DISTAL);

            Finger finger = new Finger(id, (int)frameId, (int)fingerId, (float)timeVisible, tipPosition, tipVelocity, direction, stabilizedTipPosition, (float)width, (float)length, extended, type, metacarpal, proximal, intermediate, distal);
            //Finger finger = new Finger();
            result.Add(finger);
        }
        return result;
    }

    private Bone createBone(List<object> basis, Vector start, Vector end, float width, Bone.BoneType type)
    {
        Vector basis_1 = creatVector((List<object>)basis[0]);
        Vector basis_2 = creatVector((List<object>)basis[1]);
        Vector basis_3 = creatVector((List<object>)basis[2]);
        Vector center = new Vector((end.x + start.x) / 2, (start.y + end.y) / 2, (start.z + end.z) / 2);
        Vector direction = new Vector((end.x - start.x), (end.y - start.y), (end.z - start.z));
        double length = Math.Sqrt((end.x - start.x) * (end.x - start.x) + (end.y - start.y) * (end.y - start.y) + (end.z - start.z) * (end.z - start.z));

        //LeapQuaternion orientation = createQuaternion(basis_1, basis_2, basis_3);
        LeapQuaternion orientation = createQuaternion(basis_1, basis_2, basis_3);
        Bone bone = new Bone(start, end, center, direction, (float)length, width, type, orientation);

        return bone;
    }

    private LeapQuaternion createQuaternion(Vector arm_1, Vector arm_2, Vector arm_3)
    {
        Vector3 arm1_3 = new Vector3(arm_1.x, arm_1.y, arm_1.z);
        Vector3 arm2_3 = new Vector3(arm_2.x, arm_2.y, arm_2.z);
        Vector3 arm3_3 = new Vector3(arm_3.x, arm_3.y, arm_3.z);
        Quaternion basisQ = Quaternion.LookRotation(arm3_3, arm2_3);
        LeapQuaternion basis = new LeapQuaternion(basisQ.x, basisQ.y, basisQ.z, basisQ.w);
        return basis;

    }

    public void SetPolicy(Controller.PolicyFlag policy)
    {
        switch (policy)
        {
            case Controller.PolicyFlag.POLICY_DEFAULT:
                flag = BACKGROUND_OFF;
                Debug.Log("SetPolicy: POLICY_DEFAULT");
                break;
            case Controller.PolicyFlag.POLICY_BACKGROUND_FRAMES:
                flag = BACKGROUND_ON;
                Debug.Log("SetPolicy: POLICY_BACKGROUND_FRAMES");
                break;
            case Controller.PolicyFlag.POLICY_OPTIMIZE_HMD:
                flag = HMD_ON;
                break;
            case Controller.PolicyFlag.POLICY_ALLOW_PAUSE_RESUME:
                Debug.Log("SetPolicy: POLICY_ALLOW_PAUSE_RESUME");
                break;
            default:
                break;
        }
    }


    public void ClearPolicy(Controller.PolicyFlag policy)
    {
        switch (policy)
        {
            case Controller.PolicyFlag.POLICY_DEFAULT:
                flag = BACKGROUND_OFF;
                Debug.Log("SetPolicy: POLICY_DEFAULT");
                break;
            case Controller.PolicyFlag.POLICY_BACKGROUND_FRAMES:
                flag = BACKGROUND_OFF;
                Debug.Log("SetPolicy: POLICY_BACKGROUND_FRAMES");
                break;
            case Controller.PolicyFlag.POLICY_OPTIMIZE_HMD:
                flag = HMD_OFF;
                break;
            case Controller.PolicyFlag.POLICY_ALLOW_PAUSE_RESUME:
                Debug.Log("SetPolicy: POLICY_ALLOW_PAUSE_RESUME");
                break;
            default:
                break;
        }

    }

    public void StopConnection()
    {
        //if (w != null)
        //{
        //    w.Close(1000, "Done");
        //}

    }

    public void StartConnection()
    {
        if (!IsConnected)
        {

            Start();
        }
    }

}
