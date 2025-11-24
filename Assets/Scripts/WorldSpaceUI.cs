using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Rendering;
using TMPro; 

public class WorldSpaceUI : MonoBehaviour
{
    private const string shaderTestMode = "unity_GUIZTestMode";
    
    // 기본값은 Always (깊이 무시하고 항상 앞에 렌더링)
    [SerializeField] UnityEngine.Rendering.CompareFunction desiredUIComparison = UnityEngine.Rendering.CompareFunction.Always;
    
    [Tooltip("수동 할당 시 사용. 비어있으면 모든 자식 UI 컴포넌트를 자동으로 찾습니다.")]
    [SerializeField] Graphic[] uiElementsToApplyTo;
    
    private Dictionary<Material, Material> materialMappings = new Dictionary<Material, Material>();
    
    
    protected virtual void Start()
    {
        // 1. Graphic 컴포넌트 (Image, Text 등 레거시 UI) 처리
        if (uiElementsToApplyTo == null || uiElementsToApplyTo.Length == 0)
        {
            uiElementsToApplyTo = gameObject.GetComponentsInChildren<Graphic>(true); // 비활성화된 자식 포함 검색
        }
        ApplyGraphicFixes(uiElementsToApplyTo);

        // 2. TextMeshProUGUI (TMP_Text) 컴포넌트 처리
        TMP_Text[] tmpTexts = gameObject.GetComponentsInChildren<TMP_Text>(true); // 비활성화된 자식 포함 검색
        ApplyTMPFixes(tmpTexts);
    }
    
    /// <summary>
    /// UnityEngine.UI.Graphic (Image, Text 등) 컴포넌트에 깊이 무시 설정을 적용합니다.
    /// </summary>
    private void ApplyGraphicFixes(Graphic[] graphics)
    {
        if (graphics == null) return;
        
        foreach (var graphic in graphics)
        {
            Material material = graphic.materialForRendering;
            if (material == null)
            {
                Debug.LogWarning($"{nameof(WorldSpaceUI)}: Graphic skipping target without material {graphic.name}.");
                continue;
            }
            
            Material materialCopy = GetMaterialCopy(material);
            materialCopy.SetInt(shaderTestMode, (int) desiredUIComparison);
            graphic.material = materialCopy;
        }
    }

    /// <summary>
    /// TextMeshProUGUI (TMP_Text) 컴포넌트에 깊이 무시 설정을 적용합니다.
    /// </summary>
    private void ApplyTMPFixes(TMP_Text[] tmpTexts)
    {
        if (tmpTexts == null) return;
        
        foreach (var tmpText in tmpTexts)
        {
            // TMP는 폰트 재질(fontSharedMaterial)을 사용합니다.
            Material material = tmpText.fontSharedMaterial; 

            if (material == null)
            {
                Debug.LogWarning($"{nameof(WorldSpaceUI)}: TMP skipping target without material {tmpText.name}.");
                continue;
            }

            // TMP는 MaterialForRendering 대신 fontSharedMaterial을 사용하고, 
            // 할당할 때도 fontSharedMaterial을 사용해야 합니다.
            Material materialCopy = GetMaterialCopy(material);
            materialCopy.SetInt(shaderTestMode, (int) desiredUIComparison);
            tmpText.fontSharedMaterial = materialCopy; 
        }
    }

    /// <summary>
    /// 재질 복사본을 Dictionary에서 가져오거나 새로 생성합니다.
    /// </summary>
    private Material GetMaterialCopy(Material originalMaterial)
    {
        if (originalMaterial == null) return null;

        if (!materialMappings.TryGetValue(originalMaterial, out Material materialCopy))
        {
            // 원본 Material의 복사본을 생성합니다.
            materialCopy = new Material(originalMaterial);
            materialMappings.Add(originalMaterial, materialCopy);
        }
        return materialCopy;
    }

    // 오브젝트 파괴 시 생성된 Material 복사본을 정리합니다.
    private void OnDestroy()
    {
        foreach (var materialCopy in materialMappings.Values)
        {
            Destroy(materialCopy);
        }
        materialMappings.Clear();
    }
}