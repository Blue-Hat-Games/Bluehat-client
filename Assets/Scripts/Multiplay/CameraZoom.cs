using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public float perspectiveZoomSpeed = 0.1f; //줌인,줌아웃할때 속도(perspective모드 용)      
    public float orthoZoomSpeed = 0.1f; //줌인,줌아웃할때 속도(OrthoGraphic모드 용)  


    private void Update()
    {
        if (Input.touchCount == 2) //손가락 2개가 눌렸을 때
        {
            var touchZero = Input.GetTouch(0); //첫번째 손가락 터치를 저장
            var touchOne = Input.GetTouch(1); //두번째 손가락 터치를 저장

            //터치에 대한 이전 위치값을 각각 저장함
            //처음 터치한 위치(touchZero.position)에서 이전 프레임에서의 터치 위치와 이번 프로임에서 터치 위치의 차이를 뺌
            var touchZeroPrevPos = touchZero.position - touchZero.deltaPosition; //deltaPosition는 이동방향 추적할 때 사용
            var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // 각 프레임에서 터치 사이의 벡터 거리 구함
            var prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude; //magnitude는 두 점간의 거리 비교(벡터)
            var touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // 거리 차이 구함(거리가 이전보다 크면(마이너스가 나오면)손가락을 벌린 상태_줌인 상태)
            var deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // 만약 카메라가 OrthoGraphic모드 라면
            if (GetComponent<Camera>().orthographic)
            {
                GetComponent<Camera>().orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;
                GetComponent<Camera>().orthographicSize = Mathf.Max(GetComponent<Camera>().orthographicSize, 0.1f);
            }
            else
            {
                GetComponent<Camera>().fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;
                GetComponent<Camera>().fieldOfView = Mathf.Clamp(GetComponent<Camera>().fieldOfView, 0.1f, 179.9f);
            }
        }
    }
}