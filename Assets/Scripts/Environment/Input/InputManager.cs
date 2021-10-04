using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    //Camera Fixed 
    public bool canMoveCamera = false;

    public CameraOrbit cameraOrbit;

    [SerializeField] Transform _position;

    bool _startUpdate = true;

    public bool StartUpdate { get { return _startUpdate; } set { _startUpdate = value; } }

    private void Update()
    {
        if (!_startUpdate)
            return;

        ChangeCameraButton();
      
        EnableConfigMenu();
    }

    /// <summary>
    /// Muda estilo de camera
    /// </summary>
    void ChangeCameraButton()
    {
        if (GameManagerScenes._gms)            
        if (GameManagerScenes._gms.Paused == true)
            return;

        if (Input.GetButtonDown("Change Camera"))
        {
            canMoveCamera = !canMoveCamera;
        }

        MoveCameraMouse();
        MoveCamera();
    }

    /// <summary>
    /// move camera em quando clicar com o botao
    /// </summary>
    void MoveCameraMouse()
    {
        if (Input.GetButton("Move Camera"))
        {
            cameraOrbit.moveCamera = true;          
        }
        else
        if(cameraOrbit.moveCamera)
        {
            cameraOrbit.moveCamera = false;
        }
    }

   public void ChangeCamera(bool move=false)
    {
        canMoveCamera = move;       

        MoveCamera();
    }

    void MoveCamera()
    {
        GameManagerScenes gms = GameManagerScenes._gms;

        bool isMobile = true;

        if (gms != null)
        {
            isMobile = gms.IsMobile;
        }

        if (!isMobile)
        {
            if (!canMoveCamera)
            {
                if (cameraOrbit.gameObject.transform.position != _position.position)
                    cameraOrbit.gameObject.transform.position  = _position.position;

                if (cameraOrbit.gameObject.transform.rotation != _position.rotation)
                    cameraOrbit.gameObject.transform.rotation  = _position.rotation;

                return;
            }

            cameraOrbit.OrbitCamera();
        }
    }

   void EnableConfigMenu()
    {
        if (Input.GetButtonUp("Pause"))
        {
            GetComponent<ButtonManager>().AtivePausedPainel();
        }
    }

    public void AttSensibilityMouse(float value)
    {
        cameraOrbit.mouseSensibility = value;

        cameraOrbit.AttMouse();
    }
}
