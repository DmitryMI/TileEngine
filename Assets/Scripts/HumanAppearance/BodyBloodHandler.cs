using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.HumanAppearance
{
    class BodyBloodHandler : BodyEffectHandler
    {
        protected override void Update()
        {
            HeadEffect = Humanoid.Health.NetHealthData.HeadSkinDamage > 0;
            ChestEffect = Humanoid.Health.NetHealthData.ChestSkinDamage > 0;
            LeftArmEffect = Humanoid.Health.NetHealthData.LeftArmSkinDamage > 0;
            RightArmEffect = Humanoid.Health.NetHealthData.RightArmSkinDamage > 0;
            LeftLegEffect = Humanoid.Health.NetHealthData.LeftLegSkinDamage > 0;
            RightLegEffect = Humanoid.Health.NetHealthData.RightLegSkinDamage > 0;

            base.Update();
        }
    }
}
