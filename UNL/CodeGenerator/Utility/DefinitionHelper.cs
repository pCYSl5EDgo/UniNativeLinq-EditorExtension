using Mono.Cecil;

namespace UniNativeLinq.Editor.CodeGenerator
{
    public static class DefinitionHelper
    {
        public static TypeDefinition ToDefinition(this GenericInstanceType genericInstanceType)
            => genericInstanceType.ElementType.ToDefinition();

        public static TypeDefinition ToDefinition(this TypeReference typeReference)
        {
            switch (typeReference)
            {
                case TypeDefinition answer: return answer;
                case GenericInstanceType answer: return answer.ToDefinition();
                default: return typeReference.Resolve();
            }
        }
    }
}