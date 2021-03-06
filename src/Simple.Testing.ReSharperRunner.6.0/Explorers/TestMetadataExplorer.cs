extern alias resharper;
using resharper::JetBrains.Metadata.Reader.API;
using resharper::JetBrains.ProjectModel;
using resharper::JetBrains.ReSharper.Psi;
using resharper::JetBrains.ReSharper.Psi.Caches;
using resharper::JetBrains.ReSharper.UnitTestFramework;
#if RESHARPER_61
using resharper::JetBrains.ReSharper.UnitTestFramework.Elements;
#endif

namespace Simple.Testing.ReSharperRunner.Explorers
{
  [MetadataUnitTestExplorer]
  public class TestMetadataExplorer : IUnitTestMetadataExplorer
  {
    readonly TestProvider _provider;
#if RESHARPER_61
    readonly IUnitTestElementManager _manager;
    readonly PsiModuleManager _psiModuleManager;
    readonly CacheManager _cacheManager;
#endif

#if RESHARPER_61
    public TestMetadataExplorer(TestProvider provider, IUnitTestElementManager manager, PsiModuleManager psiModuleManager, CacheManager cacheManager)
    {
      _manager = manager;
      _psiModuleManager = psiModuleManager;
      _cacheManager = cacheManager;
#else
    public TestMetadataExplorer(TestProvider provider)
    {
#endif
      _provider = provider;
    }

    public void ExploreAssembly(IProject project, IMetadataAssembly assembly, UnitTestElementConsumer consumer)
    {
      new AssemblyExplorer(_provider,
#if RESHARPER_61
        _manager, _psiModuleManager, _cacheManager, 
#endif
        assembly, project, consumer).Explore();
    }

    public IUnitTestProvider Provider
    {
      get { return _provider; }
    }
  }
}