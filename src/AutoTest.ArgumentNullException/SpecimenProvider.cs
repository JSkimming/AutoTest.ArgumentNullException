﻿// Copyright (c) 2013 - 2017 James Skimming. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace AutoTest.ArgNullEx
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using AutoFixture;
    using AutoFixture.Kernel;

    /// <summary>
    /// Provides parameter and instance specimens for a null parameter invocation of a method.
    /// </summary>
    public sealed class SpecimenProvider : ISpecimenProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpecimenProvider"/> class.
        /// </summary>
        /// <param name="fixture">The specimen fixture.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="fixture"/> parameter is
        /// <see langword="null"/>.</exception>
        public SpecimenProvider(IFixture fixture)
        {
            if (fixture == null)
                throw new ArgumentNullException(nameof(fixture));

            Builder = fixture;
            GlobalCustomizations(fixture);
        }

        /// <summary>
        /// Gets the <see cref="ISpecimenBuilder"/> used to create specimens.
        /// </summary>
        public ISpecimenBuilder Builder { get; }

        /// <summary>
        /// Gets the specimens for the <paramref name="parameters"/>.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="nullIndex">The index of the null parameter.</param>
        /// <returns>The specimens for the <paramref name="parameters"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="parameters"/> parameter is
        /// <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="parameters"/> list is empty or the
        /// <paramref name="nullIndex"/> is outside the range of the <paramref name="parameters"/>.</exception>
        object[] ISpecimenProvider.GetParameterSpecimens(IList<ParameterInfo> parameters, int nullIndex)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            if (parameters.Count == 0)
                throw new ArgumentException("There are no parameters", nameof(parameters));
            if (nullIndex >= parameters.Count)
            {
                string error =
                    $"The nullIndex '{nullIndex}' is beyond the range of the parameters '{parameters.Count}'.";
                throw new ArgumentException(error, nameof(nullIndex));
            }

            // Simple optimization, if the only parameter is to be null.
            if (parameters.Count == 1)
                return new object[1];

            object[] data = new object[parameters.Count];
            for (int parameterIndex = 0; parameterIndex < parameters.Count; ++parameterIndex)
            {
                ParameterInfo parameter = parameters[parameterIndex];

                // The parameter under test needs to be null.
                // As well as nullable output parameters.
                if (parameterIndex == nullIndex || (parameter.IsNullable() && parameter.IsOut))
                    continue;

                data[parameterIndex] = ResolveParameter(parameter);
            }

            return data;
        }

        /// <summary>
        /// Creates an instance of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to create.</param>
        /// <returns>The instance of the <paramref name="type"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is
        /// <see langword="null"/>.</exception>
        object ISpecimenProvider.CreateInstance(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return Resolve(type);
        }

        /// <summary>
        /// Added the global customizations to the <paramref name="fixture"/>.
        /// </summary>
        /// <param name="fixture">The test fixture.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="fixture"/> parameter is
        /// <see langword="null"/>.</exception>
        private static void GlobalCustomizations(IFixture fixture)
        {
            if (fixture == null)
                throw new ArgumentNullException(nameof(fixture));

            // Don't need to create complex graphs, just need objects.
            ThrowingRecursionBehavior throwingRecursionBehavior =
                fixture.Behaviors.OfType<ThrowingRecursionBehavior>().SingleOrDefault();
            if (throwingRecursionBehavior != null)
            {
                fixture.Behaviors.Remove(throwingRecursionBehavior);
                fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            }

            fixture.OmitAutoProperties = true;
        }

        /// <summary>
        /// Resolves the <paramref name="parameter"/> specimen.
        /// </summary>
        /// <param name="parameter">The <see cref="ParameterInfo"/> that describes what to create.</param>
        /// <returns>The <paramref name="parameter"/> specimen.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="parameter"/> parameter is
        /// <see langword="null"/>.</exception>
        private object ResolveParameter(ParameterInfo parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            // If the parameter IsByRef then the underlying type needs to be resolved.
            return parameter.ParameterType.IsByRef
                       ? Resolve(parameter.ParameterType.GetElementType())
                       : Resolve(parameter);
        }

        /// <summary>
        /// Resolves the <paramref name="request"/> specimen.
        /// </summary>
        /// <param name="request">The request that describes what to create.</param>
        /// <returns>The <paramref name="request"/> specimen.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="request"/> parameter is
        /// <see langword="null"/>.</exception>
        private object Resolve(object request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return new SpecimenContext(Builder).Resolve(request);
        }
    }
}
