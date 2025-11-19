using System;
using io.github.ykysnk.utils;
using UnityEditor;
using UnityEngine;

namespace io.github.ykysnk.WorldFluid.Editor;

public class SetupFluid : EditorWindow
{
    private const string MenuPath = "yky/World Fluid/";
    private const string ObjMenuPath = "GameObject/yky/World Fluid/";

    private static void Log(object message) => Utils.Log(nameof(SetupFluid), message);
    private static void LogWarning(object message) => Utils.LogWarning(nameof(SetupFluid), message);

    [MenuItem(MenuPath + "Setup Rigid body(s) (Basic)")]
    private static void SetupBasic()
    {
        var setupObjects = 0;
        var rigidBodies = FindObjectsOfType<Rigidbody>();

        foreach (var rb in rigidBodies)
            try
            {
                if (rb.gameObject.TryGetComponent<BaseFluid>(out _) ||
                    rb.gameObject.TryGetComponent<BaseFluidInteractor>(out _)) continue;
                rb.gameObject.AddComponent<BasicFluidInteractor>();

                setupObjects++;
            }
            catch (ArgumentException e)
            {
                LogWarning(e);
            }

        Log("Successfully Setup of " + setupObjects + " Rigid body(s)");
    }

    [MenuItem(MenuPath + "Setup Rigid body(s) (Complex)")]
    private static void SetupComplex()
    {
        var setupObjects = 0;
        var rigidBodies = FindObjectsOfType<Rigidbody>();

        foreach (var rb in rigidBodies)
            try
            {
                if (rb.gameObject.TryGetComponent<BaseFluid>(out _) ||
                    rb.gameObject.TryGetComponent<BaseFluidInteractor>(out _)) continue;
                rb.gameObject.AddComponent<ComplexFluidInteractor>();

                setupObjects++;
            }
            catch (ArgumentException e)
            {
                LogWarning(e);
            }

        Log("Successfully Setup of " + setupObjects + " Rigid body(s)");
    }

    [MenuItem(ObjMenuPath + "Setup Rigid body(s) (Basic)")]
    private static void SetupBasicOnSelection()
    {
        var gameObjects = Selection.gameObjects;
        var setupObjects = 0;

        foreach (var obj in gameObjects)
            try
            {
                if (obj.TryGetComponent<BaseFluid>(out _) || obj.TryGetComponent<BasicFluidInteractor>(out _)) continue;
                if (!obj.TryGetComponent<Rigidbody>(out _))
                    obj.AddComponent<Rigidbody>();
                if (obj.TryGetComponent<BaseFluidInteractor>(out var baseFluid))
                    DestroyImmediate(baseFluid);
                obj.AddComponent<BasicFluidInteractor>();

                setupObjects++;
            }
            catch (ArgumentException e)
            {
                LogWarning(e);
            }

        Log("Successfully Setup of " + setupObjects + " Rigid body(s)");
    }

    [MenuItem(ObjMenuPath + "Setup Rigid body(s) (Complex)")]
    private static void SetupComplexOnSelection()
    {
        var gameObjects = Selection.gameObjects;
        var setupObjects = 0;

        foreach (var obj in gameObjects)
            try
            {
                if (obj.TryGetComponent<BaseFluid>(out _) ||
                    obj.TryGetComponent<ComplexFluidInteractor>(out _)) continue;
                if (!obj.TryGetComponent<Rigidbody>(out _))
                    obj.AddComponent<Rigidbody>();
                if (obj.TryGetComponent<BaseFluidInteractor>(out var baseFluid))
                    DestroyImmediate(baseFluid);
                obj.AddComponent<ComplexFluidInteractor>();

                setupObjects++;
            }
            catch (ArgumentException e)
            {
                LogWarning(e);
            }

        Log("Successfully Setup of " + setupObjects + " Rigid body(s)");
    }
}