using UnityEngine;

public class MirrorManager : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject mirrorPrefab;                 // 생성할 Mirror 프리팹

    private MirrorTransformController currentSelected;     // 현재 선택된 Mirror

    void Update()
    {
        /* ===============================
         * Mirror 생성
         * =============================== */
        // Z 키를 누르면 (0,0,0) 위치에 Mirror 생성
        if (Input.GetKeyDown(KeyCode.Z))
        {
            GameObject go = Instantiate(mirrorPrefab, Vector3.zero, Quaternion.identity);

            // 생성된 오브젝트에 MirrorInteractable이 없으면 자동으로 추가
            var mi = go.GetComponent<MirrorTransformController>();
           // if (mi == null) mi = go.AddComponent<MirrorInteractable>();

            // 생성한 Mirror를 바로 선택 상태로 변경
            Select(mi);
        }

        /* ===============================
         * Mirror 선택
         * =============================== */
        // 마우스 클릭 시, 클릭한 오브젝트가 Mirror라면 선택 변경
        if (Input.GetMouseButtonDown(0))
        {
            if (TryPickMirror(out var picked))
            {
                Select(picked);
            }
        }

        /* ===============================
         * Mirror 삭제 (선택사항)
         * =============================== */
        // Delete 키를 누르면 현재 선택된 Mirror 삭제
        if (Input.GetKeyDown(KeyCode.Delete) && currentSelected != null)
        {
            Destroy(currentSelected.gameObject);
            currentSelected = null;
        }
    }

    /// <summary>
    /// 선택된 Mirror를 변경하는 함수
    /// - 이전 선택 해제
    /// - 새 대상 선택
    /// </summary>
    private void Select(MirrorTransformController mi)
    {
        // 이미 선택된 대상이면 처리하지 않음
        if (currentSelected == mi) return;

        // 기존 선택 해제
        if (currentSelected != null)
            currentSelected.SetSelected(false);

        // 새 대상 선택
        currentSelected = mi;

        if (currentSelected != null)
            currentSelected.SetSelected(true);
    }

    /// <summary>
    /// 마우스가 클릭한 오브젝트가 Mirror인지 검사하고,
    /// 맞으면 해당 MirrorInteractable을 반환
    /// </summary>
    private bool TryPickMirror(out MirrorTransformController mi)
    {
        mi = null;

        Camera cam = Camera.main;
        if (cam == null) return false;

        // 화면 클릭 위치로 Ray 생성
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // Raycast로 클릭한 오브젝트 판별
        if (Physics.Raycast(ray, out RaycastHit hit, 500f))
        {
            // Collider가 자식에 있어도 찾을 수 있게 Parent까지 검색
            mi = hit.collider.GetComponentInParent<MirrorTransformController>();
            return mi != null;
        }

        return false;
    }
}
