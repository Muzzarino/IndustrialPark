﻿using SharpDX;
using System;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class ScaleGizmo : GizmoBase
    {
        public ScaleGizmo(GizmoType type) : base(type)
        {
            vertices = new Vector3[SharpRenderer.cubeVertices.Count];
        }

        public void SetPosition(BoundingBox Box, Matrix Rotation)
        {
            switch (type)
            {
                case GizmoType.ScaleX:
                    transformMatrix = Matrix.Scaling(Math.Max(Math.Abs(Box.Width) / 6f, 1f)) * Matrix.Translation(Math.Abs(Box.Width) + 1f, 0f, 0f) * Rotation * Matrix.Translation(Box.Center);
                    break;
                case GizmoType.ScaleY:
                    transformMatrix = Matrix.Scaling(Math.Max(Math.Abs(Box.Height) / 6f, 1f)) * Matrix.Translation(0f, Math.Abs(Box.Height) + 1f, 0f) * Rotation * Matrix.Translation(Box.Center);
                    break;
                case GizmoType.ScaleZ:
                    transformMatrix = Matrix.Scaling(Math.Max(Math.Abs(Box.Depth) / 6f, 1f)) * Matrix.Translation(0f, 0f, Math.Abs(Box.Depth) + 1f) * Rotation * Matrix.Translation(Box.Center);
                    break;
                case GizmoType.ScaleAll:
                    transformMatrix = Matrix.Scaling(Box.Size.Length() / 8f) * Rotation * Matrix.Translation(Box.Center);
                    break;
            }

            for (int i = 0; i < SharpRenderer.cubeVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.cubeVertices[i], transformMatrix);
            boundingBox = BoundingBox.FromPoints(vertices);
        }

        public override SharpMesh Mesh => SharpRenderer.Cube;

        protected override List<Models.Triangle> triangleList => SharpRenderer.cubeTriangles;
    }
}