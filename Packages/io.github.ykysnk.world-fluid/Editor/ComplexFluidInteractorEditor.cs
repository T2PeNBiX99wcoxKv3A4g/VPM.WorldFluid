using System.Linq;
using io.github.ykysnk.utils.Editor;
using UnityEditor;
using UnityEngine;

namespace io.github.ykysnk.WorldFluid.Editor;

[CustomEditor(typeof(ComplexFluidInteractor))]
public class ComplexFluidInteractorEditor : BasicEditor
{
    private const string Name = "CornerFloater";
    protected override bool IsBaseOnOldInspectorGUI => true;

    protected override void OnInspectorGUIDraw()
    {
        var complexFluidInteractor = (ComplexFluidInteractor)target;

        if (!GUILayout.Button("Use Bounding Box Corners")) return;

        complexFluidInteractor.coll = complexFluidInteractor.GetComponent<Collider>();

        var corners = complexFluidInteractor.DefineCorners();
        
        foreach (var transform in complexFluidInteractor.floaters)
            DestroyImmediate(transform.gameObject);

        complexFluidInteractor.floaters = corners.Select(corner => new GameObject(Name)
            {
                transform =
                {
                    position = complexFluidInteractor.transform.position + corner,
                    parent = complexFluidInteractor.transform
                }
            })
            .Select(cornerObj => cornerObj.transform).ToArray();
    }
}