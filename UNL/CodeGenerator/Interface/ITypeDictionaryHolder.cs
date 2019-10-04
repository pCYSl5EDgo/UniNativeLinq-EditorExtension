using System.Collections.Generic;
using Mono.Cecil;

namespace UniNativeLinq.Editor.CodeGenerator
{
    public interface ITypeDictionaryHolder
    {
        Dictionary<string, TypeDefinition> Dictionary { set; }
    }
}
