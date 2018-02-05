﻿using UnityEngine;

[SelectionBase]
public class VoxelChunk : MonoBehaviour
{
    #region Public Variables
    private Material[] voxelMaterials;

    public GameObject voxelPrefab;
    #endregion

    #region Private Variables
    private int resolution;

    private bool[] voxels;

    private float voxelSize;
    #endregion

    #region Public Methods
    public void Initialize(int resolution, float size)
    {
        this.resolution = resolution;
        voxelSize = size / resolution;
        voxels = new bool[resolution * resolution];
        voxelMaterials = new Material[voxels.Length];

        for (int i = 0, y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++, i++)
            {
                CreateVoxel(i, x, y);
            }
        }

        SetVoxelColors();
    }

    public void Apply(VoxelStencil stencil)
    {
        int xStart = stencil.XStart;
        if (xStart < 0)
        {
            xStart = 0;
        }
        int xEnd = stencil.XEnd;
        if (xEnd >= resolution)
        {
            xEnd = resolution - 1;
        }
        int yStart = stencil.YStart;
        if (yStart < 0)
        {
            yStart = 0;
        }
        int yEnd = stencil.YEnd;
        if (yEnd >= resolution)
        {
            yEnd = resolution - 1;
        }

        for (int y = yStart; y <= yEnd; y++)
        {
            int i = y * resolution + xStart;
            for (int x = xStart; x <= xEnd; x++, i++)
            {
                voxels[i] = stencil.Apply(x, y, voxels[i]);
            }
        }
        SetVoxelColors();
    }
    #endregion

    #region Private Methods
    private void SetVoxelColors()
    {
        for (int i = 0; i < voxels.Length; i++)
        {
            voxelMaterials[i].color = voxels[i] ? Color.black : Color.white;
        }
    }

    private void CreateVoxel(int i, int x, int y)
    {
        GameObject obj = Instantiate(voxelPrefab) as GameObject;
        obj.transform.parent = transform;
        obj.transform.localPosition = new Vector3((x + 0.5f) * voxelSize, (y + 0.5f) * voxelSize, -0.01f);
        obj.transform.localScale = Vector3.one * voxelSize * 0.1f;
        voxelMaterials[i] = obj.GetComponent<MeshRenderer>().material;
    }
    #endregion
}
