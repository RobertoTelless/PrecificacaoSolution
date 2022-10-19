using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using Ninject.Web.Common;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using ModelServices.Interfaces.Repositories;
using ApplicationServices.Services;
using ModelServices.EntitiesServices;
using DataServices.Repositories;
using Ninject.Web.Common.WebHost;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Presentation.Start.NinjectWebCommons), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Presentation.Start.NinjectWebCommons), "Stop")]

namespace Presentation.Start
{
    public class NinjectWebCommons
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind(typeof(IAppServiceBase<>)).To(typeof(AppServiceBase<>));
            kernel.Bind<IUsuarioAppService>().To<UsuarioAppService>();
            kernel.Bind<ILogAppService>().To<LogAppService>();
            kernel.Bind<IConfiguracaoAppService>().To<ConfiguracaoAppService>();
            kernel.Bind<INoticiaAppService>().To<NoticiaAppService>();
            kernel.Bind<INotificacaoAppService>().To<NotificacaoAppService>();
            kernel.Bind<ITemplateAppService>().To<TemplateAppService>();
            kernel.Bind<ITarefaAppService>().To<TarefaAppService>();
            kernel.Bind<IAgendaAppService>().To<AgendaAppService>();
            kernel.Bind<IAssinanteAppService>().To<AssinanteAppService>();
            kernel.Bind<ICategoriaAgendaAppService>().To<CategoriaAgendaAppService>();
            kernel.Bind<ICategoriaNotificacaoAppService>().To<CategoriaNotificacaoAppService>();
            kernel.Bind<ICategoriaUsuarioAppService>().To<CategoriaUsuarioAppService>();
            kernel.Bind<IEmpresaAppService>().To<EmpresaAppService>();
            kernel.Bind<IFormaPagRecAppService>().To<FormaPagRecAppService>();
            kernel.Bind<IMaquinaAppService>().To<MaquinaAppService>();
            kernel.Bind<IPeriodicidadeAppService>().To<PeriodicidadeAppService>();
            kernel.Bind<IPessoaExternaAppService>().To<PessoaExternaAppService>();
            kernel.Bind<IPlataformaEntregaAppService>().To<PlataformaEntregaAppService>();
            kernel.Bind<ICargoAppService>().To<CargoAppService>();
            kernel.Bind<ICentroCustoAppService>().To<CentroCustoAppService>();
            kernel.Bind<IGrupoCCAppService>().To<GrupoCCAppService>();
            kernel.Bind<ISubgrupoAppService>().To<SubgrupoAppService>();
            kernel.Bind<IBancoAppService>().To<BancoAppService>();
            kernel.Bind<IContaBancariaAppService>().To<ContaBancariaAppService>();
            kernel.Bind<IVideoAppService>().To<VideoAppService>();
            kernel.Bind<ICategoriaFornecedorAppService>().To<CategoriaFornecedorAppService>();
            kernel.Bind<IFornecedorAppService>().To<FornecedorAppService>();
            kernel.Bind<IFornecedorCnpjAppService>().To<FornecedorCnpjAppService>();
            kernel.Bind<ICategoriaClienteAppService>().To<CategoriaClienteAppService>();
            kernel.Bind<IClienteAppService>().To<ClienteAppService>();
            kernel.Bind<IClienteCnpjAppService>().To<ClienteCnpjAppService>();
            kernel.Bind<ICategoriaProdutoAppService>().To<CategoriaProdutoAppService>();
            kernel.Bind<ISubcategoriaProdutoAppService>().To<SubcategoriaProdutoAppService>();
            kernel.Bind<IUnidadeAppService>().To<UnidadeAppService>();
            kernel.Bind<ITamanhoAppService>().To<TamanhoAppService>();
            kernel.Bind<IProdutoAppService>().To<ProdutoAppService>();
            kernel.Bind<IProdutoEstoqueFilialAppService>().To<ProdutoEstoqueFilialAppService>();
            kernel.Bind<IProdutotabelaPrecoAppService>().To<ProdutoTabelaPrecoAppService>();
            kernel.Bind<IMovimentoEstoqueProdutoAppService>().To<MovimentoEstoqueProdutoAppService>();
            kernel.Bind<ITipoEmbalagemAppService>().To<TipoEmbalagemAppService>();
            kernel.Bind<IFichaTecnicaAppService>().To<FichaTecnicaAppService>();

            kernel.Bind(typeof(IServiceBase<>)).To(typeof(ServiceBase<>));
            kernel.Bind<IUsuarioService>().To<UsuarioService>();
            kernel.Bind<ILogService>().To<LogService>();
            kernel.Bind<IPerfilService>().To<PerfilService>();
            kernel.Bind<IConfiguracaoService>().To<ConfiguracaoService>();
            kernel.Bind<INotificacaoService>().To<NotificacaoService>();
            kernel.Bind<INoticiaService>().To<NoticiaService>();
            kernel.Bind<ITipoPessoaService>().To<TipoPessoaService>();
            kernel.Bind<ITemplateService>().To<TemplateService>();
            kernel.Bind<ITarefaService>().To<TarefaService>();
            kernel.Bind<IAgendaService>().To<AgendaService>();
            kernel.Bind<IAssinanteService>().To<AssinanteService>();
            kernel.Bind<ICategoriaAgendaService>().To<CategoriaAgendaService>();
            kernel.Bind<ICategoriaNotificacaoService>().To<CategoriaNotificacaoService>();
            kernel.Bind<ICategoriaUsuarioService>().To<CategoriaUsuarioService>();
            kernel.Bind<IEmpresaService>().To<EmpresaService>();
            kernel.Bind<IFormaPagRecService>().To<FormaPagRecService>();
            kernel.Bind<IMaquinaService>().To<MaquinaService>();
            kernel.Bind<IPeriodicidadeService>().To<PeriodicidadeService>();
            kernel.Bind<IPessoaExternaService>().To<PessoaExternaService>();
            kernel.Bind<IPlataformaEntregaService>().To<PlataformaEntregaService>();
            kernel.Bind<ICargoService>().To<CargoService>();
            kernel.Bind<ICentroCustoService>().To<CentroCustoService>();
            kernel.Bind<IGrupoCCService>().To<GrupoCCService>();
            kernel.Bind<ISubgrupoService>().To<SubgrupoService>();
            kernel.Bind<IBancoService>().To<BancoService>();
            kernel.Bind<IContaBancariaService>().To<ContaBancariaService>();
            kernel.Bind<IVideoService>().To<VideoService>();
            kernel.Bind<ICategoriaFornecedorService>().To<CategoriaFornecedorService>();
            kernel.Bind<IFornecedorService>().To<FornecedorService>();
            kernel.Bind<IFornecedorCnpjService>().To<FornecedorCnpjService>();
            kernel.Bind<ICategoriaClienteService>().To<CategoriaClienteService>();
            kernel.Bind<IClienteService>().To<ClienteService>();
            kernel.Bind<IClienteCnpjService>().To<ClienteCnpjService>();
            kernel.Bind<ICategoriaProdutoService>().To<CategoriaProdutoService>();
            kernel.Bind<ISubcategoriaProdutoService>().To<SubcategoriaProdutoService>();
            kernel.Bind<IUnidadeService>().To<UnidadeService>();
            kernel.Bind<ITamanhoService>().To<TamanhoService>();
            kernel.Bind<IProdutoService>().To<ProdutoService>();
            kernel.Bind<IProdutoEstoqueFilialService>().To<ProdutoEstoqueFilialService>();
            kernel.Bind<IProdutoMovimentoEstoqueService>().To<ProdutoMovimentoEstoqueService>();
            kernel.Bind<IProdutoTabelaPrecoService>().To<ProdutoTabelaPrecoService>();
            kernel.Bind<IMovimentoEstoqueProdutoService>().To<MovimentoEstoqueProdutoService>();
            kernel.Bind<ITipoEmbalagemService>().To<TipoEmbalagemService>();
            kernel.Bind<IFichaTecnicaService>().To<FichaTecnicaService>();

            kernel.Bind(typeof(IRepositoryBase<>)).To(typeof(RepositoryBase<>));
            kernel.Bind<IConfiguracaoRepository>().To<ConfiguracaoRepository>();
            kernel.Bind<IUsuarioRepository>().To<UsuarioRepository>();
            kernel.Bind<ILogRepository>().To<LogRepository>();
            kernel.Bind<IPerfilRepository>().To<PerfilRepository>();
            kernel.Bind<ITemplateRepository>().To<TemplateRepository>();
            kernel.Bind<ITipoPessoaRepository>().To<TipoPessoaRepository>();
            kernel.Bind<ICategoriaNotificacaoRepository>().To<CategoriaNotificacaoRepository>();
            kernel.Bind<INotificacaoRepository>().To<NotificacaoRepository>();
            kernel.Bind<INoticiaRepository>().To<NoticiaRepository>();
            kernel.Bind<INoticiaComentarioRepository>().To<NoticiaComentarioRepository>();
            kernel.Bind<INotificacaoAnexoRepository>().To<NotificacaoAnexoRepository>();
            kernel.Bind<ITarefaRepository>().To<TarefaRepository>();
            kernel.Bind<ITipoTarefaRepository>().To<TipoTarefaRepository>();
            kernel.Bind<ITarefaAnexoRepository>().To<TarefaAnexoRepository>();
            kernel.Bind<IUsuarioAnexoRepository>().To<UsuarioAnexoRepository>();
            kernel.Bind<IUFRepository>().To<UFRepository>();
            kernel.Bind<IAgendaRepository>().To<AgendaRepository>();
            kernel.Bind<IAgendaAnexoRepository>().To<AgendaAnexoRepository>();
            kernel.Bind<ICategoriaAgendaRepository>().To<CategoriaAgendaRepository>();
            kernel.Bind<IAssinanteRepository>().To<AssinanteRepository>();
            kernel.Bind<IPeriodicidadeRepository>().To<PeriodicidadeRepository>();
            kernel.Bind<ICargoRepository>().To<CargoRepository>();
            kernel.Bind<ICategoriaUsuarioRepository>().To<CategoriaUsuarioRepository>();
            kernel.Bind<IEmpresaRepository>().To<EmpresaRepository>();
            kernel.Bind<IFormaPagRecRepository>().To<FormaPagRecRepository>();
            kernel.Bind<IMaquinaRepository>().To<MaquinaRepository>();
            kernel.Bind<IPessoaExternaRepository>().To<PessoaExternaRepository>();
            kernel.Bind<IPessoaExternaAnexoRepository>().To<PessoaExternaAnexoRepository>();
            kernel.Bind<IPessoaExternaAnotacaoRepository>().To<PessoaExternaAnotacaoRepository>();
            kernel.Bind<IPlataformaEntregaRepository>().To<PlataformaEntregaRepository>();
            kernel.Bind<IUsuarioAnotacaoRepository>().To<UsuarioAnotacaoRepository>();
            kernel.Bind<IRegimeTributarioRepository>().To<RegimeTributarioRepository>();
            kernel.Bind<IEmpresaAnexoRepository>().To<EmpresaAnexoRepository>();
            kernel.Bind<ICentroCustoRepository>().To<CentroCustoRepository>();
            kernel.Bind<IGrupoCCRepository>().To<GrupoCCRepository>();
            kernel.Bind<ISubgrupoRepository>().To<SubgrupoRepository>();
            kernel.Bind<IBancoRepository>().To<BancoRepository>();
            kernel.Bind<IContaBancariaRepository>().To<ContaBancariaRepository>();
            kernel.Bind<IContaBancariaContatoRepository>().To<ContaBancariaContatoRepository>();
            kernel.Bind<IContaBancariaLancamentoRepository>().To<ContaBancariaLancamentoRepository>();
            kernel.Bind<ITipoContaRepository>().To<TipoContaRepository>();
            kernel.Bind<IVideoRepository>().To<VideoRepository>();
            kernel.Bind<IVideoComentarioRepository>().To<VideoComentarioRepository>();
            kernel.Bind<ICategoriaFornecedorRepository>().To<CategoriaFornecedorRepository>();
            kernel.Bind<ISexoRepository>().To<SexoRepository>();
            kernel.Bind<ITipoContribuinteRepository>().To<TipoContribuinteRepository>();
            kernel.Bind<IFornecedorRepository>().To<FornecedorRepository>();
            kernel.Bind<IFornecedorAnexoRepository>().To<FornecedorAnexoRepository>();
            kernel.Bind<IFornecedorAnotacaoRepository>().To<FornecedorAnotacaoRepository>();
            kernel.Bind<IFornecedorCnpjRepository>().To<FornecedorCnpjRepository>();
            kernel.Bind<IFornecedorContatoRepository>().To<FornecedorContatoRepository>();
            kernel.Bind<ICategoriaClienteRepository>().To<CategoriaClienteRepository>();
            kernel.Bind<IClienteRepository>().To<ClienteRepository>();
            kernel.Bind<IClienteAnexoRepository>().To<ClienteAnexoRepository>();
            kernel.Bind<IClienteAnotacaoRepository>().To<ClienteAnotacaoRepository>();
            kernel.Bind<IClienteContatoRepository>().To<ClienteContatoRepository>();
            kernel.Bind<IClienteReferenciaRepository>().To<ClienteReferenciaRepository>();
            kernel.Bind<IClienteCnpjRepository>().To<ClienteCnpjRepository>();
            kernel.Bind<IEmpresaMaquinaRepository>().To<EmpresaMaquinaRepository>();
            kernel.Bind<ICategoriaProdutoRepository>().To<CategoriaProdutoRepository>();
            kernel.Bind<ISubcategoriaProdutoRepository>().To<SubcategoriaProdutoRepository>();
            kernel.Bind<IUnidadeRepository>().To<UnidadeRepository>();
            kernel.Bind<ITamanhoRepository>().To<TamanhoRepository>();
            kernel.Bind<IProdutoAnexoRepository>().To<ProdutoAnexoRepository>();
            //kernel.Bind<IProdutoBarcodeRepository>().To<ProdutoBarcodeRepository>();
            kernel.Bind<IProdutoEstoqueFilialRepository>().To<ProdutoEstoqueFilialRepository>();
            kernel.Bind<IProdutoFornecedorRepository>().To<ProdutoFornecedorRepository>();
            //kernel.Bind<IProdutoGradeRepository>().To<ProdutoGradeRepository>();
            kernel.Bind<IProdutoMovimentoEstoqueRepository>().To<ProdutoMovimentoEstoqueRepository>();
            kernel.Bind<IProdutoOrigemRepository>().To<ProdutoOrigemRepository>();
            kernel.Bind<IProdutoRepository>().To<ProdutoRepository>();
            kernel.Bind<IProdutoTabelaPrecoRepository>().To<ProdutoTabelaPrecoRepository>();
            kernel.Bind<IMovimentoEstoqueProdutoRepository>().To<MovimentoEstoqueProdutoRepository>();
            kernel.Bind<IProdutoKitRepository>().To<ProdutoKitRepository>();
            kernel.Bind<IFichaTecnicaDetalheRepository>().To<FichaTecnicaDetalheRepository>();
            kernel.Bind<ITipoEmbalagemRepository>().To<TipoEmbalagemRepository>();
            kernel.Bind<IFichaTecnicaRepository>().To<FichaTecnicaRepository>();

        }
    }
}