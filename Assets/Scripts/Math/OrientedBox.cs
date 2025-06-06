using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace ACTGame
{
    [Serializable]
    public struct OrientedBox
    {
        public float3 center;
        //public float3 eulerAngles;
        public float3 extents;
        public quaternion rotation;

        public static bool CheckOverlap(OrientedBox a, OrientedBox b)
        {
            float3[] axes = GetTestAxes(a, b);

            foreach (float3 axis in axes)
            {
                if (!OverlapOnAxis(a, b, axis))
                    return false;
            }
            return true;
        }

        // 获取15个测试轴（6个面法线 + 9个边叉积）
        private static float3[] GetTestAxes(OrientedBox a, OrientedBox b)
        {
            float3[] axes = new float3[15];
            float3x3 rotA = math.float3x3(a.rotation);
            float3x3 rotB = math.float3x3(b.rotation);

            // 第一个OBB的3个轴
            axes[0] = rotA.c0;
            axes[1] = rotA.c1;
            axes[2] = rotA.c2;

            // 第二个OBB的3个轴
            axes[3] = rotB.c0;
            axes[4] = rotB.c1;
            axes[5] = rotB.c2;

            // 9个边叉积轴
            int index = 6;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    axes[index++] = math.cross(rotA[i], rotB[j]);
                }
            }
            return axes;
        }

        // 检查在指定轴上的投影是否重叠
        private static bool OverlapOnAxis(OrientedBox a, OrientedBox b, float3 axis)
        {
            GetProjectionInterval(a, axis, out float minA, out float maxA);
            GetProjectionInterval(b, axis, out float minB, out float maxB);
            return (minB <= maxA) && (minA <= maxB);
        }

        // 高效计算投影区间（使用数学优化）
        private static void GetProjectionInterval(OrientedBox box, float3 axis, out float min, out float max)
        {
            float3 center = box.center;
            float3x3 rot = math.float3x3(box.rotation);

            // 计算投影中心和半长
            float projCenter = math.dot(center, axis);
            float projExtent =
                math.abs(math.dot(rot.c0, axis)) * box.extents.x +
                math.abs(math.dot(rot.c1, axis)) * box.extents.y +
                math.abs(math.dot(rot.c2, axis)) * box.extents.z;

            min = projCenter - projExtent;
            max = projCenter + projExtent;
        }

        public static OrientedBox GetWorldSpaceOBB(OrientedBox localBox, float4x4 localToWorldMatrix)
        {
            float3 scale = new float3(
                math.length(localToWorldMatrix.c0.xyz),
                math.length(localToWorldMatrix.c1.xyz),
                math.length(localToWorldMatrix.c2.xyz)
            );
            return new OrientedBox
            {
                center = math.transform(localToWorldMatrix, localBox.center),
                rotation = new quaternion(localToWorldMatrix),
                extents = localBox.extents * scale
            };
        }
    }

    public static class OBBChecker
    {
        // 判断两个 OBB 是否相交
        public static bool CheckOBBIntersection(OrientedBox a, OrientedBox b)
        {
            // 获取两个盒子的变换矩阵
            float4x4 matrixA = GetOBBWorldMatrix(a);
            float4x4 matrixB = GetOBBWorldMatrix(b);

            // 分离轴定理（SAT）检测
            return SatTest(matrixA, matrixB);
        }

        private static float3[] GetOrthogonalAxes(quaternion rotation)
        {
            return new float3[] {
                math.mul(rotation, new float3(1, 0, 0)),
                math.mul(rotation, new float3(0, 1, 0)),
                math.mul(rotation, new float3(0, 0, 1))
            };
        }

        private static bool SatTest(float4x4 matrixA, float4x4 matrixB)
        {
            // 获取OBB的轴向量（排除缩放影响）
            float3[] axesA = GetNormalizedAxes(matrixA);
            float3[] axesB = GetNormalizedAxes(matrixB);

            // 生成15条候选分离轴（优化后实际为15条）
            List<float3> testAxes = new List<float3>();
            testAxes.AddRange(axesA);    // 3轴
            testAxes.AddRange(axesB);    // +3轴
            AddCrossAxes(axesA, axesB, testAxes); // +9轴

            // 遍历所有轴进行投影重叠测试
            foreach (float3 axis in testAxes)
            {
                if (!OverlapOnAxis(matrixA, matrixB, axis))
                    return false; // 存在分离轴，不相交
            }
            return true; // 所有轴重叠，发生碰撞
        }

        // 获取变换矩阵的归一化轴向
        private static float3[] GetNormalizedAxes(float4x4 matrix)
        {
            return new float3[] {
                math.normalize(matrix.c0.xyz), // X轴
                math.normalize(matrix.c1.xyz), // Y轴
                math.normalize(matrix.c2.xyz)  // Z轴
            };
        }

        // 生成两OBB轴的叉乘轴（去零优化）
        private static void AddCrossAxes(float3[] a, float3[] b, List<float3> output)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    float3 cross = math.cross(a[i], b[j]);
                    if (!math.all(cross == 0)) // 跳过平行轴
                    {
                        output.Add(math.normalize(cross));
                    }
                }
            }
        }

        // 投影重叠检测
        private static bool OverlapOnAxis(float4x4 a, float4x4 b, float3 axis)
        {
            // 计算A和B的顶点投影区间
            GetProjectionInterval(a, axis, out float minA, out float maxA);
            GetProjectionInterval(b, axis, out float minB, out float maxB);

            // 检查区间重叠
            return (minB <= maxA) && (minA <= maxB);
        }

        // 获取OBB在指定轴上的投影区间
        private static void GetProjectionInterval(float4x4 matrix, float3 axis, out float min, out float max)
        {
            // 计算本地顶点（基于size的±1范围）
            float3[] localVertices = new float3[8];
            float3 halfSize = matrix.c3.xyz * 0.5f;
            for (int i = 0; i < 8; i++)
            {
                localVertices[i] = new float3(
                    (i & 1) == 0 ? -halfSize.x : halfSize.x,
                    (i & 2) == 0 ? -halfSize.y : halfSize.y,
                    (i & 4) == 0 ? -halfSize.z : halfSize.z
                );
            }

            // 变换顶点到世界坐标并投影
            min = float.MaxValue;
            max = float.MinValue;
            foreach (float3 v in localVertices)
            {
                float3 worldPos = math.transform(matrix, v);
                float proj = math.dot(worldPos, axis);
                min = math.min(proj, min);
                max = math.max(proj, max);
            }
        }

        // 构造 OBB 的世界变换矩阵
        private static float4x4 GetOBBWorldMatrix(OrientedBox box)
        {
            return float4x4.TRS(box.center, box.rotation, box.extents);
        }

        public static float3[] GetCorners(float3 pos, float3 center, float3 size, float3 rotation)
        {
            float3 halfSizeAttackBox = size * 0.5f;
            quaternion rot = quaternion.Euler(rotation);
            float3[] corners = new float3[8];
            corners[0] = pos + math.mul(rot, center + new float3(halfSizeAttackBox.x, halfSizeAttackBox.y, halfSizeAttackBox.z));
            corners[1] = pos + math.mul(rot, center + new float3(halfSizeAttackBox.x, halfSizeAttackBox.y, -halfSizeAttackBox.z));
            corners[2] = pos + math.mul(rot, center + new float3(halfSizeAttackBox.x, -halfSizeAttackBox.y, halfSizeAttackBox.z));
            corners[3] = pos + math.mul(rot, center + new float3(halfSizeAttackBox.x, -halfSizeAttackBox.y, -halfSizeAttackBox.z));
            corners[4] = pos + math.mul(rot, center + new float3(-halfSizeAttackBox.x, halfSizeAttackBox.y, halfSizeAttackBox.z));
            corners[5] = pos + math.mul(rot, center + new float3(-halfSizeAttackBox.x, halfSizeAttackBox.y, -halfSizeAttackBox.z));
            corners[6] = pos + math.mul(rot, center + new float3(-halfSizeAttackBox.x, -halfSizeAttackBox.y, halfSizeAttackBox.z));
            corners[7] = pos + math.mul(rot, center + new float3(-halfSizeAttackBox.x, -halfSizeAttackBox.y, -halfSizeAttackBox.z));
            return corners;
        }

    }
}