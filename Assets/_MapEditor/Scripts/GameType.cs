using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._MapEditor.Scripts
{
    public class GameType
    {
        private Type _type;

        private List<GameType> ChildGameTypes;

        public GameType(Type type)
        {
            _type = type;
            ChildGameTypes = new List<GameType>();
        }

        public GameType this[int i]
        {
            get { return ChildGameTypes[i]; }
        }

        public void Add(GameType type)
        {
            ChildGameTypes.Add(type);
        }

        public Type Type => _type;

        public override string ToString()
        {
            if (_type.FullName != null) return _type.FullName;
            return "No name";
        }

        public int Length => ChildGameTypes.Count;
    }
}
