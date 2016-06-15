using System;

namespace VCSJones.FiddlerCert
{
    public struct ArrayOffset<T>
    {
        private readonly T[] _array;
        private readonly int _offset;

        public ArrayOffset(T[] array, int offset)
        {
            _array = array;
            _offset = offset;
        }

        public void CopyTo(T[] destination, int destinationOffset, int length)
        {
            Array.Copy(_array, _offset, destination, destinationOffset, length);
        }

        public int Length => _array.Length - _offset;

        public T this[int index] => _array[index + _offset];

        public static ArrayOffset<T> operator +(ArrayOffset<T> array, int addOffset)
        {
            if (addOffset > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(addOffset));
            }
            return new ArrayOffset<T>(array._array, checked(array._offset + addOffset));
        }

        public static ArrayOffset<T> operator -(ArrayOffset<T> array, int subOffset)
        {
            if (array._offset - subOffset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(subOffset));
            }
            return new ArrayOffset<T>(array._array, array._offset - subOffset);
        }
    }
}
