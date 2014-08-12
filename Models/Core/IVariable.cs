﻿// -----------------------------------------------------------------------
// <copyright file="IVariable.cs" company="APSIM Initiative">
//     Copyright (c) APSIM Initiative
// </copyright>
//-----------------------------------------------------------------------
namespace Models.Core
{
    using System;

    /// <summary>
    /// This abstract base class encapsulates the interface for a variable from a Model.
    /// source code.
    /// </summary>
    [Serializable]
    public abstract class IVariable
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets or sets the value of the property.
        /// </summary>
        public abstract object Value { get; set; }

        /// <summary>
        /// Gets a description of the property or null if not found.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Gets the units of the property (in brackets) or null if not found.
        /// </summary>
        public abstract string Units { get; }
    }
} 