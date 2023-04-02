using System;

namespace LevelGeneration
{
    public static class ArrayExtensions
    {
        public static T Choose<T>(this T[] array, Random random)
        {
            return array[random.Next(array.Length)];
        }
    }
}