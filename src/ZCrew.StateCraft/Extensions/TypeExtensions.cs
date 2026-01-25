namespace ZCrew.StateCraft;

/// <summary>
///     Provides extension members for <see cref="Type"/>.
/// </summary>
internal static class TypeExtensions
{
    private static readonly Dictionary<Type, string> typeAliases = new()
    {
        { typeof(bool), "bool" },
        { typeof(byte), "byte" },
        { typeof(sbyte), "sbyte" },
        { typeof(char), "char" },
        { typeof(decimal), "decimal" },
        { typeof(double), "double" },
        { typeof(float), "float" },
        { typeof(int), "int" },
        { typeof(uint), "uint" },
        { typeof(nint), "nint" },
        { typeof(nuint), "nuint" },
        { typeof(long), "long" },
        { typeof(ulong), "ulong" },
        { typeof(short), "short" },
        { typeof(ushort), "ushort" },
        { typeof(object), "object" },
        { typeof(string), "string" },
        { typeof(void), "void" },
    };

    extension(Type type)
    {
        /// <summary>
        ///     Gets a C#-style friendly name for the type, using language keywords for built-in types and proper syntax
        ///     for generics, arrays, and tuples.
        /// </summary>
        public string FriendlyName
        {
            get
            {
                // Check for built-in type aliases first
                if (typeAliases.TryGetValue(type, out var alias))
                {
                    return alias;
                }

                // Handle arrays (including jagged and multi-dimensional)
                if (type.IsArray)
                {
                    var elementType = type.GetElementType()!;
                    var rank = type.GetArrayRank();
                    var brackets = rank == 1 ? "[]" : $"[{new string(',', rank - 1)}]";
                    return elementType.FriendlyName + brackets;
                }

                // Handle generic types
                if (type.IsGenericType)
                {
                    var genericTypeDefinition = type.GetGenericTypeDefinition();
                    var genericArguments = type.GetGenericArguments();

                    // Handle Nullable<T>
                    if (genericTypeDefinition == typeof(Nullable<>))
                    {
                        return genericArguments[0].FriendlyName + "?";
                    }

                    // Handle ValueTuple<...>
                    if (IsValueTuple(genericTypeDefinition))
                    {
                        var args = GetValueTupleArgs(type);
                        return $"({string.Join(", ", args)})";
                    }

                    // Handle other generic types
                    var typeName = type.Name;
                    var backtickIndex = typeName.IndexOf('`');
                    if (backtickIndex > 0)
                    {
                        typeName = typeName[..backtickIndex];
                    }

                    var friendlyArgs = string.Join(", ", genericArguments.Select(type => type.FriendlyName));
                    return $"{typeName}<{friendlyArgs}>";
                }

                // Handle by-ref types
                if (type.IsByRef)
                {
                    var elementType = type.GetElementType()!;
                    return "ref " + elementType.FriendlyName;
                }

                // Handle pointer types
                if (type.IsPointer)
                {
                    var elementType = type.GetElementType()!;
                    return elementType.FriendlyName + "*";
                }

                // Default: return the type name
                return type.Name;
            }
        }
    }

    private static bool IsValueTuple(Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        var genericTypeDefinition = type.IsGenericTypeDefinition ? type : type.GetGenericTypeDefinition();

        return genericTypeDefinition == typeof(ValueTuple<>)
            || genericTypeDefinition == typeof(ValueTuple<,>)
            || genericTypeDefinition == typeof(ValueTuple<,,>)
            || genericTypeDefinition == typeof(ValueTuple<,,,>)
            || genericTypeDefinition == typeof(ValueTuple<,,,,>)
            || genericTypeDefinition == typeof(ValueTuple<,,,,,>)
            || genericTypeDefinition == typeof(ValueTuple<,,,,,,>)
            || genericTypeDefinition == typeof(ValueTuple<,,,,,,,>);
    }

    private static List<string> GetValueTupleArgs(Type type)
    {
        var args = new List<string>();
        var tupleType = type;
        while (tupleType != null)
        {
            var genericArgs = tupleType.GetGenericArguments();

            // For ValueTuple<T1..T7, TRest>, the 8th argument is the rest tuple
            if (genericArgs.Length == 8 && tupleType.GetGenericTypeDefinition() == typeof(ValueTuple<,,,,,,,>))
            {
                // Add the first 7 arguments
                for (var i = 0; i < 7; i++)
                {
                    args.Add(genericArgs[i].FriendlyName);
                }

                // Process the TRest tuple
                tupleType = genericArgs[7];
            }
            else
            {
                // Add all arguments
                foreach (var arg in genericArgs)
                {
                    args.Add(arg.FriendlyName);
                }
                tupleType = null;
            }
        }
        return args;
    }
}
