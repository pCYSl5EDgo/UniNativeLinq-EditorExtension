using Mono.Cecil;
using Mono.Cecil.Cil;

namespace UniNativeLinq.Editor.CodeGenerator
{
    public static class SumHelper
    {
        public static void GenerateSum_Array(this TypeReference returnType, MethodDefinition method, TypeReference baseEnumerable)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(returnType));
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Int32));

            var condition = Instruction.Create(OpCodes.Ldloc_1);
            var loopStart = Instruction.Create(OpCodes.Ldloc_0);
            body.GetILProcessor()
                .LdC(0)
                .StLoc(1)
                .BrS(condition)
                .Add(loopStart)
                .LdArg(0)
                .LdLoc(1)
                .LdElem(returnType)
                .Add()
                .StLoc(0)
                .LdLoc(1)
                .LdC(1)
                .Add()
                .StLoc(1)
                .Add(condition)
                .LdArg(0)
                .LdLen()
                .ConvI4()
                .BltS(loopStart)
                .LdLoc(0)
                .Ret();
        }

        public static void GenerateSum_NativeArray(this TypeReference returnType, MethodDefinition method, TypeReference nativeArray)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(nativeArray))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });
            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(returnType));
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Int32));

            var condition = Instruction.Create(OpCodes.Ldloc_1);
            var loopStart = Instruction.Create(OpCodes.Ldloc_0);

            var getElement = nativeArray.FindMethod("get_Item");
            var getLength = nativeArray.FindMethod("get_Length");
            body.GetILProcessor()
                .LdC(0)
                .StLoc(1)
                .BrS(condition)
                .Add(loopStart)
                .LdArg(0)
                .LdLoc(1)
                .Call(getElement)
                .Add()
                .StLoc(0)
                .LdLoc(1)
                .LdC(1)
                .Add()
                .StLoc(1)
                .Add(condition)
                .LdArg(0)
                .Call(getLength)
                .ConvI4()
                .BltS(loopStart)
                .LdLoc(0)
                .Ret();
        }

        public static void GenerateSum_IRefEnumerable(this TypeReference returnType, MethodDefinition method, TypeReference enumerable, TypeReference enumerator)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });
            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(returnType));
            var enumeratorVariable = new VariableDefinition(enumerator);
            body.Variables.Add(enumeratorVariable);
            body.Variables.Add(new VariableDefinition(returnType));

            var @return = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var loopStart = body.Variables.LoadLocalA(1);

            body.GetILProcessor()
                .LdArg(0)
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(1)
                .Add(loopStart)
                .LdLocA(2)
                .Call(enumerator.FindMethod("TryMoveNext"))
                .BrFalseS(@return)
                .LdLoc(0)
                .LdLoc(2)
                .Add()
                .StLoc(0)
                .BrS(loopStart)
                .Add(@return)
                .Call(enumerator.FindMethod("Dispose", 0))
                .LdLoc(0)
                .Ret();
        }
    }
}