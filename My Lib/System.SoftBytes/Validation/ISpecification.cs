//-----------------------------------------------------------------------
// <copyright file="ISpecification.cs" company="BetKernel Ltd.">
//     Copyright (c) 2010 BetKernel Ltd. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace BetKernel.Common.Validation
{
    public interface ISpecification<T>
    {
        /// <summary>
        /// Determines whether [is satisfied by] [the specified t].
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>
        /// 	<c>true</c> if [is satisfied by] [the specified t]; otherwise, <c>false</c>.
        /// </returns>
        Boolean IsSatisfiedBy(T t);
    }
}
