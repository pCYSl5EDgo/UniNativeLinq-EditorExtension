using Mono.Cecil;
using Mono.Cecil.Cil;

namespace UniNativeLinq.Editor.CodeGenerator
{
    public static class SpecialArgumentProcessorHelper
    {
        public static ILProcessor LdConvArg(this ILProcessor processor, GenericInstanceType enumerableGenericInstanceType, int specialArgumentIndex)
        {
            var variables = processor.Body.Variables;
            var variableIndex = variables.Count;
            variables.Add(new VariableDefinition(enumerableGenericInstanceType));
            var constructor = enumerableGenericInstanceType.FindMethod(".ctor", x => x.Parameters.Count == 1);
            return processor
                .LdLocA(variableIndex)
                .Dup()
                .LdArg(specialArgumentIndex)
                .Call(constructor);
        }
    }
}