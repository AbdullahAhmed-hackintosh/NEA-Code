using System;
using System.Reflection;

namespace FistOfTheFree.Guns
{
    public class Utilities // Allows for all the variables within the code to be copied
    {
        public static void CopyValues<T>(T Base, T Copy) 
        {
            Type type = Base.GetType();
            foreach(FieldInfo field in type.GetFields()) 
            {
                field.SetValue(Copy, field.GetValue(Base)); // Gets all the values for each variables and copies their base values
            }
        }
    }
}
