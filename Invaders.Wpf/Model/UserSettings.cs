using System;
using System.Runtime.Serialization;

namespace Invaders.Wpf.Model
{
    [Serializable]
    [DataContract]
    public class UserSettings : ICloneable<UserSettings>
    {
        public UserSettings()
        {
            IsScanLineActive = true;
            IsMusicActive = true;
            IsAnimationActive = true;
        }

        [DataMember] public bool IsScanLineActive { get; private set; }

        [DataMember] public bool IsMusicActive { get; private set; }

        [DataMember] public bool IsAnimationActive { get; private set; }

        public UserSettings Clone()
        {
            return new UserSettings()
            {
                IsScanLineActive = IsScanLineActive,
                IsMusicActive = IsMusicActive,
                IsAnimationActive = IsAnimationActive
            };
        }
    }
}
