using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using ReflectionAssembly = System.Reflection.Assembly;
using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.NUnit;
using NUnit.Framework;
using PersonalLogManagerClient;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace PersonalLogManagerClient.ArchitectureTests
{
    [TestFixture]
    public sealed class ClientArchitectureTests
    {
        private const string LayoutNamespace = "PersonalLogManagerClient.Layout";
        private const string PagesNamespace = "PersonalLogManagerClient.Pages";
        private const string TestAssemblyNameSuffix = "Tests";
        private const string TestProjectFileSuffix = "Tests.csproj";

        private static readonly ReflectionAssembly ProductionAssembly = typeof(Program).Assembly;
        private static readonly ReflectionAssembly TransportAssembly = ReflectionAssembly.Load("NuciAPI.Client");
        private static readonly Architecture Architecture = new ArchLoader()
            .LoadAssemblies(ProductionAssembly, TransportAssembly)
            .Build();
        private static readonly IObjectProvider<IType> TransportTypes = Types()
            .That()
            .ResideInAssembly(TransportAssembly.GetName().Name!)
            .As("NuciAPI.Client types");

        [Test]
        public void PresentationTypesShouldNotDependOnTheApiTransport()
        {
            Assert.That(HasTypesInNamespace(PagesNamespace), Is.True, "The pages selector must match production types.");
            Assert.That(HasTypesInNamespace(LayoutNamespace), Is.True, "The layout selector must match production types.");

            IArchRule presentationMustNotDependOnTransport = Types()
                .That()
                .ResideInNamespace(PagesNamespace)
                .Or()
                .ResideInNamespace(LayoutNamespace)
                .Should()
                .NotDependOnAny(TransportTypes)
                .Because("presentation components must use PersonalLogService instead of the API transport");

            presentationMustNotDependOnTransport.Check(Architecture);
        }

        [Test]
        public void ProductionAssemblyShouldNotReferenceTestAssemblies()
        {
            string[] testAssemblyReferences = ProductionAssembly
                .GetReferencedAssemblies()
                .Where(assembly => assembly.Name?.EndsWith(TestAssemblyNameSuffix, StringComparison.Ordinal) == true)
                .Select(assembly => assembly.Name!)
                .ToArray();

            Assert.That(testAssemblyReferences, Is.Empty, "Production must not reference test assemblies.");
        }

        [Test]
        public void ProductionProjectShouldNotReferenceTestProjects()
        {
            string projectPath = Path.Combine(
                FindRepositoryDirectory(),
                "PersonalLogManager.Client",
                "PersonalLogManagerClient.csproj");
            string[] testProjectReferences = XDocument.Load(projectPath)
                .Descendants("ProjectReference")
                .Select(element => (string?)element.Attribute("Include"))
                .Where(include => include is not null)
                .Select(include => include!.Replace('\\', '/').Split('/').Last())
                .Where(fileName => fileName.EndsWith(TestProjectFileSuffix, StringComparison.Ordinal))
                .ToArray();

            Assert.That(testProjectReferences, Is.Empty, "Production must not reference test projects.");
        }

        [Test]
        public void ProductionAssemblyShouldNotExposeInternalsToOtherAssemblies()
            => Assert.That(
                ProductionAssembly.GetCustomAttributes<InternalsVisibleToAttribute>(),
                Is.Empty,
                "Production must not declare InternalsVisibleTo.");

        private static bool HasTypesInNamespace(string namespaceName)
            => ProductionAssembly.GetTypes().Any(type => type.Namespace == namespaceName);

        private static string FindRepositoryDirectory()
        {
            DirectoryInfo? directory = new(AppContext.BaseDirectory);

            while (directory is not null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "PersonalLogManagerClient.slnx")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            throw new DirectoryNotFoundException("Could not find the repository solution file.");
        }
    }
}