using System;
using UnityEngine;

namespace AfkManager.Features
{
    internal sealed class AfkPlayer
    {
        public AfkPlayer(Vector3 position, Quaternion rotation)
        {
            LastPosition = position;
            LastRotation = rotation;
            LastActivity = DateTime.UtcNow;
        }

        public Vector3 LastPosition { get; set; }
        public Quaternion LastRotation { get; set; }
        public DateTime LastActivity { get; set; }
        public bool WarningSent { get; set; }

        public void Reset(Vector3 position, Quaternion rotation)
        {
            LastPosition = position;
            LastRotation = rotation;
            LastActivity = DateTime.UtcNow;
            WarningSent = false;
        }
    }
}
