﻿using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaPointer : DynaBase
    {
        public override string Note => "Version is always 1";
        
        public DynaPointer() : base()
        {
        }

        public DynaPointer(IEnumerable<byte> enumerable) : base(enumerable)
        {
            _position.X = Switch(BitConverter.ToSingle(Data, 0));
            _position.Y = Switch(BitConverter.ToSingle(Data, 4));
            _position.Z = Switch(BitConverter.ToSingle(Data, 8));
            _yaw = Switch(BitConverter.ToSingle(Data, 12));
            _pitch = Switch(BitConverter.ToSingle(Data, 16));
            _roll = Switch(BitConverter.ToSingle(Data, 20));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(_position.X)));
            list.AddRange(BitConverter.GetBytes(Switch(_position.Y)));
            list.AddRange(BitConverter.GetBytes(Switch(_position.Z)));
            list.AddRange(BitConverter.GetBytes(Switch(_yaw)));
            list.AddRange(BitConverter.GetBytes(Switch(_pitch)));
            list.AddRange(BitConverter.GetBytes(Switch(_roll)));
            return list.ToArray();
        }

        private Vector3 _position;
        private float _yaw;
        private float _pitch;
        private float _roll;

        [Category("Pointer"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionX
        {
            get => _position.X;
            set
            {
                _position.X = value;
                CreateTransformMatrix();
            }
        }
        [Category("Pointer"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionY
        {
            get => _position.Y;
            set
            {
                _position.Y = value;
                CreateTransformMatrix();
            }
        }
        [Category("Pointer"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float PositionZ
        {
            get => _position.Z;
            set
            {
                _position.Z = value;
                CreateTransformMatrix();
            }
        }
        [Category("Pointer"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float Yaw
        {
            get => _yaw;
            set
            {
                _yaw = value;
                CreateTransformMatrix();
            }
        }
        [Category("Pointer"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float Pitch
        {
            get => _pitch;
            set
            {
                _pitch = value;
                CreateTransformMatrix();
            }
        }
        [Category("Pointer"), Browsable(true), TypeConverter(typeof(FloatTypeConverter))]
        public override float Roll
        {
            get => _roll;
            set
            {
                _roll = value;
                CreateTransformMatrix();
            }
        }

        public override bool IsRenderableClickable => true;

        private Matrix world;
        private BoundingBox boundingBox;
        private Vector3[] vertices;
        protected RenderWareFile.Triangle[] triangles;

        public override void CreateTransformMatrix()
        {
            world = Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll) * Matrix.Translation(_position);

            vertices = new Vector3[SharpRenderer.pyramidVertices.Count];
            for (int i = 0; i < SharpRenderer.pyramidVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.pyramidVertices[i], world);
            boundingBox = BoundingBox.FromPoints(vertices);

            triangles = new RenderWareFile.Triangle[SharpRenderer.pyramidTriangles.Count];
            for (int i = 0; i < SharpRenderer.pyramidTriangles.Count; i++)
            {
                triangles[i] = new RenderWareFile.Triangle((ushort)SharpRenderer.pyramidTriangles[i].materialIndex,
                    (ushort)SharpRenderer.pyramidTriangles[i].vertex1, (ushort)SharpRenderer.pyramidTriangles[i].vertex2, (ushort)SharpRenderer.pyramidTriangles[i].vertex3);
            }
        }

        public override void Draw(SharpRenderer renderer, bool isSelected)
        {
            renderer.DrawPyramid(world, isSelected, 1f);
        }

        public override BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public override float GetDistance(Vector3 cameraPosition)
        {
            return Vector3.Distance(cameraPosition, new Vector3(PositionX, PositionY, PositionZ));
        }

        public override float? IntersectsWith(Ray ray)
        {
            if (ray.Intersects(ref boundingBox, out float distance))
                return TriangleIntersection(ray, distance);
            return null;
        }

        private float? TriangleIntersection(Ray r, float initialDistance)
        {
            bool hasIntersected = false;
            float smallestDistance = 1000f;

            foreach (RenderWareFile.Triangle t in triangles)
                if (r.Intersects(ref vertices[t.vertex1], ref vertices[t.vertex2], ref vertices[t.vertex3], out float distance))
                {
                    hasIntersected = true;

                    if (distance < smallestDistance)
                        smallestDistance = distance;
                }

            if (hasIntersected)
                return smallestDistance;
            else return null;
        }

        public override BoundingSphere GetGizmoCenter()
        {
            BoundingSphere boundingSphere = BoundingSphere.FromBox(boundingBox);
            boundingSphere.Radius *= 0.9f;
            return boundingSphere;
        }
    }
}