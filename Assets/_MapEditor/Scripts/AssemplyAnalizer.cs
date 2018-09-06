using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Assets._MapEditor.Scripts
{
    class AssemplyAnalizer
    {
        public static GameType GetTypeTree(Type typeBase)
        {
            return CreateTree(new GameType(typeBase));
        }

        private static GameType CreateTree(GameType typeBase)
        {
            IEnumerable<Type> list = Assembly.GetAssembly(typeBase.Type).GetTypes().Where(type => (type.IsSubclassOf(typeBase.Type) && type.BaseType == typeBase.Type));

            foreach (var type in list)
            {
                GameType child = CreateTree(new GameType(type));
                typeBase.Add(child);
            }

            return typeBase;
        }
    }
}
