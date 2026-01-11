using UnityEngine;

/// <summary>
/// 플레이 타임 중 거울 오브젝트의 생성, 선택, 삭제를 관리하는 매니저
/// - 단축키로 거울 생성
/// - 마우스 클릭으로 거울 선택
/// - 선택된 거울은 조작 가능 상태로 전환
/// </summary>
public class MirrorManager : MonoBehaviour
{
    [Header("거울 프리팹")]
    public GameObject mirrorPrefab;         // 생성할 거울 프리팹

    [Header("거울 생성 위치")]
    [SerializeField]
    private Vector3 MirrorVector = Vector3.zero;            // 거울 생성 위치 (기본값: (0,0,0))

    // 현재 선택된 거울 컨트롤러
    private MirrorTransformController currentSelected;

    void Update()
    {
        ShortcutKeys();
    }

    #region 단축키
    /// <summary>
    /// 거울 편집용 단축키
    /// 
    /// - Z 키: 거울 생성 및 자동 선택
    /// - 마우스 클릭: 씬에서 거울 선택
    /// - Delete 키: 선택된 거울 삭제
    /// </summary>
    private void ShortcutKeys()
    {
        // Z 키를 누르면 새로운 거울 생성
        if (Input.GetKeyDown(KeyCode.Z))
        {
            GameObject obj = Instantiate(mirrorPrefab, MirrorVector, Quaternion.identity);

            // 생성된 거울의 컨트롤러 가져오기
            MirrorTransformController mi = obj.GetComponent<MirrorTransformController>();

            // 생성 직후 해당 거울을 선택 상태로 설정
            Select(mi);
        }

        // 마우스 클릭 시 거울 선택 처리
        if (Input.GetMouseButtonDown(0))
        {
            if (TryPickMirror(out var picked))
            {
                Select(picked);
            }
        }

        // Delete 키로 현재 선택된 거울 삭제
        if (Input.GetKeyDown(KeyCode.Delete) && currentSelected != null)
        {
            Destroy(currentSelected.gameObject);
            currentSelected = null;
        }
    }
    #endregion

    #region 거울 선택 처리
    /// <summary>
    /// 선택된 거울을 변경하는 함수
    /// - 기존 선택 해제
    /// - 새 거울 선택
    /// </summary>
    private void Select(MirrorTransformController mi)
    {
        // 이미 선택된 대상이면 처리하지 않음
        if (currentSelected == mi) return;

        // 이전 거울 선택 해제
        if (currentSelected != null)
            currentSelected.SetSelected(false);

        // 새 거울 선택
        currentSelected = mi;

        if (currentSelected != null)
            currentSelected.SetSelected(true);
    }
    #endregion

    #region 거울 선택 판별
    /// <summary>
    /// 마우스로 클릭한 오브젝트가 거울인지 판별
    /// - Raycast로 클릭 대상 검사
    /// - 자식 콜라이더를 고려해 부모에서 컨트롤러 검색
    /// </summary>
    private bool TryPickMirror(out MirrorTransformController mi)
    {
        mi = null;

        Camera cam = Camera.main;
        if (cam == null) return false;

        // 마우스 클릭 위치 기준으로 Ray 생성
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // Raycast로 클릭한 오브젝트 판별
        if (Physics.Raycast(ray, out RaycastHit hit, 500f))
        {
            // 콜라이더가 자식에 있어도 거울 컨트롤러를 찾을 수 있도록 부모까지 검색
            mi = hit.collider.GetComponentInParent<MirrorTransformController>();
            return mi != null;
        }

        return false;
    }
    #endregion
}