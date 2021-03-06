extern alias resharper;
using System;
using System.Collections.Generic;

using resharper::JetBrains.Application;
using resharper::JetBrains.ProjectModel;
using resharper::JetBrains.ReSharper.Psi;
using resharper::JetBrains.ReSharper.Psi.Caches;
using resharper::JetBrains.ReSharper.Psi.Tree;
using resharper::JetBrains.ReSharper.UnitTestFramework;
using resharper::JetBrains.Util;

namespace Simple.Testing.ReSharperRunner.Presentation
{
	using Framework;

	internal abstract class Element : UnitTestElement
  {
    readonly string _declaringTypeName;
    readonly ProjectModelElementEnvoy _projectEnvoy;

    protected Element(IUnitTestProvider provider,

                      UnitTestElement parent,
                      ProjectModelElementEnvoy projectEnvoy,
                      string declaringTypeName,
                      bool isIgnored)
      : base(provider, parent)
    {
      if (projectEnvoy == null && !Shell.Instance.IsTestShell)
      {
        throw new ArgumentNullException("projectEnvoy");
      }

      if (declaringTypeName == null)
      {
        throw new ArgumentNullException("declaringTypeName");
      }

      _projectEnvoy = projectEnvoy;
      _declaringTypeName = declaringTypeName;

      if (isIgnored)
      {
        SetExplicit("Ignored");
      }
    }

    public virtual string GetTitlePrefix()
    {
      return String.Empty;
    }

    public override IProject GetProject()
    {
      return _projectEnvoy.GetValidProjectElement() as IProject;
    }

    protected ITypeElement GetDeclaredType()
    {
      ISolution solution = GetSolution();
      if (solution == null)
      {
        return null;
      }

      using (ReadLockCookie.Create())
      {
        IDeclarationsScope scope = DeclarationsScopeFactory.SolutionScope(solution, false);
        IDeclarationsCache cache = PsiManager.GetInstance(solution).GetDeclarationsCache(scope, true);
        return cache.GetTypeElementByCLRName(_declaringTypeName);
      }
    }

    public override string GetTypeClrName()
    {
      return _declaringTypeName;
    }

    public override UnitTestNamespace GetNamespace()
    {
      return new UnitTestNamespace(new CLRTypeName(_declaringTypeName).NamespaceName);
    }

    public override IList<IProjectFile> GetProjectFiles()
    {
      ITypeElement declaredType = GetDeclaredType();
      if (declaredType == null)
      {
        return EmptyArray<IProjectFile>.Instance;
      }

      return declaredType.GetProjectFiles();
    }

    public override UnitTestElementDisposition GetDisposition()
    {
		IDeclaredElement element = GetDeclaredElement();
      if (element == null || !element.IsValid())
      {
        return UnitTestElementDisposition.InvalidDisposition;
      }

      var locations = new List<UnitTestElementLocation>();
      element.GetDeclarations().ForEach(declaration =>
        {
          IFile file = declaration.GetContainingFile();
          if (file != null)
          {
            locations.Add(new UnitTestElementLocation(file.ProjectFile,
                                                      declaration.GetNavigationRange().TextRange,
                                                      resharper::JetBrains.ReSharper.Psi.Tree.ElementExtensions.GetDocumentRange(declaration).TextRange));
          }
        });

      return new UnitTestElementDisposition(locations, this);
    }

    public override bool Equals(object obj)
    {
      if (base.Equals(obj))
      {
        Element other = (Element)obj;
        return Equals(other._projectEnvoy, _projectEnvoy) && other._declaringTypeName == _declaringTypeName;
      }

      return false;
    }

    public override int GetHashCode()
    {
      int result = base.GetHashCode();
      result = 29 * result + _projectEnvoy.GetHashCode();
      result = 29 * result + _declaringTypeName.GetHashCode();
      return result;
    }
  }
}