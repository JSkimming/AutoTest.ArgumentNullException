﻿namespace AutoTest.ArgNullEx
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Moq;
    using Ploeh.AutoFixture;
    using Ploeh.AutoFixture.Xunit;
    using global::Xunit;
    using global::Xunit.Extensions;

    public class SpecimenProviderShould
    {
        [Theory, AutoMock]
        internal void InitialiseBuilder(
            IFixture builder,
            SpecimenProvider sut)
        {
            // Act
            Assert.Same(builder, sut.Builder);
        }

        [Theory, AutoMock]
        internal void ThrowWhenNoParameters(SpecimenProvider sut)
        {
            // AAA
            string actualParamName =
                Assert.Throws<ArgumentException>(
                    () => ((ISpecimenProvider) sut).GetParameterSpecimens(new ParameterInfo[] {}, 0))
                      .ParamName;
            Assert.Equal("parameters", actualParamName);
        }

        [Theory, AutoMock]
        internal void ThrowWhenNullIndexOutOfBounds(
            Mock<ParameterInfo>[] parameterMocks,
            SpecimenProvider sut)
        {
            // Arrange
            List<ParameterInfo> parameters = parameterMocks.Select(pm => pm.Object).ToList();

            // Act/Assert
            string actualParamName =
                Assert.Throws<ArgumentException>(
                    () => ((ISpecimenProvider)sut).GetParameterSpecimens(parameters, parameters.Count))
                      .ParamName;
            Assert.Equal("nullIndex", actualParamName);
        }

        [Theory, AutoMock]
        internal void ReturnSingleNullObjectArrayForSingleParameter(SpecimenProvider sut)
        {
            // Act
            object[] actualParameters = ((ISpecimenProvider) sut).GetParameterSpecimens(new ParameterInfo[1], 0);

            Assert.NotNull(actualParameters);
            Assert.Equal(1, actualParameters.Length);
            Assert.Null(actualParameters[0]);
        }

        [Theory, AutoMock]
        internal void CreateParameterSpecimens(SpecimenProvider sut)
        {
            // Arrange
            Action<int, Guid, object, object, string> action = (i, g, o, n, s) => { };
            ParameterInfo[] parameters = action.Method.GetParameters();

            // Act
            object[] actualParameters = ((ISpecimenProvider)sut).GetParameterSpecimens(parameters, 3);

            // Act
            Assert.NotNull(actualParameters);
            Assert.Equal(5, actualParameters.Length);
            Assert.IsType(parameters[0].ParameterType, actualParameters[0]);
            Assert.IsType(parameters[1].ParameterType, actualParameters[1]);
            Assert.IsType(parameters[2].ParameterType, actualParameters[2]);
            Assert.Null(actualParameters[3]);
            Assert.IsType(parameters[4].ParameterType, actualParameters[4]);
        }

        [Theory, AutoMock]
        internal void CreateObjectTypeSpecimen(
            [Frozen] object expectedSpecimen,
            SpecimenProvider sut)
        {
            // Act
            object actualSpecimen = ((ISpecimenProvider)sut).CreateInstance(expectedSpecimen.GetType());

            // Assert
            Assert.Same(expectedSpecimen, actualSpecimen);
        }

        [Theory, AutoMock]
        internal void CreateGuidTypeSpecimen(
            [Frozen] Guid expectedSpecimen,
            SpecimenProvider sut)
        {
            // Act
            object actualSpecimen = ((ISpecimenProvider)sut).CreateInstance(expectedSpecimen.GetType());

            // Assert
            Assert.Equal(expectedSpecimen, actualSpecimen);
        }
    }
}