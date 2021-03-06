//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Copyright (c) 2008 - 2015 Jb Evain
// Copyright (c) 2008 - 2011 Novell, Inc.
//
// Licensed under the MIT/X11 license.
//

using ILRuntime.Mono.Collections.Generic;
using System;
using System.Text;
using System.Threading;

namespace ILRuntime.Mono.Cecil
{

    public sealed class GenericInstanceMethod : MethodSpecification, IGenericInstance, IGenericContext
    {

        Collection<TypeReference> arguments;

        public bool HasGenericArguments
        {
            get { return !arguments.IsNullOrEmpty(); }
        }

        public Collection<TypeReference> GenericArguments
        {
            get
            {
                if (arguments == null)
                    Interlocked.CompareExchange(ref arguments, new Collection<TypeReference>(), null);

                return arguments;
            }
        }

        public override bool IsGenericInstance
        {
            get { return true; }
        }

        IGenericParameterProvider IGenericContext.Method
        {
            get { return ElementMethod; }
        }

        IGenericParameterProvider IGenericContext.Type
        {
            get { return ElementMethod.DeclaringType; }
        }

        public override bool ContainsGenericParameter
        {
            get { return this.ContainsGenericParameter() || base.ContainsGenericParameter; }
        }

        public override string FullName
        {
            get
            {
                StringBuilder signature = new StringBuilder();
                MethodReference method = ElementMethod;
                signature.Append(method.ReturnType.FullName)
                    .Append(" ")
                    .Append(method.DeclaringType.FullName)
                    .Append("::")
                    .Append(method.Name);
                this.GenericInstanceFullName(signature);
                this.MethodSignatureFullName(signature);
                return signature.ToString();

            }
        }

        public GenericInstanceMethod(MethodReference method)
            : base(method)
        {
        }

        internal GenericInstanceMethod(MethodReference method, int arity)
            : this(method)
        {
            arguments = new Collection<TypeReference>(arity);
        }
    }
}
