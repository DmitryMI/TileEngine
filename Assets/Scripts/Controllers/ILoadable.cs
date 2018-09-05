using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Controllers
{
    public interface ILoadable
    {
        void RegistrateDataProvider(IServerDataProvider provider);
        void OnGameLoaded(IServerDataProvider dataProvider);
    }
}
