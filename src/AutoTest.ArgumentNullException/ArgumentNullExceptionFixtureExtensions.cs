﻿namespace AutoTest.ArgNullEx
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Extension methods on <see cref="ArgumentNullExceptionFixture"/>.
    /// </summary>
    public static class ArgumentNullExceptionFixtureExtensions
    {
        /// <summary>
        /// Clears the <paramref name="mask"/> of <see cref="BindingFlags"/> on the <paramref name="fixture"/>.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="mask">The mask of <see cref="BindingFlags"/>.</param>
        /// <returns>The <paramref name="fixture"/>.</returns>
        public static IArgumentNullExceptionFixture ClearBindingFlags(this IArgumentNullExceptionFixture fixture, BindingFlags mask)
        {
            if (fixture == null)
                throw new ArgumentNullException("fixture");

            fixture.BindingFlags = fixture.BindingFlags & ~mask;

            return fixture;
        }

        /// <summary>
        /// Sets the <paramref name="mask"/> of <see cref="BindingFlags"/> on the <paramref name="fixture"/>.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="mask">The mask of <see cref="BindingFlags"/>.</param>
        /// <returns>The <paramref name="fixture"/>.</returns>
        public static IArgumentNullExceptionFixture SetBindingFlags(this IArgumentNullExceptionFixture fixture, BindingFlags mask)
        {
            if (fixture == null)
                throw new ArgumentNullException("fixture");

            fixture.BindingFlags = fixture.BindingFlags | mask;

            return fixture;
        }

        /// <summary>
        /// Removes all the filters that are instances of the <paramref name="filterType"/>.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="filterType">The type of filter.</param>
        /// <returns>The <paramref name="fixture"/>.</returns>
        public static IArgumentNullExceptionFixture RemoveFilters(this IArgumentNullExceptionFixture fixture, Type filterType)
        {
            if (fixture == null)
                throw new ArgumentNullException("fixture");
            if (filterType == null)
                throw new ArgumentNullException("filterType");

            fixture.Filters.RemoveAll(filterType.IsInstanceOfType);

            return fixture;
        }

        /// <summary>
        /// Removes all the supplied <paramref name="filtersToRemove"/> from the <paramref name="fixture"/> collection of <see name="IArgumentNullExceptionFixture.Filters"/>.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="filtersToRemove">The filters to remove.</param>
        /// <returns>The <paramref name="fixture"/>.</returns>
        public static IArgumentNullExceptionFixture RemoveFilters(this IArgumentNullExceptionFixture fixture, IEnumerable<IFilter> filtersToRemove)
        {
            if (fixture == null)
                throw new ArgumentNullException("fixture");
            if (filtersToRemove == null)
                throw new ArgumentNullException("filtersToRemove");

            foreach (IFilter filter in filtersToRemove)
            {
                fixture.Filters.Remove(filter);
            }
 
            return fixture;
        }

        /// <summary>
        /// Adds the <paramref name="filters"/> to the <paramref name="fixture"/> collection of <see name="IArgumentNullExceptionFixture.Filters"/> if they are not already in he collection.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="filters">The filters to add.</param>
        /// <returns>The <paramref name="fixture"/>.</returns>
        public static IArgumentNullExceptionFixture AddFilters(this IArgumentNullExceptionFixture fixture, IEnumerable<IFilter> filters)
        {
            if (fixture == null)
                throw new ArgumentNullException("fixture");
            if (filters == null)
                throw new ArgumentNullException("filters");

            foreach (IFilter filter in filters.Where(filter => !fixture.Filters.Contains(filter)))
            {
                fixture.Filters.Add(filter);
            }

            return fixture;
        }
    }
}
