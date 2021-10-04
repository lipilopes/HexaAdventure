using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public static CameraOrbit Instance;

    [SerializeField]
    Transform target;
    [SerializeField]
    float distance = 5.0f;
    [SerializeField]
    float speedArrow = 120.0f;
    float speedMouse = 0;

    [SerializeField]
    float yMinLimit = 0;
    [SerializeField]
    float yMaxLimit = 180f;

    [SerializeField]
    float distanceMin = .5f;
    [SerializeField]
    float distanceMax = 15f;

    [SerializeField]
    float mobileDistanceMax = 84;

    public float mouseSensibility = 1;

    // private Rigidbody rigidbody;

           bool moveCameraArrow = true;

    [HideInInspector]
    public bool moveCamera = true;

    float x = 0.0f;
    float y = 0.0f;

    ToolTip tooltip;

    Transform _default;

    void Start()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
            Instance = this;

        _default = transform;

        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        //rigidbody = GetComponent<Rigidbody>();

        //if (rigidbody != null)
        //{
        //    rigidbody.freezeRotation = true;
        //}

        tooltip = ToolTip.Instance;
    }

    private void Update()
    {
#if UNITY_ANDROID || UNITY_IOS || UNITY_WP8
        if (Input.touchCount == 2)
        {
            OrbitCamera();
        }
#endif
    }

    public void OrbitCamera()
    {
        if (GameManagerScenes._gms.Paused == true)
            return;

        //Debug.LogError("OrbitCamera()");

        if (target)
        {
            float speed = 0;

            if (speed != (speedMouse))
                speed = speedMouse;

#if UNITY_ANDROID || UNITY_IOS || UNITY_WP8
            if (Input.touchCount == 2)
            {
                speed = mouseSensibility/100;

                // A taxa de mudança do campo de visão no modo de perspectiva.  
                float perspectiveZoomSpeed = 0.5f;

                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                float deltaMagnitudeDiff = (prevTouchDeltaMag - touchDeltaMag);

                //// If the camera is orthographic...
                //if (Camera.main.orthographic)
                //{
                //    //// ... change the orthographic size based on the change in distance between the touches.
                //    Camera.main.orthographicSize += deltaMagnitudeDiff * (0.5f*speed);

                //    //// Make sure the orthographic size never drops below zero.
                //    Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize, 0.1f);
                //}
                //else
                {
                    // Otherwise change the field of view based on the change in distance between the touches.
                    Camera.main.fieldOfView += (deltaMagnitudeDiff * (perspectiveZoomSpeed)*speed);

                    // Clamp the field of view to make sure it's between 0 and 180.
                    Camera.main.fieldOfView = ClampAngle(Camera.main.fieldOfView, distanceMin, mobileDistanceMax);

                    transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
                }
            }
#else

            float dirHor  = 0;
            float dirVert = 0;

            if (moveCameraArrow)
            {
                if (Input.GetKey(KeyCode.DownArrow))
                    dirVert = (1);

                if (Input.GetKey(KeyCode.UpArrow))
                    dirVert = (-1);

                if (Input.GetKey(KeyCode.LeftArrow))
                    dirHor = (1);

                if (Input.GetKey(KeyCode.RightArrow))
                    dirHor = (-1);

                if (speed != speedArrow)
                    speed = speedArrow;
            }
            else
            {
                if (moveCamera)
                {
                    dirHor  = ((Input.GetAxis("Mouse X") * Time.deltaTime));
                    dirVert = ((Input.GetAxis("Mouse Y") * Time.deltaTime));


                    if (speed != (speedMouse))
                        speed = speedMouse;

                    //dirHor  += Input.GetAxis("Mouse X") * _mouseSensibility;
                    //dirVert -= Input.GetAxis("Mouse Y") * _mouseSensibility;
                }

            }

            distance            = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * speedMouse, distanceMin, distanceMax);

            x += (dirHor  * (speed) * 0.02f);
            y -= (dirVert * (speed) * 0.02f);

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);           

            RaycastHit hit;
            if (Physics.Linecast(target.position, transform.position, out hit))
            {
                if (hit.collider.gameObject.tag == "StopCamera")
                {
                    distance--;
                }
            }

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position    = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;
//Endif here
#endif
        }
        else
            if (tooltip!=null && tooltip.target!=null)
        {
            ChangeTarget(tooltip.target);
        }else
            ChangeTarget(TurnSystem.Instance.CurrentTurn());
    }
        
    public void MaxOrbitCamera()
    {
        float max = distanceMax;

        if (GameManagerScenes._gms.IsMobile)
            max = mobileDistanceMax;

        Camera.main.fieldOfView = /*ClampAngle(Camera.main.fieldOfView, distanceMin, */mobileDistanceMax/*)*/;
    }

    public void AttMouse()
    {
        speedMouse = (120 + distance) * mouseSensibility;
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
            
        return Mathf.Clamp(angle, min, max);
    }

    public void HowMoveCamera(bool arrows)
    {
        moveCameraArrow = arrows;

        AttMouse();
    }

    public void ChangeTarget(GameObject value,bool igual=false)
    {
        if (value != target && igual)
            return;

        if (value.GetComponent<MobManager>()!=null)
        {
            target = value.transform;

            transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
        }
    }

    public void ResetChangeTarget()
    {
        target = _default;

        transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
    }
}
