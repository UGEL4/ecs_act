
using Unity.Mathematics;

public static class TransformComponentExtenstion
{
    public static void TRS(this WorldTransformComponent self, float3 position, quaternion rotation, float3 scale, float4x4 parent)
    {
        self.value = float4x4.TRS(position, rotation, scale) * parent;
    }

    public static float4x4 TRS(float3 position, quaternion rotation, float3 scale, float4x4 parent)
    {
        return float4x4.TRS(position, rotation, scale) * parent;
    }

    public static float3 TransformPoint(this WorldTransformComponent self, float3 point)
    {
        float4 result = math.mul(new float4(point, 1.0f), self.value);
        return result.xyz;
    }

    public static float3 TransformPoint(this float4x4 self, float3 point)
    {
        float4 result = math.mul(new float4(point, 1.0f), self);
        return result.xyz;
    }

    public static float3 TransformDir(this WorldTransformComponent self, float3 dir)
    {
        float4 result = math.mul(new float4(dir, 0.0f), self.value);
        return result.xyz;
    }

    public static float3 EulerAngles(this quaternion self)
    {
        float x = self.value.x;
        float y = self.value.y;
        float z = self.value.z;
        float w = self.value.w;
        // 计算欧拉角（弧度）
        float sinr_cosp = 2 * (w * x + y * z);
        float cosr_cosp = 1 - 2 * (x * x + y * y);
        float roll_x = math.atan2(sinr_cosp, cosr_cosp);


        float sinp = 2 * (w * y - z * x);
        float half_pi = math.PI / 2;
        float signed_pitch_y = 0;
        if (sinp > 0)
        {
            signed_pitch_y = half_pi;
        }
        else
        {
            signed_pitch_y = -1 * half_pi;
        }
        float pitch_y = math.abs(sinp) >= 1 ? signed_pitch_y : math.asin(sinp); // 处理万向节死锁

        float siny_cosp = 2 * (w * z + x * y);
        float cosy_cosp = 1 - 2 * (y * y + z * z);
        float yaw_z = math.atan2(siny_cosp, cosy_cosp);
        return new float3(roll_x, pitch_y, yaw_z);
    }
}