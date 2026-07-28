using System;

namespace AfkManager.Features
{
    internal sealed class AfkPlayer
    {
        public AfkPlayer(float x, float y, float z, float pitch, float yaw)
        {
            Reset(x, y, z, pitch, yaw);
        }

        public float LastX { get; private set; }
        public float LastY { get; private set; }
        public float LastZ { get; private set; }
        public float LastPitch { get; private set; }
        public float LastYaw { get; private set; }
        public DateTime LastActivity { get; set; }
        public bool WarningSent { get; set; }

        public void UpdateSnapshot(float x, float y, float z, float pitch, float yaw)
        {
            LastX = x;
            LastY = y;
            LastZ = z;
            LastPitch = pitch;
            LastYaw = yaw;
        }

        public void Reset(float x, float y, float z, float pitch, float yaw)
        {
            UpdateSnapshot(x, y, z, pitch, yaw);
            LastActivity = DateTime.UtcNow;
            WarningSent = false;
        }
    }
}
