﻿using System.Reflection;
using ColossalFramework;
using EightyOne.Attributes;
using UnityEngine;

namespace EightyOne.Areas
{
    [TargetType(typeof(GameAreaManager))]
    public class FakeGameAreaManagerUI : GameAreaManager
    {
        private static FieldInfo _AreaTex = typeof(GameAreaManager).GetField("m_areaTex", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo _HighlightAreaIndex = typeof(GameAreaManager).GetField("m_highlightAreaIndex", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo _AreasUpdatedField = typeof(GameAreaManager).GetField("m_areasUpdated", BindingFlags.NonPublic | BindingFlags.Instance);
        public const int AREA_TEX_SIZE = 9;

        //[ReplaceMethod]
        private void UpdateAreaMapping()
        {
            if (this.AreasVisible)
            {
                this.UpdateAreaTexture();
            }
            var camController = (CameraController)typeof(GameAreaManager).GetField("m_cameraController", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
            if (camController == null)
            {
                return;
            }
            if (this.AreasVisible)
            {
                Vector3 vector = camController.transform.TransformDirection(Vector3.right);
                ItemClass.Availability mode = Singleton<ToolManager>.instance.m_properties.m_mode;
                Bounds freeBounds;
                if ((mode & ItemClass.Availability.MapEditor) != ItemClass.Availability.None)
                {
                    freeBounds = new Bounds(new Vector3(0f, 512f, 0f), new Vector3(9600f, 1024f, 9600f));
                }
                else
                {
                    freeBounds = this.GetFreeBounds();
                }
                Vector3 center = freeBounds.center;
                if (Mathf.Abs(vector.x) >= Mathf.Abs(vector.z))
                {
                    if (vector.x > 0f)
                    {
                        center.z -= freeBounds.size.z * 0.02f + 50f;
                    }
                    else
                    {
                        center.z += freeBounds.size.z * 0.02f + 50f;
                    }
                    camController.SetOverrideModeOn(center, new Vector2((vector.x <= 0f) ? 180f : 0f, 80f), freeBounds.size.z);
                }
                else
                {
                    if (vector.z > 0f)
                    {
                        center.x += freeBounds.size.x * 0.02f + 50f;
                    }
                    else
                    {
                        center.x -= freeBounds.size.x * 0.02f + 50f;
                    }
                    camController.SetOverrideModeOn(center, new Vector2((vector.z <= 0f) ? 90f : -90f, 80f), freeBounds.size.x);
                }
            }
            else
            {
                camController.SetOverrideModeOff();
            }
        }

        [ReplaceMethod]
        private Bounds GetFreeBounds()
        {
            Vector3 zero1 = Vector3.zero;
            Vector3 zero2 = Vector3.zero;
            //begin mod
            for (int z = 0; z < FakeGameAreaManager.GRID; ++z)
            {
                for (int x = 0; x < FakeGameAreaManager.GRID; ++x)
                {
                    if (this.IsUnlocked(x, z))
                    {
                        zero1.x = Mathf.Min(zero1.x, (float)(((double)(x - 1) - FakeGameAreaManager.HALFGRID) * 1920.0));
                        zero2.x = Mathf.Max(zero2.x, (float)(((double)(x + 2) - FakeGameAreaManager.HALFGRID) * 1920.0));
                        zero1.z = Mathf.Min(zero1.z, (float)(((double)(z - 1) - FakeGameAreaManager.HALFGRID) * 1920.0));
                        zero2.z = Mathf.Max(zero2.z, (float)(((double)(z + 2) - FakeGameAreaManager.HALFGRID) * 1920.0));
                        zero2.y = Mathf.Max(zero2.y, 1024f);
                    }
                }
            }
            //end mod
            Bounds bounds = new Bounds();
            bounds.SetMinMax(zero1, zero2);
            return bounds;
        }

        [ReplaceMethod]
        private void UpdateAreaTexture()
        {
            _AreasUpdatedField.SetValue(this, false);
            var areaTex = (Texture2D)_AreaTex.GetValue(this);
            var highlightAreaIndex = (int)_HighlightAreaIndex.GetValue(this);
            //begin mod
            int num1 = 0;
            //end mod
            if ((Singleton<ToolManager>.instance.m_properties.m_mode & ItemClass.Availability.MapEditor) != ItemClass.Availability.None)
            {
                //begin mod
                //end mod
            }
            else
            {
                for (int y = 0; y <= AREA_TEX_SIZE; ++y)
                {
                    for (int x1 = 0; x1 <= AREA_TEX_SIZE; ++x1)
                    {
                        int x2 = x1 - num1;
                        int z = y - num1;
                        bool flag1 = this.IsUnlocked(x2, z);
                        bool flag2 = this.CanUnlock(x2, z);
                        Color color;
                        color.r = !flag1 ? 0.0f : 1f;
                        color.g = !flag2 ? 0.0f : 1f;
                        //begin mod
                        color.b = highlightAreaIndex != z * FakeGameAreaManager.GRID + x2 ? 0.0f : (!flag2 ? (!flag1 ? 0.0f : 0.5f) : 0.5f);
                        //end mod
                        color.a = 1f;
                        areaTex.SetPixel(x1, y, color);
                    }
                }
            }
            areaTex.Apply(false);
        }

        [ReplaceMethod]
        protected new void BeginOverlayImpl(RenderManager.CameraInfo cameraInfo)
        {
            float borderAlpha = (float)typeof(GameAreaManager).GetField("m_borderAlpha", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
            float areaAlpha = (float)typeof(GameAreaManager).GetField("m_areaAlpha", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
            Material borderMaterial = (Material)typeof(GameAreaManager).GetField("m_borderMaterial", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
            Material areaMaterial = (Material)typeof(GameAreaManager).GetField("m_areaMaterial", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
            Material decorationMaterial = (Material)typeof(GameAreaManager).GetField("m_decorationMaterial", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
            Mesh borderMesh = (Mesh)typeof(GameAreaManager).GetField("m_borderMesh", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
            int ID_Color = (int)typeof(GameAreaManager).GetField("ID_Color", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
            int ID_AreaMapping = (int)typeof(GameAreaManager).GetField("ID_AreaMapping", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
            Texture2D areaTex = (Texture2D)_AreaTex.GetValue(this);

            ItemClass.Availability availability = Singleton<ToolManager>.instance.m_properties.m_mode;
            if ((availability & ItemClass.Availability.AssetEditor) != ItemClass.Availability.None)
            {
                //begin mod
                //end mod
            }
            else if ((availability & ItemClass.Availability.MapEditor) != ItemClass.Availability.None)
            {
                //begin mod
                //end mod
            }
            else
            {
                if (borderAlpha >= 1.0 / 1000.0 &&
                    borderMaterial != (UnityEngine.Object)null)
                {
                    Quaternion rotation = Quaternion.AngleAxis(90f, Vector3.up);
                    Color color = Color.white;
                    ToolController toolController = Singleton<ToolManager>.instance.m_properties;
                    if ((UnityEngine.Object)toolController != (UnityEngine.Object)null &&
                        (toolController.CurrentTool.GetErrors() & ToolBase.ToolErrors.OutOfArea) !=
                        ToolBase.ToolErrors.None)
                        color = Color.red;
                    color.a = borderAlpha;
                    //begin mod
                    for (int z = 0; z <= FakeGameAreaManager.GRID; ++z)
                    {
                        for (int x = 0; x <= FakeGameAreaManager.GRID; ++x)
                        {
                            //end mod
                            bool flag1 = this.GetArea(x, z) > 0;
                            bool flag2 = this.GetArea(x, z - 1) > 0;
                            bool flag3 = this.GetArea(x - 1, z) > 0;
                            if (flag1 != flag2)
                            {
                                //begin mod
                                Vector3 vector3 = new Vector3((float)(((double)x - FakeGameAreaManager.HALFGRID + 0.5) * 1920.0), 0.0f,
                                    (float)(((double)z - FakeGameAreaManager.HALFGRID) * 1920.0));
                                //end mod
                                Vector3 size = new Vector3(1920f, 1024f, 100f);
                                Bounds bounds = new Bounds(vector3 + new Vector3(0.0f, size.y * 0.5f, 0.0f), size);
                                if (cameraInfo.Intersect(bounds))
                                {
                                    Singleton<TerrainManager>.instance.SetWaterMaterialProperties(vector3,
                                        borderMaterial);
                                    borderMaterial.SetColor(ID_Color, color);
                                    if (borderMaterial.SetPass(0))
                                    {
                                        ++Singleton<GameAreaManager>.instance.m_drawCallData.m_overlayCalls;
                                        Graphics.DrawMeshNow(borderMesh, vector3, rotation);
                                    }
                                }
                            }
                            if (flag1 != flag3)
                            {
                                //begin mod
                                Vector3 vector3 = new Vector3((float)(((double)x - FakeGameAreaManager.HALFGRID) * 1920.0), 0.0f,
                                    (float)(((double)z - FakeGameAreaManager.HALFGRID + 0.5) * 1920.0));
                                //end mod
                                Vector3 size = new Vector3(100f, 1024f, 1920f);
                                Bounds bounds = new Bounds(vector3 + new Vector3(0.0f, size.y * 0.5f, 0.0f), size);
                                if (cameraInfo.Intersect(bounds))
                                {
                                    Singleton<TerrainManager>.instance.SetWaterMaterialProperties(vector3,
                                        borderMaterial);
                                    borderMaterial.SetColor(ID_Color, color);
                                    if (borderMaterial.SetPass(0))
                                    {
                                        ++Singleton<GameAreaManager>.instance.m_drawCallData.m_overlayCalls;
                                        Graphics.DrawMeshNow(borderMesh, vector3, Quaternion.identity);
                                    }
                                }
                            }
                        }
                    }
                }
                if (areaAlpha < 1.0 / 1000.0 ||
                    !(areaMaterial != (UnityEngine.Object)null))
                    return;
                Vector4 vector;
                vector.z = 1.0f / (1920.0f * AREA_TEX_SIZE);//6.510417E-05f;
                vector.x = (1.0f * FakeGameAreaManager.GRID) / (AREA_TEX_SIZE * 2.0f);//7.0f / 16.0f;
                vector.y = (1.0f * FakeGameAreaManager.GRID) / (AREA_TEX_SIZE * 2.0f);//7.0f / 16.0f;
                vector.w = 1.0f / (1.0f * AREA_TEX_SIZE);//0.125f;
                areaMaterial.mainTexture = areaTex;
                areaMaterial.SetColor(ID_Color, new Color(1f, 1f, 1f, areaAlpha));
                areaMaterial.SetVector(ID_AreaMapping, vector);
                Bounds freeBounds = this.GetFreeBounds();
                freeBounds.size = freeBounds.size + new Vector3(100f, 1f, 100f);
                ++Singleton<GameAreaManager>.instance.m_drawCallData.m_overlayCalls;
                Singleton<RenderManager>.instance.OverlayEffect.DrawEffect(cameraInfo, areaMaterial, 0,
                    freeBounds);
            }
        }
    }
}