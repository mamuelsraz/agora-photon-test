using UnityEngine;
using Photon.Realtime;
//using UnityEngine.InputSystem;

namespace Photon.Pun
{
    public class ClientManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        public static GameObject LocalPlayerInstance;
        public CharacterController controller;
        public float speed;

        Transform cam;
        public Transform camOffset;
        public float mouseSensitivity;
        float yRotation = 0f;

        private void Awake()
        {
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
            }
        }

        private void Start()
        {
            if (photonView.IsMine)
            {
                cam = Camera.main.transform;
                cam.parent = camOffset;
                cam.localPosition = Vector3.zero;
            }
        }

        public override void OnDisable()
        {
            // Always call the base to remove callbacks
            base.OnDisable();
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                this.Move();
                this.RotateCam();
            }
        }

        public void Move()
        {
            Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            movement = (transform.right * movement.x + transform.forward * movement.z).normalized;
            controller.Move(movement * speed * Time.deltaTime);
        }

        public void RotateCam()
        {
            Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity * Time.deltaTime;
            //Vector2 mouseInput = new Vector2(Mouse.current.delta.x.ReadValue(), Mouse.current.delta.y.ReadValue()) * mouseSensitivity * Time.deltaTime;

            yRotation -= mouseInput.y;
            yRotation = Mathf.Clamp(yRotation, -90f, 90f);

            cam.localRotation = Quaternion.Euler(yRotation, 0, 0);
            transform.Rotate(Vector3.up * mouseInput.x);
        }

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
            }
            else
            {
                // Network player, receive data
            }
        }

        #endregion
    }
}
