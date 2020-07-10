using UnityEngine;
using Phenix.Unity.DeviceInput;

public class CameraLogic : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector3 tarPos = Camera.main.transform.position + new Vector3(-FingerTouch.Instance.GetAxisX(), 0,
            -FingerTouch.Instance.GetAxisY());
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, tarPos, Time.deltaTime * 60);

        Camera.main.fieldOfView -= FingerTouch.Instance.GetAxisZoom() * 20;
    }
}
