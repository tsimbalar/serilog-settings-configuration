﻿// Copyright 2013-2016 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyModel;
using Serilog.Configuration;
using Serilog.Settings.Configuration;
using System.Reflection;

namespace Serilog
{
    /// <summary>
    /// Defines the interpretation of a null DependencyContext configuration parameter.
    /// </summary>
    public enum ConfigurationAssemblySource
    {
        /// <summary>
        /// Try to scan the assemblies already in memory. This is the default. If GetEntryAssembly is null, fallback to DLL scanning.
        /// </summary>
        UseLoadedAssemblies,

        /// <summary>
        /// Scan for assemblies in DLLs from the working directory. This is the fallback when GetEntryAssembly is null.
        /// </summary>
        AlwaysScanDllFiles
    }

    /// <summary>
    /// Extends <see cref="LoggerConfiguration"/> with support for System.Configuration appSettings elements.
    /// </summary>
    public static class ConfigurationLoggerConfigurationExtensions
    {
        /// <summary>
        /// Configuration section name required by this package.
        /// </summary>
        public const string DefaultSectionName = "Serilog";

        /// <summary>
        /// Reads logger settings from the provided configuration object using the default section name. Generally this
        /// is preferable over the other method that takes a configuration section. Only this version will populate
        /// IConfiguration parameters on target methods.
        /// </summary>
        /// <param name="settingConfiguration">Logger setting configuration.</param>
        /// <param name="configuration">A configuration object which contains a Serilog section.</param>
        /// <param name="dependencyContext">The dependency context from which sink/enricher packages can be located.</param>
        /// <param name="onNullDependencyContext">If dependency context is not supplied, either the platform default or DLL scanning may be used.</param>
        /// <returns>An object allowing configuration to continue.</returns>
        public static LoggerConfiguration Configuration(
            this LoggerSettingsConfiguration settingConfiguration,
            IConfiguration configuration,
            DependencyContext dependencyContext = null,
            ConfigurationAssemblySource onNullDependencyContext = ConfigurationAssemblySource.UseLoadedAssemblies)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            return settingConfiguration.Settings(
                new ConfigurationReader(
                    configuration,
                    dependencyContext ??
                        (onNullDependencyContext == ConfigurationAssemblySource.UseLoadedAssemblies
                        && Assembly.GetEntryAssembly() != null
                        ? DependencyContext.Default : null)));
        }

        /// <summary>
        /// Reads logger settings from the provided configuration section. Generally it is preferable to use the other
        /// extension method that takes the full configuration object.
        /// </summary>
        /// <param name="settingConfiguration">Logger setting configuration.</param>
        /// <param name="configSection">The Serilog configuration section</param>
        /// <param name="dependencyContext">The dependency context from which sink/enricher packages can be located.</param>
        /// <param name="onNullDependencyContext">If dependency context is not supplied, either the platform default or DLL scanning may be used.</param>
        /// <returns>An object allowing configuration to continue.</returns>
        public static LoggerConfiguration ConfigurationSection(
            this LoggerSettingsConfiguration settingConfiguration,
            IConfigurationSection configSection,
            DependencyContext dependencyContext = null,
            ConfigurationAssemblySource onNullDependencyContext = ConfigurationAssemblySource.UseLoadedAssemblies)
        {
            if (settingConfiguration == null) throw new ArgumentNullException(nameof(settingConfiguration));
            if (configSection == null) throw new ArgumentNullException(nameof(configSection));
            
            return settingConfiguration.Settings(
                new ConfigurationReader(
                    configSection,
                    dependencyContext ??
                        (onNullDependencyContext == ConfigurationAssemblySource.UseLoadedAssemblies
                        && Assembly.GetEntryAssembly() != null
                        ? DependencyContext.Default : null)));
        }
    }
}
