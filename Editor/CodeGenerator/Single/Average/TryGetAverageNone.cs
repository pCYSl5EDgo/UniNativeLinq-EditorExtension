using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming


namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class TryGetAverageNone : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public TryGetAverageNone(ISingleApi api)
        {
            Api = api;
            returnTypeName = api.Description;
        }

        public readonly ISingleApi Api;
        private readonly string returnTypeName;

        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }

        private TypeReference CalcReturnTypeReference(MethodDefinition method)
        {
            switch (returnTypeName)
            {
                case "Double":
                    return method.Module.TypeSystem.Double;
                case "Single":
                    return method.Module.TypeSystem.Single;
                case "Int32":
                    return method.Module.TypeSystem.Int32;
                case "UInt32":
                    return method.Module.TypeSystem.UInt32;
                case "Int64":
                    return method.Module.TypeSystem.Int64;
                case "UInt64":
                    return method.Module.TypeSystem.UInt64;
                default: throw new Exception();
            }
        }

        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            Api.GenerateSingleNoEnumerable(processor, mainModule, GenerateEach, GenerateGeneric);
        }

        private void GenerateGeneric(TypeDefinition @static, ModuleDefinition mainModule)
        {
            var method = new MethodDefinition("TryGetAverage", Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);
            var returnType = CalcReturnTypeReference(method);

            var IRefEnumerator = new GenericInstanceType(method.Module.GetType("UniNativeLinq", "IRefEnumerator`1"))
            {
                GenericArguments = { returnType }
            };
            var TEnumerator = new GenericParameter("TEnumerator", method)
            {
                HasNotNullableValueTypeConstraint = true,
                Constraints = { IRefEnumerator }
            };
            method.GenericParameters.Add(TEnumerator);

            var IRefEnumerable = new GenericInstanceType(method.Module.GetType("UniNativeLinq", "IRefEnumerable`2"))
            {
                GenericArguments = { TEnumerator, returnType }
            };
            var TEnumerable = new GenericParameter("TEnumerable", method)
            {
                HasNotNullableValueTypeConstraint = true,
                Constraints = { IRefEnumerable }
            };
            method.GenericParameters.Add(TEnumerable);

            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(TEnumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(returnType)));

            var body = method.Body;
            body.InitLocals = true;
            var enumeratorVariable = new VariableDefinition(TEnumerator);
            body.Variables.Add(enumeratorVariable);                                         // 0
            body.Variables.Add(new VariableDefinition(returnType));                         // 1
            body.Variables.Add(new VariableDefinition(returnType));                         // 2
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Int64));     // 3

            var firstSuccess = InstructionUtility.LoadConstant(1L);
            var @return = Instruction.Create(OpCodes.Ldarg_1);
            var loopStart = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);

            var processor = body.GetILProcessor()
                .LdArg(0)
                .GetEnumeratorEnumerable(TEnumerable)
                .StLoc(0)

                .LdLocA(0)
                .LdLocA(1)
                .TryMoveNextEnumerator(TEnumerator)
                .BrTrueS(firstSuccess[0])

                .LdLocA(0)
                .DisposeEnumerator(TEnumerator)
                .LdC(false)
                .Ret()

                .AddRange(firstSuccess)
                .StLoc(3)

                .Add(loopStart)
                .LdLocA(2)
                .TryMoveNextEnumerator(TEnumerator)
                .BrFalseS(@return)

                .LdLoc(3)
                .LdC(1L)
                .Add()
                .StLoc(3)

                .LdLoc(1)
                .LdLoc(2)
                .Add()
                .StLoc(1)

                .BrS(loopStart)

                .Add(@return)
                .LdLoc(1);

            switch (returnTypeName)
            {
                case "Double":
                    processor
                        .LdLoc(3)
                        .ConvR8()
                        .Div();
                    break;
                case "Single":
                    processor
                        .LdLoc(3)
                        .ConvR4()
                        .Div();
                    break;
                case "Int32":
                    processor
                        .ConvI8()
                        .LdLoc(3)
                        .Div()
                        .ConvI4();
                    break;
                case "UInt32":
                    processor
                        .ConvU8()
                        .LdLoc(3)
                        .DivUn()
                        .ConvU4();
                    break;
                case "Int64":
                    processor
                        .Div();
                    break;
                case "UInt64":
                    processor
                        .LdLoc(3)
                        .ConvU8()
                        .DivUn();
                    break;
            }

            processor
                .StObj(returnType)

                .LdLocA(0)
                .DisposeEnumerator(TEnumerator)

                .LdC(true)
                .Ret();
        }

        private void GenerateEach(string name, bool isSpecial, TypeDefinition @static, ModuleDefinition mainModule)
        {
            var method = new MethodDefinition("TryGetAverage", Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);
            var returnType = CalcReturnTypeReference(method);

            if (isSpecial)
            {
                var (baseEnumerable, _, _) = returnType.MakeSpecialTypePair(name);
                switch (name)
                {
                    case "T[]":
                        GenerateArray(method, baseEnumerable, returnType);
                        break;
                    case "NativeArray<T>":
                        GenerateNativeArray(method, baseEnumerable, returnType);
                        break;
                    default: throw new NotSupportedException(name);
                }
            }
            else
            {
                GenerateNormal(method, Dictionary[name], returnType);
            }
        }

        private void GenerateNormal(MethodDefinition method, TypeDefinition type, TypeReference returnType)
        {
            var (enumerable, enumerator, _) = returnType.MakeFromCommonType(method, type, "0");

            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(returnType)));

            var body = method.Body;
            body.InitLocals = true;
            var enumeratorVariable = new VariableDefinition(enumerator);
            body.Variables.Add(enumeratorVariable);                                         // 0
            body.Variables.Add(new VariableDefinition(returnType));                         // 1
            body.Variables.Add(new VariableDefinition(returnType));                         // 2
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Int64));     // 3

            var firstSuccess = InstructionUtility.LoadConstant(1L);
            var @return = Instruction.Create(OpCodes.Ldarg_1);
            var loopStart = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);

            var TryMoveNext = enumerator.FindMethod("TryMoveNext", 1);
            var Dispose = enumerator.FindMethod("Dispose", 0);

            var processor = body.GetILProcessor()
                .LdArg(0)
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)

                .LdLocA(0)
                .LdLocA(1)
                .Call(TryMoveNext)
                .BrTrueS(firstSuccess[0])

                .LdLocA(0)
                .Call(Dispose)
                .LdC(false)
                .Ret()

                .AddRange(firstSuccess)
                .StLoc(3)

                .Add(loopStart)
                .LdLocA(2)
                .Call(TryMoveNext)
                .BrFalseS(@return)

                .LdLoc(3)
                .LdC(1L)
                .Add()
                .StLoc(3)

                .LdLoc(1)
                .LdLoc(2)
                .Add()
                .StLoc(1)

                .BrS(loopStart)

                .Add(@return)
                .LdLoc(1);

            switch (returnTypeName)
            {
                case "Double":
                    processor
                        .LdLoc(3)
                        .ConvR8()
                        .Div();
                    break;
                case "Single":
                    processor
                        .LdLoc(3)
                        .ConvR4()
                        .Div();
                    break;
                case "Int32":
                    processor
                        .ConvI8()
                        .LdLoc(3)
                        .Div()
                        .ConvI4();
                    break;
                case "UInt32":
                    processor
                        .ConvU8()
                        .LdLoc(3)
                        .DivUn()
                        .ConvU4();
                    break;
                case "Int64":
                    processor
                        .Div();
                    break;
                case "UInt64":
                    processor
                        .LdLoc(3)
                        .ConvU8()
                        .DivUn();
                    break;
            }

            processor
                .StObj(returnType)

                .LdLocA(0)
                .Call(Dispose)

                .LdC(true)
                .Ret();
        }


        private void GenerateNativeArray(MethodDefinition method, TypeReference baseEnumerable, TypeReference returnType)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(baseEnumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(returnType)));

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Int32));
            body.Variables.Add(new VariableDefinition(returnType));

            var loopStart = Instruction.Create(OpCodes.Ldarg_0);

            var getLength = baseEnumerable.FindMethod("get_Length");
            var getItem = baseEnumerable.FindMethod("get_Item");

            var processor = body.GetILProcessor()
                .LdArg(0)
                .Call(getLength)
                .BrTrueS(loopStart)
                .LdC(false)
                .Ret()
                .Add(loopStart)
                .LdLoc(0)
                .Call(getItem)
                .LdLoc(1)
                .Add()
                .StLoc(1)
                .LdLoc(0)
                .LdC(1)
                .Add()
                .Dup()
                .StLoc(0)
                .LdArg(0)
                .Call(getLength)
                .BltS(loopStart)
                .LdArg(1)
                .LdLoc(1)
                .LdArg(0)
                .Call(getLength);

            switch (returnTypeName)
            {
                case "Double":
                    processor.ConvR8().Div();
                    break;
                case "Single":
                    processor.ConvR4().Div();
                    break;
                case "Int32":
                    processor.ConvI4().Div();
                    break;
                case "Int64":
                    processor.ConvI8().Div();
                    break;
                case "UInt32":
                    processor.ConvU4().DivUn();
                    break;
                case "UInt64":
                    processor.ConvU8().DivUn();
                    break;
            }

            processor
                .StObj(returnType)
                .LdC(true)
                .Ret();
        }

        private void GenerateArray(MethodDefinition method, TypeReference baseEnumerable, TypeReference returnType)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(returnType)));

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Int32));
            body.Variables.Add(new VariableDefinition(returnType));

            var loopStart = Instruction.Create(OpCodes.Ldarg_0);

            var processor = body.GetILProcessor()
                .LdArg(0)
                .LdLen()
                .BrTrueS(loopStart)
                .LdC(false)
                .Ret()
                .Add(loopStart)
                .LdLoc(0)
                .LdElem(returnType)
                .LdLoc(1)
                .Add()
                .StLoc(1)
                .LdLoc(0)
                .LdC(1)
                .Add()
                .Dup()
                .StLoc(0)
                .LdArg(0)
                .LdLen()
                .ConvI4()
                .BltS(loopStart)
                .LdArg(1)
                .LdLoc(1)
                .LdArg(0)
                .LdLen();

            switch (returnTypeName)
            {
                case "Double":
                    processor.ConvR8().Div();
                    break;
                case "Single":
                    processor.ConvR4().Div();
                    break;
                case "Int32":
                    processor.ConvI4().Div();
                    break;
                case "Int64":
                    processor.ConvI8().Div();
                    break;
                case "UInt32":
                    processor.ConvU4().DivUn();
                    break;
                case "UInt64":
                    processor.ConvU8().DivUn();
                    break;
            }

            processor
                .StObj(returnType)
                .LdC(true)
                .Ret();
        }
    }
}