using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Chasm.Dispatching
{
    internal struct CompiledDispatchImpl
    {
        public Entry[] _entries = new Entry[8];
        public int _count;

        public CompiledDispatchImpl() { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(object? instance, MethodInfo method)
        {
            Entry entry = new() { Instance = instance, Method = method };

            Entry[] entries = _entries;
            int count = _count;

            if (count >= entries.Length)
                entries = Grow();

            _count = count + 1;
            entries[count] = entry;
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        private Entry[] Grow()
        {
            Entry[] oldEntries = _entries;
            int newCapacity = 2 * oldEntries.Length;
            Entry[] newEntries = new Entry[newCapacity];

            Array.Copy(oldEntries, newEntries, _count);
            _entries = newEntries;
            return newEntries;
        }

        public bool Remove(object? instance, MethodInfo method)
        {
            Entry entry = new() { Instance = instance, Method = method };

            int index = Array.IndexOf(_entries, entry);
            if (index == -1) return false;

            _count--;
            if (index < _count)
                Array.Copy(_entries, index + 1, _entries, index, _count - index);
            _entries[_count] = default;
            return true;
        }

        public readonly Delegate? CompileDispatch(Type delegateType, Type argType, ref int version, int started)
        {
            if (Volatile.Read(ref version) != started) return null;

            DynamicMethod dispatch = new("Dispatch", typeof(void), [typeof(Entry[]), argType]);
            ILGenerator il = dispatch.GetILGenerator();

            Entry[] entries = _entries;
            for (int i = 0, count = _count; i < count; i++)
            {
                MethodInfo method = entries[i].Method;
                if (Volatile.Read(ref version) != started) return null;
                ParameterInfo[] pars = method.GetParameters();

                // this.Update() | this.Update(arg) | Update(instance, arg)
                if (!method.IsStatic || pars.Length == 2)
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Ldelema, typeof(Entry));
                    il.Emit(OpCodes.Ldfld, entryInstanceField);
                }
                // this.Update(arg) | Update(instance, arg)
                if (pars.Length >= 1)
                    il.Emit(OpCodes.Ldarg_1);

                il.Emit(OpCodes.Call, method);

                if (method.ReturnType != typeof(void))
                    il.Emit(OpCodes.Pop);
            }
            il.Emit(OpCodes.Ret);

            if (Volatile.Read(ref version) != started) return null;

            return dispatch.CreateDelegate(delegateType);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public readonly void DispatchNaiveDynamic(object?[] args)
        {
            Entry[] entries = _entries;
            for (int i = 0, count = _count; i < count; i++)
            {
                Entry entry = entries[i];
                MethodInfo method = entry.Method;
                method.Invoke(entry.Instance, method.GetParameters().Length == 1 ? args : []);
            }
        }

        private static readonly FieldInfo entryInstanceField = typeof(Entry).GetField(nameof(Entry.Instance))!;

        public struct Entry : IEquatable<Entry>
        {
            public object? Instance;
            public MethodInfo Method;

            public readonly bool Equals(Entry other)
                => Instance == other.Instance && Method == other.Method;
            public readonly override bool Equals(object? obj)
                => obj is Entry other && Equals(other);
            public readonly override int GetHashCode()
                => (Instance?.GetHashCode() ?? 0) ^ Method.GetHashCode();
        }
    }
}
