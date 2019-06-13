using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Controllers;

namespace Assets.Scripts.GameMechanics
{
    class StackCollectionEnumerator : IEnumerator<TileObjectStack>
    {
        private int _current = 0;
        private StackCollection _collection;
        public bool MoveNext()
        {
            if (_collection.Length - 1 == _current)
                return false;
            _current++;
            return true;
        }

        public void Reset()
        {
            _current = 0;
        }

        public TileObjectStack Current => _collection[_current];

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public void Dispose()
        {
            
        }

        public StackCollectionEnumerator(StackCollection collection)
        {
            _collection = collection;
        }
    }
}