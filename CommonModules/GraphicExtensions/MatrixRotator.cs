using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Numerics;

namespace GraphicExtensions
{
    public class MatrixRotator
    {
        public static Matrix4 GetRotationMatrixX(double angle)
        {
            if (angle == 0.0)
            {
                return Matrix4.I;
            }
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);
            return new Matrix4(new float[4, 4] {
        { 1.0f, 0.0f, 0.0f, 0.0f }, 
        { 0.0f, cos, -sin, 0.0f }, 
        { 0.0f, sin, cos, 0.0f }, 
        { 0.0f, 0.0f, 0.0f, 1.0f } });
        }

        public static Matrix4 GetRotationMatrixY(double angle)
        {
            if (angle == 0.0)
            {
                return Matrix4.I;
            }
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);
            return new Matrix4(new float[4, 4] {
        { cos, 0.0f, sin, 0.0f }, 
        { 0.0f, 1.0f, 0.0f, 0.0f }, 
        { -sin, 0.0f, cos, 0.0f }, 
        { 0.0f, 0.0f, 0.0f, 1.0f } });
        }

        public static Matrix4 GetRotationMatrixZ(double angle)
        {
            if (angle == 0.0)
            {
                return Matrix4.I;
            }
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);
            return new Matrix4(new float[4, 4] {
        { cos, -sin, 0.0f, 0.0f }, 
        { sin, cos, 0.0f, 0.0f }, 
        { 0.0f, 0.0f, 1.0f, 0.0f }, 
        { 0.0f, 0.0f, 0.0f, 1.0f } });
        }

        public static Matrix4 GetRotationMatrix(double ax, double ay, double az)
        {
            Matrix4 my = null;
            Matrix4 mz = null;
            Matrix4 result = null;
            if (ax != 0.0)
            {
                result = GetRotationMatrixX(ax);
            }
            if (ay != 0.0)
            {
                my = GetRotationMatrixY(ay);
            }
            if (az != 0.0)
            {
                mz = GetRotationMatrixZ(az);
            }
            if (my != null)
            {
                if (result != null)
                {
                    result *= my;
                }
                else
                {
                    result = my;
                }
            }
            if (mz != null)
            {
                if (result != null)
                {
                    result *= mz;
                }
                else
                {
                    result = mz;
                }
            }
            if (result != null)
            {
                return result;
            }
            else
            {
                return Matrix4.I;
            }
        }

        public static Matrix4 GetRotationMatrix(Vector3 axis, double angle)
        {
            if (angle == 0.0)
            {
                return Matrix4.I;
            }

            float x = axis.x;
            float y = axis.y;
            float z = axis.z;
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);
            float xx = x * x;
            float yy = y * y;
            float zz = z * z;
            float xy = x * y;
            float xz = x * z;
            float yz = y * z;

            float[,] matrix = new float[4, 4];

            matrix[0, 0] = xx + (1 - xx) * cos;
            matrix[1, 0] = xy * (1 - cos) + z * sin;
            matrix[2, 0] = xz * (1 - cos) - y * sin;
            matrix[3, 0] = 0.0f;

            matrix[0, 1] = xy * (1 - cos) - z * sin;
            matrix[1, 1] = yy + (1 - yy) * cos;
            matrix[2, 1] = yz * (1 - cos) + x * sin;
            matrix[3, 1] = 0.0f;

            matrix[0, 2] = xz * (1 - cos) + y * sin;
            matrix[1, 2] = yz * (1 - cos) - x * sin;
            matrix[2, 2] = zz + (1 - zz) * cos;
            matrix[3, 2] = 0.0f;

            matrix[3, 0] = 0.0f;
            matrix[3, 1] = 0.0f;
            matrix[3, 2] = 0.0f;
            matrix[3, 3] = 1.0f;

            return new Matrix4(matrix);
        }

        /// <param name="source">Should be normalized</param>
        /// <param name="destination">Should be normalized</param>
        public static Matrix4 GetRotationMatrix(Vector3 source, Vector3 destination)
        {
            Vector3 rotaxis = Vector3.CrossProduct(source, destination);
            if (rotaxis != Vector3.Zero)
            {
                rotaxis.Normalize();
                float cos = source.DotProduct(destination);
                double angle = Math.Acos(cos);
                return GetRotationMatrix(rotaxis, angle);
            }
            else
            {
                return Matrix4.I;
            }
        }
    }
}
