//-------------------------------------------------------------------------------------------------
// Code from Wintellect TEN event. http://www.wintellect.com/ten
//-------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.SoftBytes.ComponentModel
{
    public enum CloneOption
    {
        UseCodeGen,
        UseSerialization
    }

    public static class ObjectCloner
    {
        public static T MakeClone<T>(
            this T objectGraph,
            CloneOption cloneOption = CloneOption.UseCodeGen)
        {
            switch (cloneOption)
            {
                case CloneOption.UseCodeGen:
                    Func<T, T> targetCloner = MakeCloner<T>();
                    return targetCloner(objectGraph);
                case CloneOption.UseSerialization:
                    // Create a graph of objects to serialize them to the stream.
                    using (Stream stream = SerializeToMemory(objectGraph))
                    {
                        // Reset everything.
                        stream.Position = 0;

                        // Deserialize the objects and prove it worked.
                        T result = DeserializeFromMemory<T>(stream);
                        
                        return result;
                    }

                default:
                    throw new ArgumentException("CloneOption is not supported.", "cloneOption");
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "clone", Justification = "Required, else a System.InvalidProgramException is thrown (with message: Common Language Runtime detected an invalid program).")]
        private static Func<T, T> MakeCloner<T>()
        {
            Type type = typeof(T);
            var method = new DynamicMethod(
               String.Empty,   // Method name.
               type,           // Return type.
               new[] { type }, // Argument type(s).
               type);          // Owner (required to access non-public fields).

            ILGenerator il = method.GetILGenerator();
            LocalBuilder clone = il.DeclareLocal(type); // Declare a local of type T.

            // Create clone object and store in local 0.
            // ldtoken TYPE.
            // Call class System.Type System.Type::GetTypeFromHandle(valuetype System.RuntimeTypeHandle).
            // Call object System.Runtime.Serialization.FormatterServices::GetUninitializedObject(class System.Type).
            // Cast class TYPE.
            // stloc.0.
            il.Emit(OpCodes.Ldtoken, type);
            MethodInfo mi = typeof(Type).GetMethod(
                "GetTypeFromHandle",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(RuntimeTypeHandle) },
                null);
            il.EmitCall(OpCodes.Call, mi, null);
            mi = typeof(System.Runtime.Serialization.FormatterServices).GetMethod(
                "GetSafeUninitializedObject",
               BindingFlags.Public | BindingFlags.Static,
               null,
               new[] { typeof(Type) },
               null);
            il.EmitCall(OpCodes.Call, mi, null);
            il.Emit(OpCodes.Castclass, type);
            il.Emit(OpCodes.Stloc_0); // Store the result in the clone local variable.

            FieldInfo[] fields = type.GetFields(
                BindingFlags.Instance
                | BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.FlattenHierarchy);
            foreach (var field in fields)
            {
                il.Emit(OpCodes.Ldloc_0);      // Push address of clone object on virtual stack.
                il.Emit(OpCodes.Ldarg_0);      // Push address of source object on virtual stack.
                il.Emit(OpCodes.Ldfld, field); // Load field from source on stack.
                il.Emit(OpCodes.Stfld, field); // Store field from stack to clone.
            }

            il.Emit(OpCodes.Ldloc_0); // Return reference to clone object.
            il.Emit(OpCodes.Ret);

            return (Func<T, T>)method.CreateDelegate(typeof(Func<T, T>));
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposing is handled by the caller.")]
        private static MemoryStream SerializeToMemory<T>(T objectGraph)
        {
            MemoryStream stream = new MemoryStream();

            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(stream, objectGraph);

            return stream;
        }

        private static T DeserializeFromMemory<T>(Stream stream)
        {
            stream.Position = 0;

            BinaryFormatter formatter = new BinaryFormatter();

            return (T)formatter.Deserialize(stream);
        }
    }
}
