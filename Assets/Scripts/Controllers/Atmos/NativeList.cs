using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Collections;

namespace Assets.Scripts.Controllers.Atmos
{
    struct NativeList<T> where T: struct, IComparable 
    {
        private NativeArray<T> _memory;

        private int _lenght;
        private int _capacity;

        private void IncreaseCapasity()
        {
            int nCapasity;
            if (_capacity == 0)
                nCapasity = 1;
            else
                nCapasity = _capacity * 2;

            NativeArray<T> tmp = new NativeArray<T>(nCapasity, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

            for (int i = 0; i < _lenght; i++)
            {
                tmp[i] = _memory[i];
            }

            if(_lenght > 0)
                _memory.Dispose();

            _memory = tmp;
            _capacity = nCapasity;
        }

        private void DecreaseCapacity()
        {
            int nCapasity;
            nCapasity = _capacity / 2;

            NativeArray<T> tmp = new NativeArray<T>(nCapasity, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            
            for (int i = 0; i < _lenght; i++)
            {
                tmp[i] = _memory[i];
            }

            if (_lenght > 0)
                _memory.Dispose();
            _memory = tmp;
            _capacity = nCapasity;
        }

        public void Add(T value)
        {
            if (_lenght + 1 > _capacity)
            {
                IncreaseCapasity();
            }
            _memory[_lenght] = value;

            _lenght++;
        }

        public int IndexOf(T value)
        {
            for (int i = 0; i < _lenght; i++)
            {
                if (_memory[i].CompareTo(value) == 0)
                    return i;
            }

            return -1;
        }

        public void Remove(T value)
        {
            RemoveAt(IndexOf(value));
        }

        public void RemoveAt(int index)
        {
            if(index >= _lenght || index < 0)
                return;

            for (int i = index; i < _lenght - 1; i++)
            {
                _memory[i] = _memory[i + 1];
            }

            _lenght--;

            if (_lenght == _capacity / 2) 
                DecreaseCapacity();
        }

        public T this[int i]
        {
            get { return _memory[i]; }
            set { _memory[i] = value; }
        }

        public NativeList(int capacity)
        {
            _lenght = 0;
            _capacity = capacity;
            _memory = new NativeArray<T>(_capacity, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        }

        public int Length
        {
            get { return _lenght; }
        }

        public int Capacity
        {
            get { return _capacity; }
        }

        public override string ToString()
        {
            string msg = "Length: {0}, Capacity: {1}, Values: ";
            for (int i = 0; i < _lenght; i++)
            {
                msg += _memory[i].ToString() + ' ';
            }

            return String.Format(msg, _lenght, _capacity);
        }
    }
}
