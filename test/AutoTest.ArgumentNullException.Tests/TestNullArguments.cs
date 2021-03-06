﻿namespace AutoTest.ArgNullEx
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoTest.ArgNullEx.Xunit;
    using global::Xunit;

    public class TestNullArguments
    {
        [Theory, RequiresArgumentNullExceptionAutoMoq(typeof(MethodData))]
        public Task TestAllNullArguments(MethodData method)
        {
            return method.Execute();
        }

        [Theory, RequiresArgumentNullExceptionAutoMoq(typeof(MethodDataExtensions))]
        public Task TestAllNullArgumentsXunit(MethodData method)
        {
            return method.Execute();
        }
    }
}
